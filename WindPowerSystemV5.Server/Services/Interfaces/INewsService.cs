using WindPowerSystemV5.Server.Data.MongoDbModels;
using WindPowerSystemV5.Server.ViewModels;

namespace WindPowerSystemV5.Server.Services.Interfaces;

public interface INewsService
{
    Task<List<News>> Get();

    Task<News?> Get(string id);

    Task<string> Create(NewsCreationRequest news);

    Task Update(string id, NewsUpdateRequest news);

    Task Delete(string id);
}
