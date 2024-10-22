using System;
using System.Diagnostics.Eventing.Reader;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IndoorCO2App_Android;
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
        public static async Task SendJsonToApiGateway(string json, SubmissionMode submissionMode)
        {
            var successState = string.Empty;

            try
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response;
                if (submissionMode == SubmissionMode.Building)
                {
                    response = await client.PostAsync("https://wzugdkxj15.execute-api.eu-central-1.amazonaws.com/Standard/CO2", content);
                }
                else if (submissionMode == SubmissionMode.BuildingManual)
                {
                    response = await client.PostAsync("https://40zfjhm5tg.execute-api.eu-central-1.amazonaws.com/SendManualCO2Data", content);
                }
                //else if (submissionMode == SubmissionMode.Transit)
                //{
                //    //response = await client.PostAsync("TODO");
                //}
                else return;


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
