namespace FluentExceptionResults.Exceptions;

public abstract class ErrorEnumeration 
{
    private static readonly Dictionary<int, ErrorEnumeration> _all = new();

    public string Name { get; }
    public int Id { get; }
    public string? Prefix { get;}

    protected ErrorEnumeration(int id, string name,string?prefix=default)
    {
        Id = id;
        Name = name;
        Prefix = prefix;
    }
    public static implicit operator string(ErrorEnumeration code) => $"{(string.IsNullOrWhiteSpace(code.Prefix) ? "ERR" : code.Prefix)}-{code.Id}";
    public override string ToString() => Name;
    public static IReadOnlyCollection<ErrorEnumeration> GetAll() => _all.Values.ToList().AsReadOnly();


}
