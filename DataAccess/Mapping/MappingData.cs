using System;
using System.Collections.Generic;
using Core.Models;

namespace DataAccess.Mapping;

internal static class MappingData
{
    public const long AdminAccountId = 1L;
    public const long GuestAccountId = 2L;

    public const long AdminRoleId = 1L;
    public const long GuestRoleId = 2L;

    public static readonly List<Permission> AllPermissions = new()
    {
        new Permission(Permissions.ViewPersonalProfile),
        new Permission(Permissions.ViewContacts),
        new Permission(Permissions.ViewSalaryAndDocumentsData),
        new Permission(Permissions.EditFullEmployeesData),
        new Permission(Permissions.AccessAnalyticalForecastsPage),
        new Permission(Permissions.ViewAccounts),
        new Permission(Permissions.ManageAccounts),
        new Permission(Permissions.ViewRoles),
        new Permission(Permissions.ManageRoles),
        new Permission(Permissions.CanRequestCompensations),
        new Permission(Permissions.CanManageCompensations),
        new Permission(Permissions.CanManageDocuments),
        new Permission(Permissions.CanManageTenants),
        new Permission(Permissions.IsTenantsHardDeleteAllowed),
    };
    public static readonly List<Permission> NoPermissions = new()
    {
        new Permission(Permissions.GuestActionsAllowed),
    };

    public static readonly DateTime AccountsCreatedAtUtc = DateTime.SpecifyKind(new DateTime(2020,
                    01,
                    01,
                    0,
                    0,
                    0
                ),
            DateTimeKind.Utc
        );
}