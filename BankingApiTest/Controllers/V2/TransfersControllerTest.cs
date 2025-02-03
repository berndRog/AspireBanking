using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BankingApi.Core.DomainModel.Entities;
using BankingApi.Core.Dto;
using Xunit;
namespace BankingApiTest.Controllers.V2;
[Collection(nameof(SystemTestCollectionDefinition))]
public class TransfersControllerTest: BaseControllerTest {
   

   [Fact]
   public async Task GetTest() {
      // Arrange
      await _arrangeTest.ExampleAsync(_seed);
      var expected = _mapper.Map<List<TransferDto>>(
         _seed.Account1.Transfers);
      // Act
      var actionResult = await _transfersController.GetTransfersByAccountId(_seed.Account1.Id);
      // Assert
      THelper.IsOk(actionResult!, expected);
   }

   [Fact]
   public async Task GetByIdTest() {
      // Arrange
      await _arrangeTest.SendMoneyTest1(_seed);
      var expected = _mapper.Map<TransferDto>(_seed.Transfer1);
      // Act
       var actionResult = await _transfersController.GetTransferById(expected.Id);
      // Assert
      THelper.IsOk(actionResult!, expected);
   }
   
   [Fact]
   public async Task SendMoneyTest() {
      // Arrange
      await _arrangeTest.PrepareTest1(_seed);
      _seed.Transfer1.AccountId = _seed.Account1.Id;
      _seed.Transfer1.BeneficiaryId = _seed.Beneficiary1.Id;
      var transferDto = _mapper.Map<TransferDto>(_seed.Transfer1);
      // Act
      var actionResult =
         await _transfersController.SendMoney(_seed.Account1.Id, transferDto);
      // Assert
      THelper.IsCreated(actionResult, transferDto);
   }

   [Fact]
   public async Task ReverseMoneyTest() {
      // Arrange
      await _arrangeTest.SendMoneyTest1(_seed);
      var originalTransfer = _seed.Transfer1;
      var reverseTransfer = new Transfer {
         Id = Guid.NewGuid(),
         Date = DateTime.UtcNow,
         Description = "Reverse transfer",
         Amount = -originalTransfer.Amount,
         BeneficiaryId = originalTransfer.BeneficiaryId,
         AccountId = originalTransfer.AccountId
      };
      var reverseTransferDto = _mapper.Map<TransferDto>(reverseTransfer);
      
      // Act
      var actionResult = await _transfersController.ReverseMoney(
         _seed.Account1.Id,
         originalTransfer.Id,
         reverseTransferDto
      );
      
      // Assert
      THelper.IsCreated(actionResult, reverseTransferDto);
   }
}
