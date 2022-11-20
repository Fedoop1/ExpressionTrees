using ExpressionTrees.Task2.ExpressionMapping.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionTrees.Task2.ExpressionMapping.Tests
{
    [TestClass]
    public class ExpressionMappingTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var mapGenerator = MappingGenerator<Foo, Bar>
                .Create()
                .Map(f => f.Prop1, b => b.Prop1)
                .Map(f => f.Prop2, b => b.Prop2)
                .Map(f => f.Prop3, b => b.Prop3);

            var mapper = mapGenerator.Build();

            var foo = new Foo() {Prop1 = "test", Prop2 = 1, Prop3 = true};

            var bar = mapper.Map(foo);

            Assert.AreEqual(foo.Prop1, bar.Prop1);
            Assert.AreEqual(foo.Prop2, bar.Prop2);
            Assert.AreEqual(foo.Prop3, bar.Prop3);
        }
    }
}
