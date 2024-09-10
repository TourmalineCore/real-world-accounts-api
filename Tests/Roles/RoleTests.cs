using Core.Entities;
using Core.Exceptions;
using Core.Models;
using Moq;
using Tests.TestsData;

namespace Tests.Roles;

public class RoleTests
{
    [Fact]
    public void CannotSetAdminRole()
    {
        var role = new Role(BaseRoleNames.Guest, TestData.ValidPermissions);
        var exception = Assert.Throws<RoleOperationException>(() => role.Update(BaseRoleNames.Admin, TestData.ValidPermissions));
        Assert.Equal("Can't set admin role", exception.Message);
    }

    [Fact]
    public void CannotUpdateRoleWithIncorrectPermissions()
    {
        var incorrectPermissions = new List<Permission>
        {
            new(Permissions.ManageAccounts),
        };

        var role = new Role(BaseRoleNames.Guest, TestData.ValidPermissions);
        Assert.Throws<ArgumentException>(() => role.Update(BaseRoleNames.Guest, incorrectPermissions));
    }

    [Fact]
    public void CannotCreateRoleWithIncorrectPermissions()
    {
        var incorrectPermissions = new List<Permission>
        {
            new(Permissions.ManageAccounts),
        };

        Assert.Throws<ArgumentException>(() => new Role(BaseRoleNames.Guest, incorrectPermissions));
        Assert.Throws<ArgumentException>(() => new Role(It.IsAny<long>(), BaseRoleNames.Guest, incorrectPermissions));
    }

    [Fact]
    public void CanCreateRoleIfPermissionsAreCorrect()
    {
        var incorrectPermissions = new List<Permission>
        {
            new(Permissions.ViewAccounts),
        };

        Assert.Null(Record.Exception(() => new Role(BaseRoleNames.Admin, incorrectPermissions)));
        Assert.Null(Record.Exception(() => new Role(It.IsAny<long>(), BaseRoleNames.Admin, incorrectPermissions)));
    }

    [Fact]
    public void CannotCreateRoleIfNameIsEmptyOrWhitespaces()
    {
        Assert.Throws<ArgumentException>(() => new Role(string.Empty,
                    new List<Permission>
                    {
                        new(Permissions.ViewAccounts),
                    }
                )
            );

        Assert.Throws<ArgumentException>(() => new Role("  ",
                    new List<Permission>
                    {
                        new(Permissions.ViewAccounts),
                    }
                )
            );

        Assert.Throws<ArgumentException>(() => new Role(string.Empty, TestData.ValidPermissions));
        Assert.Throws<ArgumentException>(() => new Role(It.IsAny<long>(), string.Empty, TestData.ValidPermissions));
    }

    [Fact]
    public void CannotUpdateRoleIfNewNameIsEmptyOrWhitespaces()
    {
        var role = new Role(BaseRoleNames.Guest,
                new List<Permission>
                {
                    new(Permissions.GuestActionsAllowed),
                }
            );

        Assert.Throws<ArgumentException>(() => role.Update(string.Empty, TestData.ValidPermissions));
        Assert.Throws<ArgumentException>(() => role.Update("  ", TestData.ValidPermissions));
    }

    [Fact]
    public void CannotCreateRoleIfEmptyPermissions()
    {
        Assert.Throws<ArgumentException>(() => new Role(BaseRoleNames.Admin, new List<Permission>()));
    }

    [Fact]
    public void CannotUpdateRoleIfNewPermissionsAreEmpty()
    {
        var role = new Role(BaseRoleNames.Guest,
                new List<Permission>
                {
                    new(Permissions.GuestActionsAllowed),
                }
            );

        Assert.Throws<ArgumentException>(() => role.Update(BaseRoleNames.Guest, new List<Permission>()));
    }
}