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
            return expressions.Select(expression => Visit(expression));
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node.Left is not MemberExpression memberExpression || node.Right is not ConstantExpression) return base.VisitBinary(node);

            switch (node.NodeType)
            {
                case ExpressionType.Add:
                    return Expression.Increment(memberExpression);
                case ExpressionType.Subtract:
                    return Expression.Decrement(memberExpression);
                default: throw new NotSupportedException($"Operation {node.NodeType} doesn't supported");
            }
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (!this.properties.TryGetValue(node.Member.Name, out var newValue)) return base.VisitMember(node);

            return Expression.Assign(node, Expression.Constant(newValue));
        }
    }
}
