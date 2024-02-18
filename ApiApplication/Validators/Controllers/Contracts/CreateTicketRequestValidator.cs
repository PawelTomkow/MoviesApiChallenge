using ApiApplication.Controllers.Contracts.Tickets;
using FluentValidation;

namespace ApiApplication.Validators.Controllers.Contracts
{
    public class CreateTicketRequestValidator : AbstractValidator<CreateTicketRequest>
    {
        public CreateTicketRequestValidator()
        {
            RuleFor(x => x.ReservationId).NotEmpty();
        }
    }
}