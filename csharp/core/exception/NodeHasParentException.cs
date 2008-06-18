using System;

namespace urakawa.exception
{
    /// <summary>
    /// Exception thrown when a node belongs to a different presentation than expected
    /// </summary>
    public class NodeHasParentException : CheckedException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        public NodeHasParentException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        /// <param name="inner">The inner exception</param>
        public NodeHasParentException(string msg, Exception inner)
            : base(msg, inner)
        {
        }
    }
}