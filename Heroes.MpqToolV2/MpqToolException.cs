using System;

namespace Heroes.MpqToolV2
{
    /// <summary>
    /// Represents mpq parser exceptions.
    /// </summary>
    [Serializable]
    public class MpqToolException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MpqToolException"/> class.
        /// </summary>
        public MpqToolException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MpqToolException"/> class.
        /// </summary>
        /// <param name="message">The custom error message of the exception.</param>
        public MpqToolException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MpqToolException"/> class.
        /// </summary>
        /// <param name="message">The custom error message of the exception.</param>
        /// <param name="innerException">The <see cref="Exception"/> that occured.</param>
        public MpqToolException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
