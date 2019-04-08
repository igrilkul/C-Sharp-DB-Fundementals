using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.Dtos;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var usersJson = File.ReadAllText(@"D:\programming\Softuni\C-Sharp-DB-Fundementals\Databases Advanced - Entity Framework\10.JsonProcessing\ProductShop\Datasets\users.json");

            var productsJson = File.ReadAllText(@"D:\programming\Softuni\C-Sharp-DB-Fundementals\Databases Advanced - Entity Framework\10.JsonProcessing\ProductShop\Datasets\products.json");

            var categoriesJson = File.ReadAllText(@"D:\programming\Softuni\C-Sharp-DB-Fundementals\Databases Advanced - Entity Framework\10.JsonProcessing\ProductShop\Datasets\categories.json");

            var categoriesProductsJson = File.ReadAllText(@"D:\programming\Softuni\C-Sharp-DB-Fundementals\Databases Advanced - Entity Framework\10.JsonProcessing\ProductShop\Datasets\categories-products.json");

            var context = new ProductShopContext();
            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            //ImportUsers(context,usersJson);
            //ImportCategories(context, categoriesJson);
            //ImportProducts(context, productsJson);

            //ImportCategoryProducts(context, categoriesProductsJson);
            Console.WriteLine(GetUsersWithProducts(context));
        }

        public static string ImportUsers(ProductShopContext context, string
inputJson)
        {
            var users = JsonConvert.DeserializeObject<User[]>(inputJson)
                .Where(u => u.LastName != null && u.LastName.Length >= 3).ToArray();

            context.Users.AddRange(users);
            context.SaveChanges();
            string result = $"Successfully imported {users.Length}";
            return result;
        }

        public static string ImportProducts(ProductShopContext context, string
inputJson)
        {
            var products = JsonConvert.DeserializeObject<Product[]>(inputJson)
                .Where(p=>p.Name.Trim().Length>=3 && p.Name != null)
                .ToArray();

            context.Products.AddRange(products);
            context.SaveChanges();
            string result = $"Successfully imported {products.Length}";
            return result;
        }

        public static string ImportCategories(ProductShopContext context, string
inputJson)
        {
            var categories = JsonConvert.DeserializeObject<Category[]>(inputJson)
                .Where(u=> u.Name != null && u.Name.Length>=3 && u.Name.Length<=15).ToArray();

            context.Categories.AddRange(categories);
            context.SaveChanges();
            string result = $"Successfully imported {categories.Length}";
            return result;
        }

        public static string ImportCategoryProducts(ProductShopContext context,
string inputJson)
        {
            var cps = JsonConvert.DeserializeObject<CategoryProduct[]>(inputJson)
                .Where(c=>c.CategoryId!=null && c.ProductId!=null).ToArray();

            context.CategoryProducts.AddRange(cps);
            context.SaveChanges();
            string result = $"Successfully imported {cps.Length}";
            return result;
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products.Where(p => p.Price >= 500 && p.Price <= 1000)
                .Select(x=>new ProductDto{
                Name=x.Name,
                Price = x.Price,
                Seller = $"{x.Seller.FirstName} {x.Seller.LastName}"
            }).OrderBy(s=>s.Price).ToList();

            var productsJson = JsonConvert.SerializeObject(products, Formatting.Indented);
            return productsJson;
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => (u.ProductsSold.Any(s=>s.BuyerId != null)))
                .OrderBy(y=>y.LastName).ThenBy(e=>e.FirstName)
                .Select(x => new UserDto
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    SoldProducts = x.ProductsSold
                    .Where(s=>s.BuyerId!=null)
                    .Select(p => new ProductBuyerDto
                    {
                       Name = p.Name,
                       Price = p.Price,
                       BuyerFirstName = p.Buyer.FirstName,
                       BuyerLastName = p.Buyer.LastName
                    }).ToList()
                })
                .ToList();

            var usersJson = JsonConvert.SerializeObject(users, Formatting.Indented);
            return usersJson;
        }

        public static string GetCategoriesByProductsCount(ProductShopContext
context)
        {
            var categories = context.Categories
                .OrderByDescending(c => c.CategoryProducts.Count)
                .Select(x => new CategoryDto
                {
                    category = x.Name,
                    productsCount = x.CategoryProducts.Count,
                    averagePrice = Math.Round(x.CategoryProducts.Average(e => e.Product.Price),2).ToString(),

                    totalRevenue = Math.Round(x.CategoryProducts.Sum(e => e.Product.Price),2).ToString()
                }).ToList();

            var categoriesJson = JsonConvert.SerializeObject(categories, Formatting.Indented);

            return categoriesJson;
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var values = context.Users
                .Where(u => (u.ProductsSold.Any(s => s.BuyerId != null)))
                .Select(x => new
                {
                   firstName = x.FirstName,
                   lastName = x.LastName,
                   age = x.Age,

                    

                    soldProducts = new
                    {
                        count = x.ProductsSold
                    .Where(y => y.BuyerId != null)
                    .Count(),
                        products = x.ProductsSold
                    .Where(s => s.BuyerId != null)
                    .Select(p => new ProductBareDto
                    {
                        Name = p.Name,
                        Price = p.Price
                    }).ToList(),
                    }

                    

                }).OrderByDescending(g=>g.soldProducts.count)
                .ToList();

            var usersJsonClass = new
            {
                usersCount = values.Count,
                users = values
            };
            var usersJson = JsonConvert.SerializeObject(usersJsonClass, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            return usersJson;
        }

       
    }
}