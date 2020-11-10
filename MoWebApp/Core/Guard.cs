using System;
using System.Diagnostics;

namespace MoWebApp.Core
{
    /// <summary>
    /// A static helper class that includes various parameter checking routines.
    /// </summary>
    public static class Guard
    {
        /// <summary>
        /// Throws an exception if the tested string argument is null or the empty string.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if string value is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the string is empty</exception>
        /// <param name="argumentValue">Argument value to check.</param>
        /// <param name="argumentName">Name of argument being checked.</param>
        /// <exception cref="ArgumentNullException">When argumentValue is null.</exception>
        /// <exception cref="ArgumentException">When argumentValue is empty.</exception>
        [DebuggerStepThrough]
        [DebuggerNonUserCode]
        public static void ArgumentNotNullOrEmpty(string argumentValue, string argumentName)
        {
            if (argumentValue == null) { throw new ArgumentNullException(argumentName); }
            if (argumentValue.Length == 0) { throw new ArgumentException("Argument must not be empty", argumentName); }
        }

        /// <summary>
        /// Throws <see cref="ArgumentNullException"/> if the given argument is null.
        /// </summary>
        /// <param name="argumentValue">Argument value to test.</param>
        /// <param name="argumentName">Name of the argument being tested.</param>
        /// <exception cref="ArgumentNullException">When argumentValue is null.</exception>
        [DebuggerStepThrough]
        [DebuggerNonUserCode]
        public static void ArgumentNotNull(Object argumentValue, string argumentName)
        {
            if (argumentValue == null) { throw new ArgumentNullException(argumentName); }
        }
    }
}
