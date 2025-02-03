using AutoMapper;
using BankingApi.Core;
using BankingApi.Core.DomainModel.Entities;
using BankingApi.Core.Mapping;
using BankingApi.Core.UseCases;
using Microsoft.Extensions.DependencyInjection;
namespace BankingApi.Di; 
public static class DiCore {
   public static void AddCore(
      this IServiceCollection services
   ){

      services.AddAutoMapper(typeof(Owner), typeof(MappingProfile));
      services.AddAutoMapper(typeof(Account), typeof(MappingProfile));
      services.AddAutoMapper(typeof(Beneficiary), typeof(MappingProfile));

      // Auto Mapper Configurations
      new MapperConfiguration(mc => {
         mc.AddProfile(new MappingProfile());
      });
      
      services.AddScoped<IUseCasesTransfer, UseCasesTransfer>();
   }
}