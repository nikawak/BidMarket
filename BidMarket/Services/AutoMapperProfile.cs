using AutoMapper;
using BidMarket.Models;
using Braintree;

namespace BidMarket.Services
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //this.CreateMap<UserLoginDTO, AppUser>();
            this.CreateMap<RegisterUser, AppUser>().ReverseMap();
            //    .ForMember(u => u.UserName, opt => opt.MapFrom(src => src.Email.Split('@', StringSplitOptions.RemoveEmptyEntries)[0]));
            this.CreateMap<LotDTO, Lot>();
            //    .ForMember(l => l.CurrentPrice, opt => opt.MapFrom(src => src.StartPrice));
            //this.CreateMap<CreateBetDTO, Bet>()
            //    .ForMember(b => b.Time, opt => opt.MapFrom(src => DateTime.Now.AddHours(3)));
            //this.CreateMap<ReviewDTO, Review>();
        }
    }
}
