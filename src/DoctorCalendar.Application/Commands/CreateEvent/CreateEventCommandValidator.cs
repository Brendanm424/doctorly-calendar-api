using FluentValidation;

namespace DoctorCalendar.Application.Commands.CreateEvent;

public sealed class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
{
    public CreateEventCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(2000);

        RuleFor(x => x.StartUtc).NotEmpty();
        RuleFor(x => x.EndUtc).NotEmpty();

        RuleFor(x => x.Attendees)
            .NotNull();

        RuleForEach(x => x.Attendees).ChildRules(a =>
        {
            a.RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(120);

            a.RuleFor(x => x.Email)
                .NotEmpty()
                .MaximumLength(254);
        });
    }
}
