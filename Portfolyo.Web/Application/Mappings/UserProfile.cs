using AutoMapper;
using Portfolyo.Web.Application.DTOs.Users;
using Portfolyo.Web.Core.Entities;

namespace Portfolyo.Web.Application.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>();

        CreateMap<UserCreateDto, User>()
            // Parola burada DEĞİL, UserService içinde hash'lenerek atanır.
            .ForMember(destination => destination.PasswordHash, options => options.Ignore())
            .ForMember(destination => destination.Id, options => options.Ignore())
            .ForMember(destination => destination.CreatedAt, options => options.Ignore())
            .ForMember(destination => destination.UpdatedAt, options => options.Ignore())
            .ForMember(destination => destination.LastLoginAt, options => options.Ignore())
            .ForMember(destination => destination.ProfileImagePath, options => options.Ignore());

        // Mevcut kaydın üzerine yazılır; parola ve tarih alanlarına dokunulmaz.
        CreateMap<UserUpdateDto, User>()
            .ForMember(destination => destination.PasswordHash, options => options.Ignore())
            .ForMember(destination => destination.CreatedAt, options => options.Ignore())
            .ForMember(destination => destination.UpdatedAt, options => options.Ignore())
            .ForMember(destination => destination.LastLoginAt, options => options.Ignore());
    }
}
