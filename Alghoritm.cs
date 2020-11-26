using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluorineFx.IO;
using FluorineFx.AMF3;
using Newtonsoft.Json;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json.Linq;

namespace SyndicateProject
{
    class Alghoritm
    {
        public static string sessionID { get; set; }

        public static async Task<string> GetRef()
        {
            AMFMessage Msg = new AMFMessage(3);
            object[] param = { };
            byte[] bytes = Encoding.Default.GetBytes(ChecksumCalculator.createChecksum(param));
            Msg.AddHeader(new AMFHeader("sessionID", false, Convert.ToBase64String(bytes)));
            Msg.AddHeader(new AMFHeader("needClassName", false, true));
            Msg.AddBody(new AMFBody("MovieStarPlanet.WebService.Os.AMFOs.CreateOsRef", "/1", param));
            MemoryStream str = new MemoryStream();

            AMFSerializer Srl = new AMFSerializer(str);

            Srl.WriteMessage(Msg);

            HttpClient client = new HttpClient();

            byte[] bytesS = Encoding.Default.GetBytes(Encoding.Default.GetString(str.ToArray()));

            ByteArrayContent Cnt = new ByteArrayContent(bytesS);

            Cnt.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-amf");

            client.DefaultRequestHeaders.Add("Referer", "https://assets.mspcdns.com/");

            HttpResponseMessage response = await client.PostAsync("https://ws-pl.mspapis.com/msp/93.1.2/Gateway.aspx?method=MovieStarPlanet.WebService.Os.AMFOs.CreateOsRef", Cnt);

            string responseStr = AMFDecoder(await response.Content.ReadAsByteArrayAsync());
            
          
            string LOL = (string) JObject.Parse(responseStr)["RefId"];

            return LOL;
        }

        public static async Task<string> CheckRef(string Ref, string WTF)
        {
            AMFMessage Msg = new AMFMessage(3);
            object[] param = { Ref, WTF };
            byte[] bytes = Encoding.Default.GetBytes(ChecksumCalculator.createChecksum(param));
            Msg.AddHeader(new AMFHeader("sessionID", false, Convert.ToBase64String(bytes)));
            Msg.AddHeader(new AMFHeader("needClassName", false, true));
            Msg.AddBody(new AMFBody("MovieStarPlanet.WebService.Os.AMFOs.RunOsCheck", "/1", param));
            MemoryStream str = new MemoryStream();

            AMFSerializer Srl = new AMFSerializer(str);

            Srl.WriteMessage(Msg);

            HttpClient client = new HttpClient();

            byte[] bytesS = Encoding.Default.GetBytes(Encoding.Default.GetString(str.ToArray()));

            ByteArrayContent Cnt = new ByteArrayContent(bytesS);

            Cnt.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-amf");

            client.DefaultRequestHeaders.Add("Referer", "https://assets.mspcdns.com/");

            HttpResponseMessage response = await client.PostAsync("https://ws-pl.mspapis.com/msp/93.1.2/Gateway.aspx?method=MovieStarPlanet.WebService.Os.AMFOs.RunOsCheck", Cnt);

            string responseStr = AMFDecoder(await response.Content.ReadAsByteArrayAsync());



            return responseStr;
        } 

        public static async Task<string> Dex0r()
        {
            string Ref = await GetRef();
            string Result = null;
            Result = await CheckRef(Ref, "374:79:383:26:28:111:256:1598:1163:6355:5214:2452:5363:2859:5094:34173:472:32:219:633:10352:5004:1108:1754:4412:1695:697:6085:1206:759:2455:28646:896:1987:661:447:384:1132:3158:10886:6505:3512:2589:1073:3461:173:148:28516");
            if(Result == "null")
            {
                Result = await CheckRef(Ref, "489:55:83:5815:4282:2596:2510:1556:1969:632:799:1080:2696:1505:602:38861:643:56:327:365:263:715:1996:1437:420:9279:980:10625:3177:1662:565:33017:627:24:22:183:287:158:2467:9856:5719:832:3603:3110:822:3886:3947:29984");
            }
            return Result.Replace("\"", "");
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
