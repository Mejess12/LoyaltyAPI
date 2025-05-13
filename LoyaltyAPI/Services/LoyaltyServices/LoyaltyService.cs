using BCrypt.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;

public class LoyaltyService
{
    private static readonly HttpClient client = new HttpClient();

    public async Task<bool> ValidateBcryptHash(string userToken, string username, string password)
    {
        try
        {
            var loginPayload = new
            {
                Username = username,
                Password = password,
            };

            var response = await client.PostAsJsonAsync("http://192.168.254.126:8080/client/authv2", loginPayload);

            if (!response.IsSuccessStatusCode)
            {
                Console.Error.WriteLine("Failed to authenticate. HTTP status code: " + response.StatusCode);
                return false;
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var member = JsonConvert.DeserializeObject<MemberResponse>(jsonResponse);

            if (member == null)
            {
                Console.Error.WriteLine("Invalid member response received.");
                return false;
            }


            if (BCrypt.Net.BCrypt.Verify(password, member.UserToken))
            {
                return true;
            }
            else
            {
                Console.Error.WriteLine("Password verification failed.");
                return false;
            }
        }
        catch (HttpRequestException e)
        {
            Console.Error.WriteLine($"Request error: {e.Message}");
        }
        catch (JsonException e)
        {
            Console.Error.WriteLine($"JSON parsing error: {e.Message}");
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"An unexpected error occurred: {e.Message}");
        }

        return false;
    }
}
