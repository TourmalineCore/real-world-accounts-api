using System.Threading.Tasks;
using Core.Entities;

namespace Core.Contracts;

public interface IAccountsRepository : IRepository<Account>
{
    public Task<Account?> FindByLoginAsync(string login);
}