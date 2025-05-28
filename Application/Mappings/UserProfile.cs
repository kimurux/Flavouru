using AutoMapper;
using Flavouru.Domain.Entities;
using Flavouru.Shared.DTOs;

namespace Flavouru.Application.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<RegisterUserDto, User>();
        }
    }
}

