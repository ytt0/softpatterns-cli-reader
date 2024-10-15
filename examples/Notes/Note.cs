namespace Notes
{
    public interface INote
    {
        string Id { get; }
        string Content { get; }
        DateTime? Time { get; }
        string? Category { get; }
        IEnumerable<string>? Tags { get; }
    }

    public record Note(string Id, string Content, DateTime? Time, string? Category, IEnumerable<string>? Tags) : INote;

    public interface INoteFactory
    {
        INote CreateNote(string content, DateTime time, string? category, IEnumerable<string>? tags);
    }

    public class NoteFactory : INoteFactory
    {
        private readonly IRepository repository;

        public NoteFactory(IRepository repository)
        {
            this.repository = repository;
        }

        public INote CreateNote(string content, DateTime time, string? category, IEnumerable<string>? tags)
        {
            var id = this.repository.GenerateNoteId();
            return new Note(id, content, time, category, tags);
        }
    }
}
