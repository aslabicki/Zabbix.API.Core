using System;
using System.Threading.Tasks;
using ZabbixAPICore;

namespace ZabbixAPIExamples
{
    class Program
    {
        static void Main(string[] args)
        {
            string userid = null;

            Zabbix zabbix = new Zabbix("admin", "zabbix", "http://192.168.1.15/api_jsonrpc.php");
            zabbix.LoginAsync().Wait();

            // Create new user

            Task<Response> createUserResponse = zabbix.GetResponseObjectAsync("user.create", new
            {
                alias = "joe.kowalsky",
                passwd = "password",
                name = "Joe",
                surname = "Kowalsky",
                autologin = 1,
                autologout = 0,
                type = 2,
                usrgrps = new[]
                {
                    new
                    {
                        usrgrpid = 7
                    }
                },
                user_medias = new[]
                {
                    new
                    {
                        mediatypeid = 1,
                        sendto = "joe.kowalsky@example.com",
                        active = 0,
                        severity = 63,
                        period = "1-7,00:00-24:00"
                    }
                }
            });

            if (createUserResponse.Result.error != null)
            {
                var error = createUserResponse.Result.error;
                Console.WriteLine(error.GetErrorMessage());
            }
            else
            {
                userid = createUserResponse.Result.result.userids[0];
                Console.WriteLine(userid);
            }

            createUserResponse.Wait();

            // Check user created user exist

            Task<Response> checkUserExistResponse = zabbix.GetResponseObjectAsync("user.get", new
            {
                output = "userid",
                filter = new { userid = 32 }
            });

            if (checkUserExistResponse.Result.result.Count > 0)
            {
                Console.WriteLine($"User id {userid} exist");
            }
            else
            {
                Console.WriteLine($"User id {userid} not exist");
            }

            checkUserExistResponse.Wait();

            // Create new user (get json string)

            Task<string> createUserResponseJson = zabbix.GetResponseJsonAsync("user.create", new
            {
                alias = "david.kowalsky",
                passwd = "password",
                name = "David",
                surname = "Kowalsky",
                autologin = 1,
                autologout = 0,
                type = 2,
                usrgrps = new[]
                {
                    new
                    {
                        usrgrpid = 7
                    }
                },
                user_medias = new[]
                {
                    new
                    {
                        mediatypeid = 1,
                        sendto = "david.kowalsky@example.com",
                        active = 0,
                        severity = 63,
                        period = "1-7,00:00-24:00"
                    }
                }
            });

            createUserResponseJson.Wait();

            Console.WriteLine(createUserResponseJson.Result);

            zabbix.LogoutAsync().Wait();

            Console.ReadKey();
        }
    }
}