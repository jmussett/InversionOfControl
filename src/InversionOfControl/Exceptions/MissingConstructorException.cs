using System;
using System.Runtime.Serialization;

namespace InversionOfControl
{
    /// <summary>
    /// The exception that is thrown when a public constructor could not be found for a service.
    /// </summary>
    [Serializable]
    public class MissingConstructorException : Exception
    {
        private readonly Type _type;

        public override string Message
        {
            get
            {
                if (_type == null)
                    return base.Message;

                return $"Unable to locate a public constructor for type '{_type.FullName}'.";
            }
        }

        public MissingConstructorException(Type type)
            => _type = type ?? throw new ArgumentNullException(nameof(type));

        public MissingConstructorException(string message)
            : base(message) { }

        public MissingConstructorException(string message, Exception inner)
            : base(message, inner) { }

        protected MissingConstructorException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
