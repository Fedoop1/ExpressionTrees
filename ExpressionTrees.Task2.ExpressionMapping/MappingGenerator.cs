using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ExpressionTrees.Task2.ExpressionMapping
{
    public class MappingGenerator<TSource, TDestination>
    {
        private readonly Dictionary<string, string> mapping;

        private MappingGenerator()
        {
            mapping = new Dictionary<string, string>();
        }

        public static MappingGenerator<TSource, TDestination> Create() => new();

        public MappingGenerator<TSource, TDestination> Map<TPropType>(Expression<Func<TSource, TPropType>> from,
            Expression<Func<TDestination, TPropType>> to)
        {
            if (from.Body is not MemberExpression leftMemberExpression) throw new Exception();
            if (to.Body is not MemberExpression rightMemberExpression) throw new Exception();

            this.mapping[leftMemberExpression.Member.Name] = rightMemberExpression.Member.Name;

            return this;
        }

        public Mapper<TSource, TDestination> Build()
        {
            var sourceType = typeof(TSource);
            var destinationType = typeof(TDestination);

            var sourceObjParam = Expression.Parameter(sourceType, "source");

            var destinationObjVar = Expression.Variable(destinationType, "destinationVar");
            var sourceObjVar = Expression.Variable(sourceType, "sourceVar");

            var destinationObjCtor = Expression.New(destinationType);

            var returnTarget = Expression.Label(destinationType);
            var returnExp = Expression.Return(returnTarget, destinationObjVar, destinationType);
            var returnLabel = Expression.Label(returnTarget, Expression.Default(destinationType));

            var initialSequence = new Expression[]
            {
                Expression.Assign(sourceObjVar, sourceObjParam),
                Expression.Assign(destinationObjVar, destinationObjCtor),
            };

            var mappingSequence = mapping.Select(kvp =>
            {
                var fromMember = Expression.PropertyOrField(sourceObjVar, kvp.Key);
                var toMember = Expression.PropertyOrField(destinationObjVar, kvp.Value);

                return Expression.Assign(toMember, fromMember) as Expression;
            });

            var returnSequence = new Expression[]
            {
                returnExp,
                returnLabel,
            };

            var finalSequence = initialSequence.Concat(mappingSequence).Concat(returnSequence);

            var block = Expression.Block(new[] { sourceObjVar, destinationObjVar }, finalSequence);

            var mapFunction =
                Expression.Lambda<Func<TSource, TDestination>>(
                    block,
                    sourceObjParam
                );

            return new Mapper<TSource, TDestination>(mapFunction.Compile());
        }
    }
}
