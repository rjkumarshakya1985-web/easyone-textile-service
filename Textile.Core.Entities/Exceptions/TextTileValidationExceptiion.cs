namespace Textile.Core.Entities.Exceptions
{
    public class TextTileValidationException : Exception
    {
        public IReadOnlyDictionary<string, string[]> Errors { get; }

        public IReadOnlyDictionary<string, string[]> Warnings { get; }

        public TextTileValidationException(IReadOnlyDictionary<string, string[]> errors)
            : base("One or more validation failures have occurred.")
          => Errors = errors;

        public TextTileValidationException(IReadOnlyDictionary<string, string[]> errors,
            IReadOnlyDictionary<string, string[]> warnings)
            : base("One or more validation failures have occurred.")
        => (Errors, Warnings) = (errors, warnings);


    }

}
