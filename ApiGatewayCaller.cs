using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace IndoorCO2App_Multiplatform
{
    public class ApiGatewayCaller
    {
        private static HttpClient client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(20)
        };

        // Modify sendJsonToApiGateway to accept callback
        public static async Task SendJsonToApiGateway(string json, bool manualMode)
        {
            var successState = string.Empty;

            try
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response;
                if (!manualMode)
                {
                    response = await client.PostAsync("https://wzugdkxj15.execute-api.eu-central-1.amazonaws.com/Standard/CO2", content);
                }
                else
                {
                    response = await client.PostAsync("https://40zfjhm5tg.execute-api.eu-central-1.amazonaws.com/SendManualCO2Data", content);
                }

                if (response.IsSuccessStatusCode)
                {
                    successState = "success";
                }
                else
                {
                    successState = "failure";
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                successState = "failure";
            }
            catch (TaskCanceledException e)
            {
                Console.WriteLine($"Request timeout: {e.Message}");
                successState = "timeout";
            }

            if (successState == "success")
            {
                MainPage.MainPageSingleton.OnTransmissionSuccess("success");
            }
            else if (successState == "failure")
            {
                MainPage.MainPageSingleton.OnTransmissionFailed("Transmission failed. Try again!!");
            }
            else
            {
                MainPage.MainPageSingleton.OnTransmissionFailed("No Response from server. Try again!");
            }

        }
    }
}
