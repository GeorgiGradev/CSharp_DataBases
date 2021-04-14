using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using CarDealer.DTO;
using CarDealer.Models;
using System.Globalization;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            CreateMap<Customer, OrderedCustomersDTO>()
                .ForMember(to=>to.BirthDate, 
                            opt =>opt.MapFrom
                            (from=>from.BirthDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)));
            //c.BirthDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
        }
    }
}
