using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BankingApi.Core.DomainModel.Entities;
using BankingApi.Core.Dto;
using Moq;
using Xunit;
namespace BankingApiTest.Controllers.Moq;

[Collection(nameof(SystemTestCollectionDefinition))]
public class BeneficiariesControllerUt : BaseControllerUt {

   
   [Fact]
   public async Task GetBeneficiariesByAcccountId_Ok() {
      // Arrange
      _seed.Example1();
      var id = _seed.Account1.Id;
      var repoResult = new List<Beneficiary> { _seed.Beneficiary1, _seed.Beneficiary2 };
      // mock the result of the repository
      _mockAccountsRepository.Setup(r => r.FindByIdAsync(id))
         .ReturnsAsync(_seed.Account1);
      _mockBeneficiariesRepository.Setup(r => r.SelectByAccountIdAsync(id))
         .ReturnsAsync(repoResult);
      var expected = _mapper.Map<IEnumerable<BeneficiaryDto>>(repoResult);

      // Act
      var actionResult = await _beneficiariesController.GetBeneficiariesByAccountId(id);

      // Assert
      THelper.IsOk(actionResult, expected);
      
   }
   
   [Fact]
   public async Task GetBeneficiariesByAcccountId_EmptyList() {
      // Arrange
      _seed.Example1();
      var id = _seed.Account1.Id;
      var repoResult = new List<Beneficiary>(); // empty list
      
      // mock the result of the repository
      _mockAccountsRepository.Setup(r => r.FindByIdAsync(id))
         .ReturnsAsync(_seed.Account1);
      _mockBeneficiariesRepository.Setup(r => r.SelectByAccountIdAsync(id))
         .ReturnsAsync(repoResult);
      var expected = _mapper.Map<IEnumerable<BeneficiaryDto>>(repoResult);
      
      // Act
      var actionResult = await _beneficiariesController.GetBeneficiariesByAccountId(id);

      // Assert
      THelper.IsEnumerableOk(actionResult, expected);
   }
   
      
   [Fact]
   public async Task GetBeneficiaryById_Ok() {
      // Arrange
      _seed.Example1();
      var id = _seed.Beneficiary1.Id;
      var repoResult = _seed.Beneficiary1;
      // mock the result of the repository
      _mockBeneficiariesRepository.Setup(r => r.FindByIdAsync(id))
         .ReturnsAsync(repoResult);
      var expected = _mapper.Map<BeneficiaryDto>(repoResult);

      // Act
      var actionResult = await _beneficiariesController.GetBeneficiaryById(id);
      
      // Assert
      THelper.IsOk(actionResult, expected);
   }

   [Fact]
   public async Task GetBeneficiaryById_NotFound() {
      // Arrange
      _seed.Example1();
      var id = Guid.NewGuid();
      _mockBeneficiariesRepository.Setup(r => r.FindByIdAsync(id))
         .ReturnsAsync(null as Beneficiary);

      // Act
      var actionResult = await _beneficiariesController.GetBeneficiaryById(id);

      // Assert
      THelper.IsNotFound(actionResult);
   }

   [Fact]
   public async Task GetBeneficiariesByName_Ok() {
      // Arrange
      _seed.Example1();
      var firstName = _seed.Beneficiary1.FirstName;
      var lastName = _seed.Beneficiary1.LastName;
      var repoResult = _seed.Beneficiary1;
      _mockBeneficiariesRepository.Setup(r => r.FindByOwnersNameAsync(firstName, lastName))
         .ReturnsAsync(repoResult);
      var expected = _mapper.Map<IEnumerable<BeneficiaryDto>?>(repoResult);

      // Act
      var actionResult = await _beneficiariesController.GetBeneficiariesByName(firstName, lastName);

      // Assert
      //THelper.IsOk(actionResult!, expected);
   }

   [Fact]
   public async Task GetBeneficiariesByName_NotFound() {
      // Arrange
      _seed.Example1();
      var firstName = "Micky";
      var lastName = "Mouse";
      _mockBeneficiariesRepository.Setup(r => r.FindByOwnersNameAsync(firstName, lastName))
         .ReturnsAsync(new Beneficiary());

      // Act
      var actionResult = await _beneficiariesController.GetBeneficiariesByName(firstName, lastName);

      // Assert
      THelper.IsNotFound(actionResult);
   }
   
