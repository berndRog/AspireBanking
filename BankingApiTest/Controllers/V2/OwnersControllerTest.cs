using System.Collections.Generic;
using System.Threading.Tasks;
using BankingApi.Core.Dto;
using Xunit;
namespace BankingApiTest.Controllers.V2;
[Collection(nameof(SystemTestCollectionDefinition))]
public class OwnersControllerTest: BaseControllerTest {
   
   [Fact]
   public async Task GetOwnerByIdTest() {
      // Arrange
      await _arrangeTest.OwnersAsync(_seed);
      var expected = _mapper.Map<OwnerDto>(_seed.Owner1); 
      // Act
      var actionResult = await _ownersController.GetOwnerById(_seed.Owner1.Id);
      // Assert
      THelper.IsOk(actionResult, expected);
   }

   [Fact]
   public async Task GetOwnerByNameTest() {
      // Arrange
      await _arrangeTest.OwnersAsync(_seed);
      var expected = new List<OwnerDto>{ _mapper.Map<OwnerDto>(_seed.Owner5) }; 
      // Act
      var actionResult = await _ownersController.GetOwnersByName("Chris");
      // Assert
      THelper.IsEnumerableOk(actionResult, expected);
   }
   
   [Fact]
   public async Task CreateOwnerTest() {
      // Arrange
      var owner1Dto = _mapper.Map<OwnerDto>(_seed.Owner1); 
      // Act
      var actionResult = await _ownersController.CreateOwner(owner1Dto);
      // Assert
      THelper.IsCreated(actionResult, owner1Dto);
   }
   
   [Fact]
   public async Task UpdateOwnerTest() {
      // Arrange
      await _arrangeTest.OwnersAsync(_seed);
      var owner1Dto = _mapper.Map<OwnerDto>(_seed.Owner1);
      var updatedOwner1Dto = owner1Dto with {
         FirstName = "Erika Meier", 
         Email = "erika.meier@icloud.com"
      };
      
      // Act
      var actionResult = 
         await _ownersController.UpdateOwner(owner1Dto.Id, updatedOwner1Dto);      
      
      // Assert
      THelper.IsOk(actionResult!, updatedOwner1Dto);
   }
   
   [Fact]
   public async Task DeleteTest() {
      // Arrange
      _arrangeTest.ExampleAsync(_seed);
      // Act
      var actionResult =
         await _ownersController.DeleteOwner(_seed.Owner1.Id);
      // Assert
      THelper.IsNoContent(actionResult);
   }
}