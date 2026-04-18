using API.Helpers;
using DAL.DAOs;
using DAL.Factories;
using DAL.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Security.Claims;
using System.Text;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            
            builder.Services.AddTransient<IFactory<SqlConnection>, DatabaseFactory>();
            builder.Services.AddTransient<IUserDAO, UserDAO>();
            builder.Services.AddTransient<IHotelDAO, HotelDAO>();
            builder.Services.AddTransient<IHotelReservationDAO, HotelReservationDAO>();
            builder.Services.AddScoped<IRoleDAO, RoleDAO>();
            builder.Services.AddTransient<IRentalCarReservationDAO, RentalCarReservationDAO>();
            builder.Services.AddTransient<IHotelRoomDAO, HotelRoomDAO>();
            builder.Services.AddTransient<ITaxiCompanyDAO, TaxiCompanyDAO>();
            builder.Services.AddTransient<ITaxiReservationDAO, TaxiReservationDAO>();
            builder.Services.AddSingleton<PasswordHasherService>();
            builder.Services.AddSingleton<JWTService>();
            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
                        NameClaimType = ClaimTypes.Email,
                        RoleClaimType = ClaimTypes.Role
                    };
                });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();

                app.MapGet("/", () => Results.Redirect("/scalar/v1"));
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
