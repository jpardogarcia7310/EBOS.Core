using EBOS.Core.Primitives;

namespace EBOS.Core;

public class OperationResult<TResult>
{
    public TResult Result { get; set; } = default!;

    public ICollection<ErrorResult> Errors { get; } = [];

    public bool HasErrors => Errors.Any();
}