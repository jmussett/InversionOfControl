using System;
using System.Runtime.Serialization;

namespace InversionOfControl
{
    /// <summary>
    /// The exception that is thrown when a dependency for a service could not be resolved.
    /// </summary>
    [Serializable]
    public class MissingDependencyException : Exception
    {
        private readonly Type _missingType;
        private readonly Type _dependentType;
        private readonly string[] _resolutionStack;

        public override string Message
        {
            get
            {
                if (_missingType == null || _dependentType == null || _resolutionStack == null)
                    return base.Message;

                var message = $"Unable to resolve dependency '{_missingType.FullName}' for type '{_dependentType.FullName}'.";

                message += Environment.NewLine;
                message += "Resolution Stack:";

                foreach(var type in _resolutionStack)
                {
                    message += Environment.NewLine;
                    message += $"Type: {type}";
                }

                return message;
            }
        }

        public  MissingDependencyException(Type missingType, Type dependentType, string[] resolutionStack)
        {
            _missingType = missingType ?? throw new ArgumentNullException(nameof(missingType));
            _dependentType = dependentType ?? throw new ArgumentNullException(nameof(dependentType));
            _resolutionStack = resolutionStack ?? throw new ArgumentNullException(nameof(resolutionStack));
        }

        public MissingDependencyException(string message)
            : base(message) { }

        public MissingDependencyException(string message, Exception inner)
            : base(message, inner) { }

        protected MissingDependencyException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
