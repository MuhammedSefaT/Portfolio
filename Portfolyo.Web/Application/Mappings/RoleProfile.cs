using AutoMapper;
using Portfolyo.Web.Application.Common;
using Portfolyo.Web.Application.DTOs.Roles;
using Portfolyo.Web.Core.Entities;

namespace Portfolyo.Web.Application.Mappings;

public class RoleProfile : Profile
{
    public RoleProfile()
    {
        CreateMap<Role, RoleDto>();

        // NormalizedName her zaman Name'den üretilir, dışarıdan alınmaz.
        CreateMap<RoleCreateDto, Role>()
            .ForMember(destination => destination.NormalizedName,
                options => options.MapFrom(source => TextNormalizer.Normalize(source.Name)))
            .ForMember(destination => destination.Id, options => options.Ignore())
            .ForMember(destination => destination.CreatedAt, options => options.Ignore())
            .ForMember(destination => destination.UpdatedAt, options => options.Ignore());

        CreateMap<RoleUpdateDto, Role>()
            .ForMember(destination => destination.NormalizedName,
                options => options.MapFrom(source => TextNormalizer.Normalize(source.Name)))
            .ForMember(destination => destination.CreatedAt, options => options.Ignore())
            .ForMember(destination => destination.UpdatedAt, options => options.Ignore());
    }
}
