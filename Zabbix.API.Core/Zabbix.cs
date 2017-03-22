using Newtonsoft.Json;
using System;
using System.Dynamic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ZabbixAPICore
{
    public class Zabbix
    {
        private string _user;
        private string _password;
        private string _apiURL;
        private string _auth;
        private string _basicAuthentication;
        private static HttpClient _client = new HttpClient();

        public Zabbix(string user, string password, string apiURL, bool useBasicAuthorization = false)
        {
            if (!Uri.IsWellFormedUriString(apiURL, UriKind.Absolute))
            {
                throw new UriFormatException();
            }

            _user = user ?? throw new ArgumentNullException("user");
            _password = password ?? throw new ArgumentNullException("password");
            _apiURL = apiURL;
            if (useBasicAuthorization) _basicAuthentication = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(_user + ":" + _password));
        }

        public async Task Login()
        {
            dynamic userAuth = new ExpandoObject();
            userAuth.user = _user;
            userAuth.password = _password;
            Response response = await objectResponse("user.login", userAuth);
            _auth = response.result;
        }

        public async Task<bool> Logout()
        {
            Response response = await objectResponse("user.logout", new string[] { });
            var result = response.result;
            return result;
        }

        private Task<string> jsonResponse(string method, object parameters)
        {
            Request request = new Request("2.0", method, 1, _auth, parameters);
            string jsonParams = JsonConvert.SerializeObject(request);
            return sendRequest(jsonParams);
        }


        public async Task<Response> objectResponse(string method, object parameters)
        {
            Request request = new Request("2.0", method, 1, _auth, parameters);
            string jsonParams = JsonConvert.SerializeObject(request);
            return createResponse(await sendRequest(jsonParams));
        }

        private Response createResponse(string json)
        {
            Response response = JsonConvert.DeserializeObject<Response>(json);
            return response;
        }

        private async Task<string> sendRequest(string jsonParams)
        {
            if (_basicAuthentication != null) _client.DefaultRequestHeaders.Add("Authorization", "Basic " + _basicAuthentication);

            var clientResponse = await _client.PostAsync(_apiURL, new StringContent(jsonParams, Encoding.UTF8, "application/json"));
            string jsonResponse = await clientResponse.Content.ReadAsStringAsync();

            return jsonResponse;
        }

    }
}
