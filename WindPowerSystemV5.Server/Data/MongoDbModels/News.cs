using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WindPowerSystemV5.Server.Data.MongoDbModels;

public class News
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Author { get; set; } = null!;

    public List<string> Chapters { get; set; } = [];

    public DateTime CreatedDt { get; set; }
    
    public DateTime? UpdatedDt { get; set; }

    public List<NewsComment> Comments { get; set; } = [];

    public int Likes { get; set; }
}
