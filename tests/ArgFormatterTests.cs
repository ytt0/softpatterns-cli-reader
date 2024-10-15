namespace SoftPatterns.Cli.Reader.Tests
{
    [TestClass]
    public class ArgFormatterTests
    {
        public enum ArgsStyle { Posix, Dos }

        [TestMethod]
        [DataRow(ArgsStyle.Posix, "parameter", "--parameter", DisplayName = "Posix")]
        [DataRow(ArgsStyle.Dos, "parameter", "/parameter", DisplayName = "Dos")]
        public void FormatNameArg_WithName_NameIsFormattedCorrectly(ArgsStyle argsStyle, string name, string expectedFormat)
        {
            // setup

            var formatter = CreateArgFormatter(argsStyle);

            // test

            var format = formatter.FormatNameArg(name);

            Assert.AreEqual(expectedFormat, format);
        }

        [TestMethod]
        public void FormatNameGroupArg_WithGroup_GroupIsFormattedCorrectly()
        {
            // setup

            var formatter = ArgFormatter.Instance;
            const string group = "abc";
            const string expectedFormat = "-abc";

            // test

            var format = formatter.FormatNameGroupArg(group);

            Assert.AreEqual(expectedFormat, format);
        }

        private static IArgFormatter CreateArgFormatter(ArgsStyle argsStyle)
        {
            return argsStyle switch
            {
                ArgsStyle.Posix => ArgFormatter.Instance,
                ArgsStyle.Dos => DosArgFormatter.Instance,
                _ => throw new NotSupportedException($"{nameof(ArgsStyle)} value \"{argsStyle}\" is not expected"),
            };
        }
    }
}
