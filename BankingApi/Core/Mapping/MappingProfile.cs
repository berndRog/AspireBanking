using AutoMapper;
using BankingApi.Core.DomainModel.Entities;
using BankingApi.Core.Dto;
namespace BankingApi.Core.Mapping;

 public class MappingProfile : Profile {
   
   public MappingProfile() {
      // Add as many of these lines as you need to map your objects
      CreateMap<Owner, OwnerDto>()
         .ReverseMap();
      
      CreateMap<Account, AccountDto>()
        .ReverseMap();
      
      CreateMap<Beneficiary, BeneficiaryDto>()
//         .ForMember(dest => dest.Account, act => act.Ignore())
        .ReverseMap();
      
      CreateMap<Transfer, TransferDto>()
         .ReverseMap();

      CreateMap<Transaction, TransactionDto>()
         .ReverseMap();
      

   }
}
