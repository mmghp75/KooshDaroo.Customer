using KooshDaroo.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using KooshDaroo.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace KooshDaroo.Services
{
    class PharmacyService
    {
        public async Task<List<Pharmacy>> GetPharmacyAsync()
        {
            //RestClient<Pharmacy> restClient = new RestClient<Pharmacy>();
            //var PharmacyList = await restClient.GetAsync("Pharmacy");
            //return PharmacyList;

            var httpClient = new HttpClient();
            try
            {
                var json = await httpClient.GetStringAsync(App.apiAddress + "Pharmacy/");
                var taskModels = JsonConvert.DeserializeObject<List<Pharmacy>>(json);
                return taskModels;
            }
            catch (Exception e)
            {
                var x = e.Message;
                return null;
            }

        }
        public async Task<Pharmacy> GetPharmacyById(int Id)
        {
            //RestClient<Pharmacy> restClient = new RestClient<Pharmacy>();
            //var json = await restClient.GetAsyncByFieldNameReturnString("Pharmacy", "Id", Id.ToString());


            var httpClient = new HttpClient();
            var json = await httpClient.GetStringAsync(App.apiAddress + "Pharmacy/Id/" + Id);
            var taskModel = JsonConvert.DeserializeObject<List<Pharmacy>>(json);

            return taskModel[0];
        }
    }
}
