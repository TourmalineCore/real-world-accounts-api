using System.Security;
using Core.Models;

namespace Tests.Roles;

public class RolePermissionsValidatorTests
{
    [Fact]
    public void ValidatePermissions_AreEmpty_GetValidationErrors()
    {
        var exception = Assert.Throws<ArgumentException>(() => RolePermissionsValidator.ValidatePermissions(new string[] { }));
        Assert.Equal("Permissions can't be empty", exception.Message);

    }

    [Fact]
    public void ValidatePermissionsWithoutDependencies_NoValidationErrors()
    {
        var permissionsWithoutDependencies = new List<string>
        {
            Permissions.ViewAccounts,
            Permissions.ViewRoles,
        };

        var exception = Record.Exception(() => RolePermissionsValidator.ValidatePermissions(permissionsWithoutDependencies));
        Assert.Null(exception);
    }

    [Fact]
    public void ValidateAllPermissions_NoValidationErrors()
    {
        var allPermissions = new List<string>
        {
            Permissions.ViewAccounts,
            Permissions.ManageAccounts,
            Permissions.ViewRoles,
            Permissions.ManageRoles,
        };

        var exception = Record.Exception(() => RolePermissionsValidator.ValidatePermissions(allPermissions));
        Assert.Null(exception);
    }

    [Theory]
    [InlineData(Permissions.ManageAccounts, new[] { Permissions.ViewAccounts })]
    [InlineData(Permissions.ManageRoles, new[] { Permissions.ViewRoles })]
    public void ValidatePermissionsWithDependencies_DependenciesAreIncorrect_GetValidationErrors(string permission, string[] expectedPermissionDependencies)
    {
        var exception = Assert.Throws<ArgumentException>(() => RolePermissionsValidator.ValidatePermissions(new[] { permission }));
        Assert.Equal($"Permission '{permission}' requires permissions [{string.Join(',', expectedPermissionDependencies)}]", exception.Message);
    }

    [Theory]
    [InlineData(Permissions.ManageAccounts, Permissions.ViewAccounts)]
    [InlineData(Permissions.ManageRoles, Permissions.ViewRoles)]
    public void ValidatePermissionsWithDependencies_DependenciesAreCorrect_NoValidationErrors(params string[] permissions)
    {
        var exception = Record.Exception(() => RolePermissionsValidator.ValidatePermissions(permissions));
        Assert.Null(exception);
    }
}