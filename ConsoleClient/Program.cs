using System.Net.Http.Headers;
using System.Net.Http.Json;

var auth = "https://localhost:7254"; // AuthServer
var api = "https://localhost:7207"; // ResourceApi

var handler = new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
};
using var client = new HttpClient(handler);

// 1) Get token
var login = new
{
    username = "alice", 
    password = "123"
};
var tokResp = await client.PostAsJsonAsync($"{auth}/token", login);
tokResp.EnsureSuccessStatusCode();
var tok = await tokResp.Content.ReadFromJsonAsync<TokenResponse>();

Console.WriteLine(tok!.access_token[..40] + "...");

// 2) Use token to call ResourceApi
client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tok.access_token);
var secret = await client.GetStringAsync($"{api}/secret");
Console.WriteLine(secret);
Console.ReadLine();

public class TokenResponse
{
    public string access_token { get; set; } = "";
    public string token_type { get; set; } = "";
}