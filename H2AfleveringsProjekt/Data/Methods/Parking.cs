using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H2AfleveringsProjekt.Services.Models;
using H2AfleveringsProjekt.Data.Interface;

namespace H2AfleveringsProjekt.Data.Methods
{
    public class Parking : IParking
    {
        public Parkinglot CheckIn(CarType type, string plate)
        {
            Parkinglot parking = null;
            List<Car> listOfCars = null;
            List<ExtendedCar> listOfExtendedCars = null;
            List<BigCar> listOfBigCars = null;
            switch(type)
            {
                case CarType.Car:
                    parking = new Car();
                    listOfCars = parking.ListOfCars;
                    break;
                case CarType.ExtendedCar:
                    parking = new ExtendedCar();
                    listOfExtendedCars = parking.ListOfExtendedCars;
                    break;
                case CarType.BigCar:
                    parking = new BigCar();
                    listOfBigCars = parking.ListOfBigCars;
                    break;
            }
            
            return parking;
        }
        public Parkinglot CheckOut(string search)
        {
            return null;
        }
        public List<Parkinglot> GetListofCars(CarType type)
        {
            return null;
        }
    }
}
