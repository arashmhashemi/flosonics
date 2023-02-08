using FluentValidation;

using Api.Model.Session;
using Api.Repository.Session;

namespace Api.Validators;

// SessionValidator is used to validate the SessionDto.
// This class is a singleton for performance reasons, it is stateless and can be used in multiple threads.
public class SessionValidator : AbstractValidator<SessionRequest>
{
    public SessionValidator(ISessionRepository sessionRepository)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(1)
            .MaximumLength(50);

        RuleFor(x => x.DurationSeconds)
            .InclusiveBetween(1, 3600);

        RuleFor(x => x.Tags)
            .ForEach(x => x.MinimumLength(1).MaximumLength(50))
            .Must(x => x.Count() == x.Distinct(StringComparer.OrdinalIgnoreCase).Count())
            .WithMessage("Tags must be unique");

        RuleFor(x => x.Name)
            .MustAsync(async (name, _) =>
            {
                var session = await sessionRepository.GetByNameAsync(name);
                return session == null;
            })
            .WithMessage(x => $"Session with name '{x.Name}' already exists")
            .WithErrorCode("SessionNameAlreadyExists");
    }
}
