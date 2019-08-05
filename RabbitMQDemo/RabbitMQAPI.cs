using Newtonsoft.Json;
using RabbitMQDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQDemo
{
    public class RabbitMQAPI
    {
        public void callAPI()
        {
            try
            {
                var authenticationBytes = Encoding.ASCII.GetBytes("guest:guest");
                HttpClient confClient = new HttpClient();
                confClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                               Convert.ToBase64String(authenticationBytes));
                confClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                int prevValue = 0;
                int curValue = 0;
                string url = "http://localhost:15672/api/queues/%2f/Dhruv?columns=messages_unacknowledged,consumers";
                //string url = "http://localhost:15672/api/queues/%2f/Dhruv?columns=consumers";
                while (true)
                {
                    HttpResponseMessage message = confClient.GetAsync(url).Result;

                    if (message.IsSuccessStatusCode)
                    {
                        using (HttpContent content = message.Content)
                        {
                            Task<string> result = content.ReadAsStringAsync();
                            string res = result.Result;
                            var details = JsonConvert.DeserializeObject<Details>(res);

                            Console.WriteLine(details.messages_unacknowledged + " " + DateTime.Now.ToString("hh:mm:ss:ffffff") + " API with Acknowledments");

                            curValue = details.messages_unacknowledged;

                            if (curValue != prevValue)
                            {
                                Console.ReadLine();
                                Console.Clear();
                                prevValue = curValue;
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<Name> GetVirtualHostList()
        {
            List<Name> virtualHostList = new List<Name>();

            var authenticationBytes = Encoding.ASCII.GetBytes("guest:guest");
            HttpClient confClient = new HttpClient();
            confClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                           Convert.ToBase64String(authenticationBytes));
            confClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //string url = "http://localhost:15672/api/vhosts";
            string url = "http://localhost:15672/api/queues";

            HttpResponseMessage message = confClient.GetAsync(url).Result;

            if (message.IsSuccessStatusCode)
            {
                using (HttpContent content = message.Content)
                {
                    Task<string> result = content.ReadAsStringAsync();
                    string res = result.Result;

                    return JsonConvert.DeserializeObject<List<Name>>(res);
                }
            }
            return virtualHostList;
        }
    }

    public class Name
    {
        /// <summary>
        /// Queue Name
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Number of messages
        /// </summary>
        public int messages_ready { get; set; }

        /// <summary>
        /// Virtual host
        /// </summary>
        public string vhost { get; set; }
    }
}
