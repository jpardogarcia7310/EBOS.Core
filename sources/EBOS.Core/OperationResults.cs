namespace EBOS.Core;

public class OperationResult<TResult>
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public OperationResult()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        Errors = [];
    }
    public TResult Result { get; set; }
    public ICollection<ErrorResult> Errors { get; }
    public bool HasErrors => Errors != null && Errors.Any();

}
