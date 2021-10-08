using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Win32;

namespace Server
{
    class Program
    {
        static ServerData serverData = new ServerData();
        static RegistryKey key = Registry.CurrentUser;
        static void Main(string[] args)
        {
            Console.WriteLine("Start server...");
            try
            {
                serverData.socket.Bind(serverData.iPEndPoint);
                serverData.socket.Listen(10);

                Task.Factory.StartNew(() => Connect());
               
                while(true)
                {
                    Menu();
                }
               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadLine();
        }
        static void Connect()
        {
            while (true)
            {
                serverData.socketClient = serverData.socket.Accept();
                serverData.socketClientsList.Add(serverData.socketClient);
            }
        }
        static void Menu()
        {
            Console.Clear();
            Console.WriteLine("Choose:\n1.Show all clients\n2.Choose client id\n3.Set value");
            int choice = int.Parse(Console.ReadLine());
            switch (choice)
            {
                case 1:
                    ShowAllClients();
                    break;
                case 2:
                    ChooseClientId();
                    break;
                case 3:
                    SetValue();
                    break;
                default:
                    break;
            }
        }
        static void ShowAllClients()
        {
            Console.Clear();
            lock (serverData.socketClientsList)
            {
                for (int i = 0; i < serverData.socketClientsList.Count(); i++)
                {
                    Console.WriteLine($"<ID: {i}> " + $"Connected: {serverData.socketClientsList[i].Connected}");
                }
            }
            Console.ReadLine();
        }
        static void ChooseClientId()
        {
            Console.Clear();
            for (int i = 0; i < serverData.socketClientsList.Count(); i++)
            {
                Console.WriteLine($"<ID: {i}> " + $"Connected: {serverData.socketClientsList[i].Connected}");
            }
            Console.WriteLine("Choose id:");
            serverData.socketClient = serverData.socketClientsList[int.Parse(Console.ReadLine())];
        }
        static void SetValue()
        {
            Console.Clear();
            RegistryKey newKey = key.CreateSubKey("ConsoleSize", true);

            Console.WriteLine("Enter width and heiht, and address:");
            var width = Console.ReadLine();
            var height = Console.ReadLine();

            newKey.DeleteValue("width");
            newKey.DeleteValue("height");
            newKey.DeleteValue("address");

            newKey.SetValue("width", int.Parse(width));
            newKey.SetValue("height", int.Parse(height));
            newKey.SetValue("address", Console.ReadLine());
            newKey.Close();

            serverData.socketClient.Send(Encoding.Unicode.GetBytes("Size changed!"));
        }
    }
}