using System.ComponentModel.DataAnnotations.Schema;

namespace FluentExceptionResults.Results;

public class Error
{
    public string Code { get; set; } = default!;
    public string Message { get; set; } = default!;
    [NotMapped]
    public int HttpStatus { get; set; }


}