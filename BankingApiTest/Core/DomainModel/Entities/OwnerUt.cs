using System.Collections.Generic;
using FluentAssertions;
using BankingApi.Core.DomainModel.Entities;
using Xunit;

namespace BankingApiTest.Core.DomainModel.Entities;
public class OwnerUt {

   private readonly Seed _seed;

   public OwnerUt() {
      _seed = new Seed();
   }

   #region without Account
   [Fact]
   public void Ctor() {
      // Arrange
      // Act
      var actual = new Owner();
      // Assert
      actual.Should().NotBeNull();
      actual.Should().BeOfType<Owner>();
   }

   [Fact]
   public void ObjectInitializerUt() {
      // Arrange
      // Act
      var actual = new Owner {
         Id = _seed.Owner1.Id,
         FirstName = _seed.Owner1.FirstName,
         Email = _seed.Owner1.Email
      };
      // Assert
      actual.Should().NotBeNull();
      actual.Should().BeOfType<Owner>();
      actual.Id.Should().Be(_seed.Owner1.Id);
      actual.FirstName.Should().Be(_seed.Owner1.FirstName);
      actual.Email.Should().Be(_seed.Owner1.Email);
   }
   [Fact]
   public void GetterUt() {
      // Arrange
      var actual = new Owner {
         Id = _seed.Owner1.Id,
         FirstName = _seed.Owner1.FirstName,
         Email = _seed.Owner1.Email
      };
      // Act
      var actualId = actual.Id;
      var actualName = actual.FirstName;
      var actualEmail = actual.Email;
      // Assert
      actualId.Should().Be(_seed.Owner1.Id);
      actualName.Should().Be(_seed.Owner1.FirstName);
      actualEmail.Should().Be(_seed.Owner1.Email);
   }

   [Fact]
   public void SetterUt() {
      // Arrange
      Owner actual = new () {
         // Act
         FirstName = _seed.Owner1.FirstName,
         Email = _seed.Owner1.Email
      };
      // Assert
      actual.FirstName.Should().Be(_seed.Owner1.FirstName);
      actual.Email.Should().Be(_seed.Owner1.Email);
   }
   #endregion

   #region with Accounts   
   [Fact]
   public void OwnerAddAccountUt() {
      // Arrange
      // Act
      _seed.Owner1.Add(_seed.Account1);
      var expected = new List<Account> { _seed.Account1 };
      // Assert
      _seed.Owner1.Accounts.Should()
         .HaveCount(1).And
         .BeEquivalentTo(expected);
   }
   #endregion
}