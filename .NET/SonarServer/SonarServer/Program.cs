using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;

namespace SonarServer
{
    class Program
    {
        const byte DataSampleStartMarker = 255;
        static List<byte> rawSonarDataBuffer = new List<byte>();

        static void Main(string[] args)
        {
            Console.WriteLine("*** Sonar - sample code from morzel.net blog post ***");

            try
            {
                using (SerialPort sp = new SerialPort())
                {
                    sp.PortName = "COM3";
                    sp.ReceivedBytesThreshold = 3;
                    sp.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

                    sp.Open();

                    Console.WriteLine("Serial port opened!");

                    using (WebApp.Start<Startup>("http://localhost:8080/"))
                    {
                        Console.WriteLine("Server running!");

                        Console.ReadKey();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;

                TextWriter errorWriter = Console.Error;
                errorWriter.WriteLine(string.Format("Something went wrong :( {0}, {1} ", Environment.NewLine, ex));

                Console.ResetColor();
                Console.ReadKey();
            }
        }

        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;

            int count = sp.BytesToRead;
            byte[] data = new byte[count];
            sp.Read(data, 0, count);

            rawSonarDataBuffer.AddRange(data);

            Console.WriteLine("Data received from Arduino. Bytes count=" + count);
            ProcessSonarData();
        }

        private static void ProcessSonarData()
        {
            if (rawSonarDataBuffer.Count >= 3)
            {
                int lastUsedIndex = 0;
                int index = rawSonarDataBuffer.IndexOf(DataSampleStartMarker);
                var sonarDataForClients = new List<SonarData>();

                while (index != -1 && index < rawSonarDataBuffer.Count - 2)
                {
                    byte angle = rawSonarDataBuffer[index + 1];
                    byte distance = rawSonarDataBuffer[index + 2];

                    if (angle != DataSampleStartMarker && distance != DataSampleStartMarker)
                    {
                        Console.WriteLine(string.Format("Sondar data sample found (angle={0}, distance={1})", angle, distance));
                        sonarDataForClients.Add(new SonarData() { Angle = angle, Distance = distance });
                        lastUsedIndex = index;
                    }

                    index = rawSonarDataBuffer.IndexOf(DataSampleStartMarker, Math.Min(index + 1, rawSonarDataBuffer.Count - 1));
                }

                rawSonarDataBuffer.RemoveRange(0, lastUsedIndex + 3);

                if (sonarDataForClients.Count > 0)
                {
                    SendSonarDataToClients(sonarDataForClients);
                }
            }
        }

        private static void SendSonarDataToClients(List<SonarData> sonarDataForClients)
        {
            var hub = GlobalHost.ConnectionManager.GetHubContext<SonarHub>();
            hub.Clients.All.sonarData(sonarDataForClients);

            Console.WriteLine("Sonar data items sent to clients. Samples count=" + sonarDataForClients.Count);
        }
    }
}
