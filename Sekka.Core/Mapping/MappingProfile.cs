using AutoMapper;

namespace Sekka.Core.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Entity → DTO mappings are registered in Sekka.Application.Mapping.ApplicationMappingProfile
        // This profile is kept for any Core-level mappings (DTO → DTO transformations)
    }
}
