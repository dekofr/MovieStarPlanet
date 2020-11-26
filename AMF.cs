using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluorineFx.IO;
using FluorineFx.AMF3;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json.Linq;

namespace SyndicateProject
{
    class AMF
    {
        public static string GetEndpoint(string Server)
        {
            WebClient Client = new WebClient();
            string Response = Client.DownloadString("https://disco.mspapis.com/disco/v1/services/msp/pl?services=mspwebservice");
            string Object = (string)JObject.Parse(Response)["Services"][0]["Endpoint"];
            return Object;
        }
        public static async Task<(string Ticket, string ActorId, string StarCoins, string Diamonds)> LoginToMSP(string Username, string Password, string Server)
        {
           
            object[] Body = {  Username, Password, new object[] { }, null, null, "e638656bf22d8b4fdd27c1327f6640ac" };
            AMFMessage Msg = new AMFMessage(3);
            Msg.AddHeader(new AMFHeader("sessionID", false, await Alghoritm.Dex0r()));
            Msg.AddHeader(new AMFHeader("id", false, ChecksumCalculator.createChecksum(Body)));
            Msg.AddHeader(new AMFHeader("needClassName", false, true));
            Msg.AddBody(new AMFBody("MovieStarPlanet.WebService.User.AMFUserServiceWeb.Login", "/1", Body));

            MemoryStream Str = new MemoryStream();
            AMFSerializer Serializer = new AMFSerializer(Str);
            Serializer.WriteMessage(Msg);

            byte[] BodyArray = Encoding.Default.GetBytes(Encoding.Default.GetString(Str.ToArray()));

            
            HttpClient Client = new HttpClient();
            ByteArrayContent Cnt = new ByteArrayContent(BodyArray);
            Client.DefaultRequestHeaders.Add("Referer", "https://assets.mspcdns.com/");
            Cnt.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-amf");
            HttpResponseMessage Response = await Client.PostAsync(GetEndpoint(Server) + "/Gateway.aspx?method=MovieStarPlanet.WebService.User.AMFUserServiceWeb.Login", Cnt);

            string AMFResponse = AMFDecoder(await Response.Content.ReadAsByteArrayAsync());

            string Ticket = (string)JObject.Parse(AMFResponse)["loginStatus"]["ticket"];
            return (Ticket, "", "", "");
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
