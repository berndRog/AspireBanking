using System.Collections.Generic;
using System.Threading.Tasks;
using BankingApi.Core.Dto;
using Xunit;
namespace BankingApiTest.Controllers.V2;
[Collection(nameof(SystemTestCollectionDefinition))]
public class AccountsControllerTest: BaseControllerTest {

   [Fact]
   public async Task GetAccountsByOwnerIdTest() {
      // Arrange
      await _arrangeTest.Owner1WithAccountsAsync(_seed);
      var expected = new List<AccountDto> {
         _mapper.Map<AccountDto>(_seed.Account1),
         _mapper.Map<AccountDto>(_seed.Account2)
      };
      // Act
      var actionResult = await _accountsController.GetAccountsByOwnerId(_seed.Owner1.Id);
      // Assert
      THelper.IsOk(actionResult!, expected);
   }
   [Fact]
   public async Task GetAccountByIdTest() {
      // Arrange
      await _arrangeTest.Owner1WithAccountsAsync(_seed);
      var expected = _mapper.Map<AccountDto>(_seed.Account1);
      // Act
      var actionResult = await _accountsController.GetAccountById(_seed.Account1.Id);
      // Assert
      THelper.IsOk(actionResult!, expected);
   }

   [Fact]
   public async Task FindByIbanTest() {
      // Arrange
      await _arrangeTest.ExampleAsync(_seed);
      var expected = _mapper.Map<AccountDto>(_seed.Account6);
      // Act
      var actionResult = await _accountsController.GetAccountByIban("DE50 1000 0000 0000 0000 00");
      // Assert
      THelper.IsOk(actionResult, expected);
   }
   
   [Fact]
   public async Task PostTest() {
      // Arrange
      _ownersRepository.Add(_seed.Owner1);
      await _dataContext.SaveAllChangesAsync();
      _dataContext.ClearChangeTracker();   
      _seed.Owner1.Add(_seed.Account1);
      var expected = _mapper.Map<AccountDto>(_seed.Account1);
      // Act
      var actionResult =
         await _accountsController.CreateAccount(_seed.Owner1.Id, expected);
      // Assert
      THelper.IsCreated(actionResult, expected);
   }
   
   
   [Fact]
   public async Task DeleteTest() {
      // Arrange
      _arrangeTest.ExampleAsync(_seed);
      // Act
      var actionResult =
         await _accountsController.DeleteAccount(_seed.Owner1.Id,_seed.Account1.Id);
      // Assert
      THelper.IsNoContent(actionResult);
   }
}