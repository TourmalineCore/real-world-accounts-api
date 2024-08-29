using System;
using System.Collections.Generic;
using System.Linq;
using Application.Roles;
using Core.Entities;

namespace Application.Accounts;

public readonly struct AccountDto
{
    public AccountDto(Account account, string callerCorporateEmail)
    {
        Id = account.Id;
        Login = account.Login;
        IsBlocked = account.IsBlocked;
        Roles = account.AccountRoles.Select(x => new RoleDto(x.Role));
        TenantId = account.TenantId;
        TenantName = account.Tenant.Name;

        // Refactor: use Instant in controllers instead of date time
        CreationDate = account.CreatedAt.ToDateTimeUtc();
        CanChangeAccountState = account.Login != callerCorporateEmail && !account.IsAdmin;
    }

    public long Id { get; }

    public string Login { get; }

    public DateTime CreationDate { get; }

    public bool IsBlocked { get; }

    public long TenantId { get; }

    public string TenantName { get; }

    public IEnumerable<RoleDto> Roles { get; }

    public bool CanChangeAccountState { get; }
}