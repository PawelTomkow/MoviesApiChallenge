using ApiApplication.Controllers.Contracts.Reservations;
using FluentValidation;

namespace ApiApplication.Validators.Controllers.Contracts
{
    public class CreateReservationRequestValidator : AbstractValidator<CreateReservationRequest>
    {
        public CreateReservationRequestValidator()
        {
            RuleFor(x => x.ShowtimeId)
                .NotEmpty()
                .WithMessage("ShowtimeId can not be empty.");
            RuleFor(x => x.Seats)
                .Must(list => list != null && list.Count > 0)
                .WithMessage("You need reserve minimum 1 seat.");
            RuleFor(x => x.AuditoriumId)
                .NotEmpty()
                .WithMessage("AuditoriumId can not be empty.");
        }
    }
}