namespace Textile.Core.Managers.Common.Exceptions
{
    public class DuplicateEntityException : Exception
    {
        public DuplicateEntityException(string message)
            : base(message) { }
    }
}
