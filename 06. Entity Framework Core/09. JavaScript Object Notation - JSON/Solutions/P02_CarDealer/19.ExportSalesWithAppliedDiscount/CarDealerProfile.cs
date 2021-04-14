using AutoMapper;
using CarDealer.DTO;
using CarDealer.Models;
using CarDealer.DTO.CarsAndParts;
using System.Linq;
using CarDealer.DTO.TotalSales;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {

            //CreateMap<Customer, TotalSalesByCustomerDTO>()
            //    .ForMember(to => to.Name,
            //                opt => opt.MapFrom
            //                (from => from.Name));

            //CreateMap<Car, TotalSalesByCustomerDTO>()
            //    .ForMember(to => to.BoughtCars,
            //                opt => opt.MapFrom
            //                (from => from.Sales.Count));

            //CreateMap<PartCar, TotalSalesByCustomerDTO>()
            //               .ForMember(to => to.SpentMoney,
            //                opt => opt.MapFrom
            //                (from => from.Part.Price * from.Part.Quantity));
        }
    }
}
