using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ExpressionTrees.Task1.ExpressionsTransformer
{
    public class IncDecExpressionVisitor<TEntity> : ExpressionVisitor where TEntity : class
    {
        private readonly TEntity target;
        private readonly IDictionary<string, object> properties;

        public IncDecExpressionVisitor(TEntity target, IDictionary<string, object> properties)
        {
            this.target = target;
            this.properties = properties;
        }

        public IEnumerable<Expression> Transform(params Expression<Func<TEntity, object>>[] expressions)
        {
            return expressions.Select(Visit);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node.Left is not MemberExpression memberExpression || node.Right is not ConstantExpression ||
                node.NodeType is not ExpressionType.Add and ExpressionType.Decrement) return base.VisitBinary(node);

            var targetParameter = Expression.Parameter(typeof(TEntity), "target");
            var targetProp = Expression.PropertyOrField(targetParameter, memberExpression.Member.Name);

            var expression = node.NodeType == ExpressionType.Add
                ? Expression.PostIncrementAssign(targetProp)
                : Expression.PostDecrementAssign(targetProp);

            Expression.Lambda<Action<TEntity>>(expression, false, targetParameter).Compile()(target);

            return expression;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (!this.properties.TryGetValue(node.Member.Name, out var newValue)) return base.VisitMember(node);

            var targetParameter = Expression.Parameter(typeof(TEntity), "target");
            var targetProp = Expression.PropertyOrField(targetParameter, node.Member.Name);
            var expression = Expression.Assign(targetProp, Expression.Constant(newValue));

            Expression.Lambda<Action<TEntity>>(expression, false, targetParameter).Compile()(target);

            return expression;
        }
    }
}
