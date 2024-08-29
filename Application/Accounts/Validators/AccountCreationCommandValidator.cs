using Application.Accounts.Commands;
using Application.Options;
using Core.Contracts;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Application.Accounts.Validators;

public class AccountCreationCommandValidator : AbstractValidator<AccountCreationCommand>
{
    private readonly AccountValidationOptions _accountValidOptions;

    public AccountCreationCommandValidator(IRolesRepository rolesRepository, IAccountsRepository accountsRepository, IOptions<AccountValidationOptions> accountValidOptions)
    {
        _accountValidOptions = accountValidOptions.Value;

        RuleFor(x => x.RoleIds).RolesMustBeValid(rolesRepository);
    }

    private bool IsCorporateEmail(string corporateEmail)
    {
        return corporateEmail.Contains(_accountValidOptions.CorporateEmailDomain);
    }
}