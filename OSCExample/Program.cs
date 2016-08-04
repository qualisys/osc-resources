// Qualisys Track Manager OSC realtime streaming example
// For more information, http://www.github.com/qualisys, http://www.qualisys.com
// This examples uses SharpOSC component from http://github.com/ValdemarOrn/SharpOSC
using System;
using System.Text;
using SharpOSC;

namespace OSCExample
{
    class Program
    {
        static void Main(string[] args)
        {
            // OSC message/bundle callback handler.
            // In this example it just outputs all the sent responses and arguments to the console.
            HandleOscPacket packetCallback = delegate (OscPacket packet)
            {
                if (packet is OscMessage)
                {
                    var message = packet as OscMessage;
                    StringBuilder builder = new StringBuilder();
                    builder.AppendLine(message.Address);
                    foreach (var argument in message.Arguments)
                    {
                        builder.AppendFormat(" {0} ", argument);
                    }
                    builder.AppendLine();
                    Console.WriteLine(builder.ToString());
                }
                else if (packet is OscBundle)
                {
                    StringBuilder builder = new StringBuilder();
                    var bundle = packet as OscBundle;
                    foreach (var message in bundle.Messages)
                    {
                        builder.AppendLine(message.Address);
                        foreach (var argument in message.Arguments)
                        {
                            builder.AppendFormat(" {0} ", argument);
                        }
                        builder.AppendLine();
                        Console.WriteLine(builder.ToString());
                    }
                }
            };

            // Create a OSC message listener object, lets print out all qtm server replies in a callback method
            var listener = new UDPListener(55555, packetCallback);

            // Create a OSC message sender object
            var sender = new SharpOSC.UDPSender("127.0.0.1", 22225);

            // Connect
            sender.Send(new SharpOSC.OscMessage("/qtm", "connect", 55555));

            // Ask for QTM version
            sender.Send(new SharpOSC.OscMessage("/qtm", "qtmversion"));

            // Ask for general camera system and 3d marker parameters (returned as xml)
            sender.Send(new SharpOSC.OscMessage("/qtm", "getparameters", "General", "3d"));

            // Start streaming of frames (just 3d and 6dof data)
            sender.Send(new SharpOSC.OscMessage("/qtm", "StreamFrames", "AllFrames", "3d", "6d"));

            // Pressing the escape key will stop streaming and disconnect from qtm
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    if (Console.ReadKey(false).Key == ConsoleKey.Escape)
                        break;
                }
            }

            // Stop streaming
            sender.Send(new SharpOSC.OscMessage("/qtm", "StreamFrames", "Stop"));

            // Disconnect from qtm
            sender.Send(new SharpOSC.OscMessage("/qtm", "disconnect"));
        }
    }
}
