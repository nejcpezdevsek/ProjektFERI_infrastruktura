using System;
using System.IO;
using System.IO.Ports;
using System.Net;

namespace NRSVmesnik
{
    class Program
    {
        static void Main(string[] args)
        {
            //cesta
            char[] buffer = new char[10];
            string name;
            try
            {
                SerialPort mySerialPort = new SerialPort("COM8", 9600);
                mySerialPort.Open();
                Console.WriteLine("Čakam podatke...");
                mySerialPort.Read(buffer, 0, 1);
                Console.WriteLine("Podatki prejeti!");

                var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:3000/bumps");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    Console.WriteLine("Vpišite ime poti: ");
                    name = Console.ReadLine();
                    string json = "{\"name\":\"" + name + "\"," +
                                    "\"bumps\":\"" + Convert.ToInt32(buffer[0]) + "\"}";

                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                }
                mySerialPort.Close();
            }
            catch (IOException ex)
            {
                return;
            }
            Console.ReadKey();
        }
    }
}
