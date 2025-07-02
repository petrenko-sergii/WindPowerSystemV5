namespace WindPowerSystemV5.Server.Data.MongoDbModels;

public class NewsComment
{
    public string UserId { get; set; } = null!;

    public string Comment { get; set; } = null!;

    public DateTime CreatedDt { get; set; }
}
