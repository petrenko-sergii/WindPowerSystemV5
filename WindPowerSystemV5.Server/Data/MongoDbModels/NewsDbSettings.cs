namespace WindPowerSystemV5.Server.Data.MongoDbModels;

public class NewsDbSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string NewsCollectionName { get; set; } = null!;
}
