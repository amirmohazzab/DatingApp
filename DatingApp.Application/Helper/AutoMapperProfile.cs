using AutoMapper;
using DatingApp.Application.Extensions;
using DatingApp.Domain.DTOs;
using DatingApp.Domain.Entities.Message;
using DatingApp.Domain.Entities.Photo;
using DatingApp.Domain.Entities.User;

namespace DatingApp.Api.Helper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, MemberDTO>()
                //.ForMember(x => x.Age, c => c.MapFrom(v => v.DateOfBirth.CalculateAge()))
                .ForMember(x => x.PhotoUrl, c => c.MapFrom(v => v.Photos.FirstOrDefault(b => b.IsMain).Url));

            CreateMap<RegisterDTO, User>();

            CreateMap<Photo, PhotoDTO>();
            CreateMap<MemberUpdateDto, User>();
            CreateMap<User, LikeDto>()
                .ForMember(x => x.PictureUrl, c => c.MapFrom(v => v.Photos.FirstOrDefault(b => b.IsMain).Url));

            CreateMap<Message, MessageDto>()
                .ForMember(x => x.SenderPhotoUrl, c => c.MapFrom(v => v.Sender.Photos.FirstOrDefault(b => b.IsMain).Url))
                .ForMember(x => x.ReceiverPhotoUrl, c => c.MapFrom(v => v.Receiver.Photos.FirstOrDefault(b => b.IsMain).Url));
            CreateMap<MessageDto, Message>();

        }
    }
}
