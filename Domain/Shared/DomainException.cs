namespace Domain.Shared;

public class DomainException : Exception
{
    public string[] Fields { get; }

    public DomainException(string field, string message)
        : base(message)
    {
        Fields = new string[] { field };
    }

    public DomainException(string[] fields, string message)
        : base(message)
    {
        Fields = fields;
    }

    public DomainException(string message)
        : base(message)
    {
        Fields = new string[] { "other" };
    }
}
