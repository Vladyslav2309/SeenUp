using Core.Constants;
using Core.Helpers;
using Infastructure.Entities.Identity;
using Infastructure.Entities;
using Infastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Core.Services
{
    public static class SeederDB
    {
        public static void SeedData(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppEFContext>();
                var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                context.Database.Migrate();

                var userManager = scope.ServiceProvider
                    .GetRequiredService<UserManager<UserEntity>>();

                var roleManager = scope.ServiceProvider
                    .GetRequiredService<RoleManager<RoleEntity>>();

                #region Seed Roles and Users

                if (!context.Roles.Any())
                {
                    foreach (var role in Roles.All)
                    {
                        var result = roleManager.CreateAsync(new RoleEntity
                        {
                            Name = role
                        }).Result;
                    }
                }

                if (!context.Users.Any())
                {
                    UserEntity user = new()
                    {
                        FirstName = "Admin",
                        LastName = "Admin",
                        Email = "admin@gmail.com",
                        UserName = "admin@gmail.com",
                    };
                    var result = userManager.CreateAsync(user, "Qwerty1-")
                        .Result;
                    if (result.Succeeded)
                    {
                        result = userManager
                            .AddToRoleAsync(user, Roles.Admin)
                            .Result;
                    }
                }

                #endregion

                #region Seed categories
                if (!context.Categories.Any())
                {
                    CategoryEntity cat1 = new()
                    {
                        Name = "Ноутбуки",
                        Image = ImageWorker
                        .SaveImage(@"https://content.rozetka.com.ua/goods/images/big/269608304.jpg", configuration).Result
                    };
                    CategoryEntity cat2 = new()
                    {
                        Name = "Монітори",
                        Image = ImageWorker.SaveImage(@"https://content1.rozetka.com.ua/goods/images/big/175133890.jpg", configuration).Result
                    };
                    CategoryEntity cat3 = new()
                    {
                        Name = "Планшети",
                        Image = ImageWorker.SaveImage(@"https://content.rozetka.com.ua/goods/images/big/247716706.jpg", configuration).Result
                    };
                    CategoryEntity cat4 = new()
                    {
                        Name = "Комп'ютери",
                        Image = ImageWorker.SaveImage(@"https://content1.rozetka.com.ua/goods/images/big/381505878.jpg", configuration).Result
                    };
                    CategoryEntity cat5 = new()
                    {
                        Name = "Клавіатури та миші",
                        Image = ImageWorker.SaveImage(@"https://content2.rozetka.com.ua/goods/images/big/322547872.jpg", configuration).Result
                    };

                    context.Categories.AddRange(cat1, cat2, cat3, cat4, cat5);
                    context.SaveChanges();
                }
                #endregion

                #region Seed Products
                if (!context.Products.Any())
                {
                    #region Product 1
                    ProductEntity prod1 = new()
                    {
                        Name = "Ноутбук HP ZBook Power G10 (7C3M9AV_V1) Grey",
                        Price = 137155,
                        Description = "Екран 15.6\" IPS (2560x1440) WQHD, матовий / Intel Core i9-13900HK (4.1 - 5.4 ГГц) / RAM 32 ГБ / SSD 1 ТБ / nVidia RTX 3000, 8 ГБ / без ОД / LAN / Wi-Fi / Bluetooth / веб-камера / DOS / 2.01 кг / сірий",
                        CategoryId = 1
                    };

                    ProductImageEntity prod1Img1 = new()
                    {
                        Name = ImageWorker.SaveImage(@"https://content1.rozetka.com.ua/goods/images/big/358629733.jpg", configuration).Result,
                        Priority = 1,
                        ProductId = 1
                    };

                    ProductImageEntity prod1Img2 = new()
                    {
                        Name = ImageWorker.SaveImage(@"https://content1.rozetka.com.ua/goods/images/big/358629734.jpg", configuration).Result,
                        Priority = 2,
                        ProductId = 1
                    };

                    ProductImageEntity prod1Img3 = new()
                    {
                        Name = ImageWorker.SaveImage(@"https://content2.rozetka.com.ua/goods/images/big/358629735.jpg", configuration).Result,
                        Priority = 3,
                        ProductId = 1
                    };

                    ProductImageEntity prod1Img4 = new()
                    {
                        Name = ImageWorker.SaveImage(@"https://content1.rozetka.com.ua/goods/images/big/358629736.jpg", configuration).Result,
                        Priority = 4,
                        ProductId = 1
                    };

                    ProductImageEntity prod1Img5 = new()
                    {
                        Name = ImageWorker.SaveImage(@"https://content.rozetka.com.ua/goods/images/big/358629737.jpg", configuration).Result,
                        Priority = 5,
                        ProductId = 1
                    };
                    #endregion
                    #region Product 2
                    ProductEntity prod2 = new()
                    {
                        Name = "Ноутбук Apple MacBook Air 13.6\" M2 8/256GB 2022 (MLXW3UA/A) Space Gray",
                        Price = 53499,
                        Description = "Екран 13.6\" Liquid Retina (2560x1664), глянсовий / Apple M2 / RAM 8 ГБ / SSD 256 ГБ / Apple M2 Graphics (8 ядер) / Wi-Fi / Bluetooth / macOS Monterey / 1.24 кг / сірий",
                        CategoryId = 1
                    };

                    ProductImageEntity prod2Img1 = new()
                    {
                        Name = ImageWorker.SaveImage(@"https://content2.rozetka.com.ua/goods/images/big/269256825.jpg", configuration).Result,
                        Priority = 1,
                        ProductId = 2
                    };

                    ProductImageEntity prod2Img2 = new()
                    {
                        Name = ImageWorker.SaveImage(@"https://content2.rozetka.com.ua/goods/images/big/269256826.jpg", configuration).Result,
                        Priority = 2,
                        ProductId = 2
                    };

                    ProductImageEntity prod2Img3 = new()
                    {
                        Name = ImageWorker.SaveImage(@"https://content2.rozetka.com.ua/goods/images/big/269256827.jpg", configuration).Result,
                        Priority = 3,
                        ProductId = 2
                    };

                    ProductImageEntity prod2Img4 = new()
                    {
                        Name = ImageWorker.SaveImage(@"https://content2.rozetka.com.ua/goods/images/big/269256830.jpg", configuration).Result,
                        Priority = 4,
                        ProductId = 2
                    };

                    ProductImageEntity prod2Img5 = new()
                    {
                        Name = ImageWorker.SaveImage(@"https://content1.rozetka.com.ua/goods/images/big/269256831.jpg", configuration).Result,
                        Priority = 5,
                        ProductId = 2
                    };
                    #endregion
                    #region Product 3
                    ProductEntity prod3 = new()
                    {
                        Name = "Ноутбук Dell Latitude 5540 (N096L554015UA_UBU) Grey",
                        Price = 42525,
                        Description = "Екран 15.6\" IPS (1920x1080) Full HD, матовий / Intel Core i5-1345U (3.5 - 4.7 Гц) / RAM 16 ГБ / SSD 256 ГБ / Intel Iris Xe Graphics / без ОД / Wi-Fi / LAN / Bluetooth / веб- камера / Ubuntu Linux / 1.61 кг / сірий",
                        CategoryId = 1
                    };

                    ProductImageEntity prod3Img1 = new()
                    {
                        Name = ImageWorker.SaveImage(@"https://content2.rozetka.com.ua/goods/images/big/338714373.jpg", configuration).Result,
                        Priority = 1,
                        ProductId = 3
                    };

                    ProductImageEntity prod3Img2 = new()
                    {
                        Name = ImageWorker.SaveImage(@"https://content2.rozetka.com.ua/goods/images/big/338714375.jpg", configuration).Result,
                        Priority = 2,
                        ProductId = 3
                    };

                    ProductImageEntity prod3Img3 = new()
                    {
                        Name = ImageWorker.SaveImage(@"https://content2.rozetka.com.ua/goods/images/big/338714376.jpg", configuration).Result,
                        Priority = 3,
                        ProductId = 3
                    };

                    ProductImageEntity prod3Img4 = new()
                    {
                        Name = ImageWorker.SaveImage(@"https://content.rozetka.com.ua/goods/images/big/338714377.jpg", configuration).Result,
                        Priority = 4,
                        ProductId = 3
                    };

                    ProductImageEntity prod3Img5 = new()
                    {
                        Name = ImageWorker.SaveImage(@"https://content.rozetka.com.ua/goods/images/big/338714378.jpg", configuration).Result,
                        Priority = 5,
                        ProductId = 3
                    };
                    #endregion

                    context.Products.AddRange(prod1, prod2, prod3);
                    context.SaveChanges();

                    context.ProductImages.AddRange(prod1Img1, prod1Img2, prod1Img3, prod1Img4, prod1Img5,
                        prod2Img1, prod2Img2, prod2Img3, prod2Img4, prod2Img5,
                        prod3Img1, prod3Img2, prod3Img3, prod3Img4, prod3Img5);
                    context.SaveChanges();
                }
                #endregion

                #region Seed OrderStatuses
                if (!context.OrderStatuses.Any())
                {
                    foreach (var orderStatus in OrderStatuses.All)
                    {
                        context.OrderStatuses.Add(new()
                        {
                            Name = orderStatus
                        });
                    }
                    context.SaveChanges();
                }
                #endregion

                #region Seed Sales

                if(!context.Sales.Any())
                {
                    SaleEntity sale = new SaleEntity
                    {
                        DateCreated = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
                        ExpireTime = DateTime.SpecifyKind(DateTime.Now.AddDays(100), DateTimeKind.Utc),
                        Name = "Знижки до 43% на кавомашини Philips",
                        Image = ImageWorker.SaveImage("https://content1.rozetka.com.ua/promotions/main_image_ua/original/385897161.jpg", configuration).Result,
                        Description = "Період проведення акції: з 28 листопада до 28 грудня 2023",
                    };
                    context.Sales.Add(sale);
                    context.SaveChanges();
                }

                #endregion
            }
        }
    }
}
