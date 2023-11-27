using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Sandbox.ModAPI;
using System.Text;
using SEChatGPT.Library;

public abstract class BaseHttpClient
{
    protected readonly HttpClient httpClient;

    protected BaseHttpClient()
    {
        httpClient = new HttpClient();
    }

    protected async Task<T> PostAsync<T>(string url, object requestBody)
    {
        try
        {
            var jsonContent = JsonConvert.SerializeObject(requestBody);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var httpResponse = await httpClient.PostAsync(url, httpContent).ConfigureAwait(false);
            var jsonResponse = await httpResponse.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(jsonResponse);
        }
        catch (HttpRequestException httpEx)
        {
            DisplayError($"An error occurred while sending an HTTP request. {httpEx.Message}");
            return default(T);
        }
        catch (JsonException jsonEx)
        {
            DisplayError($"An error occurred while processing the JSON response. {jsonEx.Message}");
            return default(T);
        }
    }

    protected void DisplayError(string message)
    {
        MyAPIGateway.Utilities.ShowMessage("SEChatGPT", $"Error: {message}");
    }

    // You can also implement common GET, PUT, DELETE etc. methods here if needed.
}