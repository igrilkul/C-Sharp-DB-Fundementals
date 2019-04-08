using AutoMapper;
using CarDealer.Data;
using CarDealer.Dtos.Export;
using CarDealer.Dtos.Import;
using CarDealer.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            Mapper.Initialize(x =>
            {
                x.AddProfile<CarDealerProfile>();
            });

            var suppliersXml = File.ReadAllText("../../../Datasets/suppliers.xml");

            var partsXml = File.ReadAllText("../../../Datasets/parts.xml");

            var carsXml = File.ReadAllText("../../../Datasets/cars.xml");

            var customersXml = File.ReadAllText("../../../Datasets/customers.xml");

            var salesXml = File.ReadAllText("../../../Datasets/sales.xml");
            
            //var optionsBuilder = new DbContextOptionsBuilder();
            //optionsBuilder.EnableSensitiveDataLogging()
            // .UseSqlServer(@"Server=DESKTOP-A97NGR1\SQLEXPRESS;Database=CarDealer;Trusted_Connection=True;"); 
            //var options = optionsBuilder.Options;

            using (var context = new CarDealerContext())
                //{
                //    context.Database.EnsureDeleted();
                //    context.Database.EnsureCreated();

                //    System.Console.WriteLine(ImportSuppliers(context,suppliersXml));
                //    System.Console.WriteLine(ImportParts(context,partsXml));
                //    System.Console.WriteLine(ImportCars(context,carsXml));
                //    System.Console.WriteLine(ImportCustomers(context,customersXml));
                //    System.Console.WriteLine(ImportSales(context,salesXml));

                System.Console.WriteLine(GetSalesWithAppliedDiscount(context));
            }


        

        public static string ImportSuppliers(CarDealerContext context, string
inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportSupplierDto[]), new XmlRootAttribute("Suppliers"));

            var suppliersDto = (ImportSupplierDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var suppliers = new List<Supplier>();

            foreach (var supplierDto in suppliersDto)
            {
                var supplier = Mapper.Map<Supplier>(supplierDto);
                suppliers.Add(supplier);
            }

            context.Suppliers.AddRange(suppliers);
            var count = context.SaveChanges();
            return $"Successfully imported {count}";
        }

        public static string ImportParts(CarDealerContext context, string
inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportPartDto[]), new XmlRootAttribute("Parts"));
            var supplierCount = context.Suppliers.Count();

            var partsDto = (ImportPartDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var parts = new List<Part>();

            foreach (var partDto in partsDto)
            {
                if(partDto.SupplierId>supplierCount)
                {
                    continue;
                }

                var part = Mapper.Map<Part>(partDto);
                parts.Add(part);
            }

            context.Parts.AddRange(parts);
            var count = context.SaveChanges();
            return $"Successfully imported {count}";
        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCarDto[]), new XmlRootAttribute("Cars"));
            var partsCount = context.Parts.Count();

            var carsDto = (ImportCarDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var cars = new List<Car>();
            var partsCars = new List<PartCar>();

            foreach (var carDto in carsDto)
            {
                var car = new Car();
                car.Make = carDto.Make;
                car.Model = carDto.Model;
                car.TravelledDistance = (long)carDto.TraveledDistance;
               
                foreach(var partId in carDto.parts.Select(x=>x.PartId).Distinct())
                {
                    if (partId <= partsCount)
                    {
                        var partCar = new PartCar();
                        partCar.Car = car;
                        partCar.PartId = partId;
                        
                        //car.PartCars.Add(partCar);
                        partsCars.Add(partCar);
                    }
                }

                cars.Add(car);
                
            }

            

            context.Cars.AddRange(cars);
            var carCount = context.SaveChanges();
            context.PartCars.AddRange(partsCars);
            var partCarCount = context.SaveChanges();

            return $"Successfully imported {carCount}";
        }

        public static string ImportCustomers(CarDealerContext context, string
inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCustomerDto[]), new XmlRootAttribute("Customers"));

            var customersDto = (ImportCustomerDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var customers = new List<Customer>();

            foreach (var customerDto in customersDto)
            {
                var customer = Mapper.Map<Customer>(customerDto);
                customers.Add(customer);
            }

            context.Customers.AddRange(customers);
            var count = context.SaveChanges();
            return $"Successfully imported {count}";
        }

        public static string ImportSales(CarDealerContext context, string
inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportSaleDto[]), new XmlRootAttribute("Sales"));

            var salesDto = (ImportSaleDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var sales = new List<Sale>();

            int carCount = context.Cars.Count();
            int customerCount = context.Customers.Count();

            foreach (var saleDto in salesDto)
            {
                if(!(context.Cars.Find(saleDto.CarId) == null))
                {
                    var sale = Mapper.Map<Sale>(saleDto);
                    sales.Add(sale);
                }
            } 

            context.Sales.AddRange(sales);
            var count = context.SaveChanges();
            return $"Successfully imported {count}";
        }

        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(c => c.TravelledDistance > 2000000)
                .OrderBy(w => w.Make).ThenBy(r => r.Model)
                .Take(10)
                .Select(x=>new ExportCarsWithDistanceDto
                {
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance
                })
                .ToArray();

            XmlSerializer serializer = new XmlSerializer(typeof(ExportCarsWithDistanceDto[]), new XmlRootAttribute("cars"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("","")
            });

            serializer.Serialize(new StringWriter(sb), cars, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var cars = context.Cars
               .Where(c => c.Make == "BMW")
               .OrderBy(w => w.Model).ThenByDescending(r => r.TravelledDistance)
               .Select(x => new ExportBmwCarsDto
               {
                   Id = x.Id,
                   Model = x.Model,
                   TravelledDistance = x.TravelledDistance
               })
               .ToArray();

            XmlSerializer serializer = new XmlSerializer(typeof(ExportBmwCarsDto[]), new XmlRootAttribute("cars"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("","")
            });

            serializer.Serialize(new StringWriter(sb), cars, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(x => new ExportLocalSupplierDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    PartsCount = x.Parts.Count
                }).ToArray();

            XmlSerializer serializer = new XmlSerializer(typeof(ExportLocalSupplierDto[]), new XmlRootAttribute("suppliers"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("","")
            });

            serializer.Serialize(new StringWriter(sb), suppliers, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var carsParts = context.Cars
                .OrderByDescending(e=>e.TravelledDistance).ThenBy(t=>t.Model)
                .Take(5)
                .Select(x => new ExportCarPartsCarDto
                {
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance,
                    parts = x.PartCars.Select(p => new ExportCarPartsPartDto
                    {
                        Name = p.Part.Name,
                        Price = p.Part.Price
                    }).OrderByDescending(y=>y.Price)
                    .ToArray()
                }).ToArray();

            XmlSerializer serializer = new XmlSerializer(typeof(ExportCarPartsCarDto[]), new XmlRootAttribute("cars"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("","")
            });

            serializer.Serialize(new StringWriter(sb), carsParts, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(c => c.Sales.Count >= 1)
                .Select(x => new ExportCustomerSalesDto
                {
                    FullName = x.Name,
                    BoughtCars = x.Sales.Count,
                    SpentMoney = x.Sales.Sum(r => r.Car.PartCars.Sum(t => t.Part.Price))
                })
                .OrderByDescending(i=>i.SpentMoney)
                .ToArray();

            XmlSerializer serializer = new XmlSerializer(typeof(ExportCustomerSalesDto[]), new XmlRootAttribute("customers"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("","")
            });

            serializer.Serialize(new StringWriter(sb), customers, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Select(x => new ExportSaleDto
                {
                    Car = new ExportSaleCarDto
                    {
                        Make = x.Car.Make,
                        Model = x.Car.Model,
                        TravelledDistance = x.Car.TravelledDistance
                    },

                    Discount = x.Discount,
                    CustomerName = x.Customer.Name,
                    Price = x.Car.PartCars.Sum(p => p.Part.Price),
                    PriceWithDiscount = (x.Car.PartCars.Sum(p => p.Part.Price) - (x.Car.PartCars.Sum(p => p.Part.Price) * (x.Discount / 100.0m))).ToString("0.####")
                }).ToArray();

            XmlSerializer serializer = new XmlSerializer(typeof(ExportSaleDto[]), new XmlRootAttribute("sales"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("","")
            });

            serializer.Serialize(new StringWriter(sb), sales, namespaces);

            return sb.ToString().TrimEnd();
        }
    }

    
}