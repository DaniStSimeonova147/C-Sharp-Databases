using AutoMapper;
using CarDealer.Dtos.Export;
using CarDealer.Dtos.Import;
using CarDealer.Models;
using System.Linq;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            //P09
            this.CreateMap<ImportSupplierDto, Supplier>();

            //P10
            this.CreateMap<ImportPartDto, Part>();

            //P12
            this.CreateMap<ImportCustomerDto, Customer>();

            //P13
            this.CreateMap<ImportSaleDto, Sale>();

            //P14
            this.CreateMap<Car, ExportCarsWithDistanceDto>();

            //P15
            this.CreateMap<Car, ExportCarsBmwDto>();

            //P16
            this.CreateMap<Supplier, ExportLocalSuppliersDto>();

            //P17
            this.CreateMap<Part, ExportCarPartDto>();

            this.CreateMap<Car, ExportCarDto>()
                .ForMember(x => x.Parts, y => y
                .MapFrom(s => s.PartCars.Select(pc => pc.Part)
                                         //.OrderByDescending(p => p.Price)
                                         ));

            //P18
            this.CreateMap<Customer, ExportCustomerDto>()
                .ForMember(x => x.BoughtCars, y => y.MapFrom(c => c.Sales.Count))
                .ForMember(x => x.SpentMoney, y => y.MapFrom(c => c.Sales.Sum(s => s.Car.PartCars.Sum(p => p.Part.Price))));

            //P19
            this.CreateMap<Car, ExportCarForSaleDto>();

            this.CreateMap<Sale, ExportSalesDto>()
                .ForMember(x => x.CustomerName, y => y.MapFrom(s => s.Customer.Name))
                .ForMember(x => x.Price, y => y.MapFrom(s => s.Car.PartCars.Sum(pc => pc.Part.Price)))
                .ForMember(x => x.PriceWithDiscount, y => y.MapFrom(s => s.Car.PartCars.Sum(pc => pc.Part.Price) - (s.Car.PartCars.Sum(pc => pc.Part.Price) * s.Discount /100 )));
                

        }
    }
}
