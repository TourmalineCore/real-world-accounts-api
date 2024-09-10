using System.Threading.Tasks;

namespace Application.HttpClients;

public interface IHttpClient
{
    Task SendRequestToRegisterNewAccountAsync(long accountId, string login, string token);

    Task SendRequestToBlockUserAsync(long accountId);

    Task SendRequestToUnblockUserAsync(long accountId);
}