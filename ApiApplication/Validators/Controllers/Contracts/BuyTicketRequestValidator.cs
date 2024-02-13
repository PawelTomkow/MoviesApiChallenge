using ApiApplication.Controllers.Contracts.Tickets;
using FluentValidation;

namespace ApiApplication.Validators.Controllers.Contracts
{
    public class BuyTicketRequestValidator : AbstractValidator<BuyTicketRequest>
    {
        public BuyTicketRequestValidator()
        {
            RuleFor(x => x.ReservationId).NotEmpty();
        }
    }
}