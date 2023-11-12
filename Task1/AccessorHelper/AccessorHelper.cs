using System.Linq.Expressions;

namespace NAccessorHelper
{
    public class AccessorHelper
    {
        public static Func<T, U> CreateAccessor<T, U>(string path)
        {
            var parameter = Expression.Parameter(typeof(T));
            var memberExpression = CreateMemberExpression(parameter, path);
            var lambdaExpression = Expression.Lambda<Func<T, U>>(memberExpression, parameter);

            return lambdaExpression.Compile();
        }

        private static Expression CreateMemberExpression(Expression expression, string path)
        {
            foreach (var propertyName in path.Split('.'))
            {
                expression = Expression.PropertyOrField(expression, propertyName);
            }

            return expression;
        }
    }
}