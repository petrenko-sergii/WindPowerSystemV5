# Tests

Applies to tests in `WindPowerSystemV5.Server.Tests`.

Always structure every test with Arrange-Act-Assert and mark the sections with comments:

```csharp
[Fact]
public async Task GetCity_CityExists_ReturnsCity()
{
    // Arrange
    ...

    // Act
    ...

    // Assert
    ...
}
```

- Use exactly `// Arrange`, `// Act` and `// Assert` (see `CitiesControllerTests.cs`).
- Add the comments to every new test, including `[Theory]` tests and tests that expect an exception.
- If a section is empty (e.g. nothing to arrange), omit that comment rather than leaving it blank.
- When the Act and Assert are one expression (e.g. `Assert.ThrowsAsync`), use `// Act & Assert`.
