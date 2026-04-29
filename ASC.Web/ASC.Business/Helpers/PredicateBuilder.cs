using System.Linq.Expressions;

namespace ASC.Business.Helpers
{
    public static class PredicateBuilder
    {
        public static Expression<Func<T, bool>> True<T>()
        {
            return param => true;
        }

        public static Expression<Func<T, bool>> False<T>()
        {
            return param => false;
        }

        public static Expression<Func<T, bool>> And<T>(
            this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            return Compose(first, second, Expression.AndAlso);
        }

        public static Expression<Func<T, bool>> Or<T>(
            this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            return Compose(first, second, Expression.OrElse);
        }

        private static Expression<Func<T, bool>> Compose<T>(
            Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second,
            Func<Expression, Expression, Expression> merge)
        {
            var parameter = first.Parameters[0];

            var secondBody = new ReplaceParameterVisitor(
                second.Parameters[0],
                parameter
            ).Visit(second.Body);

            return Expression.Lambda<Func<T, bool>>(
                merge(first.Body, secondBody!),
                parameter
            );
        }

        private class ReplaceParameterVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression _oldParameter;
            private readonly ParameterExpression _newParameter;

            public ReplaceParameterVisitor(
                ParameterExpression oldParameter,
                ParameterExpression newParameter)
            {
                _oldParameter = oldParameter;
                _newParameter = newParameter;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == _oldParameter ? _newParameter : base.VisitParameter(node);
            }
        }
    }
}