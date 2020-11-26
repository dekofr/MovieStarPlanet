using FluorineFx.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MovieStarPlanet
{
    internal class AMF
    {
        public static string GetEndpoint(string Server)
        {
            WebClient Client = new WebClient();
            string Response = Client.DownloadString($"https://disco.mspapis.com/disco/v1/services/msp/{Server}?services=mspwebservice");
            string Endpoint = JObject.Parse(Response)["Services"][0]["Endpoint"];
            return Endpoint;
        }

        public static async Task<(string Ticket, string Username, string StarCoins, string ActorId)> LoginToMSP(string username, string password, string server)
        {
            string ticket = null;
            string user = null;
            string starcoins = null;
            string actorid = null;
            try
            {
                object[] param = { username, password, new object[] { }, null, null, "85efde44997720dfdd8d1850b5b22925" };
                AMFMessage msg = new AMFMessage(3);
                msg.AddHeader(new AMFHeader("sessionID", true, await Alghoritm.Dex0r()));
                msg.AddHeader(new AMFHeader("id", false, Hash.createChecksum(param)));
                msg.AddHeader(new AMFHeader("needClassName", false, true));
                msg.AddBody(new AMFBody("MovieStarPlanet.WebService.User.AMFUserServiceWeb.Login", "/1", param));
                MemoryStream str = new MemoryStream();
                AMFSerializer srl = new AMFSerializer(str);
                srl.WriteMessage(msg);

                byte[] bytes = Encoding.Default.GetBytes(Encoding.Default.GetString(str.ToArray()));

                ByteArrayContent byteArray = new ByteArrayContent(bytes);
                HttpClient client = new HttpClient();
                HttpWebRequest.DefaultWebProxy = new WebProxy();

                client.DefaultRequestHeaders.Add("Referer", "https://assets.mspcdns.com/");

                byteArray.Headers.ContentType = new MediaTypeHeaderValue("application/x-amf");
                HttpResponseMessage response = await client.PostAsync(GetEndpoint(server) + "/Gateway.aspx?method=MovieStarPlanet.WebService.User.AMFUserServiceWeb.Login", byteArray);
                string _response = AMFDecoder(await response.Content.ReadAsByteArrayAsync());

                if (_response.Contains("InvalidCredentials"))
                {
                    Console.WriteLine("Invalid credentials!");
                }
                else
                {
                    JObject json = JObject.Parse(_response);
                    object loginStatus = json["loginStatus"];


                    string _loginStatus = loginStatus.ToString();
                    JObject _json = JObject.Parse(_loginStatus);

                    ticket = (string)_json["ticket"];
                    user = (string)_json["actor"]["Name"];

                    starcoins = (string)_json["actor"]["Money"];

                    actorid = (string)_json["actor"]["ActorId"];



                    ticket = ticket.Remove(ticket.Length - 1);
                    Console.WriteLine("Successfully logged in!");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something went wrong: {ex}");
            }


            return (ticket, user, starcoins, actorid);

        }
        
        public static string AMFDecoder(byte[] body)
        {
            string result = "";
            AMFMessage amfmessage = null;
            MemoryStream memoryStream = new MemoryStream(body);
            try
            {
                AMFDeserializer amfdeserializer = new AMFDeserializer(memoryStream);
                amfmessage = amfdeserializer.ReadAMFMessage();
            }
            catch (DecoderFallbackException ex)
            {
                memoryStream.Position = 0L;
                object obj = new AMFReader(memoryStream)
                {
                    FaultTolerancy = true
                }.ReadAMF3Data();
                amfmessage = new AMFMessage(3);
                amfmessage.AddBody(new AMFBody(string.Empty, string.Empty, obj));
                Console.WriteLine(ex.ToString());
            }
            foreach (AMFBody amfbody in amfmessage.Bodies)
            {
                result = JsonConvert.SerializeObject(amfbody.Content);
            }
            return result;
        }
    }
}
       
