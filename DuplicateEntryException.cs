namespace Aarati_s_Journal.Exceptions;

public class DuplicateEntryException : Exception
{
    public DuplicateEntryException(string message) : base(message) { }
}
