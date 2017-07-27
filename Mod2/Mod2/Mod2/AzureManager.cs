using Microsoft.WindowsAzure.MobileServices;
using Mod2.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod2
{
    public class AzureManager
    {

        private static AzureManager instance;
        private MobileServiceClient client;
        private IMobileServiceTable<NotHotdogModel> notHotDogTable;

        private AzureManager()
        {
            this.client = new MobileServiceClient("http://amplifii.azurewebsites.net");
            this.notHotDogTable = this.client.GetTable<NotHotdogModel>();
        }

        public MobileServiceClient AzureClient
        {
            get { return client; }
        }

        public static AzureManager AzureManagerInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AzureManager();
                }

                return instance;
            }
        }

        public async Task<List<NotHotdogModel>> GetHotDogInformation()
        {
            return await this.notHotDogTable.ToListAsync();
        }

        public async Task PostHotDogInformation(NotHotdogModel notHotDogModel)
        {
            await this.notHotDogTable.InsertAsync(notHotDogModel);
        }
    }
}
