using AutoMapper;
using AutoMapper.QueryableExtensions;
using ProductShop.Data;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            Mapper.Initialize(cfg => cfg.AddProfile<ProductShopProfile>());

            using (var db = new ProductShopContext())
            {
                //db.Database.EnsureDeleted();
                //db.Database.EnsureCreated();

               // var inputXml = File.ReadAllText("./../../../Datasets/categories-products.xml");

                var result = GetUsersWithProducts(db);

                Console.WriteLine(result);
            }
        }

        //P01
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportUserDto[]),
                new XmlRootAttribute("Users"));

            ImportUserDto[] userDtos;

            using (var reader = new StringReader(inputXml))
            {
                userDtos = (ImportUserDto[])xmlSerializer.Deserialize(reader);
            }

            var users = Mapper.Map<User[]>(userDtos);

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Length}";
        }

        //P02
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            var xmlSerilizer = new XmlSerializer(typeof(ImportProductDto[]),
                new XmlRootAttribute("Products"));

            ImportProductDto[] productDtos;

            using (var reader = new StringReader(inputXml))
            {
                productDtos = (ImportProductDto[])xmlSerilizer.Deserialize(reader);
            }

            var products = Mapper.Map<Product[]>(productDtos);

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Length}";
        }

        //P03
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportCategoryDto[]),
                new XmlRootAttribute("Categories"));

            ImportCategoryDto[] categoryDtos;

            using (var reader = new StringReader(inputXml))
            {
                categoryDtos = ((ImportCategoryDto[])xmlSerializer
                    .Deserialize(reader))
                    .Where(c => c.Name != null)
                    .ToArray();
            }

            var categories = Mapper.Map<Category[]>(categoryDtos);

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Length}";
        }

        //P04
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportCategoryProductDto[]),
                new XmlRootAttribute("CategoryProducts"));

            ImportCategoryProductDto[] categoryProductDtos;

            using (var reader = new StringReader(inputXml))
            {
                categoryProductDtos = ((ImportCategoryProductDto[])xmlSerializer
                    .Deserialize(reader))
                    .Where(cp => context.Categories.Any(c => c.Id == cp.CategoryId) &&
                                 context.Products.Any(p => p.Id == cp.ProductId))
                    .ToArray();
            }

            var categoryProducts = Mapper.Map<CategoryProduct[]>(categoryProductDtos);

            context.CategoryProducts.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Length}";
        }

        //P05
        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Take(10)
                .ProjectTo<ExportProductsInRangeDto>()
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportProductsInRangeDto[]),
                new XmlRootAttribute("Products"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            var result = new StringBuilder();

            using (var writer = new StringWriter(result))
            {
                xmlSerializer.Serialize(writer, products, namespaces);
            }

            return result.ToString().TrimEnd();
        }

        //P06
        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Count > 1)
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Take(5)
                .ProjectTo<ExportUserWithSoldProductsDto>()
                .ToArray();

            var xmlSerialize = new XmlSerializer(typeof(ExportUserWithSoldProductsDto[]),
                new XmlRootAttribute("Users"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(String.Empty, string.Empty);

            var result = new StringBuilder();

            using (var writer = new StringWriter(result))
            {
                xmlSerialize.Serialize(writer, users, namespaces);
            }

            return result.ToString().TrimEnd();
        }

        //P07
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .ProjectTo<ExportCategoriesByProductsCountDto>()
                .OrderByDescending(c => c.Count)
                .ThenBy(c => c.TotalRevenue)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportCategoriesByProductsCountDto[]),
                new XmlRootAttribute("Categories"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            var result = new StringBuilder();

            using (var writer = new StringWriter(result))
            {
                xmlSerializer.Serialize(writer, categories, namespaces);
            }

            return result.ToString().TrimEnd();
        }

        //P08
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Any())
               .OrderByDescending(p => p.ProductsSold.Count())
               .Select(u => new ExportUserDto
               {
                   FirstName = u.FirstName,
                   LastName = u.LastName,
                   Age = u.Age,
                   SoldProducts = new ExportProductDto
                   {
                       Count = u.ProductsSold.Count(),
                       Products = u.ProductsSold
                       .Select(p => new ExportSoldProductsDto
                       {
                           Name = p.Name,
                           Price = p.Price
                       })
                       .OrderByDescending(p => p.Price)
                       .ToArray()
                   }
               })
               .Take(10)
               .ToArray();

            var userAndProductDto = new ExportUserAndProductDto()
            {
                Count = context.Users.Count(u => u.ProductsSold.Any()),
                Users = users
            };

            var xmlSerializer = new XmlSerializer(typeof(ExportUserAndProductDto),
                new XmlRootAttribute("Users"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            var result = new StringBuilder();

            using (var writer = new StringWriter(result))
            {
                xmlSerializer.Serialize(writer, userAndProductDto, namespaces);
            }

            return result.ToString().TrimEnd();
        }
    }
}