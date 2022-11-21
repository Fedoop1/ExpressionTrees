/*
 * Create a class based on ExpressionVisitor, which makes expression tree transformation:
 * 1. converts expressions like <variable> + 1 to increment operations, <variable> - 1 - into decrement operations.
 * 2. changes parameter values in a lambda expression to constants, taking the following as transformation parameters:
 *    - source expression;
 *    - dictionary: <parameter name: value for replacement>
 * The results could be printed in console or checked via Debugger using any Visualizer.
 */
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ExpressionTrees.Task1.ExpressionsTransformer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Expression Visitor for increment/decrement.");
            Console.WriteLine();

            var numStorage = new NumStorage(1, 2, 3);

            var propDictionary = new Dictionary<string, object>()
            {
                [nameof(NumStorage.Num1)] = 9,
                [nameof(NumStorage.Num2)] = 9,
                [nameof(NumStorage.Num3)] = 9,
            };

            IEnumerable<Expression> expressions = default;

            var incDecExpressionVisitor = new IncDecExpressionVisitor<NumStorage>(numStorage, propDictionary);

            PrintObject(numStorage);

            Console.WriteLine("Increment first num, decrement second field, increment third field");

            expressions = incDecExpressionVisitor.Transform(s => s.Num1 + 1, s => s.Num2 - 1, s => s.Num3 + 1);

            PrintExpression(expressions);
            PrintObject(numStorage);

            Console.WriteLine("Update initial object. New values: Num1 = 9, Num2 = 9, Num3 = 9");

            expressions = incDecExpressionVisitor.Transform(s => s.Num1, s => s.Num3);

            PrintExpression(expressions);
            PrintObject(numStorage);

            Console.ReadLine();
        }

        private static void PrintExpression(IEnumerable<Expression> expressions)
        {
            foreach (var expression in expressions)
            {
                Console.WriteLine(expression);
            }
        }

        private static void PrintObject<TEntity>(TEntity entity) => Console.WriteLine($"Current value: {entity}");
    }

    public record NumStorage(int Num1, int Num2, int Num3);
}
