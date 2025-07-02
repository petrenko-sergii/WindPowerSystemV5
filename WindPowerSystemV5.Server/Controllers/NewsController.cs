using Microsoft.AspNetCore.Mvc;
using WindPowerSystemV5.Server.Data.MongoDbModels;
using WindPowerSystemV5.Server.Services.Interfaces;
using WindPowerSystemV5.Server.ViewModels;

namespace WindPowerSystemV5.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NewsController : ControllerBase
{
    private readonly INewsService _newsService;

    public NewsController(INewsService newsService)
    {
        _newsService = newsService;
    }

    [HttpGet]
    public async Task<List<News>> Get()
    {
        return await _newsService.Get();
    }

    [HttpGet("{id}")]
    public async Task<News?> Get(string id)
    {
        return await _newsService.Get(id);
    }

    [HttpPost]
    public async Task<string> Create([FromBody] NewsCreationRequest news)
    {
        return await _newsService.Create(news);
    }

    [HttpPut("{id}")]
    public async Task Update(string id, [FromBody] NewsUpdateRequest news)
    {
         await _newsService.Update(id, news);
    }

    [HttpDelete("{id}")]
    public async Task Delete(string id)
    {
        await _newsService.Delete(id);
    }
}
