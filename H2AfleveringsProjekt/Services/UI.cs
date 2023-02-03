using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H2AfleveringsProjekt.Data.Interface;
using H2AfleveringsProjekt.Data.Methods;
using H2AfleveringsProjekt.Services.Models;

// TODO: Tilføje try catches, til at tage imod Exceptions!

namespace H2AfleveringsProjekt.Services
{
    public class UI
    {
        private readonly IParking _parking;
        public UI()
        {
            ServiceProvider services = new ServiceCollection().AddSingleton<IParking, Parking>().BuildServiceProvider();
            _parking = services.GetRequiredService<IParking>();

            for (int i = 0; i < 10; i++)
                _parking.ListOfCars.Add(new Car() { ParkingSpot = i + 1 }) ;
            for (int i = 0; i < 5; i++)
                _parking.ListOfExtendedCars.Add(new ExtendedCar() { ParkingSpot = i + 1 });
            for (int i = 0; i < 3; i++)
                _parking.ListOfBigCars.Add(new BigCar() { ParkingSpot = i + 1 });

            Task.Run(() => _parking.RunCarWash());
        }
        public async Task ShowTicketlist()
        {

            while(true)
            {
                Console.Clear();
                Console.WriteLine("What type parking spot would you like to see?\n");
                Console.WriteLine($"A: {CarType.Car}");
                Console.WriteLine($"B: {CarType.ExtendedCar}");
                Console.WriteLine($"C: {CarType.BigCar}");
                var key = Console.ReadKey(true);
                Console.Clear();
                switch (key.Key)
                {
                    case ConsoleKey.A:
                        await ShowCarsAsync(_parking.ListOfCars);
                        return;
                    case ConsoleKey.B:
                        await ShowCarsAsync(_parking.ListOfExtendedCars);
                        return;
                    case ConsoleKey.C:
                        await ShowCarsAsync(_parking.ListOfBigCars);
                        return;
                }


            }
        }
        public async Task UnregisterCar()
        {
            while(true)
            {
                Console.Write("Enter your plate number: ");
                string plateNumber = Console.ReadLine().ToLower();
                Console.Clear();
                if (String.IsNullOrEmpty(plateNumber))
                    continue;
                try
                {
                    KeyValuePair<int, int> info = await _parking.CheckOut(plateNumber);
                    Console.WriteLine($"You have paid: {info.Value} $ for {info.Key} hours");
                    break;
                }catch(OverflowException er)
                {
                    Console.WriteLine(er.Message);
                    break;
                }
                catch (Exception) { }
            }
        }
        public async Task RegisterCar()
        { 
            try
            {
                Console.Clear();
                CarType type = await ChooseType();
                string plate = String.Empty;
                 while (String.IsNullOrEmpty(plate))
                {
                    Console.Write("Enter your numberplate: ");
                    plate = Console.ReadLine().Trim();
                    Console.Clear();
                    Console.WriteLine($"You have gotten parking slot {await _parking.CheckIn(type, plate)}");
                }

            }
            catch(Exception er) 
                { Console.WriteLine(er.Message); }
            
        }
        public async Task WashCar()
        {
            _parking.WashCar(WashType.Basic, new Ticket() { NumerberPlate = "", TicketID = 1, ParkStart = DateTime.Now, Type = CarType.Car});
            _parking.WashCar(WashType.Economic, new Ticket() { NumerberPlate = "", TicketID = 2, ParkStart = DateTime.Now.AddSeconds(1), Type = CarType.Car});
            _parking.WashCar(WashType.Basic, new Ticket() { NumerberPlate = "", TicketID = 3, ParkStart = DateTime.Now.AddSeconds(2), Type = CarType.Car});
        }

        #region Private & Protected methods
        private async Task ShowCarsAsync<T>(List<T> thisList) where T : ICar
        {
            if (!thisList.Any(x => x.ticket != null))
            {
                Console.WriteLine($"There is no cars of this type.");
                return;
            }
            foreach (var car in thisList.Where(x => x.ticket != null).ToList())
            {
                Console.WriteLine($"---------[  {car.ticket.TicketID}  ]---------");
                Console.WriteLine($"Platenumber:     {car.ticket.NumerberPlate}");
                Console.WriteLine($"Parked:          {car.ticket.ParkStart}");
                Console.WriteLine($"Parking spot:    {car.ParkingSpot}\n");
            }
        }
        private async Task<CarType> ChooseType()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Prices are per hour \n");
                Console.WriteLine($"A: {CarType.Car}           | Price (hour): {(int)CarType.Car} $");
                Console.WriteLine($"B: {CarType.ExtendedCar}   | Price (hour): {(int)CarType.ExtendedCar} $");
                Console.WriteLine($"C: {CarType.BigCar}        | Price (hour): {(int)CarType.BigCar} $");
                

                var key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.A:
                        return CarType.Car;
                    case ConsoleKey.B:
                        return CarType.ExtendedCar;
                    case ConsoleKey.C:
                        return CarType.BigCar;
                }
            }
        }
        #endregion
    }
}
