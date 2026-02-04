using FluentValidation;

namespace DoctorCalendar.Application.Commands.UpdateEvent;

public sealed class UpdateEventCommandValidator : AbstractValidator<UpdateEventCommand>
{
    public UpdateEventCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(2000);

        RuleFor(x => x.Version)
            .NotEmpty();

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
