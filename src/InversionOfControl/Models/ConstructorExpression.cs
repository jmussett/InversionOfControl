using System;
using System.Linq.Expressions;
using System.Reflection;

namespace InversionOfControl
{
    public class ConstructorExpression
    {
        public DependencyChain[] ParameterChains { get; set; }
        public Func<object[], object> Activate { get; set; }

        public void BuildExpression(ConstructorInfo info)
        {
            // The parameter of the lambda is the arguments for the constructor.

            var parameterArray = Expression.Parameter(typeof(object[]), "objects");

            // We need to create a block, constructing the new class instance and returning it.

            var returnTarget = Expression.Label(typeof(object));
            var returnLabel = Expression.Label(returnTarget, Expression.Constant(null));

            var parameters = info.GetParameters();

            var arguments = new Expression[parameters.Length];

            // For each paramter in the constructor, create a new argument expression for each indexed item in the array.
            for (var i = 0; i < parameters.Length; i++)
            {
                var index = Expression.ArrayAccess(parameterArray, Expression.Constant(i, typeof(int)));
                var argument = Expression.Convert(index, parameters[i].ParameterType);

                arguments[i] = argument;
            }

            var newExpression = Expression.New(info, arguments);
            var returnObject = Expression.Return(returnTarget, newExpression, typeof(object));
            var block = Expression.Block(returnObject, returnLabel);

            // Construct lambda expression from block and input parameters.
            var expression = Expression.Lambda<Func<object[], object>>(block, parameterArray);

            Activate = expression.Compile();
        }
    }
}
