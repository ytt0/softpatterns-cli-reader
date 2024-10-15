namespace SoftPatterns.Cli.Reader.Tests
{
    [TestClass]
    public partial class ParametersReaderTests
    {
        public static string TestParser(string value)
        {
            return value;
        }

        public static string TestFailingParser(string value)
        {
            throw new Exception("Expected test parser failure");
        }

        public static void Assert_AreEqual(ValueTypeDescription? expectedDescription, ValueTypeDescription? actualDescription)
        {
            Assert_AreEqual(expectedDescription?.ValueTypeName, actualDescription?.ValueTypeName);
            CollectionAssert.AreEquivalent(expectedDescription?.AvailableValues?.ToArray(), actualDescription?.AvailableValues?.ToArray());
        }

        public static void Assert_AreEqual(ValueTypeName? expectedName, ValueTypeName? actualName)
        {
            Assert.AreEqual(expectedName?.Name, actualName?.Name);
            Assert.AreEqual(expectedName?.NamePlural, actualName?.NamePlural);
        }

        public static TBase Assert_ThrowsExceptionBase<TBase>(Action action) where TBase : Exception
        {
            try
            {
                action();
                Assert.Fail($"Exception of type {typeof(TBase).Name} was expected");
                return default;
            }
            catch (TBase e)
            {
                return e;
            }
        }

        public static string[] CreateValueArgs(bool isNamedValue, params string[] args)
        {
            return isNamedValue ? ["--parameter", .. args] : args;
        }
    }
}