namespace SoftPatterns.Cli.Reader.Tests
{
    [TestClass]
    public class UsageStringBuilderTests
    {
        [TestMethod]
        public void AppendHeaderLine_WithoutBody_HeaderIsAdded()
        {
            // setup

            var expectedString = JoinLines(
                "header line1",
                "header line2");

            var builder = new UsageStringBuilder();

            // test

            builder.AppendHeaderLine("header line1"); // add header
            builder.AppendHeaderLine("header line2");

            Assert.AreEqual(expectedString, builder.ToString()); // no empty line was added
        }

        [TestMethod]
        public void AppendHeaderLine_WithBody_HeaderIsSeparatedFromBody()
        {
            // setup

            var expectedString = JoinLines(
                "header line",
                "", // expected empty line
                "body line");

            var builder = new UsageStringBuilder();
            builder.AppendLine("body line"); // add body

            // test

            builder.AppendHeaderLine("header line"); // add header

            Assert.AreEqual(expectedString, builder.ToString()); // empty line was added between header and body
        }

        [TestMethod]
        public void AppendSection_WithoutBody_SectionIsAdded()
        {
            // setup

            var expectedString = "section header";

            var builder = new UsageStringBuilder();

            // test

            builder.AppendSection("section header"); // add section

            Assert.AreEqual(expectedString, builder.ToString()); // section was added without a leading empty line
        }

        [TestMethod]
        public void AppendSection_WithBody_AnEmptyLineIsAddedBeforeSection()
        {
            // setup

            var expectedString = JoinLines(
                "body line",
                "", // expected empty line
                "section header");

            var builder = new UsageStringBuilder();
            builder.AppendLine("body line"); // add body

            // test

            builder.AppendSection("section header"); // add section

            Assert.AreEqual(expectedString, builder.ToString()); // section was added without a leading empty line
        }

        [TestMethod]
        public void AppendParameter_WithoutLines_HeaderIsAddedWithoutTrailingSpaces()
        {
            // setup

            const string indentation = " ";
            const int columnSize = 2;

            var expectedString = indentation + "header";

            var builder = new UsageStringBuilder(indentation.Length, columnSize);

            // test

            builder.AppendParameter("header"); // add parameter

            Assert.AreEqual(expectedString, builder.ToString()); // parameter header was added without a trailing spaces
        }

        [TestMethod]
        public void AppendParameter_WithoutHeaderOverflow_HeaderAndFirstLineAreAddedAsASingleLine()
        {
            // setup

            const string header = "header";
            const string line = "line";

            const string indentation = " ";
            var columnSize = header.Length; // header would not overflow

            var expectedString = indentation + header + " " + line;

            var builder = new UsageStringBuilder(indentation.Length, columnSize);

            // test

            builder.AppendParameter(header, line); // add parameter

            Assert.AreEqual(expectedString, builder.ToString()); // parameter header and line were added as a single line
        }

        [TestMethod]
        public void AppendParameter_WithMultipleLines_LinesAreIndentedCorrectly()
        {
            // setup

            const string header = "header";
            const string line1 = "line1";
            const string line2 = "line2";

            const string indentation = " ";
            var columnSize = header.Length; // header would not overflow

            var expectedString = JoinLines(indentation + header + " " + line1, indentation + new string(' ', columnSize + 1) + line2);

            var builder = new UsageStringBuilder(indentation.Length, columnSize);

            // test

            builder.AppendParameter(header, line1, line2); // add two lines

            Assert.AreEqual(expectedString, builder.ToString()); // lines were added
        }

        [TestMethod]
        public void AppendParameter_WithHeaderOverflow_FirstLineIsAddedInSeparateLine()
        {
            // setup

            const string header = "header";
            const string line = "line";

            const string indentation = " ";
            var columnSize = header.Length - 1; // header would overflow

            var expectedString = JoinLines(indentation + header, indentation + new string(' ', columnSize + 1) + line);

            var builder = new UsageStringBuilder(indentation.Length, columnSize);

            // test

            builder.AppendParameter(header, line); // add parameter

            Assert.AreEqual(expectedString, builder.ToString()); // a line break was added
        }

        [TestMethod]
        public void AppendLine_Completes_LineIsAppended()
        {
            // setup

            const string indentation = " ";
            const int columnSize = 2;

            const string line = "line";

            var expectedString = line;

            var builder = new UsageStringBuilder(indentation.Length, columnSize);

            // test

            builder.AppendLine(line); // append line

            Assert.AreEqual(expectedString, builder.ToString()); // line was appended
        }

        [TestMethod]
        public void AppendIndentedLine_Completes_LineIsAppendedWithIndentation()
        {
            // setup

            const string indentation = " ";
            const int columnSize = 2;

            const string line = "line";

            var expectedString = indentation + line;

            var builder = new UsageStringBuilder(indentation.Length, columnSize);

            // test

            builder.AppendIndentedLine(line); // append line

            Assert.AreEqual(expectedString, builder.ToString()); // line was appended with indentation
        }

        private static string JoinLines(params string[] lines)
        {
            return String.Join(Environment.NewLine, lines);
        }
    }
}
