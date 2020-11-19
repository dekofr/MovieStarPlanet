using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Management;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Console = Colorful.Console;

namespace dex0fame
{
    internal class Program
    {

        public static string Version = "3.5";
        public static string HookURL = "";

        private static async Task Main(string[] args)
        {

            Console.WriteLine("Checking update...", Color.Yellow);
            try
            {
                CheckUpdate();
            }
            catch (Exception)
            {
                Console.WriteLine("Something went wrong!", Color.Red);
                Thread.Sleep(5000);
                Environment.Exit(1337);
            }
            Console.WriteLine("No updates!", Color.Green);
            Thread.Sleep(1000);
            Console.WriteLine("Checking access...", Color.Yellow);
            CheckAccess();
            Thread.Sleep(1000);
            if (File.Exists(@"settings.json"))
            {
                Console.Write("Found settings. Do you want to load them? [Y/N]: ");
                string Load = Console.ReadLine().Replace("Found settings. Do you want to load them? [Y/N]: ", "");
                if (Load.ToLower() == "y")
                {
                    var data = JObject.Parse(File.ReadAllText(@"settings.json"));
                    string Endpoint1 = GrabEndpoint(data["Server"].ToString());
                    if (string.IsNullOrEmpty(Endpoint1))
                    {
                        Console.WriteLine("[STATUS]: ERROR", Color.Red);
                        Thread.Sleep(5000);
                        Environment.Exit(1337);
                    }
                    AMF.Gateway = Endpoint1;
                    await Task.Run(() => AMF.Dextruct0r(data["Username"].ToString(), Encoding.Default.GetString(Convert.FromBase64String(data["Password"].ToString())), data["Server"].ToString()));
                    Console.ReadKey();
                }
            }

            Console.Write("Username: ");
            string Username = Console.ReadLine().Replace("Username: ", "");
            Console.Write("Password: ");
            string Password = Console.ReadLine().Replace("Password: ", "");
            Console.Write("Server: ");
            string Server = Console.ReadLine().Replace("Server: ", "");
            string Endpoint = GrabEndpoint(Server);
            if (string.IsNullOrEmpty(Endpoint))
            {
                Console.WriteLine("[STATUS]: ERROR", Color.Red);
                Thread.Sleep(5000);
                Environment.Exit(1337);
            }
            AMF.Gateway = Endpoint;
            Console.Write("Save settings? (Y/N): ");
            string Save = Console.ReadLine().Replace("Save settings? (Y/N): ", "");
            if (Save.ToLower() == "y")
            {
                SaveSettings(Username, Password, Server);
            }
            await Task.Run(() => AMF.Dextruct0r(Username, Password, Server));
            Console.ReadKey();
        }



        private static void SaveSettings(string username, string password, string server)
        {
            System.IO.File.WriteAllText(@"settings.json", JsonConvert.SerializeObject(new Settings
            {
                Username = username,
                Password = Convert.ToBase64String(Encoding.Default.GetBytes(password)),
                Server = server
            }));
        }

