

using IndoorCO2App;
using Newtonsoft.Json;
using System.Text;

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private async void OnDeletePreviousClicked(object sender, EventArgs e)
        {
            //make button non-interactable and change button text to "fetching last submission"
            if (BluetoothManager.deviceID.Length == 0)
            {
                //button should actually be disabled then anyways but okay
                return;
            }
            _DeleteLastSubmissionButton.Text = "fetching Last Submission";
            string id = UserIDManager.GetEncryptedID(BluetoothManager.deviceID,false);
            Logger.circularBuffer.Add("requesting last submission of: "+ id);
            var content = new StringContent(id, Encoding.UTF8, "text/plain");
            using var client = new HttpClient();
            using var response = await client.PostAsync("https://sl5m6xu9qf.execute-api.eu-central-1.amazonaws.com/GetLastSubmission", content);
            string r = await response.Content.ReadAsStringAsync();
            LastSubmissionInfo submissionInfo = null;
            if (r != null && r.Length > 1)
            {
                try
                {
                    submissionInfo = JsonConvert.DeserializeObject<LastSubmissionInfo>(r);
                }
                catch (Exception)
                {
                    await DisplayAlert("No Entry Found", "No deletable entry in database. To delete entries older than 24 hours, send an email to aurelwuensch@proton.me", "OK");
                    return;
                }
            }
            
            if(submissionInfo== null)
            {
                await DisplayAlert("No Entry Found", "No deletable entry in database. To delete entries older than 24 hours, send an email to aurelwuensch@proton.me", "OK");
                return;
            }

            DateTime dateTime = DateTimeOffset.FromUnixTimeMilliseconds(submissionInfo.startTime).UtcDateTime;

            // If you need it in the local time zone:
            DateTime localDateTime = dateTime.ToLocalTime();

            _DeleteLastSubmissionButton.Text = "Delete Previous Submission";
            if (r!=null && r.Length > 1)
            {
                bool result = await DisplayAlert("Entry found", $"your last Entry is: {submissionInfo.locationName} from {localDateTime.ToString()}", "Delete", "Cancel");
                if (result)
                {
                    var content2 = new StringContent(id, Encoding.UTF8, "text/plain");
                    using var responseDelete = await client.PostAsync("https://nlg29ka74k.execute-api.eu-central-1.amazonaws.com/DeleteLastSubmission", content2);
                    if (responseDelete.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        {
                            await DisplayAlert("Entry Deleted", "Entry successfully Deleted", "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Deletion Failed", "Error Deleting Entry", "OK");
                    }

                }
            }
            else
            {
                await DisplayAlert("No Entry Found", "No deletable entry in database. To delete entries older than 24 hours, send an email to aurelwuensch@proton.me", "OK");
            }

        }

    }

}
