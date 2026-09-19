using AutoMapper;
using Portfolyo.Web.Application.DTOs.UserRoles;
using Portfolyo.Web.Core.Entities;

namespace Portfolyo.Web.Application.Mappings;

public class UserRoleProfile : Profile
{
    public UserRoleProfile()
    {
        CreateMap<UserRole, UserRoleDto>();

        CreateMap<UserRoleCreateDto, UserRole>()
            .ForMember(destination => destination.Id, options => options.Ignore())
            .ForMember(destination => destination.AssignedAt, options => options.Ignore())
            .ForMember(destination => destination.CreatedAt, options => options.Ignore())
            .ForMember(destination => destination.UpdatedAt, options => options.Ignore());

        // Atama tarihi güncellemede korunur.
        CreateMap<UserRoleUpdateDto, UserRole>()
            .ForMember(destination => destination.AssignedAt, options => options.Ignore())
            .ForMember(destination => destination.CreatedAt, options => options.Ignore())
            .ForMember(destination => destination.UpdatedAt, options => options.Ignore());
    }
}
