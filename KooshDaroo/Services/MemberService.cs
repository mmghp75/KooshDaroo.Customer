using System;
using System.Collections.Generic;
using System.Text;
using KooshDaroo.Models;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace KooshDaroo.Services
{
    class MemberService
    {
        public async Task<List<Member>> GetMemberAsync()
        {
            //RestClient<Member> restClient = new RestClient<Member>();
            //var memberList = await restClient.GetAsync("Member");
            //return memberList;

            var httpClient = new HttpClient();

            try
            {
                var json = await httpClient.GetStringAsync(App.apiAddress + "Member/");
                var taskModels = JsonConvert.DeserializeObject<List<Member>>(json);
                return taskModels;
            }
            catch (Exception e)
            {
                var x = e.Message;
                return null;
            }

        }

        public async Task<List<Member>> GetMemberByPhoneNoAsync(string phoneNo)
        {
            //RestClient<Member> restClient = new RestClient<Member>();
            //var member = await restClient.GetAsyncByFieldName("Member","PhoneNo",phoneNo);
            var httpClient = new HttpClient();
            var json = await httpClient.GetStringAsync(App.apiAddress + "Member/PhoneNo/" + phoneNo);
            var members = JsonConvert.DeserializeObject<List<Member>>(json);

            return members;
        }
        public async Task<Member> PostMemberAsync(Member member)
        {
            //RestClient<Member> restClient = new RestClient<Member>();
            //Member r = await restClient.PostAsync("Member",member);

            var httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(member);
            HttpContent httpContent = new StringContent(json);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            try
            {
                var result = await httpClient.PostAsync(App.apiAddress + "Member/", httpContent);
                var jsonString = await result.Content.ReadAsStringAsync();
                var r = JsonConvert.DeserializeObject<Member>(jsonString);

                return r;
            }
            catch (Exception ex)
            {
                return member;
            }
        }

    }
}

