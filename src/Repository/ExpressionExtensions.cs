using System;
using System.Linq.Expressions;

namespace Snippets.Repository
{
    public static class ExpressionExtensions
    {
        #region Public static methods

        /// <summary>
        /// Determines if the expression is equivalent to the specified expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="path"></param>
        /// <returns>true if both <paramref name="value"/> and <paramref name="path"/> refer to the same class member.</returns>
        public static bool PathEquals<T>(this Expression<Func<T, object>> value,
                                              Expression<Func<T, object>> path)
        {
            if (path == null)
            {
                return false;
            }
            if (ReferenceEquals(value, path))
            {
                return true;
            }
            return value.ToPathString() == path.ToPathString();
        }

        public static bool StartsWith<T>(this Expression<Func<T, object>> value,
                                              Expression<Func<T, object>> path)
        {
            if (path == null)
            {
                return false;
            }
            if (ReferenceEquals(value, path))
            {
                return true;
            }

            string valueAsString = value.ToPathString();
            string pathAsString  = path.ToPathString();

            if (valueAsString == pathAsString)
            {
                return true;
            }

            return valueAsString.StartsWith(pathAsString + ".");
        }

        /// <summary>
        /// Gets the include path represented by the expression as a string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToPathString<T>(this Expression<Func<T, object>> value)
        {
            return GetPathString(value);
        }

        #endregion

        #region Private static methods

        /// <summary>
        /// Gets the include path represented by the expression as a string.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        /// <example>
        /// <para>"x => x.Foo" returns Foo</para>
        /// <para>"x => x.Foo.Select(f => f.Bar)" returns Foo.Bar</para>
        /// </example>
        private static string GetPathString(Expression expr)
        {
            if (expr == null)
            {
                return null;
            }

            switch (expr.NodeType)
            {
                case ExpressionType.Lambda:
                    // Recurse down to the next level
                    return GetPathString(((LambdaExpression)expr).Body);

                case ExpressionType.Call:
                    var args = ((MethodCallExpression)expr).Arguments;
                    if (args.Count != 2)
                    {
                        throw new ArgumentException();
                    }
                    // Recurse down to the next level
                    return GetPathString(args[0]) + "." + GetPathString(args[1]);

                case ExpressionType.MemberAccess:
                    return ((MemberExpression)expr).Member.Name;

                default:
                    throw new ArgumentException(@"Invalid Expression type", "expr");
            }
        }

        #endregion
    }
}
