using System;

namespace Bluegrams.Application
{
    /// <summary>
    /// The exception that is thrown if downloading an app update fails.
    /// </summary>
    public class UpdateFailedException : Exception
    {
        public UpdateFailedException(string message) : base(message)
        { }

        public UpdateFailedException(string message, Exception innerException) : base(message, innerException)
        { }
    }
}
