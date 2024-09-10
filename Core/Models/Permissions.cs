using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Core.Models;

public static class Permissions
{
    public const string ViewAccounts = "ViewAccounts";
    public const string ManageAccounts = "ManageAccounts";
    public const string ViewRoles = "ViewRoles";
    public const string ManageRoles = "ManageRoles";
    public const string CanManageTenants = "CanManageTenants";
    public const string IsTenantsHardDeleteAllowed = "IsTenantsHardDeleteAllowed";
    public const string GuestActionsAllowed = "GuestActionsAllowed";

    public static bool IsPermissionExists(string permissionName)
    {
        var permissionNames = GetPermissionNames();
        return permissionNames.Contains(permissionName);
    }

    private static IEnumerable<string?> GetPermissionNames()
    {
        return typeof(Permissions)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Select(x => x.GetValue(null)?.ToString());
    }
}