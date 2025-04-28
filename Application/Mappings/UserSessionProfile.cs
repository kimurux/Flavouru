using AutoMapper;
using Flavouru.Domain.Entities;
using Flavouru.Shared.DTOs;

namespace Flavouru.Application.Mappings
{
    public class UserSessionProfile : Profile
    {
        public UserSessionProfile()
        {
            CreateMap<UserSession, UserSessionDto>();
        }
    }
}

