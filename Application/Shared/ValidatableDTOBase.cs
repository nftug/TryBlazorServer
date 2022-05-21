using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.Shared;

public abstract class ValidatableDTOBase : IValidatableObject
{
    protected abstract void AssertCanCreate();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        DomainException? exception = null;

        try
        {
            AssertCanCreate();
        }
        catch (DomainException e)
        {
            exception = e;
        }

        if (exception != null)
        {
            yield return new ValidationResult(exception.Message, exception.Fields);
        }
    }
}
