using EasyNetQ;
using Messages;
using System;

namespace Subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var bus = RabbitHutch.CreateBus("host=localhost"))
            {
                bus.Subscribe<TextMessage>("test", HandleTextMessage);
                Console.WriteLine("Listening for message. Hit <return> to quit.");
                Console.ReadLine();

            }
        }

        private static void HandleTextMessage(TextMessage textMessage)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"Got message: {textMessage.Text}");
            Console.ResetColor();
        }
    }
}
