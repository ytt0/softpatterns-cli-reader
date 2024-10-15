
namespace SoftPatterns.Cli.Reader.Tests
{
    [TestClass]
    public class ParameterNameFormatterTests
    {
        [TestMethod]
        public void FormatParameter_WithName_NameIsFormatted()
        {
            // setup

            var formatter = new ParameterNameFormatter();
            var name = "name";
            var nameToken = new ParameterToken(ParameterTokenType.Name, 0, "name", 2, new ParameterToken(ParameterTokenType.RawArg, 0, "--parameter", 0, null));
            var expectedFormat = $"--name parameter";

            // test

            var format = formatter.FormatParameter(nameToken, name);
            Assert.AreEqual(expectedFormat, format);
        }

        [TestMethod]
        public void FormatParameter_WithoutNameAndWithNameToken_NameIsFormatted()
        {
            // setup

            var formatter = new ParameterNameFormatter();
            var nameToken = new ParameterToken(ParameterTokenType.Name, 0, "parameter", 2, new ParameterToken(ParameterTokenType.RawArg, 0, "--parameter", 0, null));
            var expectedFormat = $"--parameter parameter";

            // test

            var format = formatter.FormatParameter(nameToken, null);
            Assert.AreEqual(expectedFormat, format);
        }

        [TestMethod]
        public void FormatParameter_WithoutNameAndWithSingleShortNameToken_ShortNameIsFormatted()
        {
            // setup

            var formatter = new ParameterNameFormatter();
            var nameToken = new ParameterToken(ParameterTokenType.ShortName, 0, "a", 1, new ParameterToken(ParameterTokenType.NameGroup, 0, "a", 0, new ParameterToken(ParameterTokenType.RawArg, 0, "-a", 0, null)));
            var expectedFormat = $"-a parameter";

            // test

            var format = formatter.FormatParameter(nameToken, null);
            Assert.AreEqual(expectedFormat, format);
        }

        [TestMethod]
        public void FormatParameter_WithNameAndSingleShortNameToken_ShortNameIsFormatted()
        {
            // setup

            var formatter = new ParameterNameFormatter();
            var name = "name";
            var nameToken = new ParameterToken(ParameterTokenType.ShortName, 0, "a", 1, new ParameterToken(ParameterTokenType.NameGroup, 0, "a", 0, new ParameterToken(ParameterTokenType.RawArg, 0, "-a", 0, null)));
            var expectedFormat = $"name parameter (-a)";

            // test

            var format = formatter.FormatParameter(nameToken, name);
            Assert.AreEqual(expectedFormat, format);
        }

        [TestMethod]
        public void FormatParameter_WithNameAndWithShortNameGroupToken_ShortNameIsFormatted()
        {
            // setup

            var formatter = new ParameterNameFormatter();
            var name = "name";
            var nameToken = new ParameterToken(ParameterTokenType.ShortName, 0, "a", 1, new ParameterToken(ParameterTokenType.NameGroup, 0, "abc", 0, new ParameterToken(ParameterTokenType.RawArg, 0, "-abc", 0, null)));
            var expectedFormat = $"name parameter ('a' at '-abc' parameter group)";

            // test

            var format = formatter.FormatParameter(nameToken, name);
            Assert.AreEqual(expectedFormat, format);
        }

        [TestMethod]
        public void FormatParameter_WithoutNameAndWithShortNameGroupToken_ShortNameIsFormatted()
        {
            // setup

            var formatter = new ParameterNameFormatter();
            var nameToken = new ParameterToken(ParameterTokenType.ShortName, 0, "a", 1, new ParameterToken(ParameterTokenType.NameGroup, 0, "abc", 0, new ParameterToken(ParameterTokenType.RawArg, 0, "-abc", 0, null)));
            var expectedFormat = $"'a' parameter (at '-abc' parameter group)";

            // test

            var format = formatter.FormatParameter(nameToken, null);
            Assert.AreEqual(expectedFormat, format);
        }



        [TestMethod]
        public void FormatSwitch_WithName_NameIsFormatted()
        {
            // setup

            var formatter = new ParameterNameFormatter();
            var name = "name";
            var nameToken = new ParameterToken(ParameterTokenType.Name, 0, "name", 2, new ParameterToken(ParameterTokenType.RawArg, 0, "--parameter", 0, null));
            var expectedFormat = $"--name switch";

            // test

            var format = formatter.FormatSwitch(nameToken, name);
            Assert.AreEqual(expectedFormat, format);
        }

        [TestMethod]
        public void FormatSwitch_WithoutNameAndWithNameToken_NameIsFormatted()
        {
            // setup

            var formatter = new ParameterNameFormatter();
            var nameToken = new ParameterToken(ParameterTokenType.Name, 0, "parameter", 2, new ParameterToken(ParameterTokenType.RawArg, 0, "--parameter", 0, null));
            var expectedFormat = $"--parameter switch";

            // test

            var format = formatter.FormatSwitch(nameToken, null);
            Assert.AreEqual(expectedFormat, format);
        }

        [TestMethod]
        public void FormatSwitch_WithoutNameAndWithSingleShortNameToken_ShortNameIsFormatted()
        {
            // setup

            var formatter = new ParameterNameFormatter();
            var nameToken = new ParameterToken(ParameterTokenType.ShortName, 0, "a", 1, new ParameterToken(ParameterTokenType.NameGroup, 0, "a", 0, new ParameterToken(ParameterTokenType.RawArg, 0, "-a", 0, null)));
            var expectedFormat = $"-a switch";

            // test

            var format = formatter.FormatSwitch(nameToken, null);
            Assert.AreEqual(expectedFormat, format);
        }

        [TestMethod]
        public void FormatSwitch_WithNameAndSingleShortNameToken_ShortNameIsFormatted()
        {
            // setup

            var formatter = new ParameterNameFormatter();
            var name = "name";
            var nameToken = new ParameterToken(ParameterTokenType.ShortName, 0, "a", 1, new ParameterToken(ParameterTokenType.NameGroup, 0, "a", 0, new ParameterToken(ParameterTokenType.RawArg, 0, "-a", 0, null)));
            var expectedFormat = $"name switch (-a)";

            // test

            var format = formatter.FormatSwitch(nameToken, name);
            Assert.AreEqual(expectedFormat, format);
        }

        [TestMethod]
        public void FormatSwitch_WithNameAndWithShortNameGroupToken_ShortNameIsFormatted()
        {
            // setup

            var formatter = new ParameterNameFormatter();
            var name = "name";
            var nameToken = new ParameterToken(ParameterTokenType.ShortName, 0, "a", 1, new ParameterToken(ParameterTokenType.NameGroup, 0, "abc", 0, new ParameterToken(ParameterTokenType.RawArg, 0, "-abc", 0, null)));
            var expectedFormat = $"name switch ('a' at '-abc' parameter group)";

            // test

            var format = formatter.FormatSwitch(nameToken, name);
            Assert.AreEqual(expectedFormat, format);
        }

        [TestMethod]
        public void FormatSwitch_WithoutNameAndWithShortNameGroupToken_ShortNameIsFormatted()
        {
            // setup

            var formatter = new ParameterNameFormatter();
            var nameToken = new ParameterToken(ParameterTokenType.ShortName, 0, "a", 1, new ParameterToken(ParameterTokenType.NameGroup, 0, "abc", 0, new ParameterToken(ParameterTokenType.RawArg, 0, "-abc", 0, null)));
            var expectedFormat = $"'a' switch (at '-abc' parameter group)";

            // test

            var format = formatter.FormatSwitch(nameToken, null);
            Assert.AreEqual(expectedFormat, format);
        }

    }
}
