using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Application.HttpClients;

public class AccountHttpClient : IHttpClient
{
    private readonly HttpClient _client;
    private readonly HttpUrls _urls;

    public AccountHttpClient(IOptions<HttpUrls> urls)
    {
        _client = new HttpClient();
        _urls = urls.Value;
    }

    public async Task SendRequestToRegisterNewAccountAsync(long accountId, string corporateEmail, string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        await _client.PostAsJsonAsync($"{_urls.AuthServiceUrl}/api/auth/register",
                new
                {
                    AccountId = accountId,
                    CorporateEmail = corporateEmail,
                }
            );
    }

    public async Task SendRequestToBlockUserAsync(long accountId)
    {
        await _client.PostAsJsonAsync($"{_urls.AuthServiceUrl}/internal/block-user",
                new
                {
                    AccountId = accountId,
                }
            );
    }

    public async Task SendRequestToUnblockUserAsync(long accountId)
    {
        await _client.PostAsJsonAsync($"{_urls.AuthServiceUrl}/internal/unblock-user",
                new
                {
                    AccountId = accountId,
                }
            );
    }
}