namespace ZabbixAPICore
{
    public class Error
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public string Data { get; set; }

        public string GetErrorMsg()
        {
            return string.Format($"Code: {Code}, Data: {Data}, Message: {Message}");
        }
    }
}
