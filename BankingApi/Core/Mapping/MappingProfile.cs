using AutoMapper;
using BankingApi.Core.DomainModel.Entities;
using BankingApi.Core.Dto;
namespace BankingApi.Core.Mapping;

 public class MappingProfile : Profile {
   
   public MappingProfile() {
      // Add as many of these lines as you need to map your objects
      
      // Entity -> DTO: Simple mapping since DTO is immutable and only needs getters.
      CreateMap<Owner, OwnerDto>();
      // DTO -> Entity: Use ConstructUsing to call the proper constructor.
      CreateMap<OwnerDto, Owner>()
         .ConstructUsing(dto => new Owner(dto.Id, dto.FirstName, dto.LastName, dto.Email, dto.UserName, dto.UserId));
         // Optionally, if you want to map the Id for update scenarios,
         //.ForMember(dest => dest.Id, opt => opt.Ignore());

      CreateMap<Account, AccountDto>();
      CreateMap<AccountDto, Account>()
         .ConstructUsing(dto => new Account(dto.Id, dto.Iban, dto.Balance, dto.OwnerId));


      CreateMap<Beneficiary, BeneficiaryDto>();
      CreateMap<BeneficiaryDto, Beneficiary>()
         .ConstructUsing(dto => new Beneficiary(dto.Id, dto.FirstName, dto.LastName, 
            dto.Iban, dto.AccountId));
      
      CreateMap<Transfer, TransferDto>();
      CreateMap<TransferDto, Transfer>()
         .ConstructUsing(dto => new Transfer(dto.Id, dto.Date, dto.Description, dto.Amount, 
            dto.AccountId, dto.BeneficiaryId));

      CreateMap<Transaction, TransactionDto>();
      CreateMap<TransactionDto, Transaction>()
         .ConstructUsing(dto => 
            new Transaction(dto.Id, dto.Date, dto.Amount, dto.AccountId, dto.TransferId));

   }
}
