using NAccessorHelper;

namespace Tests
{
    public class A
    {
        public B B { get; set; }
    }

    public class B
    {
        public C C { get; set; }
        public string AsString()
        {
            return String.Format("C inside me = \"{0}\"", C.AsString());
        }
    }

    public class C
    {
        public D D { get; set; }
        public string SomeString { get; set; }
        public string AsString()
        {
            return String.Format("SomeString = {0}, value of D.SomeInt = {1}", SomeString, D.SomeInt);
        }
    }

    public class D
    {
        public int SomeInt { get; set; }
    }

    [TestClass]
    public class TestAccessorHelper
    {
        [TestMethod]
        public void TestAccessor()
        {
            var obj = new A
            {
                B = new B
                {
                    C = new C
                    {
                        D = new D
                        {
                            SomeInt = 42
                        },
                        SomeString = "some string"
                    }
                }
            };

            var intAccessor = AccessorHelper.CreateAccessor<A, int>("B.C.D.SomeInt");
            int value = intAccessor(obj);
            Assert.AreEqual(value, 42);

            var stringAccessor = AccessorHelper.CreateAccessor<A, string>("B.C.SomeString");
            string stringValue = stringAccessor(obj);
            Assert.AreEqual(stringValue, "some string");

            var CAccessor = AccessorHelper.CreateAccessor<A, C>("B.C");
            C CValue = CAccessor(obj);
            Assert.AreEqual(CValue.AsString(), "SomeString = some string, value of D.SomeInt = 42");

            var BAccessor = AccessorHelper.CreateAccessor<A, B>("B");
            B BValue = BAccessor(obj);
            Assert.AreEqual(BValue.AsString(), "C inside me = \"SomeString = some string, value of D.SomeInt = 42\"");
        }
    }
}