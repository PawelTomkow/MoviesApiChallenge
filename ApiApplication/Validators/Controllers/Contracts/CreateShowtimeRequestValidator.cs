using ApiApplication.Controllers.Contracts.Showtimes;
using FluentValidation;

namespace ApiApplication.Validators.Controllers.Contracts
{
    public class CreateShowtimeRequestValidator : AbstractValidator<CreateShowtimeRequest>
    {
        public CreateShowtimeRequestValidator()
        {
            RuleFor(x => x.AuditoriumId)
                .Must(x => x > 0)
                .WithMessage("AuditoriumId must be greater then 0.");

            RuleFor(x => x.SessionDate).NotNull();

            RuleFor(x => x.ImdbMovieId)
                .NotEmpty()
                .WithMessage("MovieId can not be empty.");
        }
    }
}