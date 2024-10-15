namespace SoftPatterns.Cli.Reader
{
    public interface IUsageStringBuilder
    {
        void AppendHeaderLine(string line);
        void AppendSection(string header);
        void AppendParameter(string header, params string[] lines);
        void AppendLine(string line);
        void AppendIndentedLine(string line);
    }

    public static class UsageStringBuilderExtensions
    {
        public static void AppendHeaderLine(this IUsageStringBuilder builder)
        {
            builder.AppendHeaderLine(String.Empty);
        }

        public static void AppendLine(this IUsageStringBuilder builder)
        {
            builder.AppendLine(String.Empty);
        }
    }

    public class UsageStringBuilder : IUsageStringBuilder
    {
        private readonly string indentation;
        private readonly int columnSize;
        private readonly List<string> headerLines;
        private readonly List<string> bodyLines;

        public UsageStringBuilder(int indentationSize = 4, int columnSize = 21)
        {
            this.indentation = new string(' ', indentationSize);
            this.columnSize = columnSize;
            this.headerLines = [];
            this.bodyLines = [];
        }

        public override string ToString()
        {
            return String.Join(Environment.NewLine, this.headerLines.Count > 0 && this.bodyLines.Count > 0 ?
                this.headerLines.Concat([String.Empty]).Concat(this.bodyLines) :
                this.headerLines.Concat(this.bodyLines));
        }

        public void AppendHeaderLine(string line)
        {
            this.headerLines.Add(line);
        }

        public void AppendSection(string header)
        {
            if (this.bodyLines.Count > 0)
            {
                this.bodyLines.Add(String.Empty);
            }

            this.bodyLines.Add(header);
        }

        public void AppendParameter(string header, params string[] lines)
        {
            if (lines.Length == 0)
            {
                this.bodyLines.Add(this.indentation + header);
                return;
            }

            var stringBuilder = new StringBuilder();
            stringBuilder.Append(this.indentation);
            stringBuilder.Append(header.PadRight(this.columnSize));

            if (header.Length > this.columnSize)
            {
                stringBuilder.AppendLine();
                stringBuilder.Append(this.indentation);
                stringBuilder.Append(new string(' ', this.columnSize));
            }

            stringBuilder.Append(' ');
            stringBuilder.Append(lines[0]);

            this.bodyLines.Add(stringBuilder.ToString());

            for (var i = 1; i < lines.Length; i++)
            {
                this.bodyLines.Add(lines[i].PadLeft(lines[i].Length + this.indentation.Length + this.columnSize + 1));
            }
        }

        public void AppendLine(string line)
        {
            this.bodyLines.Add(line);
        }

        public void AppendIndentedLine(string line)
        {
            this.bodyLines.Add(this.indentation + line);
        }

        public static UsageStringBuilder CreateCompact()
        {
            return new UsageStringBuilder(3, 9);
        }

        public static UsageStringBuilder CreateDos()
        {
            return new UsageStringBuilder(2, 11);
        }
    }
}