        public static string GrabEndpoint(string server)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    JObject jobject = JObject.Parse(webClient.DownloadString("https://disco.mspapis.com/disco/v1/services/msp/" + server + "?services=mspwebservice"));
                    var Endpoint = (string)jobject["Services"][0]["Endpoint"];
                    return Endpoint;
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        private static void CheckAccess()
        {

            using (WebClient c = new WebClient())
            {
                c.Proxy = new WebProxy();
                string data = c.DownloadString("http://dex0fame.5v.pl/checkAccess.php?HWID=" + GetAccessFormat());
                string access = GetAccessFormat();
                if (string.IsNullOrEmpty(data))
                {
                    Console.Clear();
                    Console.WriteLine("You are not activated!", Color.DarkRed);
                    Console.WriteLine("Contact deko#1337 or ajdot#0022 for activation!");
                    Console.WriteLine("Your Key: " + access);
                    Thread.Sleep(10000);
                    Environment.Exit(1337);
                }
                else
                {
                    (string Error, string Activated, string Blacklisted) = GetAccessType();
                    data = Activated;
                    if (data == Error)
                    {
                        Console.Clear();
                        Console.WriteLine("An error occured!", Color.DarkRed);
                        Thread.Sleep(10000);
                        Environment.Exit(1337);
                    }
                    if (data == Activated)
                    {
                        Console.Clear();
                        Console.WriteLine(@"      $$\                      $$$$$$\  $$$$$$$$\                               ");
                        Console.WriteLine(@"      $$ |                    $$$ __$$\ $$  _____|                              ");
                        Console.WriteLine(@" $$$$$$$ | $$$$$$\  $$\   $$\ $$$$\ $$ |$$ |   $$$$$$\  $$$$$$\$$$$\   $$$$$$\  ");
                        Console.WriteLine(@"$$  __$$ |$$  __$$\ \$$\ $$  |$$\$$\$$ |$$$$$\ \____$$\ $$  _$$  _$$\ $$  __$$\ ");
                        Console.WriteLine(@"$$ /  $$ |$$$$$$$$ | \$$$$  / $$ \$$$$ |$$  __|$$$$$$$ |$$ / $$ / $$ |$$$$$$$$ |");
                        Console.WriteLine(@"$$ |  $$ |$$   ____| $$  $$<  $$ |\$$$ |$$ |  $$  __$$ |$$ | $$ | $$ |$$   ____|");
                        Console.WriteLine(@"\$$$$$$$ |\$$$$$$$\ $$  /\$$\ \$$$$$$  /$$ |  \$$$$$$$ |$$ | $$ | $$ |\$$$$$$$\ ");
                        Console.WriteLine(@" \_______| \_______|\__/  \__| \______/ \__|   \_______|\__| \__| \__| \_______|");
                        Console.WriteLine("\n| Created by deko#1337 & ajdot#0022 <3 | Version v" + Version + " |\n");
                        Console.WriteLine($"Welcome to Dex0Fame v.{Version}!", Color.Green);
                        return;
                    }
                    if (data == Blacklisted)
                    {
                        Console.Clear();
                        Console.WriteLine("You are blacklisted! Contact deko#1337 or ajdot#0022 for more information!", Color.DarkRed);
                        Thread.Sleep(10000);
                        Environment.Exit(1337);
                    }
                }
            }
        }


        private static (string Error, string Activated, string Blacklisted) GetAccessType()
        {
            string AccessData = GetAccessFormat();
            SHA512Managed sh = new SHA512Managed();
            return (BitConverter.ToString(sh.ComputeHash(Encoding.Default.GetBytes($"ERROR-{AccessData}"))).Replace("-", "").ToLower(),
                BitConverter.ToString(sh.ComputeHash(Encoding.Default.GetBytes($"USER-ACTIVATED-{AccessData}"))).Replace("-", "").ToLower(),
                BitConverter.ToString(sh.ComputeHash(Encoding.Default.GetBytes($"USER-BLACKLISTED-{AccessData}"))).Replace("-", "").ToLower());
        }

        private static void CheckUpdate()
        {
            using (WebClient c = new WebClient())
            {
                c.Proxy = new WebProxy();
                string version = c.DownloadString("http://dex0fame.5v.pl/Version.php");
                if (version != Version)
                {
                    Console.WriteLine("New update! (" + version + ")", Color.Green);
                    Console.WriteLine("Downloading... Please Wait!", Color.Yellow);
                    c.DownloadFile("http://dex0fame.5v.pl/dex0Fame.exe", $"dex0Fame v.{version}.exe");
                    Console.WriteLine("Downloaded! Please wait!", Color.Green);
                    Thread.Sleep(1500);
                    Process.Start($"dex0Fame v.{version}.exe");
                    Environment.Exit(1337);
                }
            }
        }

        private static string GetAccessFormat()
        {
            string hwid = GetHwid();
            OperatingSystem os = Environment.OSVersion;
            string full = string.Concat(new string[] {
                Environment.UserName,
                Environment.MachineName,
                "dexoFame",
                "ajdot is not dead, take this hint ;)",
                (os != null) ? os.ToString() : "deko and ajdot are the gods ;)",
                hwid
            });
            SHA1Managed sh = new SHA1Managed();
            string hash = BitConverter.ToString(sh.ComputeHash(Encoding.Default.GetBytes(full))).Replace("-", "").ToLower();
            return $"DEX0-{hash.Substring(0, 10)}-FAM3";
        }

        private static string GetHwid()
        {
            ManagementObjectSearcher mos = new ManagementObjectSearcher("Select ProcessorId From Win32_processor");
            string hwid = string.Empty;
            ManagementObjectCollection mbsList = mos.Get();
            foreach (ManagementObject mo in mbsList)
            {
                hwid = mo["ProcessorId"].ToString();
                break;
            }
            return $"{hwid}";
        }
    }

    internal class Settings
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Server { get; set; }

    }
}
