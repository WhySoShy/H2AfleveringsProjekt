using H2AfleveringsProjekt.Data.Interface;
using H2AfleveringsProjekt.Data.Methods;
using H2AfleveringsProjekt.Services.Models;
using H2AfleveringsProjekt.Services;
using Microsoft.Extensions.DependencyInjection;

namespace H2AfleveringsProjekt.Test
{
    public class UnitTest1
    {

        [Fact]
        public async Task CheckInCar()
        {
            ServiceProvider services = new ServiceCollection().AddSingleton<IParking, Parking>().BuildServiceProvider();
            IParking _parking = services.GetRequiredService<IParking>();

            //Arrange
            string expected = "car";
            await _parking.CheckIn(CarType.Car, expected);
            //Act
            string actual = _parking.FindCar("car").ticket.NumerberPlate;
            //Assert
            Assert.Equal(expected, actual);
        }
        [Fact]
        public async Task OverFlowCheckIn()
        {
            ServiceProvider services = new ServiceCollection().AddSingleton<IParking, Parking>().BuildServiceProvider();
            IParking _parking = services.GetRequiredService<IParking>();

            //Arrange
            Exception exception = await Assert.ThrowsAsync<OverflowException>(async () =>
            {
                for (int i = 0; i < 11; i++)
                    await _parking.CheckIn(CarType.Car, $"car{i}");
            });
            //Assert
            Assert.NotNull(exception);

        }
        [Fact]
        public async Task MultipleNumberPlates()
        {
            IParking _parking = new ServiceCollection().AddSingleton<IParking, Parking>().BuildServiceProvider().GetRequiredService<IParking>();

            //Arrange
            Exception exception = await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _parking.CheckIn(CarType.Car, "car");
                await _parking.CheckIn(CarType.Car, "car");
            });

            //Assert
            Assert.NotNull(exception);
        }
        [Fact]
        public async Task CarNotFoundCheckOut()
        {
            IParking _parking = new ServiceCollection().AddSingleton<IParking, Parking>().BuildServiceProvider().GetRequiredService<IParking>();

            //Arrange
            Exception exception = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _parking.CheckOut("car"));
            //Assert
            Assert.NotNull(exception);

        }
    }
}