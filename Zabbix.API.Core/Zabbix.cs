using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ZabbixAPICore
{
    public class Zabbix
    {
        private string user;
        private string password;
        private string apiURL;
        private string auth;
        private string basicAuthentication;
        private static HttpClient _client = new HttpClient();

        public Zabbix(string user, string password, string apiURL, bool useBasicAuthorization = false)
        {
            if (!Uri.IsWellFormedUriString(apiURL, UriKind.Absolute))
            {
                throw new UriFormatException();
            }

            this.user = user ?? throw new ArgumentNullException(nameof(user));
            this.password = password ?? throw new ArgumentNullException(nameof(password));
            this.apiURL = apiURL;
            if (useBasicAuthorization) basicAuthentication = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(this.user + ":" + this.password));
        }

        public async Task LoginAsync()
        {
            dynamic userAuth = new ExpandoObject();
            userAuth.user = user;
            userAuth.password = password;
            Response response = await GetResponseObjectAsync("user.login", userAuth);
            auth = response.result;
        }

        public async Task<bool> LogoutAsync()
        {
            Response response = await GetResponseObjectAsync("user.logout", new string[] { });
            var result = response.result;
            return result;
        }

        public async Task<string> GetResponseJsonAsync(string method, object parameters)
        {
            Request request = new Request("2.0", method, 1, auth, parameters);

            string jsonParams = JsonConvert.SerializeObject(request);
            string jsonResponse = await SendRequestAsync(jsonParams);

            return jsonResponse;
        }

        public async Task<string> GetDeleteResponseJSonAsync(string method, List<int> parameters)
        {
            DeleteRequest request = new DeleteRequest("2.0", method, 1, auth, parameters);

            string jsonParams = JsonConvert.SerializeObject(request);
            string jsonResponse = await SendRequestAsync(jsonParams);

            return jsonResponse;
        }

        public async Task<Response> GetResponseObjectAsync(string method, object parameters)
        {
            string jsonResponse = await GetResponseJsonAsync(method, parameters);
            var objectResponse = ConvertJsonToResponse(jsonResponse);

            return objectResponse;
        }

        private Response ConvertJsonToResponse(string json)
        {
            Response response = JsonConvert.DeserializeObject<Response>(json);
            return response;
        }

        private async Task<string> SendRequestAsync(string jsonParams)
        {
            if (basicAuthentication != null) _client.DefaultRequestHeaders.Add("Authorization", "Basic " + basicAuthentication);

            var clientResponse = await _client.PostAsync(apiURL, new StringContent(jsonParams, Encoding.UTF8, "application/json"));
            string jsonResponse = await clientResponse.Content.ReadAsStringAsync();

            return jsonResponse;
        }
    }
}