using MediatR;

namespace Banking.Account.Command.Application.Features.BankAccounts.Commands.CloseAccound
{
    public class CloseAccountCommand : IRequest<bool>
    {
        public string Id { get; set; } = string.Empty;
    }
}
