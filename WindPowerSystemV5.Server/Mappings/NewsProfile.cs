using AutoMapper;
using WindPowerSystemV5.Server.ViewModels;
using WindPowerSystemV5.Server.Data.MongoDbModels;

namespace WindPowerSystemV5.Server.Mappings;

public class NewsProfile : Profile
{
    public NewsProfile()
    {
        CreateMap<NewsCreationRequest, News>();
        CreateMap<NewsUpdateRequest, News>();
    }
}
