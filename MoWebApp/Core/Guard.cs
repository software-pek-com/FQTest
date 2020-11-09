using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace MoWebApp.Core
{
    /// <summary>
    /// A static helper class that includes various parameter checking routines.
    /// </summary>
    public static class Guard
    {
        /// <summary>
        /// This value comes from Binding.IndexerName. We don't use it directly here, as it would required an additional
        /// PresentationCore reference which we want to avoid. It signals a binding where all elements bound by indexers
        /// should be refreshed.
        /// </summary>
        private const string indexerPropertyName = "Item[]";

        /// <summary>
        /// Verifies that the argument value is not equal to the given invalid value.
        /// Typically used to check value types not being the default value.
        /// </summary>
        /// <exception cref="ArgumentException">Arguments are equal.</exception>
        public static void ArgumentNotEqual(object argumentValue, object invalidValue, string argumentName)
        {
            if (Equals(argumentValue, invalidValue))
            {
                var errorMessage = string.Format("value cannot be equal to {0}", invalidValue);
                throw new ArgumentException(errorMessage, argumentName);
            }
        }

        /// <summary>
        /// Verifies that the argument value is equal to the given valid value.
        /// </summary>
        /// <exception cref="ArgumentException">Arguments are not equal.</exception>
        public static void ArgumentEqual(object argumentValue, object validValue, string argumentName)
        {
            if (!Equals(argumentValue, validValue))
            {
                var errorMessage = string.Format("value must be equal to {0}", validValue);
                throw new ArgumentException(errorMessage, argumentName);
            }
        }

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> if the given argument is negative.
        /// </summary>
        [DebuggerStepThrough]
        [DebuggerNonUserCode]
        public static void ArgumentNonNegative(double argumentValue, string argumentName)
        {
            if (argumentValue < 0.0)
            {
                throw new ArgumentOutOfRangeException(argumentName, "value must be non negative");
            }
        }

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> if the given argument is negative.
        /// </summary>
        [DebuggerStepThrough]
        [DebuggerNonUserCode]
        public static void ArgumentNonNegative(int argumentValue, string argumentName)
        {
            if (argumentValue < 0)
            {
                throw new ArgumentOutOfRangeException(argumentName, "value must be non negative");
            }
        }

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> if the given argument is smaller or equal to min.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"> if tested value is smaller or equal to min.</exception>
        /// <param name="min">The minimum of the int to throw (valid is +1).</param>
        /// <param name="argumentValue">Argument value to test.</param>
        /// <param name="argumentName">Name of the argument being tested.</param>
        [DebuggerStepThrough]
        [DebuggerNonUserCode]
        public static void ArgumentBigger(int min, int argumentValue, string argumentName)
        {
            if (argumentValue <= min)
            {
                string errorMessage = string.Format("value must be > {0}", min);
                throw new ArgumentOutOfRangeException(argumentName, errorMessage);
            }
        }

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> if the given argument is smaller or equal to min.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"> if tested value is smaller or equal to min.</exception>
        /// <param name="min">The minimum of the int to throw (valid is +1).</param>
        /// <param name="argumentValue">Argument value to test.</param>
        /// <param name="argumentName">Name of the argument being tested.</param>
        [DebuggerStepThrough]
        [DebuggerNonUserCode]
        public static void ArgumentBigger(long min, long argumentValue, string argumentName)
        {
            if (argumentValue <= min)
            {
                string errorMessage = string.Format("value must be > {0}", min);
                throw new ArgumentOutOfRangeException(argumentName, errorMessage);
            }
        }

        /// <summary>
        /// Overload of <see cref="ArgumentBigger(int, int, string)"/>: This is for <see cref="double"/> instead of <see cref="int"/>.
        /// </summary>
        [DebuggerStepThrough]
        [DebuggerNonUserCode]
        public static void ArgumentBigger(double min, double argumentValue, string argumentName)
        {
            if (argumentValue <= min)
            {
                string errorMessage = string.Format("value must be > {0}", min);
                throw new ArgumentOutOfRangeException(argumentName, errorMessage);
            }
        }

        /// <summary>
        /// Overload of <see cref="ArgumentBigger(int, int, string)"/>: This is for <see cref="decimal"/> instead of <see cref="int"/>.
        /// </summary>
        [DebuggerStepThrough]
        [DebuggerNonUserCode]
        public static void ArgumentBigger(decimal min, decimal argumentValue, string argumentName)
        {
            if (argumentValue <= min)
            {
                string errorMessage = string.Format("value must be > {0}", min);
                throw new ArgumentOutOfRangeException(argumentName, errorMessage);
            }
        }

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> if <paramref name="argumentValue"/> is less than <paramref name="min"/>.
        /// Uses <see cref="IComparable{T}"/> for comparison.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"/>
        [DebuggerStepThrough]
        [DebuggerNonUserCode]
        public static void ArgumentNotLess<T>(T min, T argumentValue, string argumentName)
            where T : struct, IComparable<T>
        {
            if (argumentValue.CompareTo(min) < 0)
            {
                throw new ArgumentOutOfRangeException(argumentName, $"Value {argumentValue} must be >= {min}");
            }
        }

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> if <paramref name="argumentValue"/> is greater than <paramref name="min"/>.
        /// Uses <see cref="IComparable{T}"/> for comparison.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"/>
        [DebuggerStepThrough]
        [DebuggerNonUserCode]
        public static void ArgumentNotGreater<T>(T min, T argumentValue, string argumentName)
            where T : struct, IComparable<T>
        {
            if (argumentValue.CompareTo(min) > 0)
            {
                throw new ArgumentOutOfRangeException(argumentName, $"Value {argumentValue} must be <= {min}");
            }
        }

        [DebuggerStepThrough]
        [DebuggerNonUserCode]
        public static void ArgumentInRange<T>(T min, T max, T argumentValue, string argumentName)
            where T : struct, IComparable<T>
        {
            // Check the given range makes sense.
            if (max.CompareTo(min) < 0)
            {
                throw new ArgumentOutOfRangeException(
                    argumentName,
                    argumentValue,
                    $"Min ({min}) has to be less than or equal to max ({max}).");
            }

            var isLessThanMin = argumentValue.CompareTo(min) < 0;
            var isGreaterThanMax = max.CompareTo(argumentValue) < 0;

            if (isLessThanMin || isGreaterThanMax)
            {
                throw new ArgumentOutOfRangeException(
                    argumentName,
                    argumentValue,
                    $"Value {argumentValue} is not in the inclusive range [{min},{max}].");
            }
        }


        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> if the given argument is not in the specified range (expressed as int values),
        /// i.e. &lt; <paramref name="min"/> or &gt; <paramref name="max"/>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"> if tested value is not in the specified range.</exception>
        /// <param name="min">The minimum of the range.</param>
        /// <param name="max">The maximum of the range; is assumed to be &gt;= <paramref name="min"/>.</param>
        /// <param name="argumentValue">Argument value to test.</param>
        /// <param name="argumentName">Name of the argument being tested.</param>
        [DebuggerStepThrough]
        [DebuggerNonUserCode]
        public static void ArgumentInRange(int min, int max, int argumentValue, string argumentName)
        {
            if (argumentValue < min || argumentValue > max)
            {
                string errorMessage = string.Format("value {0} is not in the range [{1}, {2}]", argumentValue, min, max);
                throw new ArgumentOutOfRangeException(argumentName, argumentValue, errorMessage);
            }
        }

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> if the given argument is not in the specified range (expressed as double values),
        /// i.e. &lt; <paramref name="min"/> or &gt; <paramref name="max"/>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"> if tested value is not in the specified range.</exception>
        /// <param name="min">The minimum of the range.</param>
        /// <param name="max">The maximum of the range; is assumed to be &gt;= <paramref name="min"/>.</param>
        /// <param name="argumentValue">Argument value to test.</param>
        /// <param name="argumentName">Name of the argument being tested.</param>
        [DebuggerStepThrough]
        [DebuggerNonUserCode]
        public static void ArgumentInRange(double min, double max, double argumentValue, string argumentName)
        {
            if (argumentValue < min || argumentValue > max)
            {
                string errorMessage = string.Format("value {0} is not in the range [{1}, {2}]", argumentValue, min, max);
                throw new ArgumentOutOfRangeException(argumentName, argumentValue, errorMessage);
            }
        }

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> if the given argument is not in the specified range (expressed as decimal values),
        /// i.e. &lt; <paramref name="min"/> or &gt; <paramref name="max"/>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"> if tested value is not in the specified range.</exception>
        /// <param name="min">The minimum of the range.</param>
        /// <param name="max">The maximum of the range; is assumed to be &gt;= <paramref name="min"/>.</param>
        /// <param name="argumentValue">Argument value to test.</param>
        /// <param name="argumentName">Name of the argument being tested.</param>
        [DebuggerStepThrough]
        [DebuggerNonUserCode]
        public static void ArgumentInRange(decimal min, decimal max, decimal argumentValue, string argumentName)
        {
            if (argumentValue < min || argumentValue > max)
            {
                string errorMessage = string.Format("value {0} is not in the range [{1}, {2}]", argumentValue, min, max);
                throw new ArgumentOutOfRangeException(argumentName, argumentValue, errorMessage);
            }
        }

        /// <summary>
        /// Throws <see cref="ArgumentException"/> if the given argument is not a date-only value (i.e. if its time part is non-zero).
        /// </summary>
        /// <param name="argumentValue">Argument value to test.</param>
        /// <param name="argumentName">Name of the argument being tested.</param>
        /// <exception cref="ArgumentException">When <paramref name="argumentValue"/> is not a date-only value.</exception>
        /// <remarks>
        /// This check is useful because <see cref="DateTimeOffset"/> has to be used for both date-only values and for date &amp; time values.
        /// </remarks>
        [DebuggerStepThrough]
        [DebuggerNonUserCode]
        public static void ArgumentDateOnly(DateTimeOffset argumentValue, string argumentName)
        {
            if (argumentValue != argumentValue.Date)
            {
                throw new ArgumentException("value must be date-only (no time part is allowed)", argumentName);
            }
        }

        /// <summary>
        /// Throws <see cref="ArgumentException"/> if the given argument is not a date-only value (i.e. if its time part is non-zero).
        /// </summary>
        /// <param name="argumentValue">Argument value to test.</param>
        /// <param name="argumentName">Name of the argument being tested.</param>
        /// <exception cref="ArgumentException">When <paramref name="argumentValue"/> is not a date-only value.</exception>
        /// <remarks>
        /// This check is useful because <see cref="DateTimeOffset"/> has to be used for both date-only values and for date &amp; time values.
        /// </remarks>
        [DebuggerStepThrough]
        [DebuggerNonUserCode]
        public static void ArgumentDateOnly(DateTimeOffset? argumentValue, string argumentName)
        {
            if (!argumentValue.HasValue) { return; }

            Guard.ArgumentDateOnly(argumentValue.Value, argumentName);
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

        /// <summary>
        /// Throws <see cref="ArgumentNullException"/> if the given argument is null.
        /// </summary>
        /// <exception cref="ArgumentNullException"> if tested value if null.</exception>
        /// <param name="argumentValue">Argument value to test.</param>
        /// <param name="argumentName">Name of the argument being tested.</param>
        /// <param name="detailedMessage">The exception message when argumentValue is null.</param>
        /// <exception cref="ArgumentNullException">When argumentValue is null.</exception>
        [DebuggerStepThrough]
        [DebuggerNonUserCode]
        public static void ArgumentNotNull(Object argumentValue, string argumentName, string detailedMessage)
        {
            if (argumentValue == null) { throw new ArgumentNullException(argumentName, detailedMessage); }
        }

        /// <summary>
        /// Throws <see cref="ArgumentException"/> if the property of the given argument (as described through the path) is empty.
        /// </summary>
        /// <param name="argumentValue">the value to test</param>
        /// <param name="argumentPath">the path to the faulty property; may not be null, should not be empty</param>
        [DebuggerStepThrough]
        [DebuggerNonUserCode]
        public static void ArgumentPropertyNotEmpty(Guid argumentValue, string argumentPath)
        {
            if (argumentValue == Guid.Empty)
            {
                string argumentName = argumentPath.GetPathRoot();
                string defaultMessage = string.Format("argument '{0}' is invalid because '{1}' is empty", argumentName, argumentPath);

                throw new ArgumentException(defaultMessage, argumentPath);
            }
        }

        /// <summary>
        /// Throws <see cref="ArgumentException"/> if the property of the given argument (as described through the path) is null.
        /// </summary>
        /// <param name="argumentValue">the value to test</param>
        /// <param name="argumentPath">the path to the faulty property; may not be null, should not be empty</param>
        /// <param name="detailedMessage">the detailed message for the exception</param>
        public static void ArgumentPropertyNotNull<T>(T argumentValue, string argumentPath, string detailedMessage)
            where T : class // To prevent the use of value types.
        {
            if (argumentValue == null) { throw new ArgumentException(argumentPath, detailedMessage); }
        }

        /// <summary>
        /// Throws <see cref="ArgumentException"/> if the property of the given argument (as described through the path) is null.
        /// </summary>
        /// <param name="argumentValue">the value to test</param>
        /// <param name="argumentPath">the path to the faulty property; may not be null, should not be empty</param>
        public static void ArgumentPropertyNotNull<T>(T argumentValue, string argumentPath)
            where T : class // To prevent the use of value types.
        {
            if (argumentValue == null)
            {
                string argumentName = argumentPath.GetPathRoot();
                string defaultMessage = string.Format("argument '{0}' is invalid because '{1}' is null", argumentName, argumentPath);

                throw new ArgumentException(argumentPath, defaultMessage);
            }
        }

        /// <summary>
        /// Throws <see cref="ArgumentException"/> if the property of the given argument (as described through the path) is null.
        /// </summary>
        /// <param name="argumentValue">the value to test</param>
        /// <param name="argumentPath">the path to the faulty property; may not be null, should not be empty</param>
        public static void ArgumentPropertyNotNull<T>(T? argumentValue, string argumentPath)
            where T : struct // To prevent the use of value types.
        {
            if (!argumentValue.HasValue)
            {
                string argumentName = argumentPath.GetPathRoot();
                string defaultMessage = string.Format("argument '{0}' is invalid because '{1}' is null", argumentName, argumentPath);

                throw new ArgumentException(argumentPath, defaultMessage);
            }
        }

        /// <summary>
        /// Throws <see cref="ArgumentException"/> if the string property of the given argument (as described through the path) is null or empty.
        /// </summary>
        /// <param name="argumentValue">the value to test</param>
        /// <param name="argumentPath">the path to the faulty property; may not be null, should not be empty</param>
        /// <param name="detailedMessage">the detailed message for the exception</param>
        public static void ArgumentPropertyNotNullOrEmpty(string argumentValue, string argumentPath, string detailedMessage)
        {
            if (string.IsNullOrEmpty(argumentValue)) { throw new ArgumentException(argumentPath, detailedMessage); }
        }

        /// <summary>
        /// Throws <see cref="ArgumentException"/> if the string property of the given argument (as described through the path) is
        /// null or empty.
        /// </summary>
        /// <param name="argumentValue">the value to test</param>
        /// <param name="argumentPath">the path to the faulty property; may not be null, should not be empty</param>
        public static void ArgumentPropertyNotNullOrEmpty(string argumentValue, string argumentPath)
        {
            if (string.IsNullOrEmpty(argumentValue))
            {
                var argumentName = argumentPath.GetPathRoot();
                var defaultMessage = string.Format("argument '{0}' is invalid because '{1}' is null or empty", argumentName, argumentPath);

                throw new ArgumentException(argumentPath, defaultMessage);
            }
        }

        /// <summary>
        /// Throws <see cref="ArgumentException"/> if the string property of the given argument (as described through the path) is
        /// null, empty or contains only whitespace.
        /// </summary>
        /// <param name="argumentValue">the value to test</param>
        /// <param name="argumentPath">the path to the faulty property; may not be null, should not be empty</param>
        public static void ArgumentPropertyNotNullOrWhitespace(string argumentValue, string argumentPath)
        {
            if (string.IsNullOrWhiteSpace(argumentValue))
            {
                var argumentName = argumentPath.GetPathRoot();
                var defaultMessage = string.Format("argument '{0}' is invalid because '{1}' is null or whitespace", argumentName, argumentPath);

                throw new ArgumentException(argumentPath, defaultMessage);
            }
        }

        /// <remarks>
        /// This method is deliberately NOT in the stringExtensions class of Eurofins.Shell.Core, because that class already uses this one.
        /// </remarks>
        private static string GetPathRoot(this string source)
        {
            ArgumentNotNull(source, "source");

            int firstSeparator = source.IndexOf('.');
            if (firstSeparator > -1)
            {
                return source.Substring(0, firstSeparator);
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Throws an exception if the tested argument is <see cref="Guid.Empty"/>.
        /// </summary>
        /// <param name="argumentValue">Argument value to check.</param>
        /// <param name="argumentName">Name of argument being checked.</param>
        /// <exception cref="ArgumentException">Thrown if the argument is empty</exception>
        [DebuggerStepThrough]
        [DebuggerNonUserCode]
        public static void ArgumentNotEmpty(Guid argumentValue, string argumentName)
        {
            if (argumentValue == Guid.Empty)
            {
                throw new ArgumentException("Guid must not be empty", argumentName);
            }
        }

        /// <summary>
        /// Throws an exception if the tested enumeration argument is null or empty (i.e. contains no item).
        /// </summary>
        /// <param name="argumentValue">Argument value to check; may be null, in which case the test passes (you may want to first call ArgumentNotNull, see remarks).</param>
        /// <param name="argumentName">Name of argument being checked.</param>
        /// <exception cref="ArgumentException">Thrown if the enumeration argument is empty</exception>
        /// <remarks>
        /// For the client code, to avoid multiple evaluations of the potentially costly enumeration, it is advised to follow the following code pattern
        /// ('enumeration' being a param of the client code method):
        /// <code>
        /// Guard.ArgumentNotNull(enumeration, "enumeration");
        /// var enumerationAsArray = enumeration.ToArray();
        /// Guard.ArgumentNotEmpty(enumerationAsArray, "enumeration");
        /// </code>
        /// <b>Note:</b> At this point, the rest of the method must use 'enumerationAsArray'. Also notice the params passed to the 2nd Guard.
        /// <para/>This code pattern is the reason why no method 
        /// <c>ArgumentNotNullOrEmpty&lt;T&gt;(IEnumerable&lt;T&gt; argumentValue, string argumentName)</c>
        /// is available here.
        /// </remarks>
        [DebuggerStepThrough]
        [DebuggerNonUserCode]
        public static void ArgumentNotEmpty<T>(IEnumerable<T> argumentValue, string argumentName)
        {
            if (argumentValue == null)
            {
                throw new ArgumentNullException(argumentName);
            }

            if (!argumentValue.Any())
            {
                throw new ArgumentException("Enumeration must not be empty", argumentName);
            }
        }

        /// <summary>
        /// Throws an exception if the tested item collection argument is null (it may be empty)
        /// or any of its elements is null.
        /// </summary>
        /// <param name="argumentValue">Argument values to check.</param>
        /// <param name="argumentName">Name of argument being checked.</param>
        /// <exception cref="ArgumentNullException">When argumentValue is null.</exception>
        /// <exception cref="ArgumentNullException">If any item in the collection is null.</exception>
        /// <exception cref="ArgumentException">If any item in the collection is empty</exception>
        [DebuggerStepThrough]
        [DebuggerNonUserCode]
        public static void ArgumentNotNull<T>(IEnumerable<T> argumentValue, string argumentName)
        {
            if (argumentValue == null)
            {
                throw new ArgumentNullException(argumentName);
            }

            int i = 0;
            foreach (var argumentItemValue in argumentValue)
            {
                ArgumentNotNull(argumentItemValue, string.Format("{0}[{1}]", argumentName, i++));
            }
        }

        /// <summary>
        /// Throws an exception if the tested item collection argument is null, empty
        /// or any of its elements is null.
        /// </summary>
        /// <param name="argumentValue">Argument values to check.</param>
        /// <param name="argumentName">Name of argument being checked.</param>
        /// <exception cref="ArgumentNullException">When argumentValue is null.</exception>
        /// <exception cref="ArgumentNullException">If any item in the collection is null.</exception>
        /// <exception cref="ArgumentException">If the collection is empty.</exception>
        [DebuggerStepThrough]
        [DebuggerNonUserCode]
        public static void ArgumentNotNullOrEmpty<T>(IEnumerable<T> argumentValue, string argumentName)
        {
            ArgumentNotEmpty(argumentValue, argumentName);

            int i = 0;
            foreach (var argumentItemValue in argumentValue)
            {
                ArgumentNotNull(argumentItemValue, string.Format("{0}[{1}]", argumentName, i++));
            }
        }

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
        /// Throws an exception if the tested string argument is null, empty or contains only white space characters.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if string value is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the string is empty or contains only white space characters.</exception>
        /// <param name="argumentValue">Argument value to check.</param>
        /// <param name="argumentName">Name of argument being checked.</param>
        /// <exception cref="ArgumentNullException">When argumentValue is null.</exception>
        /// <exception cref="ArgumentException">When argumentValue is empty or contains only white space characters.</exception>
        [DebuggerStepThrough]
        [DebuggerNonUserCode]
        public static void ArgumentNotNullOrWhitespace(string argumentValue, string argumentName)
        {
            if (argumentValue == null) { throw new ArgumentNullException(argumentName); }
            if (string.IsNullOrWhiteSpace(argumentValue))
            {
                throw new ArgumentException("Argument must not be empty or contain only whitespace characters", argumentName);
            }
        }

        /// <summary>
        /// Throws an exception if the tested string collection argument is null (it may be empty) or any of its items is null or empty.
        /// </summary>
        /// <param name="argumentValue">Argument values to check.</param>
        /// <param name="argumentName">Name of argument being checked.</param>
        /// <exception cref="ArgumentNullException">When argumentValue is null.</exception>
        /// <exception cref="ArgumentNullException">If any item in the collection is null.</exception>
        /// <exception cref="ArgumentException">If any item in the collection is empty</exception>
        [DebuggerStepThrough]
        [DebuggerNonUserCode]
        public static void ArgumentNotNullOrEmpty(IEnumerable<string> argumentValue, string argumentName)
        {
            if (argumentValue == null)
            {
                throw new ArgumentNullException(argumentName);
            }

            int i = 0;
            foreach (var argumentItemValue in argumentValue)
            {
                ArgumentNotNullOrEmpty(argumentItemValue, string.Format("{0}[{1}]", argumentName, i++));
            }
        }

        /// <summary>
        /// Throws an exception if the tested string collection argument is null (it may be empty) or any of its items is null, empty or whitespace.
        /// </summary>
        /// <param name="argumentValue">Argument values to check.</param>
        /// <param name="argumentName">Name of argument being checked.</param>
        /// <exception cref="ArgumentNullException">When argumentValue is null.</exception>
        /// <exception cref="ArgumentNullException">If any item in the collection is null.</exception>
        /// <exception cref="ArgumentException">If any item in the collection is empty</exception>
        [DebuggerStepThrough]
        [DebuggerNonUserCode]
        public static void ArgumentNotNullOrWhitespace(IEnumerable<string> argumentValue, string argumentName)
        {
            if (argumentValue == null)
            {
                throw new ArgumentNullException(argumentName);
            }

            int i = 0;
            foreach (var argumentItemValue in argumentValue)
            {
                ArgumentNotNullOrWhitespace(argumentItemValue, string.Format("{0}[{1}]", argumentName, i++));
            }
        }

        /// <summary>
        /// Verifies that an argument type is assignable from the provided type (meaning
        /// interfaces are implemented, or classes exist in the base class hierarchy).
        /// </summary>
        /// <param name="assignmentTargetType">The argument type that will be assigned to.</param>
        /// <param name="assignmentValueType">The type of the value being assigned.</param>
        /// <param name="argumentName">Argument name.</param>
        /// <exception cref="ArgumentNullException">When assignmentTargetType or assignmentValueType is null.</exception>
        /// <exception cref="ArgumentException">When target type is not assignable.</exception>
        [DebuggerStepThrough]
        [DebuggerNonUserCode]
        public static void TypeIsAssignable(Type assignmentTargetType, Type assignmentValueType, string argumentName)
        {
            TypeIsAssignable(assignmentTargetType, assignmentValueType, argumentName, string.Format("Types are not assignable: {0} from {1}", assignmentTargetType, assignmentValueType));
        }

        /// <summary>
        /// Verifies that an argument type is assignable from the provided type (meaning
        /// interfaces are implemented, or classes exist in the base class hierarchy).
        /// </summary>
        /// <param name="assignmentTargetType">The argument type that will be assigned to.</param>
        /// <param name="assignmentValueType">The type of the value being assigned.</param>
        /// <param name="argumentName">Argument name.</param>
        /// <param name="detailedMessage">The detailed exception message to throw when the type is not assignable</param>
        /// <exception cref="ArgumentNullException">When <paramref name="assignmentTargetType"/> or <paramref name="assignmentValueType"/> is null.</exception>
        /// <exception cref="ArgumentException">When target type is not assignable.</exception>
        [DebuggerStepThrough]
        [DebuggerNonUserCode]
        public static void TypeIsAssignable(Type assignmentTargetType, Type assignmentValueType, string argumentName, string detailedMessage)
        {
            if (assignmentTargetType == null) { throw new ArgumentNullException("assignmentTargetType"); }
            if (assignmentValueType == null) { throw new ArgumentNullException("assignmentValueType"); }

            if (!assignmentTargetType.GetTypeInfo().IsAssignableFrom(assignmentValueType.GetTypeInfo()))
            {
                Debug.WriteLine(detailedMessage, "DEBUG");
                throw new ArgumentException(detailedMessage, argumentName);
            }
        }

        /// <summary>
        /// Verifies that the two types are equal
        /// </summary>
        /// <param name="assignmentTargetType">The argument type that will be assigned to.</param>
        /// <param name="assignmentValueType">The type of the value being assigned.</param>
        /// <param name="argumentName">Argument name.</param>
        /// <param name="detailedMessage">The detailed exception to throw when the type is not assignable</param>
        /// <exception cref="ArgumentNullException">When <paramref name="assignmentTargetType"/> or <paramref name="assignmentValueType"/> is null.</exception>
        /// <exception cref="ArgumentException">When target type is not assignable.</exception>
        [DebuggerStepThrough]
        [DebuggerNonUserCode]
        public static void TypeEquals(Type assignmentTargetType, Type assignmentValueType, string argumentName, string detailedMessage)
        {
            if (assignmentTargetType == null) { throw new ArgumentNullException("assignmentTargetType"); }
            if (assignmentValueType == null) { throw new ArgumentNullException("argumentName"); }

            if (assignmentTargetType != assignmentValueType)
            {
                throw new ArgumentException(detailedMessage, argumentName);
            }
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the given argument value is not defined in enum <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The enum type; must be an enum type (this is checked only at runtime).</typeparam>
        /// <param name="enumValue">The enum value.</param>
        /// <param name="argumentName">Name of argument being checked.</param>
        /// <exception cref="System.ArgumentException">When value is not defined in enum <typeparamref name="T"/>.</exception>
        /// <exception cref="InvalidOperationException">When <typeparamref name="T"/> is not an enum</exception>
        [DebuggerStepThrough]
        [DebuggerNonUserCode]
        public static void ArgumentEnum<T>(T enumValue, string argumentName)
            where T : struct // Note: that's a compromise since 'where T : Enum' is not supported in C#. The real check is done at runtime.
        {
            Type enumType = typeof(T);
            if (!enumType.GetTypeInfo().IsEnum) // Note: property Type.IsEnum is not available in portable .NET, so we need to get the TypeInfo to achieve the same effect.
            {
                string errorMessage = string.Format("Type {0} is not an enum", enumType);
                throw new InvalidOperationException(errorMessage);
            }

            if (!Enum.IsDefined(enumType, enumValue))
            {
                string errorMessage = string.Format("Value {0} not defined in enum {1}", enumValue, enumType);
                throw new ArgumentException(errorMessage, argumentName);
            }
        }
    }
}
