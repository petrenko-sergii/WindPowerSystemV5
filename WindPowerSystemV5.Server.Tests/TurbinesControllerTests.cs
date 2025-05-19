using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WindPowerSystemV5.Server.Controllers;
using WindPowerSystemV5.Server.Data.Models;
using WindPowerSystemV5.Server.Data;

namespace WindPowerSystemV5.Server.Tests;

public class TurbinesControllerTests
{
    [Fact]
    public async Task Get_TurbinesExistInDatabase_ReturnsAllTurbinesWithTypeInfo()
    {
        // Arrange
        using ApplicationDbContext context = CreateContext();

        context.Add(new TurbineType()
        {
            Id = 1,
            Manufacturer = "TestManufacturer",
            Model = "TestModel",
            Capacity = 800
        });

        context.Add(new Turbine()
        {
            Id = 1,
            SerialNumber = "SN-001",
            Status = Data.Enums.TurbineStatus.ReadyNoWind,
            TurbineTypeId = 1
        });

        context.Add(new Turbine()
        {
            Id = 2,
            SerialNumber = "SN-002",
            Status = Data.Enums.TurbineStatus.Run,
            TurbineTypeId = 1
        });

        context.SaveChanges();

        var logger = new LoggerFactory().CreateLogger<TurbinesController>();
        var controller = new TurbinesController(context, logger);

        // Act
        var turbines = await controller.Get();

        // Assert
        Assert.NotNull(turbines.Value);
        Assert.Equal(2, turbines.Value.Count());
        Assert.All(turbines.Value, t =>
        {
            Assert.Equal("TestManufacturer", t.Manufacturer);
            Assert.Equal("TestModel", t.Model);
        });
    }

    [Fact]
    public async Task Get_TurbineExistsInDatabase_ReturnsTurbine()
    {
        // Arrange
        using ApplicationDbContext context = CreateContext();

        context.Add(new Turbine()
        {
            Id = 3,
            SerialNumber = "SN-003",
            Status = Data.Enums.TurbineStatus.Installed,
            TurbineTypeId = 1
        });
        context.SaveChanges();

        var logger = new LoggerFactory().CreateLogger<TurbinesController>();
        var controller = new TurbinesController(context, logger);

        // Act
        var existingTurbine = (await controller.Get(3)).Value;
        var notExistingTurbine = (await controller.Get(4)).Value;

        // Assert
        Assert.NotNull(existingTurbine);
        Assert.Null(notExistingTurbine);
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "WIND-POWER-SYSTEM")
            .Options;

        return new ApplicationDbContext(options);
    }
}
