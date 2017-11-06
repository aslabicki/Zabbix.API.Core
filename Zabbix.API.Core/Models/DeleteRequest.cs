using System.Collections.Generic;

namespace ZabbixAPICore
{
    internal class DeleteRequest
    {
        public string jsonrpc { get; set; }
        public string method { get; set; }
        public int id { get; set; }
        public string auth { get; set; }
        public List<int> @params { get; set; }


        public DeleteRequest(string jsonrpc, string method, int id, string auth, List<int> @params)
        {
            this.jsonrpc = jsonrpc;
            this.method = method;
            this.id = id;
            this.auth = auth;
            this.@params = @params;
        }
    }
}
