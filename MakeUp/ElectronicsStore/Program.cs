using Infastructure.Entities.Identity;
using Infastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Core.Mapper;
using Core.Interfaces;
using Core.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AppEFContext>(opt =>
         opt.UseNpgsql(builder.Configuration.GetConnectionString("ElectronicsStoreDB")));

builder.Services.AddIdentity<UserEntity, RoleEntity>(options =>
{
    options.Stores.MaxLengthForKeys = 128;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 5;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
    .AddEntityFrameworkStores<AppEFContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAutoMapper(typeof(AppMapProfile));
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<ISmtpEmailService, SmtpEmailService>();
builder.Services.AddTransient<IProductsService, ProductsService>();
builder.Services.AddTransient<ICategoriesService, CategoriesService>();
builder.Services.AddTransient<ISalesService, SalesService>();
builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<IOrderService, OrderService>();
builder.Services.AddTransient<IUsersService, UsersService>();

var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<String>("JWTSecretKey")));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(cfg =>
{
    cfg.RequireHttpsMetadata = false;
    cfg.SaveToken = true;
    cfg.TokenValidationParameters = new TokenValidationParameters()
    {
        IssuerSigningKey = signinKey,
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors();

var app = builder.Build();

app.UseCors(x=>x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

var dir = Path.Combine(Directory.GetCurrentDirectory(), "images");
if (!Directory.Exists(dir))
    Directory.CreateDirectory(dir);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(dir),
    RequestPath = "/images"
});

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.SeedData();

app.Run();
