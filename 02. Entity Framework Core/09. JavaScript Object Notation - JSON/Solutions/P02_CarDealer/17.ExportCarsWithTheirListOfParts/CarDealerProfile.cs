using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using CarDealer.DTO;
using CarDealer.Models;
using System.Globalization;
using CarDealer.DTO.CarsAndParts;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            //CreateMap<Customer, OrderedCustomersDTO>()
            //    .ForMember(to=>to.BirthDate, 
            //                opt =>opt.MapFrom
            //                (from=>from.BirthDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)));


            //CreateMap<Supplier, ExportLocalSuppliersDTO>()
            //    .ForMember(to => to.PartsCount,
            //                opt => opt.MapFrom
            //                (from => from.Parts.Count));

            CreateMap<PartCar, CarPartsDTO>()
                .ForMember(to => to.Car,
                            opt => opt.MapFrom
                            (from => from.Car))
                .ForMember(to => to.Parts,
                            opt => opt.MapFrom
                            (from => from.Part.PartCars));

            CreateMap<Car, CarsDTO>();
            CreateMap<Part, PartsDTO>();
            
        }
    }
}
