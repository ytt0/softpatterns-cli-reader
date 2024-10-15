namespace Notes
{
    public interface IRepository
    {
        string GenerateNoteId();
        bool NoteExists(string id);
        void AddNote(INote note);
        INote ReadNote(string id);
        void RemoveNote(string id);
        IEnumerable<string> GetNoteIds();
        string GetNotePath(string id);
    }

    public partial class Repository : IRepository
    {
        private const string TimeFormat = "yyyy-MM-dd HH:mm";

        private readonly string repositoryPath;
        private readonly Random random;
        private bool isRepositoryPathValid;

        public Repository(string repositoryPath)
        {
            this.repositoryPath = repositoryPath;
            this.random = new Random();
        }

        public string GenerateNoteId()
        {
            for (var i = 0; i < 3; i++)
            {
                var id = this.random.Next(0x10000).ToString("x4");
                var notePath = GetNotePath(id);

                if (!File.Exists(notePath))
                {
                    return id;
                }
            }

            throw new Exception("Failed to generate a new note id");
        }

        public bool NoteExists(string id)
        {
            return Path.Exists(GetNotePath(id));
        }

        public void AddNote(INote note)
        {
            if (!this.isRepositoryPathValid)
            {
                Directory.CreateDirectory(this.repositoryPath);
                this.isRepositoryPathValid = true;
            }

            var notePath = GetNotePath(note.Id);
            if (File.Exists(notePath))
            {
                throw new Exception($"Failed to add a new note, a note already exists at \"{notePath}\"");
            }

            var content = new StringBuilder();
            content.Append(note.Content);

            if (note.Time != null || note.Category != null || note.Tags?.Any() == true)
            {
                content.AppendLine();

                if (note.Time != null)
                {
                    content.AppendLine();
                    content.Append("# time: ");
                    content.Append(note.Time.Value.ToString(TimeFormat));
                }

                if (note.Category != null)
                {
                    content.AppendLine();
                    content.Append("# category: ");
                    content.Append(note.Category);
                }

                if (note.Tags?.Any() == true)
                {
                    content.AppendLine();
                    content.Append("# tags: ");
                    content.Append(String.Join(", ", note.Tags));
                }
            }

            File.WriteAllText(notePath, content.ToString());
        }

        public INote ReadNote(string id)
        {
            var notePath = GetNotePath(id);

            if (!File.Exists(notePath))
            {
                throw new Exception($"Note does not exist at \"{notePath}\"");
            }

            var lines = File.ReadAllLines(notePath);

            var content = String.Join(Environment.NewLine, lines.Where(line => !line.StartsWith("#")));

            var metadata = lines.Select(line => KeyValueRegex().Match(line)).Where(match => match.Success).ToDictionary(match => match.Groups["key"].Value, match => match.Groups["value"].Value);


            var time = metadata.TryGetValue("time", out var value) && DateTime.TryParseExact(value, TimeFormat, null, DateTimeStyles.AssumeLocal, out var timeValue) ? (DateTime?)timeValue : null;
            var tags = metadata.TryGetValue("tags", out value) ? value.Split(',').Select(value => value.Trim()).ToArray() : null;
            var category = metadata.TryGetValue("category", out value) ? value : null;

            return new Note(id, content, time, category, tags);
        }

        public void RemoveNote(string id)
        {
            File.Delete(GetNotePath(id));
        }

        public IEnumerable<string> GetNoteIds()
        {
            if (!Directory.Exists(this.repositoryPath))
            {
                return [];
            }

            return Directory.GetFiles(this.repositoryPath).Select(file => Path.GetFileName(file)!).Where(id => id.Length == 4).ToArray();
        }

        public string GetNotePath(string id)
        {
            return Path.Combine(this.repositoryPath, id);
        }

        [GeneratedRegex("^#\\s*(?<key>\\w+)\\s*:\\s*(?<value>.*)$")]
        private static partial Regex KeyValueRegex();
    }
}
