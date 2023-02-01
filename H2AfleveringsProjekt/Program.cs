using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H2AfleveringsProjekt.Services;

namespace H2AfleveringsProjekt
{
    internal class Program
    {
        static void Main(string[] args)
        {
            UI ui = new UI();



            Running();
            void CreateMenu()
            {
                Console.WriteLine("Velkommen til parkering \n");
                Console.WriteLine("A: Register your car");
                Console.WriteLine("B: Unregister your car");
                Console.WriteLine("Z: See every registed cars");
            }

            void Running()
            {
                do
                {
                    Console.Clear();
                    CreateMenu();
                    var key = Console.ReadKey(true);
                    switch (key.Key)
                    {
                        case ConsoleKey.A:
                            ui.RegisterCar();
                            break;
                        case ConsoleKey.B:
                            ui.UnregisterCar();
                            break;
                        case ConsoleKey.Z:
                            ui.ShowTicketlist();
                            break;
                        default:
                            continue;
                    }

                    Console.WriteLine("\nPress any button to exit");
                    Console.ReadKey(true);
                } while (true);
            }

        }
    }
}
