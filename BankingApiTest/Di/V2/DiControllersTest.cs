using AutoMapper;
using BankingApi.Controllers;
using BankingApi.Controllers.V2;
using BankingApi.Controllers.V3;
using BankingApi.Core.DomainModel.Entities;
using BankingApi.Core.Mapping;
using Microsoft.Extensions.DependencyInjection;
namespace BankingApiTest.Di.V2;

public static class DiControllersTest {
   public static IServiceCollection AddControllersTest(
      this IServiceCollection services
   ) {
      services.AddAutoMapper(typeof(Owner), typeof(MappingProfile));
      // Auto Mapper Configurations
      var mapperConfig = new MapperConfiguration(mc => { mc.AddProfile(new MappingProfile()); });
      
      // Controllers
      services.AddScoped<OwnersController>();
      services.AddScoped<AccountsController>();
      services.AddScoped<BeneficiariesController>();
      services.AddScoped<TransfersController>();
      services.AddScoped<TransactionsController>();

      return services;
   }
}