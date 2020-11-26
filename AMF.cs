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
using Console = Colorful.Console;

namespace dex0fame
{
    internal class AMF
    {
        public static string Gateway = "";

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
                HttpResponseMessage response = await client.PostAsync(Gateway + "/Gateway.aspx?method=MovieStarPlanet.WebService.User.AMFUserServiceWeb.Login", byteArray);
                string _response = AMFDecoder(await response.Content.ReadAsByteArrayAsync());

                if (_response.Contains("InvalidCredentials"))
                {
                    ticket = "Error";
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
                }

            }
            catch (Exception)
            {
                ticket = "[STATUS]: Something went wrong!";
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
        public static async Task Level(string Ticket, string ActorClickitemRellid, string server)
        {
            for (; ; )
            {
                try
                {
                    Console.WriteLine("Sending req");
                    HttpClient client = new HttpClient();
                    object[] content = { new TicketHeader { Ticket = Ticket, anyAttribute = null }, Convert.ToInt32(ActorClickitemRellid), 100 };
                    AMFMessage amf = new AMFMessage(3);
                    amf.AddHeader(new AMFHeader("id", false, Hash.createChecksum(content)));
                    amf.AddHeader(new AMFHeader("needClassName", false, true));
                    amf.AddBody(new AMFBody("MovieStarPlanet.WebService.Pets.AMFPetService.PlayedPetGame", "/1", content));

                    MemoryStream str = new MemoryStream();
                    AMFSerializer serializer = new AMFSerializer(str);

                    serializer.WriteMessage(amf);
                    byte[] bytes = Encoding.Default.GetBytes(Encoding.Default.GetString(str.ToArray()));

                    ByteArrayContent btrContent = new ByteArrayContent(bytes);





                    btrContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-amf");
                    client.DefaultRequestHeaders.Add("Referer", "https://assets.mspcdns.com/");
                    HttpResponseMessage response = await client.PostAsync(Gateway + "/Gateway.aspx?method=MovieStarPlanet.WebService.Pets.AMFPetService.PlayedPetGame", btrContent);

                    HttpWebRequest.DefaultWebProxy = new WebProxy();
                    string responseString = AMFDecoder(await response.Content.ReadAsByteArrayAsync());
                    Console.WriteLine(response.StatusCode.ToString());
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("[ INFO ] Earned " + responseString + " fame!");
                    }
                    else
                    {
                        Console.WriteLine("[ STATUS ] Shadow-banned... Change VPN server!");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ex: " + ex.ToString());
                }
            }
        }
        public static async Task<string> GetPetId(string ticket, string actorid, string server)
        {

            object[] content = { new TicketHeader { Ticket = ticket, anyAttribute = null }, Convert.ToInt32(actorid) };
            AMFMessage amf = new AMFMessage(3);
            amf.AddHeader(new AMFHeader("id", false, Hash.createChecksum(content)));
            amf.AddHeader(new AMFHeader("needClassName", false, true));
            amf.AddBody(new AMFBody("MovieStarPlanet.WebService.Pets.AMFPetService.GetClickItemsForActor", "/1", content));

            MemoryStream str = new MemoryStream();
            AMFSerializer serializer = new AMFSerializer(str);

            serializer.WriteMessage(amf);
            byte[] bytes = Encoding.Default.GetBytes(Encoding.Default.GetString(str.ToArray()));

            ByteArrayContent btrContent = new ByteArrayContent(bytes);



            HttpClient client = new HttpClient();

            btrContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-amf");
            client.DefaultRequestHeaders.Add("Referer", "https://assets.mspcdns.com/");
            HttpWebRequest.DefaultWebProxy = new WebProxy();
            HttpResponseMessage response = await client.PostAsync(Gateway + "/Gateway.aspx?method=MovieStarPlanet.WebService.Pets.AMFPetService.GetClickItemsForActor", btrContent);
            string _response = AMFDecoder(await response.Content.ReadAsByteArrayAsync());

            string rellid = null;
            if (_response.Contains("ActorClickItemRelId"))
            {
                JArray responseArr = JArray.Parse(_response);
                JObject responseObj = JObject.Parse(responseArr[0].ToString());
                rellid = (string)responseObj["ActorClickItemRelId"];
            }
            else
            {
                rellid = "ERROR-NO-PETS";
            }
            return rellid;
        }

        public static async Task Dextruct0r(string Username, string Password, string Server)
        {
            (string Ticket, string Username, string StarCoins, string ActorId) Result = await LoginToMSP(Username, Password, Server);
            Console.WriteLine("");
            Console.WriteLine("|---------------------------|");
            Console.WriteLine("|    >>>     0x1     <<<    |");
            Console.WriteLine("|                           |");
            Console.WriteLine("| # Username: " + Username);
            Console.WriteLine("| # ActorId: " + Result.ActorId);
            Console.WriteLine("| # StarCoins: " + Result.StarCoins);
            Console.WriteLine("|                           |");
            Console.WriteLine("|    >>>     0x1    <<<     |");
            Console.WriteLine("|---------------------------|");
            Console.WriteLine("");
            Console.WriteLine("# Grabbing old boonie information's...");
            string PetID = await GetPetId(Result.Ticket, Result.ActorId, Server);
            if (PetID == "ERROR-NO-PETS")
            {
                Console.WriteLine("[ STATUS ] You dont have any old boonie, you need to buy it with dex0Pet", ConsoleColor.Red);
            }
            else
            {
                Console.WriteLine("[ STATUS ] Successfully grabbed information's about boonie", ConsoleColor.Green);
                 Task.Run(() =>  Level(Result.Ticket, PetID, Server));
                 Task.Run(() =>  Level(Result.Ticket, PetID, Server));
                 Task.Run(() =>  Level(Result.Ticket, PetID, Server));
                 Task.Run(() =>  Level(Result.Ticket, PetID, Server));
                 Task.Run(() =>  Level(Result.Ticket, PetID, Server));
                 Task.Run(() =>  Level(Result.Ticket, PetID, Server));
                 Task.Run(() =>  Level(Result.Ticket, PetID, Server));
                 Task.Run(() =>  Level(Result.Ticket, PetID, Server));
                 Task.Run(() =>  Level(Result.Ticket, PetID, Server));
                 Task.Run(() =>  Level(Result.Ticket, PetID, Server));
                 Task.Run(() =>  Level(Result.Ticket, PetID, Server));
                 Task.Run(() =>  Level(Result.Ticket, PetID, Server));
                 Task.Run(() =>  Level(Result.Ticket, PetID, Server));
                 Task.Run(() =>  Level(Result.Ticket, PetID, Server));
                 Task.Run(() =>  Level(Result.Ticket, PetID, Server));
            }
        }
    }
}
