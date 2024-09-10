using Core.Entities;
using Core.Models;

namespace Tests.TestsData;

public static class TestData
{
    public static class RoleNames
    {
        public const string Admin = "Admin";
        public const string Guest = "Guest";
    }

    public static readonly List<Permission> ValidPermissions = new()
    {
        new Permission(Permissions.ViewAccounts),
    };

    public static readonly List<Role> ValidAccountRoles = new()
    {
        new Role(BaseRoleNames.Admin,
                new List<Permission>
                {
                    new(Permissions.ViewAccounts),
                    new(Permissions.ViewRoles),
                }
            ),
        new Role(BaseRoleNames.Guest,
                new List<Permission>
                {
                    new(Permissions.GuestActionsAllowed),
                }
            ),
    };

    public static readonly List<Role> AllRoles = new()
    {
        new Role(1,
                RoleNames.Admin,
                new List<Permission>
                {
                    new(Permissions.ViewAccounts),
                    new(Permissions.ManageAccounts),
                    new(Permissions.ViewRoles),
                    new(Permissions.ManageRoles),
                    new(Permissions.CanManageTenants),
                    new(Permissions.IsTenantsHardDeleteAllowed),
                }
            ),
        new Role(2,
                RoleNames.Guest,
                new List<Permission>
                {
                    new(Permissions.GuestActionsAllowed),
                }
            ),
    };
}