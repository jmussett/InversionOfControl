using System;
using System.Runtime.Serialization;

namespace InversionOfControl
{
    /// <summary>
    /// The exception that is thrown when a circular dependency was found in the dependency chain.
    /// </summary>
    [Serializable]
    public class CircularDependencyException : Exception
    {
        private Type _circularType;
        private string[] _resolutionStack;

        public override string Message
        {
            get
            {
                if (_circularType == null || _resolutionStack == null)
                    return base.Message;

                var message =  $"A circular dependency was found when attempting to instanciate type '{_circularType.FullName}'.";

                message += Environment.NewLine;
                message += "Resolution Stack:";

                foreach (var type in _resolutionStack)
                {
                    message += Environment.NewLine;
                    message += $"Type: {type}";
                }

                return message;
            }
        }

        public CircularDependencyException(Type circularType, string[] resolutionStack)
        {
            _circularType = circularType ?? throw new ArgumentNullException(nameof(circularType));
            _resolutionStack = resolutionStack ?? throw new ArgumentNullException(nameof(resolutionStack));
        }

        public CircularDependencyException(string message)
            : base(message) { }

        public CircularDependencyException(string message, Exception inner)
            : base(message, inner) { }

        protected CircularDependencyException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
