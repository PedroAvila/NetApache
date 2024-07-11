using Banking.Account.Command.Application.Features.BankAccounts.Commands.OpenAccount;
using Banking.Cqrs.Core.Domain;
using Banking.Cqrs.Core.Events;

namespace Banking.Account.Command.Application.Aggregates
{
    public class AccountAggregate : AggregateRoot
    {
        public bool Active { get; set; }
        public double Balance { get; set; }

        public AccountAggregate()
        {
            
        }

        public AccountAggregate(OpenAccountCommand command)
        {
            var accountOpenedEvent = new AccountOpenedEvent(command.Id, command.AccountHolder, command.AccountType, DateTime.Now, command.OpeningBalance);

            RaiseEvent(accountOpenedEvent);
        }

        public void Apply(AccountOpenedEvent accountOpenedEvent)
        {
            Id = accountOpenedEvent.Id;
            Active = true;
            Balance = accountOpenedEvent.OpeningBalance;
        }

        public void DepositFunds(double amount)
        {
            if (!Active)
                throw new Exception("Los fondos no pueden ser depositados en una cuenta cancelada");

            if (amount <= 0) throw new Exception("El deposito de dinero tiene que ser mayor a cero");

            var fundsDepositEvent = new FundsDepositedEvent(Id)
            {
                Id = Id,
                Amount = amount,
            };

            RaiseEvent(fundsDepositEvent);
        }

        public void Apply(FundsDepositedEvent fundsDepositEvent)
        {
            Id = fundsDepositEvent.Id;
            Balance += fundsDepositEvent.Amount;
        }
    }
}
