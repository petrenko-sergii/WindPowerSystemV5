using Microsoft.EntityFrameworkCore;
using WindPowerSystemV5.Server.Data;
using WindPowerSystemV5.Server.Data.Models;
using WindPowerSystemV5.Server.Data.Repositories;
using WindPowerSystemV5.Server.ViewModels;

namespace WindPowerSystemV5.Server.Tests;

public class TurbineTypeRepositoryTests
{
    private static ApplicationDbContext CreateSeededContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var context = new ApplicationDbContext(options);

        context.TurbineTypes.AddRange(
            new TurbineType { Id = 1, Manufacturer = "Vestas", Model = "V39/600", Capacity = 600, FileName = "a.jpg" },
            new TurbineType { Id = 2, Manufacturer = "Siemens Gamesa", Model = "SG 2.1-114", Capacity = 1140, FileName = "b.pdf" },
            new TurbineType { Id = 3, Manufacturer = "Nordex", Model = "N43", Capacity = 800, FileName = "c.jpg" },
            new TurbineType { Id = 4, Manufacturer = "Enercon", Model = "E-44", Capacity = 900, FileName = "d.jpg" });
        context.SaveChanges();

        return context;
    }

    [Theory]
    [InlineData("Vest")]
    [InlineData("vestas")]
    [InlineData("VEST")]
    public async Task Search_ManufacturerContains_FiltersByManufacturerIgnoringCase(string manufacturer)
    {
        // Arrange
        using var context = CreateSeededContext();
        var repository = new TurbineTypeRepository(context);

        // Act
        var result = await repository.Search(new TurbineTypeSearchCriteria { Manufacturer = manufacturer });

        // Assert
        var turbineType = Assert.Single(result);
        Assert.Equal(1, turbineType.Id);
    }

    [Theory]
    [InlineData("n43")]
    [InlineData("N4")]
    public async Task Search_ModelContains_FiltersByModelIgnoringCase(string model)
    {
        // Arrange
        using var context = CreateSeededContext();
        var repository = new TurbineTypeRepository(context);

        // Act
        var result = await repository.Search(new TurbineTypeSearchCriteria { Model = model });

        // Assert
        var turbineType = Assert.Single(result);
        Assert.Equal(3, turbineType.Id);
    }

    [Fact]
    public async Task Search_CapacityRange_ReturnsOnlyTypesInsideRange()
    {
        // Arrange
        using var context = CreateSeededContext();
        var repository = new TurbineTypeRepository(context);

        // Act
        var result = await repository.Search(
            new TurbineTypeSearchCriteria { MinCapacity = 800, MaxCapacity = 900 });

        // Assert
        Assert.Equal([3, 4], result.Select(t => t.Id).OrderBy(id => id));
    }

    [Fact]
    public async Task Search_SortByCapacityDescending_ReturnsHighestFirst()
    {
        // Arrange
        using var context = CreateSeededContext();
        var repository = new TurbineTypeRepository(context);

        // Act
        var result = await repository.Search(new TurbineTypeSearchCriteria
        {
            SortBy = TurbineTypeSearchCriteria.SortByCapacity,
            SortDescending = true,
            Limit = 1
        });

        // Assert
        var turbineType = Assert.Single(result);
        Assert.Equal(2, turbineType.Id);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Search_EqualSortValues_OrdersTiesByIdAndTakesFirstDeterministically(bool descending)
    {
        // Arrange
        using var context = CreateSeededContext();
        context.TurbineTypes.AddRange(
            new TurbineType { Id = 5, Manufacturer = "Vestas", Model = "V47", Capacity = 900, FileName = "e.jpg" },
            new TurbineType { Id = 6, Manufacturer = "Vestas", Model = "V52", Capacity = 900, FileName = "f.jpg" });
        context.SaveChanges();
        var repository = new TurbineTypeRepository(context);

        // Act
        var result = await repository.Search(new TurbineTypeSearchCriteria
        {
            MinCapacity = 900,
            MaxCapacity = 900,
            SortBy = TurbineTypeSearchCriteria.SortByCapacity,
            SortDescending = descending
        });

        // Assert
        Assert.Equal([4, 5, 6], result.Select(t => t.Id));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public async Task Search_LimitBelowOne_IsClampedToOne(int limit)
    {
        // Arrange
        using var context = CreateSeededContext();
        var repository = new TurbineTypeRepository(context);

        // Act
        var result = await repository.Search(new TurbineTypeSearchCriteria { Limit = limit });

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task GetManufacturers_ReturnsDistinctNamesOrderedWithTypeCount()
    {
        // Arrange
        using var context = CreateSeededContext();
        context.TurbineTypes.Add(
            new TurbineType { Id = 5, Manufacturer = "Vestas", Model = "V90", Capacity = 3000, FileName = "e.jpg" });
        context.SaveChanges();
        var repository = new TurbineTypeRepository(context);

        // Act
        var result = await repository.GetManufacturers();

        // Assert
        Assert.Equal(["Enercon", "Nordex", "Siemens Gamesa", "Vestas"], result.Select(m => m.Manufacturer));
        Assert.Equal(2, result.Single(m => m.Manufacturer == "Vestas").TurbineTypeCount);
    }

    [Fact]
    public async Task Search_CancelledToken_ThrowsOperationCanceled()
    {
        // Arrange
        using var context = CreateSeededContext();
        var repository = new TurbineTypeRepository(context);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => repository.Search(new TurbineTypeSearchCriteria(), cts.Token));
    }

    [Fact]
    public async Task Count_IgnoresLimit()
    {
        // Arrange
        using var context = CreateSeededContext();
        var repository = new TurbineTypeRepository(context);
        var criteria = new TurbineTypeSearchCriteria { Limit = 1 };

        // Act
        var returned = await repository.Search(criteria);
        var total = await repository.Count(criteria);

        // Assert
        Assert.Single(returned);
        Assert.Equal(4, total);
    }

    [Fact]
    public async Task Search_MinTurbineQty_FiltersByInstalledTurbines()
    {
        // Arrange
        using var context = CreateSeededContext();
        context.Turbines.Add(new Turbine { Id = 1, SerialNumber = "SN-1", TurbineTypeId = 3 });
        context.SaveChanges();
        var repository = new TurbineTypeRepository(context);

        // Act
        var result = await repository.Search(new TurbineTypeSearchCriteria { MinTurbineQty = 1 });

        // Assert
        var turbineType = Assert.Single(result);
        Assert.Equal(3, turbineType.Id);
        Assert.Equal(1, turbineType.TurbineQty);
    }
}