   [Fact]
   public async Task CreateBeneficiary_Created() {
      // Arrange
      // debit account
      var firstName = _seed.Beneficiary1.FirstName; // owner 5
      var lastName = _seed.Beneficiary1.LastName; // owner 5
      var iban = _seed.Beneficiary1.Iban; // account 6
      // credit account
      _seed.Beneficiary1.AccountId = _seed.Account1.Id;
      var beneficiary1Dto = _mapper.Map<BeneficiaryDto>(_seed.Beneficiary1);
      Beneficiary? addedBeneficiary = null;
      
      // mock the repositories 
      _mockOwnersRepository.Setup(r => r.LikeNameByAsync(firstName))
         .ReturnsAsync(new List<Owner> { _seed.Owner5 });
      _mockAccountsRepository.Setup(r => r.FindByIdAsync(_seed.Account1.Id))
         .ReturnsAsync(_seed.Account1);
      _mockAccountsRepository.Setup(r => r.FindByAsync(It.IsAny<Expression<Func<Account, bool>>>()))
         .ReturnsAsync(_seed.Account1);
      // beneficiary does not exist
      _mockBeneficiariesRepository.Setup(r => r.FindByIdAsync(_seed.Beneficiary1.Id))
         .ReturnsAsync(null as Beneficiary);
      _mockBeneficiariesRepository.Setup(r => r.Add(It.IsAny<Beneficiary>()))
         .Callback<Beneficiary>(beneficiary => addedBeneficiary = beneficiary);
      _mockDataContext.Setup(c => c.SaveAllChangesAsync())
         .ReturnsAsync(true);

      // Act
      var actionResult = await _beneficiariesController.CreateBeneficiary(_seed.Account1.Id, beneficiary1Dto);

      // Assert
      THelper.IsCreated(actionResult, beneficiary1Dto);
      _mockBeneficiariesRepository.Verify(r => r.Add(It.IsAny<Beneficiary>()), Times.Once);
      _mockDataContext.Verify(c => c.SaveAllChangesAsync(), Times.Once);
   }

   [Fact]
   public async Task CreateBeneficiary_Conflict() {
      // Arrange
      // debit account
      var name = _seed.Beneficiary1.FirstName+" "+ _seed.Beneficiary1.LastName   ; // owner 5
      var iban = _seed.Beneficiary1.Iban; // account 6
      // credit account
      _seed.Beneficiary1.AccountId = _seed.Account1.Id;
      var beneficiary1Dto = _mapper.Map<BeneficiaryDto>(_seed.Beneficiary1);
      Beneficiary? addedBeneficiary = null;
      
      // mock the repositories 
      _mockOwnersRepository.Setup(r => r.LikeNameByAsync(name))
         .ReturnsAsync(new List<Owner> { _seed.Owner5 });
      _mockAccountsRepository.Setup(r => r.FindByIdAsync(_seed.Account1.Id))
         .ReturnsAsync(_seed.Account1);
      _mockAccountsRepository.Setup(r => r.FindByAsync(It.IsAny<Expression<Func<Account, bool>>>()))
         .ReturnsAsync(_seed.Account1);
      // beneficiary already exists
      _mockBeneficiariesRepository.Setup(r => r.FindByIdAsync(_seed.Beneficiary1.Id))
         .ReturnsAsync(_seed.Beneficiary1);
      _mockBeneficiariesRepository.Setup(r => r.Add(It.IsAny<Beneficiary>()))
         .Callback<Beneficiary>(beneficiary => addedBeneficiary = beneficiary);
      _mockDataContext.Setup(c => c.SaveAllChangesAsync())
         .ReturnsAsync(true);

      // Act
      var actionResult = await _beneficiariesController.CreateBeneficiary(_seed.Account1.Id, beneficiary1Dto);

      // Assert
      THelper.IsConflict(actionResult);
      _mockOwnersRepository.Verify(r => r.Add(It.IsAny<Owner>()), Times.Never);
      _mockDataContext.Verify(c => c.SaveAllChangesAsync(), Times.Never);
   }

   [Fact]
   public async Task DeleteBeneficiary_NoContent() {
      // Arrange
      _seed.Example1();
      var id = _seed.Beneficiary1.Id;
      var repoResult = new List<Transfer> { _seed.Transfer1 };
      
      // mock the repositories
      _mockAccountsRepository.Setup(r => r.FindByIdAsync(_seed.Account1.Id))
         .ReturnsAsync(_seed.Account1);
      _mockTransfersRepository.Setup(r => r.SelectByBeneficiaryIdAsync(id))
         .ReturnsAsync(repoResult);
      
      _mockBeneficiariesRepository.Setup(r => r.FindByIdAsync(_seed.Beneficiary1.Id))
         .ReturnsAsync(_seed.Beneficiary1);
      _mockBeneficiariesRepository.Setup(r => r.Remove(It.IsAny<Beneficiary>()))
         .Verifiable();
      _mockDataContext.Setup(c => c.SaveAllChangesAsync())
         .ReturnsAsync(true);

      // Act
      var actionResult = await _beneficiariesController.DeleteBeneficiary(
         _seed.Account1.Id, _seed.Beneficiary1.Id);
      
      // Assert
      THelper.IsNoContent(actionResult!);
      _mockOwnersRepository.Verify();
      _mockDataContext.Verify(c => c.SaveAllChangesAsync(), Times.Once);
   }

}