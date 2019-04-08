using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var suppliersJson = File.ReadAllText(@"D:\programming\Softuni\C-Sharp-DB-Fundementals\Databases Advanced - Entity Framework\10.JsonProcessing\CarDealer\Datasets\suppliers.json");

            var partsJson = File.ReadAllText(@"D:\programming\Softuni\C-Sharp-DB-Fundementals\Databases Advanced - Entity Framework\10.JsonProcessing\CarDealer\Datasets\parts.json");

            var carsJson = File.ReadAllText(@"D:\programming\Softuni\C-Sharp-DB-Fundementals\Databases Advanced - Entity Framework\10.JsonProcessing\CarDealer\Datasets\cars.json");

            var customersJson = File.ReadAllText(@"D:\programming\Softuni\C-Sharp-DB-Fundementals\Databases Advanced - Entity Framework\10.JsonProcessing\CarDealer\Datasets\customers.json");

            var salesJson = File.ReadAllText(@"D:\programming\Softuni\C-Sharp-DB-Fundementals\Databases Advanced - Entity Framework\10.JsonProcessing\CarDealer\Datasets\sales.json");

            var context = new CarDealerContext();

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            ImportSuppliers(context, suppliersJson);
            ImportParts(context, partsJson);
            ImportCars(context, carsJson);
            ImportCustomers(context, customersJson);
            Console.WriteLine(ImportSales(context, salesJson));

            Console.WriteLine(GetSalesWithAppliedDiscount(context));
        }

        public static string ImportSuppliers(CarDealerContext context, string
inputJson)
        {
            var Suppliers = JsonConvert.DeserializeObject<List<Supplier>>(inputJson)
                .ToList();

            context.Suppliers.AddRange(Suppliers);
            context.SaveChanges();
            return $"Successfully imported { Suppliers.Count}";
        }

        public static string ImportParts(CarDealerContext context, string
inputJson)
        {
            var Parts = JsonConvert.DeserializeObject<List<Part>>(inputJson)
                .Where(x=>context.Suppliers.Any(s=>s.Id == x.SupplierId))
                .ToList();

            context.Parts.AddRange(Parts);
            context.SaveChanges();
            return $"Successfully imported { Parts.Count}.";
        }

        public static string ImportCars(CarDealerContext context, string
inputJson)
        {
            var values = JsonConvert.DeserializeObject<List<ImportCarDto>>(inputJson)
                .ToList();

            var cars = new List<Car>();
            var partsCars = new List<PartCar>();

            var partsCount = context.Parts.Count();

            foreach(var value in values)
            {
                //Make new car with json car values
                var car = new Car()
                {
                    Make = value.Make,
                    Model = value.Model,
                    TravelledDistance = value.TravelledDistance
                };

                //traverse the car's part ids' in the json values
                foreach(var id in value.PartsId.Distinct())
                {
                    //do a check
                    if(id<=partsCount)
                    {
                        //add car-parts many-to-many
                        partsCars.Add(new PartCar()
                        {
                            Car = car,
                            PartId = id
                        });
                    }
                }

                cars.Add(car);
            }
            context.Cars.AddRange(cars);
            context.SaveChanges();
            context.PartCars.AddRange(partsCars);
            context.SaveChanges();
            
            return $"Successfully imported { cars.Count}.";
        }

        public static string ImportCustomers(CarDealerContext context, string
inputJson)
        {
            var users = JsonConvert.DeserializeObject<List<Customer>>(inputJson)
                .ToList();

            context.Customers.AddRange(users);
            context.SaveChanges();
            return $"Successfully imported { users.Count}.";
        }

        public static string ImportSales(CarDealerContext context, string
inputJson)
        {
            var sales = JsonConvert.DeserializeObject<List<Sale>>(inputJson)
                .ToList();

            var youngDrivers = context.Customers
                .Where(x => x.IsYoungDriver)
                .Select(x => x.Id)
                .ToArray();

            foreach (var sale in sales)
            {
                if (youngDrivers.Contains(sale.CustomerId))
                {
                    sale.Discount += 5;
                }
            }

            context.Sales.AddRange(sales);
            context.SaveChanges();
            return $"Successfully imported {sales.Count}.";
        }

        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .OrderBy(c => c.BirthDate).ThenBy(s => s.IsYoungDriver)
                .Select(x => new ExportCustomerDto
                {
                    Name = x.Name,
                    BirthDate = x.BirthDate.ToString("dd/MM/yyyy"),
                    IsYoungDriver = x.IsYoungDriver
                })
                .ToList();

            var json = JsonConvert.SerializeObject(customers,Formatting.Indented);

            return json;
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(c => c.Make == "Toyota")
                .OrderBy(e => e.Model).ThenByDescending(s => s.TravelledDistance)
                .Select(x => new
                {
                    x.Id,
                    x.Make,
                    x.Model,
                    x.TravelledDistance
                })
                .ToList();

            var json = JsonConvert.SerializeObject(cars, Formatting.Indented);

            return json;
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    PartsCount = x.Parts.Count
                })
                .ToList();

            var json = JsonConvert.SerializeObject(suppliers, Formatting.Indented);

            return json;
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var carParts = context.Cars
                .Select(x => new
                {
                    car = new
                    {
                        x.Make,
                        x.Model,
                        x.TravelledDistance
                    },
                    parts = x.PartCars.Select(p => new
                    {
                        p.Part.Name,
                        Price = p.Part.Price.ToString("F2")
                    }).ToArray()
                }).ToList();

            var json = JsonConvert.SerializeObject(carParts, Formatting.Indented);

            return json;
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(c => c.Sales.Count >= 1)
                .Select(x => new
                {
                    fullName = x.Name,
                    boughtCars = x.Sales.Count,
                    spentMoney = x.Sales.Sum(s=>s.Car.PartCars.Sum(y=>y.Part.Price))
                }).OrderByDescending(o=>o.spentMoney)
                .ThenByDescending(r=>r.boughtCars)
                .ToList();

            var json = JsonConvert.SerializeObject(customers, Formatting.Indented);
            return json;
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Take(10)
                .Select(x => new
                {
                    car = new
                    {
                        x.Car.Make,
                        x.Car.Model,
                        x.Car.TravelledDistance
                    },
                    customerName = x.Customer.Name,
                    Discount = x.Discount.ToString("F2"),
                    price = x.Car.PartCars.Sum(p => p.Part.Price).ToString("F2"),
                    
                    priceWithDiscount = (x.Car.PartCars.Sum(p => p.Part.Price) - (x.Car.PartCars.Sum(p => p.Part.Price) * (x.Discount/100.0m))).ToString("F2")

                }).ToList();

            var json = JsonConvert.SerializeObject(sales, Formatting.Indented);
            Console.WriteLine(json[116]);
            return json;
        }
    }
}