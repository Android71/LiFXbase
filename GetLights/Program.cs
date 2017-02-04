using LiFX_Lib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Model_RT;
using static System.Console;
using System.Threading;

namespace GetLights
{
    class Program
    {
        static void Main(string[] args)
        {
            
            List<LightDescriptor> lights = LiFXUtils.GetLights();
            List<LiFXControlChannel> channels = new List<LiFXControlChannel>();
            

            WriteLine("Найдены LiFX лампы \r\n");
            foreach (LightDescriptor ld in lights)
                WriteLine($"{ld.Address}  {ld.MacStr}\r\n");

            //WriteLine("Ввернуть лампу, нажать Enter\r\n");

            //ReadLine();

            //List<LightDescriptor> newLights = LiFXUtils.GetNewLights(lights);

            //if (newLights.Count != 0)
            //{
            //    WriteLine("Новые LiFX лампы\r\n");
            //    foreach (LightDescriptor ld in newLights)
            //        WriteLine($"{ld.Address}  {ld.MacStr}");
            //}
            //else
            //    WriteLine("Новых ламп не найдено\r\n");

            //lights.AddRange(newLights);


            LiFX_Service service = new LiFX_Service();
            if (lights.Count == 2)
            {
                // тестирование LiFXControlChannel для 2х ламп LiFX
                LightDescriptor ld = lights[0];
                LiFXControlChannel channel = new LiFXControlChannel($"<Params IP = \"{ld.Address}\"  MAC = \"{ld.MAC}\"/>");
                channel.CSService = service;
                channels.Add(channel);

                ld = lights[1];
                channel = new LiFXControlChannel($"<Params IP = \"{ld.Address}\"  MAC = \"{ld.MAC}\"/>");
                channels.Add(channel);
                channel.CSService = service;

                double hue = 0.0;
                while (true)
                {
                    channels[0].SetRGB(hue, 1.0, 0.2);
                    channels[1].SetRGB(hue, 1.0, 0.2);
                    //Console.WriteLine($"Hue {hue}");
                    Thread.Sleep(25);

                    hue += 1.0;
                    if (hue >= 360.0)
                        hue = 0.0;
                }
            }

            if (lights.Count == 1)
            {
                // тестирование LiFXControlChannel для лампы LiFX
                LightDescriptor ld = lights[0];
                LiFXControlChannel channel = new LiFXControlChannel($"<Params IP = \"{ld.Address}\"  MAC = \"{ld.MAC}\"/>");
                channel.CSService = service;
                channels.Add(channel);

                //ld = lights[1];
                //channel = new LiFXControlChannel($"<Params IP = \"{ld.Address}\"  MAC = \"{ld.MAC}\"/>");
                //channels.Add(channel);
                //channel.CSService = service;

                double hue = 0.0;
                while (true)
                {
                    channels[0].SetRGB(hue, 1.0, 0.2);
                    //channels[1].SetRGB(hue, 1.0, 0.2);
                    //Console.WriteLine($"Hue {hue}");
                    Thread.Sleep(40);

                    hue += 1.0;
                    if (hue >= 360.0)
                        hue = 0.0;
                }
            }

            WriteLine("\r\nEnter - to Exit");
            ReadLine();
        }
    }
}
