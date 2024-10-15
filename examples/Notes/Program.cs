namespace Notes
{
    public enum ProcessResult { Success = 0, Failure = 1, Usage = 2, UsageFailure = 3 }

    public class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                var reader = new ParametersReader(args);

                SubstituteLegacyGlobalParametersName(reader);

                // read global parameters
                var showHelp = reader.ReadSwitch("help", "?") || args.Length == 0;
                var repositoryPath = reader.ReadParameterValue("repository", "r", null) ?? ".notes";
                var configurationPath = reader.ReadParameterValue("configuration", "c", null) ?? ".notes\\config.json";
                var configuration = Configuration.Load(configurationPath);

                if (TrySubstituteCommandAlias(reader, configuration))
                {
                    // try read some of the global parameters again
                    repositoryPath = reader.ReadParameterValue("repository", "r", null) ?? repositoryPath;
                }

                var repository = new Repository(repositoryPath);

                SubstituteLegacyCommandNames(reader);

                // try read commands

                if (reader.TryReadValue("add"))
                {
                    return (int)AddNote(reader, repository, configuration, showHelp);
                }

                if (reader.TryReadValue("edit"))
                {
                    return (int)EditNote(reader, repository, configuration, showHelp);
                }

                if (reader.TryReadValue("remove"))
                {
                    return (int)RemoveNote(reader, repository, configuration, showHelp);
                }

                if (reader.TryReadValue("list"))
                {
                    return (int)ListNotes(reader, repository, configuration, showHelp);
                }

                if (showHelp)
                {
                    var usage = new UsageStringBuilder();
                    usage.AppendHeaderLine("Usage: note.exe <command> [<args>]");
                    usage.AppendParameter("add", "Add note");
                    usage.AppendParameter("edit", "Edit note");
                    usage.AppendParameter("remove", "Remove note");
                    usage.AppendParameter("list", "List notes");
                    usage.AppendSection("Global parameters");
                    usage.AppendParameter("-?, --help", "Help");
                    usage.AppendParameter("-r, --repository", "Repository path (default is .notes)");
                    usage.AppendParameter("-c, --configuration", "Configuration path (default is .notes\\config.json)");
                    usage.AppendSection("Examples");
                    usage.AppendIndentedLine("note.exe add \"note content\" --tags cs");
                    usage.AppendIndentedLine("note.exe edit 12ab");
                    usage.AppendIndentedLine("note.exe list --tags cs");

                    Console.WriteLine(usage);
                    return (int)ProcessResult.Usage;
                }

                // command value match failed
                reader.ThrowUnmatchedValueException("command");

                return default;
            }
            catch (UsageException e)
            {
                Console.Error.WriteLine($"Error: {e.Message}");
                return (int)ProcessResult.UsageFailure;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error: {e}");
                return (int)ProcessResult.Failure;
            }
        }

        private static ProcessResult AddNote(ParametersReader reader, Repository repository, Configuration configuration, bool showHelp)
        {
            // since the first value is not a subcommand name, help can be already shown here if requested
            if (showHelp)
            {
                var usage = new UsageStringBuilder();
                usage.AppendHeaderLine("Usage: note.exe add [--content] <content> [<args>]");
                usage.AppendParameter("--content", "Content");
                usage.AppendParameter("-t, --tags", "A list of comma separated tags");
                usage.AppendParameter("-g, --category", configuration.Categories.Any() ? $"One of: {String.Join(", ", configuration.Categories)}" : "Category name");
                usage.AppendSection("Examples");
                usage.AppendIndentedLine("note.exe add \"note content\" -t cs");

                Console.WriteLine(usage);
                return ProcessResult.Usage;
            }

            // read parameters

            var content = reader.ReadParameterValue("content", null, null) ?? reader.ReadValue(new ValueTypeDescription("content"));
            var tags = reader.ReadParameterValue("tags", "t", null)?.Split(',');

            string? category = null;

            if (reader.TryMatchParameter("category", "g", out _))
            {
                // try match parameter value with configured categories
                category = configuration.Categories.FirstOrDefault(value => reader.TryReadValue(value, true, StringComparison.CurrentCultureIgnoreCase));

                if (category == null)
                {
                    // none of the configured categories matched
                    reader.ThrowUnmatchedValueException(new ValueTypeName("category", "categories"));
                }

                // end parameter values read
                reader.MatchParameterEnd();
            }

            reader.ValidateEmpty();

            var factory = new NoteFactory(repository);
            var note = factory.CreateNote(content, DateTime.Now, category, tags);

            Console.WriteLine($"Adding note {note.Id}");
            repository.AddNote(note);

            return ProcessResult.Success;
        }

        private static ProcessResult EditNote(ParametersReader reader, Repository repository, Configuration configuration, bool showHelp)
        {
            if (showHelp)
            {
                var usage = new UsageStringBuilder();
                usage.AppendHeaderLine("Usage: note.exe edit [--id] <id> [<args>]");
                usage.AppendParameter("--id", "Note Id");
                usage.AppendSection("Examples");
                usage.AppendIndentedLine("note.exe edit 12ab");
                Console.WriteLine(usage);
                return ProcessResult.Usage;
            }

            var id = reader.ReadParameterValue("id", null, null) ?? reader.ReadValue(new ValueTypeDescription("note id"));

            reader.ValidateEmpty();

            if (!repository.NoteExists(id))
            {
                throw new UsageException($"Note \"{id}\" does not exist");
            }

            var notePath = repository.GetNotePath(id);
            Console.WriteLine($"Editing note at {notePath}");
            try
            {
                using (var process = Process.Start(new ProcessStartInfo(configuration.Editor, notePath))!)
                {
                    process.WaitForExit();
                }
            }
            catch
            {
                throw new UsageException($"Failed to start editor \"{configuration.Editor}\"");
            }

            return ProcessResult.Success;
        }

        private static ProcessResult RemoveNote(ParametersReader reader, Repository repository, Configuration configuration, bool showHelp)
        {
            if (showHelp)
            {
                var usage = new UsageStringBuilder();
                usage.AppendHeaderLine("Usage: note.exe remove [--id] <id> [<args>]");
                usage.AppendParameter("--id", "Note Id");
                usage.AppendParameter("-f, --force", "Force remove");
                usage.AppendSection("Examples");
                usage.AppendIndentedLine("note.exe remove 12ab -f");
                Console.WriteLine(usage);
                return ProcessResult.Usage;
            }

            var force = reader.ReadSwitch("force", "f");
            var id = reader.ReadParameterValue("id", null, null) ?? reader.ReadValue(new ValueTypeDescription("note id"));

            reader.ValidateEmpty();

            if (!repository.NoteExists(id))
            {
                throw new UsageException($"Note \"{id}\" does not exist");
            }

            if (force)
            {
                Console.WriteLine($"Removing note {id} at {repository.GetNotePath(id)}");
                repository.RemoveNote(id);
            }
            else
            {
                Console.WriteLine($"Would remove note {id} at {repository.GetNotePath(id)}");
            }

            return ProcessResult.Success;
        }

        private static ProcessResult ListNotes(ParametersReader reader, Repository repository, Configuration configuration, bool showHelp)
        {
            if (showHelp)
            {
                var usage = new UsageStringBuilder();
                usage.AppendHeaderLine("Usage: note.exe list [<args>]");
                usage.AppendParameter("-n, --count <number>", "Most recent notes count");
                usage.AppendSection("Examples");
                usage.AppendIndentedLine("note.exe list -n 10");
                Console.WriteLine(usage);
                return ProcessResult.Usage;
            }

            var count = reader.ReadParameterValue("count", "n", Int32.Parse, 1000);

            reader.ValidateEmpty();

            var ids = repository.GetNoteIds();

            if (!ids.Any())
            {
                Console.WriteLine("Notes repository is empty");
                return ProcessResult.Success;
            }

            var notes = ids.Select(repository.ReadNote).OrderByDescending(note => note.Time).Take(count).ToArray();

            foreach (var note in notes)
            {
                Console.WriteLine($"{note.Id} - {(note.Time?.ToString("yyyy-MM-dd HH:mm") ?? "??")} - {note.Content.TrimEnd([.. Environment.NewLine])}");
            }

            return ProcessResult.Success;
        }

        private static void SubstituteLegacyGlobalParametersName(ParametersReader reader)
        {
            reader.TrySubstituteParameter("help", "h", "help"); // replace "h" parameter short name, with "help" parameter name, this will make both "-h", and "-?" valid help switches
            reader.TrySubstituteParameter("settings", null, "configuration"); // replace old "settings" parameter name, with "configuration" parameter name
        }

        private static void SubstituteLegacyCommandNames(ParametersReader reader)
        {
            // command "delete" was renamed to "remove"
            reader.TrySubstituteValue("delete", "remove", false);
        }

        private static bool TrySubstituteCommandAlias(ParametersReader reader, Configuration configuration)
        {
            var isReplaced = false;

            if (configuration.Aliases != null)
            {
                foreach (var pair in configuration.Aliases)
                {
                    var alias = pair.Key;
                    var args = pair.Value.Split(' ');

                    if (reader.TrySubstituteValueArgs(alias, args))
                    {
                        isReplaced = true;
                    }
                }
            }

            return isReplaced;
        }
    }
}
