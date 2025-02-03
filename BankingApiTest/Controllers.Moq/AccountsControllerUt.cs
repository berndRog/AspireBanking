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
public class AccountsControllerUt : BaseControllerUt {

   [Fact]
   public async Task GetAccountsByOwnerId_Ok() {
      // Arrange
      _seed.Example1();
      var ownerId = _seed.Owner1.Id;
      var repoResult = new List<Account>() { _seed.Account1, _seed.Account2 };
      // mock the result of the repository
      _mockAccountsRepository.Setup(r => r.SelectByOwnerIdAsync(ownerId))
         .ReturnsAsync(repoResult);
      var expected = _mapper.Map<IEnumerable<AccountDto>>(repoResult);

      // Act
      var actionResult = await _accountsController.GetAccountsByOwnerId(ownerId);

      // Assert
      THelper.IsEnumerableOk(actionResult, expected);
   }

   [Fact]
   public async Task GetAccountsByOwnerId_EmptyList() {
      // Arrange
      _seed.InitAccountsForOwner1();
      var ownerId = _seed.Owner2.Id;
      var repoResult = new List<Account>();
      _mockAccountsRepository.Setup(r => r.FilterByAsync(It.IsAny<Expression<Func<Account, bool>>>()))
         .ReturnsAsync(repoResult);
      var expected = _mapper.Map<IEnumerable<AccountDto>>(repoResult);

      // Act
      var actionResult = await _accountsController.GetAccountsByOwnerId(ownerId);

      // Assert
      THelper.IsEnumerableOk(actionResult, expected);
   }

 
   [Fact]
   public async Task GetAccountByIban_Ok() {
      // Arrange
      _seed.InitAccountsForOwner1();
      var repoResult = _seed.Account1;
      var iban = _seed.Account1.Iban;
      // mock the result of the repository
      _mockAccountsRepository.Setup(r => r.FindByAsync(It.IsAny<Expression<Func<Account, bool>>>()))
         .ReturnsAsync(repoResult);
      var expected = _mapper.Map<AccountDto>(repoResult);

      // Act
      var actionResult = await _accountsController.GetAccountByIban(iban);

      // Assert
      THelper.IsOk(actionResult, expected);
   }

   [Fact]
   public async Task GetAccountByIban_NotFound() {
      // Arrange
      _seed.InitAccountsForOwner1();
      var iban = _seed.Account8.Iban;
      // mock the result of the repository
      _mockAccountsRepository.Setup(r => r.FindByAsync(It.IsAny<Expression<Func<Account, bool>>>()))
         .ReturnsAsync(null as Account);

      // Act
      var actionResult = await _accountsController.GetAccountByIban(iban);

      // Assert
      THelper.IsNotFound(actionResult);
   }
   
   [Fact]
   public async Task CreateAccount_Created() {
      // Arrange
      var owner = _seed.Owner1;
      var account = _seed.Account1;
      var accountDto = _mapper.Map<AccountDto>(_seed.Account1);
      var expected = accountDto with { OwnerId = owner.Id };
      
      // mock the repository's methods
      _mockOwnersRepository.Setup(r => r.FindByIdAsync(owner.Id))
         .ReturnsAsync(owner);
      _mockAccountsRepository.Setup(r => r.FindByIdAsync(account.Id))
         .ReturnsAsync(null as Account);
      _mockAccountsRepository.Setup(r => r.Add(It.IsAny<Account>()))
         .Callback<Account>(a => account = a);
      _mockDataContext.Setup(c => c.SaveAllChangesAsync())
         .ReturnsAsync(true);
         
      // Act
      var actionResult = await _accountsController.CreateAccount(owner.Id, accountDto);

      // Assert
      THelper.IsCreated(actionResult, expected);
      _mockAccountsRepository.Verify(r => r.Add(It.IsAny<Account>()), Times.Once);
      _mockDataContext.Verify(c => c.SaveAllChangesAsync(), Times.Once);
   }

   [Fact]
   public async Task CreateAccount_Conflict() {
      // Arrange
      var owner = _seed.Owner1;
      var account = _seed.Account1;
      owner.Add(account);
      var accountDto = _mapper.Map<AccountDto>(_seed.Account1);
      // mock the repository's methods
      _mockOwnersRepository.Setup(repository => 
            repository.FindByIdAsync(owner.Id))
               .ReturnsAsync(owner);
      _mockAccountsRepository.Setup(repository => 
            repository.FindByIdAsync(account.Id))
               .ReturnsAsync(account);

      // Act
      var actionResult = await _accountsController.CreateAccount(owner.Id, accountDto);

      // Assert
      THelper.IsConflict(actionResult);
      _mockOwnersRepository.Verify(r => r.Add(It.IsAny<Owner>()), Times.Never);
      _mockDataContext.Verify(c => c.SaveAllChangesAsync(), Times.Never);
   }

}