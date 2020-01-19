using System;
using System.Runtime.Serialization;

namespace InversionOfControl
{
    /// <summary>
    /// The exception that is thrown when a service was not registered.
    /// </summary>
    [Serializable]
    public class MissingServiceException : Exception
    {
        private readonly Type _missingType;

        public override string Message
        {
            get
            {
                if (_missingType == null)
                    return base.Message;

                return $"Type not registered for service '{_missingType.FullName}'.";
            }
        }

        public MissingServiceException(Type missingType)
            => _missingType = missingType ?? throw new ArgumentNullException(nameof(missingType));

        public MissingServiceException(string message)
            : base(message) { }

        public MissingServiceException(string message, Exception inner)
            : base(message, inner) { }

        protected MissingServiceException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
