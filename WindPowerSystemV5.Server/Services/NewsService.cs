using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WindPowerSystemV5.Server.Config;
using WindPowerSystemV5.Server.Data.MongoDbModels;
using WindPowerSystemV5.Server.Services.Interfaces;
using WindPowerSystemV5.Server.Utils.Exceptions;
using WindPowerSystemV5.Server.ViewModels;

namespace WindPowerSystemV5.Server.Services;

public class NewsService : INewsService
{
    private readonly IMapper _mapper;
    private readonly IMongoCollection<News> _newsCollection;

    public NewsService(
        IMapper mapper,
        IOptions<NewsDbSettings> newsDbSettings)
    {
        _mapper = mapper;

        var mongoClient = new MongoClient(
            newsDbSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            newsDbSettings.Value.DatabaseName);

        _newsCollection = mongoDatabase.GetCollection<News>(CollectionNames.News);
    }

    public async Task<List<News>> Get()
    {
        return await _newsCollection.Find(_ => true).ToListAsync();
    }

    public async Task<News?> Get(string id)
    {
        var news = await _newsCollection.Find(n => n.Id == id).FirstOrDefaultAsync();

        if (news == null) 
        {
            throw new NotFoundException($"News with ID {id} is not found.");
        }

        return news;
    }

    public async Task<string> Create(NewsCreationRequest news)
    {
        if(news.Chapters.Count == 0)
        {
            throw new BadRequestException("News must have at least one chapter.");
        }

        var newsToCreate= _mapper.Map<News>(news);

        newsToCreate.CreatedDt = DateTime.UtcNow;
        await _newsCollection.InsertOneAsync(newsToCreate);

        return $"News with id: '{newsToCreate.Id}' has been created.";
    }

    public async Task Update(string id, NewsUpdateRequest updatedNews)
    {
        if (updatedNews.Chapters.Count == 0)
        {
            throw new BadRequestException("News must have at least one chapter.");
        }

        var existingNews = await _newsCollection.Find(n => n.Id == id).FirstOrDefaultAsync();

        if (existingNews == null)
        {
            throw new NotFoundException($"News with ID {id} is not found.");
        }

        _mapper.Map(updatedNews, existingNews);
        existingNews.UpdatedDt = DateTime.UtcNow;

        await _newsCollection.ReplaceOneAsync(n => n.Id == id, existingNews);
    }

    public async Task Delete(string id)
    {
        var result = await _newsCollection.DeleteOneAsync(n => n.Id == id);

        if (result.DeletedCount == 0)
        {
            throw new NotFoundException($"News with ID {id} is not found.");
        }
    }
}
