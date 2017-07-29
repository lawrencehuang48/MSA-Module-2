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
        private IMobileServiceTable<RiceModel> RiceTable;

        private AzureManager()
        {
            this.client = new MobileServiceClient("http://amplifii.azurewebsites.net");
            this.RiceTable = this.client.GetTable<RiceModel>();
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

        public async Task<List<RiceModel>> GetRiceInformation()
        {
            return await this.RiceTable.ToListAsync();
        }

        public async Task PostRiceInformation(RiceModel RiceModel)
        {
            await this.RiceTable.InsertAsync(RiceModel);
        }

        public async Task UpdateRiceInformation(RiceModel RiceModel)
        {
            await this.RiceTable.UpdateAsync(RiceModel);
        }

        public async Task DeleteRiceInformation(RiceModel RiceModel)
        {
            await this.RiceTable.DeleteAsync(RiceModel);
        }
    }
}
