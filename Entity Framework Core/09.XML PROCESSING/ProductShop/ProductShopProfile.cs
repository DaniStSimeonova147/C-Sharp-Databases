using AutoMapper;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using System.Linq;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            //P01
            this.CreateMap<ImportUserDto, User>();

            //P02
            this.CreateMap<ImportProductDto, Product>();

            //P03
            this.CreateMap<ImportCategoryDto, Category>();

            //P04
            this.CreateMap<ImportCategoryProductDto, CategoryProduct>();

            //P05
            this.CreateMap<Product, ExportProductsInRangeDto>()
                .ForMember(x => x.Buyer, y => y.MapFrom(p => $"{p.Buyer.FirstName} {p.Buyer.LastName}"));

            //P06
            this.CreateMap<Product, ExportSoldProductsDto>();

            this.CreateMap<User, ExportUserWithSoldProductsDto>()
                .ForMember(x => x.SoldProducts, y => y.MapFrom(u => u.ProductsSold));

            //P07
            this.CreateMap<Category, ExportCategoriesByProductsCountDto>()
                .ForMember(x => x.Count, y => y.MapFrom(c => c.CategoryProducts.Count))
                .ForMember(x => x.AvgPrice, y => y.MapFrom(c => c.CategoryProducts.Average(cp => cp.Product.Price)))
                .ForMember(x => x.TotalRevenue, y => y.MapFrom(c => c.CategoryProducts.Sum(cp => cp.Product.Price)));

            //P08

        }
    }
}
