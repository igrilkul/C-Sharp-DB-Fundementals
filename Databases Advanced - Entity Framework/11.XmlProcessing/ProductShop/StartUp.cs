using AutoMapper;
using ProductShop.Data;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            Mapper.Initialize(x => 
            {
                x.AddProfile<ProductShopProfile>();
            });

            var usersXml = File.ReadAllText("../../../Datasets/users.xml");
            var productsXml = File.ReadAllText("../../../Datasets/products.xml");
            var categoriesXml = File.ReadAllText("../../../Datasets/categories.xml");
            var categoriesProductsXml = File.ReadAllText("../../../Datasets/categories-products.xml");

            using(ProductShopContext context = new ProductShopContext())
            {
                //context.Database.EnsureDeleted();
                //context.Database.EnsureCreated();

                //System.Console.WriteLine(ImportUsers(context,usersXml));
                //System.Console.WriteLine(ImportProducts(context,productsXml));
                //System.Console.WriteLine(ImportCategories(context,categoriesXml));
                //System.Console.WriteLine(ImportCategoryProducts(context, categoriesProductsXml));

                System.Console.WriteLine(GetUsersWithProducts(context));
            }
        }

        public static string ImportUsers(ProductShopContext context, string
inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportUserDto[]), new XmlRootAttribute("Users"));

            var usersDto = (ImportUserDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var users = new List<User>();

            foreach(var userDto in usersDto)
            {
                var user = Mapper.Map<User>(userDto);
                users.Add(user);
            }

            context.Users.AddRange(users);
           var count = context.SaveChanges();
            return $"Successfully imported {count}";
        }

        public static string ImportProducts(ProductShopContext context, string
inputXml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ImportProductDto[]), new XmlRootAttribute("Products"));

            var productsDto = (ImportProductDto[])serializer.Deserialize(new StringReader(inputXml));

            var products = new List<Product>();

            foreach(var productDto in productsDto)
            {
                var product = Mapper.Map<Product>(productDto);
                products.Add(product);
            }

            context.Products.AddRange(products);
            var count = context.SaveChanges();
            return $"Successfully imported {count}";
        }

        public static string ImportCategories(ProductShopContext context, string
inputXml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ImportCategoryDto[]), new XmlRootAttribute("Categories"));

            var categoriesDto = (ImportCategoryDto[])serializer.Deserialize(new StringReader(inputXml));

            var categories = new List<Category>();

            foreach(var categoryDto in categoriesDto)
            {
                var category = Mapper.Map<Category>(categoryDto);
                categories.Add(category);
            }

            context.Categories.AddRange(categories);
            var count = context.SaveChanges();
            return $"Successfully imported {count}";
        }

        public static string ImportCategoryProducts(ProductShopContext context,
string inputXml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ImportCategoryProductDto[]), new XmlRootAttribute("CategoryProducts"));

            var categoriesProductsDto = (ImportCategoryProductDto[])serializer.Deserialize(new StringReader(inputXml));

            var categoriesProducts = new List<CategoryProduct>();

            foreach (var categoryProductDto in categoriesProductsDto)
            {
                var categoryProduct = Mapper.Map<CategoryProduct>(categoryProductDto);
                categoriesProducts.Add(categoryProduct);
            }

            context.CategoryProducts.AddRange(categoriesProducts);
            var count = context.SaveChanges();
            return $"Successfully imported {count}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .OrderBy(s => s.Price)
                .Select(p => new ExportProductsInRangeDto
                {
                    Name = p.Name,
                    Price = p.Price,
                    Buyer = p.Buyer.FirstName +" "+p.Buyer.LastName
                })
                .Take(10)
                .ToArray();

            XmlSerializer serializer = new XmlSerializer(typeof(ExportProductsInRangeDto[]), new XmlRootAttribute("Products"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("","")
            });

            serializer.Serialize(new StringWriter(sb), products,namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var userProducts = context.Users
                .Where(s => s.ProductsSold.Any())
                .OrderBy(u => u.LastName).ThenBy(r => r.FirstName)
                .Take(5)
                .Select(x => new ExportSoldProductsUserDto
                {
                    FirstName = x.FirstName,
                    lastName = x.LastName,
                    SoldProducts = x.ProductsSold.Select(p => new ExportSoldProductsProductDto
                    {
                        Name = p.Name,
                        Price = p.Price
                    }).ToHashSet()
                }).ToArray();

            XmlSerializer serializer = new XmlSerializer(typeof(ExportSoldProductsUserDto[]), new XmlRootAttribute("Users"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("","")
            });

            serializer.Serialize(new StringWriter(sb), userProducts, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetCategoriesByProductsCount(ProductShopContext
context)
        {
            var categories = context.Categories
                .Select(x => new ExportCategoriesByProductsDto
                {
                    Name = x.Name,
                    Count = x.CategoryProducts.Count,
                    AveragePrice = x.CategoryProducts.Average(p => p.Product.Price),
                    TotalRevenue = x.CategoryProducts.Sum(p => p.Product.Price)
                })
                .OrderByDescending(s=>s.Count).ThenBy(y=>y.TotalRevenue)
                .ToArray();

            XmlSerializer serializer = new XmlSerializer(typeof(ExportCategoriesByProductsDto[]), new XmlRootAttribute("Categories"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("","")
            });

            serializer.Serialize(new StringWriter(sb), categories, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {

           var userProducts = context.Users
               .Where(s => s.ProductsSold.Count>=1)
               .Select(x => new ExportUserWithAgeCountProductsDto
               {
                   FirstName = x.FirstName,
                   LastName = x.LastName,
                   Age = x.Age,
                   SoldProducts = new ExportSoldProductsDto
                   {
                       Count = x.ProductsSold.Count,

                       Products = x.ProductsSold.Select(s => new ExportSoldProductsProductDto
                       {
                           Name = s.Name,
                           Price = s.Price
                       })
                       .OrderByDescending(t=>t.Price)
                       .ToArray()
                   }
               })
               .OrderByDescending(q=>q.SoldProducts.Count)
               .Take(10)
               .ToArray();

            var users = new ExportUsersDto
            {
                Count = context.Users.Count(s => s.ProductsSold.Count >= 1),
                Users = userProducts
            };

            XmlSerializer serializer = new XmlSerializer(typeof(ExportUsersDto), new XmlRootAttribute("Users"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("","")
            });

            serializer.Serialize(new StringWriter(sb), users, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}