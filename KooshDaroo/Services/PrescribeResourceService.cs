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
    class PrescribeResourceService
    {
        public async Task<List<PrescribeResource>> GetPrescribeResourceAsync()
        {
            //RestClient<PrescribeResource> restClient = new RestClient<PrescribeResource>();
            //var tblMemberList = await restClient.GetAsync("PrescribeResource");
            //return tblMemberList;

            var httpClient = new HttpClient();
            try
            {
                var json = await httpClient.GetStringAsync(App.apiAddress + "PrescribeResource/");
                var taskModels = JsonConvert.DeserializeObject<List<PrescribeResource>>(json);
                return taskModels;
            }
            catch (Exception e)
            {
                var x = e.Message;
                return null;
            }

        }
        public async Task<PrescribeResource> GetPrescribeResourceById(int Id)
        {
            //RestClient<PrescribeResource> restClient = new RestClient<PrescribeResource>();
            //var json = await restClient.GetAsyncByFieldNameReturnString("PrescribeResource", "Id", Id.ToString());
            //var taskModel = JsonConvert.DeserializeObject<List<PrescribeResource>>(json);
            //return taskModel[0];

            var httpClient = new HttpClient();
            var json = await httpClient.GetStringAsync(App.apiAddress + "PrescribeResource/Id/" + Id);
            var taskModel = JsonConvert.DeserializeObject<List<PrescribeResource>>(json);

            return taskModel[0];

        }

        public async Task<PrescribeResource> PostPrescribeResourceAsync(PrescribeResource prescribeResource)
        {
            //RestClient<PrescribeResource> restClient = new RestClient<PrescribeResource>();
            //var jsonString = await restClient.PostAsyncReturnString("PrescribeResource", prescribeResource);
            //PrescribeResource result = JsonConvert.DeserializeObject<PrescribeResource>(jsonString);
            //return result;

            var httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(prescribeResource);
            HttpContent httpContent = new StringContent(json);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await httpClient.PostAsync(App.apiAddress + "PrescribeResource/", httpContent);
            var jsonString = await result.Content.ReadAsStringAsync();
            var r = JsonConvert.DeserializeObject<PrescribeResource>(jsonString);
            return r;

        }
        public async Task<bool> PutPrescribeResourceAsync(PrescribeResource prescribeResource)
        {
            //RestClient<PrescribeResource> restClient = new RestClient<PrescribeResource>();
            //var result = await restClient.PutAsync("PrescribeResource", prescribeResource.id, prescribeResource);
            //return result;

            var httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(prescribeResource);
            HttpContent httpContent = new StringContent(json);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await httpClient.PutAsync(App.apiAddress + "PrescribeResource/" + prescribeResource.id, httpContent);

            return result.IsSuccessStatusCode;
        }
    }
}
