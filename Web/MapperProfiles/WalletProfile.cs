using AutoMapper;
using Web.Data.Entities;
using Web.Models.Wallets;

namespace Web.MapperProfiles
{
    public class WalletProfile : Profile
    {
        public WalletProfile()
        {
            CreateMap<Wallet, WalletInfoDto>(MemberList.Destination)
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.CurrencyCode));
        }
    }
}
