using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;

using CarDealer.Data;
using CarDealer.Models;
using CarDealer.Dtos.Export;
using CarDealer.Dtos.Import;

using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            Mapper.Initialize(cfg => cfg.AddProfile<CarDealerProfile>());

            using (var db = new CarDealerContext())
            {
                //db.Database.EnsureDeleted();
                //db.Database.EnsureCreated();

                //var inputXml = File.ReadAllText("./../../../Datasets/sales.xml");

                var result = GetSalesWithAppliedDiscount(db);

                Console.WriteLine(result);
            }
        }

        //P09
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportSupplierDto[]),
                new XmlRootAttribute("Suppliers"));

            ImportSupplierDto[] supplierDtos;

            using (var reader = new StringReader(inputXml))
            {
                supplierDtos = (ImportSupplierDto[])xmlSerializer.Deserialize(reader);
            }

            var suppliers = Mapper.Map<Supplier[]>(supplierDtos);

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Length}";
        }

        //P10
        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportPartDto[]),
                new XmlRootAttribute("Parts"));

            ImportPartDto[] partDtos;

            using (var reader = new StringReader(inputXml))
            {
                partDtos = ((ImportPartDto[])xmlSerializer
                    .Deserialize(reader))
                    .Where(p => context.Suppliers.Any(s => s.Id == p.SupplierId))
                    .ToArray();
            }

            var parts = Mapper.Map<Part[]>(partDtos);

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Length}";
        }

        //P11
        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportCarDto[]),
                new XmlRootAttribute("Cars"));

            ImportCarDto[] carDtos;

            using (var reader = new StringReader(inputXml))
            {
                carDtos = (ImportCarDto[])xmlSerializer
                    .Deserialize(reader);
            }

            List<Car> cars = new List<Car>();
            List<PartCar> partCars = new List<PartCar>();

            foreach (var carDto in carDtos)
            {
                var car = new Car()
                {
                    Make = carDto.Make,
                    Model = carDto.Model,
                    TravelledDistance = carDto.TravelledDistance
                };

                var parts = carDto
                    .Parts
                    .Where(pdto => context.Parts.Any(p => p.Id == pdto.Id))
                    .Select(p => p.Id)
                    .Distinct();

                foreach (var partId in parts)
                {
                    var partCar = new PartCar()
                    {
                        PartId = partId,
                        Car = car
                    };

                    partCars.Add(partCar);
                }

                cars.Add(car);
            }

            context.Cars.AddRange(cars);
            context.PartCars.AddRange(partCars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }

        //P12
        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportCustomerDto[]),
                new XmlRootAttribute("Customers"));

            ImportCustomerDto[] customerDtos;

            using (var reader = new StringReader(inputXml))
            {
                customerDtos = (ImportCustomerDto[])xmlSerializer.Deserialize(reader);
            }

            var customers = Mapper.Map<Customer[]>(customerDtos);

            context.Customers.AddRange(customers);

            context.SaveChanges();

            return $"Successfully imported {customers.Length}";
        }

        //P13
        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportSaleDto[]),
                new XmlRootAttribute("Sales"));

            ImportSaleDto[] saleDtos;

            using (var reader = new StringReader(inputXml))
            {
                saleDtos = ((ImportSaleDto[])xmlSerializer.Deserialize(reader))
                    .Where(s => context.Cars.Any(c => c.Id == s.CarId))
                    .ToArray();
            }

            var sales = Mapper.Map<Sale[]>(saleDtos);

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Length}";
        }

        //P14
        public static string GetCarsWithDistance(CarDealerContext context)
        {
            StringBuilder result = new StringBuilder();

            var cars = context.Cars
                .Where(c => c.TravelledDistance > 2000000)
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Take(10)
                .ProjectTo<ExportCarsWithDistanceDto>()
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportCarsWithDistanceDto[]),
                new XmlRootAttribute("cars"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            using (var writer = new StringWriter(result))
            {
                xmlSerializer.Serialize(writer, cars, namespaces);
            }

            return result.ToString().TrimEnd();
        }

        //P15
        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(c => c.Make == "BMW")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .ProjectTo<ExportCarsBmwDto>()
                .ToArray();

            var xmlSerializar = new XmlSerializer(typeof(ExportCarsBmwDto[]),
                new XmlRootAttribute("cars"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            var result = new StringBuilder();

            using (var writer = new StringWriter(result))
            {
                xmlSerializar.Serialize(writer, cars, namespaces);
            }

            return result.ToString().TrimEnd();
        }

        //P16
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            StringBuilder result = new StringBuilder();

            var suppliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .ProjectTo<ExportLocalSuppliersDto>()
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportLocalSuppliersDto[]),
                new XmlRootAttribute("suppliers"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            using (var writer = new StringWriter(result))
            {
                xmlSerializer.Serialize(writer, suppliers, namespaces);
            }

            return result.ToString().TrimEnd();
        }

        //P17
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            StringBuilder result = new StringBuilder();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportCarDto[]),
                new XmlRootAttribute("cars"));

            var cars = context.Cars
                .OrderByDescending(c => c.TravelledDistance)
                .ThenBy(c => c.Model)
                .Take(5)
                .ProjectTo<ExportCarDto>()
                .ToArray();

            foreach (var car in cars)
            {
                car.Parts = car.Parts
                    .OrderByDescending(p => p.Price)
                    .ToArray();
            }

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            using (var writer = new StringWriter(result))
            {
                xmlSerializer.Serialize(writer, cars, namespaces);
            }

            return result.ToString().TrimEnd();
        }


        //P18
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(c => c.Sales.Count >= 1)
                .ProjectTo<ExportCustomerDto>()
                .OrderByDescending(c => c.SpentMoney)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportCustomerDto[]),
               new XmlRootAttribute("customers"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            var result = new StringBuilder();

            using (var writer = new StringWriter(result))
            {
                xmlSerializer.Serialize(writer, customers, namespaces);
            }

            return result.ToString().TrimEnd();
        }

        //P19
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .ProjectTo<ExportSalesDto>()
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportSalesDto[]),
                new XmlRootAttribute("sales"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            var result = new StringBuilder();

            using (var writer = new StringWriter(result))
            {
                xmlSerializer.Serialize(writer, sales, namespaces);
            }

            return result.ToString().TrimEnd();
        }
    }
}