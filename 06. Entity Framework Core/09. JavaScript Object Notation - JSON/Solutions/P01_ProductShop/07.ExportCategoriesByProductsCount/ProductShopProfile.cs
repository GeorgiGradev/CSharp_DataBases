using AutoMapper;
using ProductShop.Data;
using ProductShop.DTO.Users;
using ProductShop.Models;
using System.Linq;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            this.CreateMap<Product, UserSoldProductDTO>()
                 .ForMember(x => x.BuyersFirstName, y => y.MapFrom(x => x.Buyer.FirstName))
                 .ForMember(x => x.BuyerLastName, y => y.MapFrom(x => x.Buyer.LastName));

            this.CreateMap<User, UsersWithSoldProductsDTO>()
                .ForMember(x => x.SoldProducts, y => y.MapFrom(x => x.ProductsSold.Where(p => p.Buyer != null)));

        }
        public class AutoMapperConfiguration
        {
            public static void Configure()
            {
                Mapper.Initialize(x =>  x.AddProfile<ProductShopProfile>());

                Mapper.Configuration.AssertConfigurationIsValid();
            }
        }
    }
}
