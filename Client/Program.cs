using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.Win32;

namespace Client
{
    class Program
    {
        static ClientData clientData = new ClientData();
        static List<string> messages = new List<string>();
        static RegistryKey key = Registry.CurrentUser;
        static void Main(string[] args)
        {
            try
            {
                if (key.GetSubKeyNames().ToList().Contains("ConsoleSize"))
                {
                    GetData();
                }
                else
                    CreateData();

                clientData.socket.Connect(clientData.iPEndPoint);

                clientData.GetMsg();
                GetData();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadLine();
        }
        static void GetData()
        {
            RegistryKey newKey = key.OpenSubKey("ConsoleSize", true);
            Console.WindowWidth = (int)newKey.GetValue("width");
            Console.WindowHeight = (int)newKey.GetValue("height");

            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(newKey.GetValue("address").ToString().Split(":")[0]), 
                int.Parse(newKey.GetValue("address").ToString().Split(":")[1]));

            clientData.iPEndPoint = ipEndPoint;

            Console.WriteLine("width: " + Console.WindowWidth);
            Console.WriteLine("height: " + Console.WindowHeight);
            Console.WriteLine("address: " + ipEndPoint);

            newKey.Close();
        }
        static void CreateData()
        {
            RegistryKey newKey = key.CreateSubKey("ConsoleSize", true);
            newKey.SetValue("width", Console.WindowWidth);
            newKey.SetValue("height", Console.WindowHeight);
            newKey.SetValue("address", (IPEndPoint)clientData.iPEndPoint);
            newKey.Close();
        }
    }
}
