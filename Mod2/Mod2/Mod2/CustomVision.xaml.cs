using Mod2.DataModels;
using Mod2.Model;
using Newtonsoft.Json;
using Plugin.Geolocator;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace Mod2
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomVision : ContentPage
    {

        public CustomVision()
        {
            InitializeComponent();
        }

        private async void LoadCamera(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                PhotoSize = PhotoSize.Medium,
                Directory = "Sample",
                Name = $"{DateTime.UtcNow}.jpg"
            });

            if (file == null)
                return;

            image.Source = ImageSource.FromStream(() =>
            {
                return file.GetStream();
            });

            await postLocationAsync();

            await MakePredictionRequest(file);
        }

        async Task postLocationAsync()
        {

            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 50;


            var position = await locator.GetPositionAsync(TimeSpan.FromMilliseconds(1000));



            RiceModel model = new RiceModel()
            {
                Longitude = (float)position.Longitude,
                Latitude = (float)position.Latitude
            };

            await AzureManager.AzureManagerInstance.PostRiceInformation(model);
        }

        static byte[] GetImageAsByteArray(MediaFile file)
        {
            var stream = file.GetStream();
            BinaryReader binaryReader = new BinaryReader(stream);
            return binaryReader.ReadBytes((int)stream.Length);
        }

        async Task MakePredictionRequest(MediaFile file)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Prediction-Key", "e255f72dcb9b4f69917f05309ab246d2");

            string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Prediction/f4777059-a48b-4ab0-b43f-d585c83e164e/image?iterationId=92c71ce9-4e94-4ced-a344-031e5939af17";

            HttpResponseMessage response;

            byte[] byteData = GetImageAsByteArray(file);

            using (var content = new ByteArrayContent(byteData))
            {

                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(url, content);


                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    EvaluationModel responseModel = JsonConvert.DeserializeObject<EvaluationModel>(responseString);

                    double max = responseModel.Predictions.Max(m => m.Probability);

                    TagLabel.Text = (max >= 0.5) ? "This is a photo of rice (Probability: " + max + ")." : " This is not a photo of rice (Probability: " + max + ").";

                    file.Dispose();
                }
            }

        }
    }
}