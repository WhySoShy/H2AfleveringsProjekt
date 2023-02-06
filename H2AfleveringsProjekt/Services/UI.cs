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
            for (int i = 1; i <= 10; i++)
                _parking.CheckIn(CarType.Car, $"car{i}");

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
                    return;
                }catch(OverflowException er)
                {
                    Console.WriteLine(er.Message);
                    Console.ReadLine();
                    return;
                }
                catch (Exception) { }
            }
        }
        public async Task RegisterCar()
        { 
            try
            {
                Console.Clear();
                CarType type = await ChooseCarType();
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
            while(true)
            {
                Console.Write("Enter your TicketID or numberplate: ");
                string search = Console.ReadLine();
                Console.Clear();

                if (String.IsNullOrEmpty(search))
                {
                    DesignError("Your search field cannot be empty!");
                    continue;
                }
                var obj = _parking.FindCarAsync<ICar>(search);
                if (obj == null)
                {
                    DesignError("Could not find your car");
                    continue;
                }
                try
                {
                    WashType type = await ChooseWashType();
                    _parking.WashCar(type, obj.ticket);
                    break;
                }catch { }
            }
        }
        public void GetWashList()
        {
            Console.WriteLine($"Estimated waiting time for hall1: {_parking.EstimatedTime(_parking.WashHall1).Minutes} minutes");
            Console.WriteLine($"Estimated waiting time for hall2: {_parking.EstimatedTime(_parking.WashHall2).Minutes} minutes");
        }

        #region Private & Protected methods
        /// <summary>
        /// Displays your error message in red and changes the text to white again.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private async Task DesignError(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.White;
        }
        private async Task ShowCarsAsync<T>(List<T> thisList) where T : ICar
        {
            if (!thisList.Any(x => x.ticket != null))
            {
                Console.WriteLine($"There is no cars of this type.");
                return;
            }
            foreach (var car in thisList.Where(x => x.ticket != null).ToList())
            {
                Console.WriteLine($"---------[  {car.ParkingSpot}  ]---------");
                Console.WriteLine($"TicketID:       {car.ticket.TicketID}");
                Console.WriteLine($"Platenumber:     {car.ticket.NumerberPlate}");
                Console.WriteLine($"Parked:          {car.ticket.ParkStart}");
                if (car.ticket.CarWash != null)
                {
                    Console.WriteLine($"---------[  CarWash  ]---------");
                    Console.WriteLine($"Wash type:   {car.ticket.CarWash.WashType}");
                    Console.WriteLine($"Wash end:    {car.ticket.CarWash.WashEnd}");
                    Console.WriteLine($"Wash price:  {car.ticket.CarWash.Price}$");
                }
                Console.WriteLine("\n");
            }
        }
        private async Task<CarType> ChooseCarType()
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
        private async Task<WashType> ChooseWashType()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Prices are per wash \n");
                Console.WriteLine($"A: {WashType.Economic}      | Time: {(int)WashType.Economic}minutes      | Price: {WashPrice.Economic}   | EST: {_parking.EstimatedTime().Minutes}m");
                Console.WriteLine($"B: {WashType.Basic}         | Time: {(int)WashType.Basic}minutes      | Price: {WashPrice.Basic}      | EST: {_parking.EstimatedTime().Minutes}m");
                Console.WriteLine($"C: {WashType.Premium}       | Time: {(int)WashType.Premium}minutes      | Price: {WashPrice.Premium}    | EST: {_parking.EstimatedTime().Minutes}m");
                ConsoleKeyInfo key = Console.ReadKey(true);

                switch(key.Key)
                {
                    case ConsoleKey.A:
                        return WashType.Economic;
                    case ConsoleKey.B:
                        return WashType.Basic;
                    case ConsoleKey.C:
                        return WashType.Premium;
                }
            }
        }
        #endregion
    }
}
