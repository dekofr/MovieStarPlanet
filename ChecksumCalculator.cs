using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace dex0fame
{
    class Hash
    {
        public static string saltCharactersString = "9UN8#ODsMCn07J^R";

        public static byte[] hashBytes { get; set; }
        public static string saltCharactersAllowedString = "UC4#Ti#DuwkJCov)!27Po#6d-FPzIET6kAmDqMsSK3^BKzhg0p+/Zaa4Z$iI+Xl2oHåpu+XA";

        public static string[] saltCharactersOrigin = null;

        public static string[] saltCharactersAllowed = null;

        public static string[] saltCharactersAllowedCopy = null;

        public static int[] saltNumericsOrigin = null;

        public static int[] saltNumerics = null;

        public static byte[] saltNumericsBytes;



        public static string createChecksum(object[] param1) 
        {
            string loc2 = fromArray(param1);
            string loc3 = getEndData(param1);
            string loc4 = logData();
            return SHA1withUTF8(loc2 + loc3 + loc4);
        }

        public static string SHA1withUTF8(string s)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);

            var sha1 = SHA1.Create();

            byte[] hashBytes = sha1.ComputeHash(bytes);
            return HexStringFromBytes(hashBytes);
        }
        public static string HexStringFromBytes(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }
            return sb.ToString();
        }
        public static string getEndData(object[] param1)
        {
            string loc3 = null;
            object[] loc4 = null;
            try
            {
                foreach(object loc2 in param1)
                {
                    if(loc2 is TicketHeader)
                    {
                        string ticket = (loc2 as TicketHeader).Ticket;
                        Stack<string> stack = new Stack<string>(ticket.Split(new char[] { ',' }));
                        return stack.Pop().Substring(0, 5);
                    }
                }
            } catch (Exception)
            {

            }
            return "v1n3g4r";
        }
        private static string logData()
        {
            List<string> list = new List<string>();
            List<int> list2 = new List<int>();
            List<byte[]> list3 = new List<byte[]>();
            if (saltNumericsOrigin == null)
            {
                int i = 0;
                while (i < saltCharactersAllowedString.Length)
                {
                    list.Add(saltCharactersAllowedString[i].ToString());
                    i++;
                }
                i = 0;
                saltCharactersAllowed = list.ToArray();
                saltCharactersAllowedCopy = list.ToArray();
                list.Clear();
                while (i < saltCharactersString.Length)
                {
                    list.Add(saltCharactersString[i].ToString());
                    i++;
                }
                i = 0;
                saltCharactersOrigin = list.ToArray();
                saltNumericsOrigin = new int[0];
                while (i < saltCharactersOrigin.Length)
                {
                    list2.Add(saltCharactersOrigin[i][0]);
                    i++;
                }
                saltNumericsOrigin = list2.ToArray();
            }
            int j = 0;
            while (j < saltCharactersAllowedString.Length)
            {
                saltCharactersAllowedCopy[j] = saltCharactersAllowed[j];
                j++;
            }
            j = 0;
            while (j < saltCharactersOrigin.Length)
            {
                int loc4 = saltNumericsOrigin[j];
                if (j % 2 == 0) loc4 += j; else loc4 -= j;
                int loc5 = loc4 % saltCharactersAllowedString.Length;
                list3.Add(Encoding.UTF8.GetBytes(saltCharactersAllowedCopy[loc5]));
                swapSaltIndexes(loc5, saltCharactersAllowedCopy);
                j++;
            }
            saltNumericsBytes = Combine(list3.ToArray());
            return Encoding.UTF8.GetString(saltNumericsBytes);
        }
        public static void swapSaltIndexes(int param1, object[] param2)
        {
            int loc3 = param2.Length;
            int loc4 = (param1 + 14) % loc3;
            int loc5 = (loc4 - 1 < 0) ? (loc3 - 1) : (loc4 - 1);
            int loc6 = (loc4 + 1 > loc3 - 1) ? 0 : (loc4 + 1);
            object loc7 = param2[loc6];
            param2[loc5] = param2[loc6];
            param2[loc6] = loc7;
        }
        private static byte[] Combine(params byte[][] arrays)
        {
            byte[] array = new byte[arrays.Sum((byte[] a) => a.Length)];
            int num = 0;
            foreach (byte[] array2 in arrays)
            {
                Buffer.BlockCopy(array2, 0, array, num, array2.Length);
                num += array2.Length;
            }
            return array;
        }

        public static string fromArray(object[] param1)
        {
            if(param1 == null)
            {
                return "";
            }
            string loc2 = "";
            foreach(object loc3 in param1)
            {
                loc2 = loc2 + fromObjectInner(loc3);
            }
            return loc2;
        }
        public static string fromObjectInner(object param1)
        {
            if (param1 == null || param1 is TicketHeader)
            {
                return "";
            }
            if (param1 is int || param1 is string)
            {
                return param1.ToString();
            }
            if(param1 is double || param1 is float)
            {
                return fromNumber(Convert.ToInt32(param1));
            }
            if(param1 is bool)
            {
                return !(bool)param1 ? "True" : "False";
            }
            if(param1 is object[])
            {
                return fromArray(param1 as object[]);
            }
            return "";
        }
        public static string fromObject(object param1)
        {
            string loc4 = null;
            object loc5 = null;
            var checkedObjects = new Dictionary<object, object> { };
            if(param1 == null)
            {
                return "";
            }
            return "";
        }
        public static string fromByteArray(byte[] param1)
        {
            int loc2 = 20;
            if(param1.Length <= loc2)
            {

            }
            byte[] loc3;
            int loc4 = param1.Length / loc2;
            int loc5 = 0;
            while(loc5 < loc2)
            {
                
            }
            return "";
        }

        public static string fromNumber(double param1)
        {
            string loc2 = param1.ToString();
            int loc3 = loc2.IndexOf(".");
            if(loc3 >= 0 && loc2.Length > loc3 + 5)
            {
                return loc2.Substring(0, loc3 + 5);
            }
            return loc2;
        }
    }
}
