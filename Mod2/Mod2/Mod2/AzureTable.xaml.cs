using Microsoft.WindowsAzure.MobileServices;
using Mod2.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace Mod2
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AzureTable : ContentPage
    {
            Geocoder geoCoder;

        public AzureTable()
        {
            InitializeComponent();
            geoCoder = new Geocoder();
        }

        async void Handle_ClickedAsync(object sender, System.EventArgs e)
        {
            List<NotHotDogModel> notHotDogInformation = await AzureManager.AzureManagerInstance.GetRiceInformation();

            foreach (NotHotDogModel model in notHotDogInformation)
            {
                var position = new Position(model.Latitude, model.Longitude);
                var possibleAddresses = await geoCoder.GetAddressesForPositionAsync(position);
                foreach (var address in possibleAddresses)
                    model.City = address;
            }
            RiceList.ItemsSource = notHotDogInformation;
            notHotDogInformation.Reverse();
        }
    }
}