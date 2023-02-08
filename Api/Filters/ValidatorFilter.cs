using FluentValidation;

namespace Api.Filters;

public class ValidatorFilter<T> : IEndpointFilter
{
    private readonly IValidator<T> _validator;

    public ValidatorFilter(IValidator<T> validator)
    {
        _validator = validator;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var validatable = context.Arguments.SingleOrDefault(x => x is T);

        if (validatable != null)
        {
            var result = await _validator.ValidateAsync((T)validatable);
            if (!result.IsValid)
            {
                return Results.BadRequest(result.Errors);
            }
        }
        else
        {
            throw new InvalidOperationException("No validatable argument found.");
        }

        return await next(context);
    }
}