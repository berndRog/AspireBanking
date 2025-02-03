using AutoMapper;
using Azure.Identity;
using BankingApi.Core.Dto;
using BankingApi.Core.Mapping;
using FluentAssertions;
using Xunit;
namespace BankingApiTest.Core.Dto;

public class OwnerDtoUt {
   private readonly Seed _seed;

   public OwnerDtoUt(){
      var config = new MapperConfiguration(config =>
         config.AddProfile(new MappingProfile())
      );
      var mapper = new Mapper(config);
      _seed = new Seed();
   }
   
   [Fact]
   public void CtorAndGetterUt(){
      // Arrange
      // Act
      var actual = new OwnerDto(
         Id: _seed.Owner1.Id,
         FirstName: _seed.Owner1.FirstName,
         LastName: _seed.Owner1.LastName,
         Email: _seed.Owner1.Email,
         UserName: null,
         UserId: null,
         UserRole: null
      );
      // Assert
      actual.Should().NotBeNull();
      actual.Should().BeOfType<OwnerDto>();
      actual.Id.Should().Be(_seed.Owner1.Id);
      actual.FirstName.Should().Be(_seed.Owner1.FirstName);
      actual.Email.Should().Be(_seed.Owner1.Email);
   }
}