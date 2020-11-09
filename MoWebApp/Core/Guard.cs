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
