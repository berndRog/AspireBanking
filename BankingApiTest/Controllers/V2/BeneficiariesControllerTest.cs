using System.Collections.Generic;
using System.Threading.Tasks;
using BankingApi.Core.DomainModel.Entities;
using BankingApi.Core.Dto;
using Xunit;
namespace BankingApiTest.Controllers.V2;
[Collection(nameof(SystemTestCollectionDefinition))]
public class BeneficiariesControllerTest: BaseControllerTest {

   [Fact]
   public async Task GetBeneficiariesByAccountId() {
      // Arrange
      await _arrangeTest.ExampleAsync(_seed);
      var expected = new List<BeneficiaryDto> {
         _mapper.Map<BeneficiaryDto>(_seed.Beneficiary1),
         _mapper.Map<BeneficiaryDto>(_seed.Beneficiary2)
      };
      // Act
      var actionResult = 
         await _beneficiariesController.GetBeneficiariesByAccountId(_seed.Account1.Id);
      
      // Assert
      THelper.IsOk(actionResult!, expected);
   }

   [Fact]
   public async Task GetBeneficiaryById() {
      // Arrange
      await _arrangeTest.ExampleAsync(_seed);
      var expected = _mapper.Map<BeneficiaryDto>(_seed.Beneficiary1);
      // Act
      var actionResult
         = await _beneficiariesController.GetBeneficiaryById(_seed.Beneficiary1.Id);
      // Assert
      THelper.IsOk(actionResult!, expected);
   }

   
   [Fact]
   public async Task GetBeneficiariesByName_Ok() {
      // Arrange
      await _arrangeTest.ExampleAsync(_seed);
      var firstName = "Erika";
      var lastName = "Mustermann";
      var expected = _seed.Beneficiary1;

      // Act
      var actionResult = await _beneficiariesController.GetBeneficiariesByName(firstName, lastName);

      // Assert
      //THelper.IsOk(actionResult!, expected!);
   }
   
   
   [Fact]
   public async Task PostTest() {
      // Arrange
      await _arrangeTest.OwnersWithAccountsAsync(_seed);
      var beneficiary1Dto = _mapper.Map<BeneficiaryDto>(_seed.Beneficiary1);
      var expected = beneficiary1Dto with { AccountId = _seed.Account1.Id };
      // Act
      var actionResult
         = await _beneficiariesController.CreateBeneficiary(_seed.Account1.Id, beneficiary1Dto);
      // Assert
      THelper.IsCreated(actionResult, expected);
   }
   
}