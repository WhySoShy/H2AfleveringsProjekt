using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H2AfleveringsProjekt.Services.Models;

namespace H2AfleveringsProjekt.Data.Interface
{
    public interface IParking
    {

        /// <returns>Et af 3 følgende objekter (BigCar, Car, Extendedcar)</returns>
        Parkinglot CheckIn(CarType type, string plate);
        /// <returns>Et af 3 følgende objekter (BigCar, Car, Extendedcar)</returns>
        Parkinglot CheckOut(string search);
        List<Parkinglot> GetListofCars(CarType type);
    }
}
