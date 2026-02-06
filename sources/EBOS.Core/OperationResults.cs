using EBOS.Core.Primitives;
using EBOS.Core.Primitives.Interfaces;
using System.Collections.ObjectModel;

namespace EBOS.Core;

/// <summary>
/// Operation result that can contain a value and an error collection.
/// Exposes errors as IReadOnlyCollection to prevent external modifications.
/// </summary>
public class OperationResult<TResult>
{
    private readonly List<IErrorResult> _errors;

    public OperationResult()
    {
        _errors = [];
    }

    public OperationResult(TResult result) : this()
    {
        Result = result;
    }

    public TResult Result { get; set; } = default!;
    public IReadOnlyCollection<IErrorResult> Errors => _errors.AsReadOnly();
    public bool HasErrors => _errors.Count > 0;

    public void AddError(IErrorResult error)
    {
        if (error is null) throw new ArgumentNullException(nameof(error));
        _errors.Add(error);
    }

    public void AddError(string message, int code = 0, string? exceptionMsg = null)
    {
        if (string.IsNullOrWhiteSpace(message)) 
            throw new ArgumentException("Message cannot be null or whitespace.", nameof(message));
        _errors.Add(new ErrorResult(message, code, exceptionMsg));
    }

    public void AddErrors(IEnumerable<IErrorResult> errors)
    {
        if (errors is null) throw new ArgumentNullException(nameof(errors));
        _errors.AddRange(errors);
    }

    public void AddException(Exception ex, int code = -1)
    {
        if (ex is null) 
            throw new ArgumentNullException(nameof(ex));
        _errors.Add(new ErrorResult(ex.Message, code, ex.ToString()));
    }

    public void ClearErrors() => _errors.Clear();

    public Collection<IErrorResult> ToCollection() => new(_errors.ToList());

    public ReadOnlyCollection<IErrorResult> ToReadOnlyCollection() => _errors.AsReadOnly();

    public KeyedErrors ToKeyedCollection()
    {
        var keyed = new KeyedErrors();
        foreach (var err in _errors)
        {
            if (keyed.ContainsKey(err.Code))
                keyed.Remove(err.Code);
            keyed.Add(err);
        }
        return keyed;
    }
}
