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
    class PrescribeService
    {
        public async Task<List<Prescribe>> GetPrescribeAsync()
        {
            //RestClient<Prescribe> restClient = new RestClient<Prescribe>();
            //var PrescribeList = await restClient.GetAsync("Prescribe");
            //return PrescribeList;

            var httpClient = new HttpClient();
            try
            {
                var json = await httpClient.GetStringAsync(App.apiAddress + "Prescribe/");
                var taskModels = JsonConvert.DeserializeObject<List<Prescribe>>(json);

                return taskModels;
            }
            catch (Exception e)
            {
                var x = e.Message;
                return null;
            }

        }

        public async Task<Prescribe> PostPrescribeAsync(Prescribe prescribe)
        {
            //RestClient<Prescribe> restClient = new RestClient<Prescribe>();
            //var result = await restClient.PostAsync("Prescribe", Prescribe);
            //return result;

            var httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(prescribe);
            HttpContent httpContent = new StringContent(json);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            try
            {
                var result = await httpClient.PostAsync(App.apiAddress + "Prescribe/", httpContent);
                var jsonString = await result.Content.ReadAsStringAsync();
                var r = JsonConvert.DeserializeObject<Prescribe>(jsonString);

                return r;
            }
            catch (Exception ex)
            {
                return prescribe;
            }

        }
        public async Task<bool> CanPrescribeAsync()
        {
            KooshDarooDatabase odb = new KooshDarooDatabase();
            var oLoginItemS = odb.GetItemsAsync();
            if (oLoginItemS.Result.Count == 0)
                return false;
            else
            {
                //var result = await restClient.GetAsyncByFieldNameFirst("Prescribe","PhoneNo", oLoginItemS.Result[0].PhoneNo);
                //return (DateTime.Now.ToOADate()-result.DateOf.ToOADate() > 0.5);
                return ((DateTime.Now.ToOADate() - oLoginItemS.Result[0].LastPrescribeDate.ToOADate()) >= 0.5);
            }

        }
    }
}
