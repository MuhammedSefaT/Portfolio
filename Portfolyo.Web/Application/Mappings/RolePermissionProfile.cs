using AutoMapper;
using Portfolyo.Web.Application.DTOs.RolePermissions;
using Portfolyo.Web.Core.Entities;

namespace Portfolyo.Web.Application.Mappings;

public class RolePermissionProfile : Profile
{
    public RolePermissionProfile()
    {
        CreateMap<RolePermission, RolePermissionDto>();
    }
}
