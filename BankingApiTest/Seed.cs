using System;
using System.Collections.Generic;
using BankingApi.Core.DomainModel.Entities;
using BankingApi.Core.Misc;
namespace BankingApiTest;

public class Seed {

   #region fields
   public Owner Owner1{ get; }
   public Owner Owner2{ get; }
   public Owner Owner3{ get; }
   public Owner Owner4{ get; }
   public Owner Owner5{ get; }
   public Owner Owner6{ get; }

   public Account Account1{ get; }
   public Account Account2{ get; }
   public Account Account3{ get; }
   public Account Account4{ get; }
   public Account Account5{ get; }
   public Account Account6{ get; }
   public Account Account7{ get; }
   public Account Account8{ get; }

   public Beneficiary Beneficiary1{ get; }
   public Beneficiary Beneficiary2{ get; }
   public Beneficiary Beneficiary3{ get; }
   public Beneficiary Beneficiary4{ get; }
   public Beneficiary Beneficiary5{ get; }
   public Beneficiary Beneficiary6{ get; }
   public Beneficiary Beneficiary7{ get; }
   public Beneficiary Beneficiary8{ get; }
   public Beneficiary Beneficiary9{ get; }
   public Beneficiary Beneficiary10{ get; }
   public Beneficiary Beneficiary11{ get; }

   public Transfer Transfer1{ get;}
   public Transfer Transfer2{ get; }
   public Transfer Transfer3{ get; }
   public Transfer Transfer4{ get; }
   public Transfer Transfer5{ get; }
   public Transfer Transfer6{ get; }
   public Transfer Transfer7{ get; }
   public Transfer Transfer8{ get; }
   public Transfer Transfer9{ get; }
   public Transfer Transfer10{ get; }
   public Transfer Transfer11{ get; }

   public Transaction Transaction1{ get; }
   public Transaction Transaction2{ get; }
   public Transaction Transaction3{ get; }
   public Transaction Transaction4{ get; }
   public Transaction Transaction5{ get; }
   public Transaction Transaction6{ get; }
   public Transaction Transaction7{ get; }
   public Transaction Transaction8{ get; }
   public Transaction Transaction9{ get; }
   public Transaction Transaction10{ get; }
   public Transaction Transaction11{ get; }
   public Transaction Transaction12{ get; }
   public Transaction Transaction13{ get; }
   public Transaction Transaction14{ get; }
   public Transaction Transaction15{ get; }
   public Transaction Transaction16{ get; }
   public Transaction Transaction17{ get; }
   public Transaction Transaction18{ get; }
   public Transaction Transaction19{ get; }
   public Transaction Transaction20{ get; }
   public Transaction Transaction21{ get; }
   public Transaction Transaction22{ get; }
   
   public List<Owner> Owners{ get; private set; }
   public List<Account> Accounts{ get; private set; } 
   public List<Beneficiary> Beneficiaries{ get; private set; } 
   public List<Transfer> Transfers{ get; private set; }
   public List<Transaction> Transactions{ get; private set; } 
   #endregion

   public Seed(){
      #region Owners
      Owner1 = new Owner() {
         Id = new Guid("10000000-0000-0000-0000-000000000000"),
         FirstName = "Erika",
         LastName = "Mustermann",
         Email = "erika.mustermann@t-online.de"
      };
      Owner2 = new Owner {
         Id = new Guid("20000000-0000-0000-0000-000000000000"),
         FirstName = "Max",
         LastName = "Mustermann",
         Email = "max.mustermann@gmail.com"
      };
      Owner3 = new Owner {
         Id = new Guid("30000000-0000-0000-0000-000000000000"),
         FirstName = "Arno",
         LastName = "Arndt",
         Email = "a.arndt@t-online.de"
      };
      Owner4 = new Owner {
         Id = new Guid("40000000-0000-0000-0000-000000000000"),
         FirstName = "Benno",
         LastName = "Bauer",
         Email = "b.bauer@gmail.com"
      };
      Owner5 = new Owner {
         Id = new Guid("50000000-0000-0000-0000-000000000000"),
         FirstName = "Christine",
         LastName = "Conrad",
         Email = "c.conrad@gmx.de"
      };
      Owner6 = new Owner {
         Id = new Guid("60000000-0000-0000-0000-000000000000"),
         FirstName = "Dana",
         LastName = "Deppe",
         Email = "d.deppe@icloud.com"
      };
      #endregion

      #region Accounts
      Account1 = new Account {
         Id = new Guid("01000000-0000-0000-0000-000000000000"),
         Iban = "DE10 1000 0000 0000 0000 00",
         Balance = 2100.0
      };
      Account2 = new Account {
         Id = new Guid("02000000-0000-0000-0000-000000000000"),
         Iban = "DE10 2000 0000 0000 0000 00",
         Balance = 2000.0
      };
      Account3 = new Account {
         Id = new Guid("03000000-0000-0000-0000-000000000000"),
         Iban = "DE20 1000 0000 0000 0000 00",
         Balance = 3000.0
      };
      Account4 = new Account {
         Id = new Guid("04000000-0000-0000-0000-000000000000"),
         Iban = "DE30 1000 0000 0000 0000 00",
         Balance = 2500.0
      };
      Account5 = new Account {
         Id = new Guid("05000000-0000-0000-0000-000000000000"),
         Iban = "DE40 1000 0000 0000 0000 00",
         Balance = 1900.0
      };
      Account6 = new Account {
         Id = new Guid("06000000-0000-0000-0000-000000000000"),
         Iban = "DE50 1000 0000 0000 0000 00",
         Balance = 3500.0
      };
      Account7 = new Account {
         Id = new Guid("07000000-0000-0000-0000-000000000000"),
         Iban = "DE50 2000 0000 0000 0000 00",
         Balance = 3100.0
      };
      Account8 = new Account {
         Id = new Guid("08000000-0000-0000-0000-000000000000"),
         Iban = "DE60 1000 0000 0000 0000 00",
         Balance = 4300.0
      };
      #endregion

      #region Beneficiaries
      Beneficiary1 = new Beneficiary{
         Id = new Guid("00100000-0000-0000-0000-000000000000"),
         FirstName = Owner5.FirstName,
         LastName = Owner5.LastName,
         Iban = Account6.Iban
      };
      Beneficiary2 = new Beneficiary{
         Id = new Guid("00200000-0000-0000-0000-000000000000"),
         FirstName = Owner5.FirstName,
         LastName = Owner5.LastName,
         Iban = Account7.Iban
      };
      Beneficiary3 = new Beneficiary{
         Id = new Guid("00300000-0000-0000-0000-000000000000"),
         FirstName = Owner3.FirstName,
         LastName = Owner3.LastName,
         Iban = Account4.Iban
      };
      Beneficiary4 = new Beneficiary{
         Id = new Guid("00400000-0000-0000-0000-000000000000"),
         FirstName = Owner4.FirstName,
         LastName = Owner4.LastName,
         Iban = Account5.Iban
      };
      Beneficiary5 = new Beneficiary{
         Id = new Guid("00500000-0000-0000-0000-000000000000"),
         FirstName = Owner3.FirstName,
         LastName = Owner3.LastName,
         Iban = Account4.Iban
      };
      Beneficiary6 = new Beneficiary{
         Id = new Guid("00600000-0000-0000-0000-000000000000"),
         FirstName = Owner4.FirstName,
         LastName = Owner4.LastName,
         Iban = Account5.Iban
      };
      Beneficiary7 = new Beneficiary{
         Id = new Guid("00700000-0000-0000-0000-000000000000"),
         FirstName = Owner6.FirstName,
         LastName = Owner6.LastName,
         Iban = Account8.Iban
      };
      Beneficiary8 = new Beneficiary{
         Id = new Guid("00800000-0000-0000-0000-000000000000"),
         FirstName = Owner2.FirstName,
         LastName = Owner2.LastName,
         Iban = Account3.Iban
      };
      Beneficiary9 = new Beneficiary{
         Id = new Guid("00900000-0000-0000-0000-000000000000"),
         FirstName = Owner6.FirstName,
         LastName = Owner6.LastName,
         Iban = Account8.Iban
      };
      Beneficiary10 = new Beneficiary{
         Id = new Guid("01000000-0000-0000-0000-000000000000"),
         FirstName = Owner1.FirstName,
         LastName = Owner1.LastName,
         Iban = Account1.Iban
      };
      Beneficiary11 = new Beneficiary{
         Id = new Guid("01100000-0000-0000-0000-000000000000"),
         FirstName = Owner1.FirstName,
         LastName = Owner1.LastName,
         Iban = Account2.Iban
      };
      #endregion

      #region Transfers
      Transfer1 = new Transfer{
         Id = new Guid("00010000-0000-0000-0000-000000000000"),
         Date = new DateTime(2023, 01, 01, 08, 00, 00).ToUniversalTime(),
         Description = "Erika an Chris1",
         Amount = 345.0
      };
      Transfer2 = new Transfer{
         Id = new Guid("00020000-0000-0000-0000-000000000000"),
         Date = new DateTime(2023, 02, 01, 09, 00, 00).ToUniversalTime(),
         Description = "Erika an Chris2",
         Amount = 231.0,
      };
      Transfer3 = new Transfer{
         Id = new Guid("00030000-0000-0000-0000-000000000000"),
         Date = new DateTime(2023, 03, 01, 10, 00, 00).ToUniversalTime(),
         Description = "Erika an Arne",
         Amount = 289.00
      };
      Transfer4 = new Transfer{
         Id = new Guid("00040000-0000-0000-0000-000000000000"),
         Date = new DateTime(2023, 04, 01, 11, 00, 00).ToUniversalTime(),
         Description = "Erika an Benno",
         Amount = 125.0
      };
      Transfer5 = new Transfer{
         Id = new Guid("00050000-0000-0000-0000-000000000000"),
         Date = new DateTime(2023, 05, 01, 12, 00, 00).ToUniversalTime(),
         Description = "Max an Arne",
         Amount = 167.0
      };
      Transfer6 = new Transfer{
         Id = new Guid("00060000-0000-0000-0000-000000000000"),
         Date = new DateTime(2023, 06, 01, 13, 00, 00).ToUniversalTime(),
         Description = "Max an Benno",
         Amount = 289.0
      };
      Transfer7 = new Transfer{
         Id = new Guid("00070000-0000-0000-0000-000000000000"),
         Date = new DateTime(2023, 07, 01, 14, 00, 00).ToUniversalTime(),
         Description = "Max an Dana",
         Amount = 312.0
      };
      Transfer8 = new Transfer{
         Id = new Guid("00080000-0000-0000-0000-000000000000"),
         Date = new DateTime(2023, 08, 01, 15, 00, 00).ToUniversalTime(),
         Description = "Arne an Max",
         Amount = 278.0
      };
      Transfer9 = new Transfer{
         Id = new Guid("00090000-0000-0000-0000-000000000000"),
         Date = new DateTime(2023, 09, 01, 16, 00, 00).ToUniversalTime(),
         Description = "Arne an Christ2",
         Amount = 356.0
      };
      Transfer10 = new Transfer{
         Id = new Guid("00100000-0000-0000-0000-000000000000"),
         Date = new DateTime(2023, 10, 01, 17, 00, 00).ToUniversalTime(),
         Description = "Benno an Erika1",
         Amount = 398.0
      };
      Transfer11 = new Transfer{
         Id = new Guid("00110000-0000-0000-0000-000000000000"),
         Date = new DateTime(2023, 11, 01, 18, 00, 00).ToUniversalTime(),
         Description = "Benno an Erika2",
         Amount = 89.0
      };
      #endregion

      #region Transaction
      Transaction1 = new Transaction{
         Id = new Guid("00001000-0000-0000-0000-000000000000")
      };
      Transaction2 = new Transaction{
         Id = new Guid("00002000-0000-0000-0000-000000000000")
      };
      Transaction3 = new Transaction{
         Id = new Guid("00003000-0000-0000-0000-000000000000")
      };
      Transaction4 = new Transaction{
         Id = new Guid("00004000-0000-0000-0000-000000000000")
      };
      Transaction5 = new Transaction{
         Id = new Guid("00005000-0000-0000-0000-000000000000")
      };
      Transaction6 = new Transaction{
         Id = new Guid("00006000-0000-0000-0000-000000000000")
      };
      Transaction7 = new Transaction{
         Id = new Guid("00007000-0000-0000-0000-000000000000")
      };
      Transaction8 = new Transaction{
         Id = new Guid("00008000-0000-0000-0000-000000000000")
      };
      Transaction9 = new Transaction{
         Id = new Guid("00009000-0000-0000-0000-000000000000")
      };
      Transaction10 = new Transaction{
         Id = new Guid("00010000-0000-0000-0000-000000000000")
      };
      Transaction11 = new Transaction{
         Id = new Guid("00011000-0000-0000-0000-000000000000")
      };
      Transaction12 = new Transaction{
         Id = new Guid("00012000-0000-0000-0000-000000000000")
      };
      Transaction13 = new Transaction{
         Id = new Guid("00013000-0000-0000-0000-000000000000")
      };
      Transaction14 = new Transaction{
         Id = new Guid("00014000-0000-0000-0000-000000000000")
      };
      Transaction15 = new Transaction{
         Id = new Guid("00015000-0000-0000-0000-000000000000")
      };
      Transaction16 = new Transaction{
         Id = new Guid("00016000-0000-0000-0000-000000000000")
      };
      Transaction17 = new Transaction{
         Id = new Guid("00017000-0000-0000-0000-000000000000")
      };
      Transaction18 = new Transaction{
         Id = new Guid("00018000-0000-0000-0000-000000000000")
      };
      Transaction19 = new Transaction{
         Id = new Guid("00019000-0000-0000-0000-000000000000")
      };
      Transaction20 = new Transaction{
         Id = new Guid("00020000-0000-0000-0000-000000000000")
      };
      Transaction21 = new Transaction{
         Id = new Guid("00021000-0000-0000-0000-000000000000")
      };
      Transaction22 = new Transaction{
         Id = new Guid("00022000-0000-0000-0000-000000000000")
      };
      #endregion

      Owners = [Owner1, Owner2, Owner3, Owner4, Owner5, Owner6];
      Accounts = [Account1, Account2, Account3, Account4, Account5, Account6, Account7, Account8];
      Beneficiaries = [
         Beneficiary1, Beneficiary2, Beneficiary3, Beneficiary4, Beneficiary5,
         Beneficiary6, Beneficiary7, Beneficiary8, Beneficiary9, Beneficiary10, Beneficiary11
      ];
      Transfers = [
         Transfer1, Transfer2, Transfer3, Transfer4, Transfer5, Transfer6, Transfer7,
         Transfer8, Transfer9, Transfer10, Transfer11
      ];
      Transactions = [
         Transaction1, Transaction2, Transaction3, Transaction4, Transaction5,
         Transaction6, Transaction7, Transaction8, Transaction9, Transaction10, Transaction11, Transaction12,
         Transaction13, Transaction14, Transaction15, Transaction16, Transaction17, Transaction18, Transaction19,
         Transaction20, Transaction21, Transaction22
      ];
   }
   
   // Setup Relations between Owners and Accounts
   public Seed InitAccounts(){
      Owner1.Add(Account1); // Owner 1 with 2 accounts 1+2
      Owner1.Add(Account2);
      Owner2.Add(Account3); // Owner 2 witn account 3
      Owner3.Add(Account4); // Owner 3 with account 4
      Owner4.Add(Account5); // Owner 4 with account 5
      Owner5.Add(Account6); // Owner 5 with 2 accounts 6+7
      Owner5.Add(Account7);
      Owner6.Add(Account8); // Owner 6 wiht account 8
      return this;
   }
   
   // Setup Relations between Owner1 and Account1, Account2
   public Seed InitAccountsForOwner1(){
      Owner1.Add(Account1); // Owner 1 with 2 accounts 1+2
      Owner1.Add(Account2);
      return this;
   }

   // Setup Relations between Accounts and Beneficiaries
   public Seed InitBeneficiaries(){
      // Accounts
      Account1.Add(Beneficiary1); // Account 1
      Account1.Add(Beneficiary2);
      Account2.Add(Beneficiary3); // Account 2
      Account2.Add(Beneficiary4);
      Account3.Add(Beneficiary5); // Account 3
      Account3.Add(Beneficiary6);
      Account3.Add(Beneficiary7);
      Account4.Add(Beneficiary8); // Account 4
      Account4.Add(Beneficiary9);
      Account5.Add(Beneficiary10); // Account 5
      Account5.Add(Beneficiary11);
      return this;
   }

   public Seed InitBeneficiariesForAccoutsOfOwner1(){
      Account1.Add(Beneficiary1); // Account 1
      Account1.Add(Beneficiary2);
      Account2.Add(Beneficiary3); // Account 2
      Account2.Add(Beneficiary4);
      return this;
   }

   public Seed InitTransfersTransactions(){
      // Transaction
      SendMoney(Account1, Beneficiary1, Account6, Transfer1, Transaction1, Transaction2); // From Account 1
      SendMoney(Account1, Beneficiary2, Account7, Transfer2, Transaction3, Transaction4);
      SendMoney(Account2, Beneficiary3, Account4, Transfer3, Transaction5, Transaction6); // From Account 2
      SendMoney(Account2, Beneficiary4, Account5, Transfer4, Transaction7, Transaction8);

      SendMoney(Account3, Beneficiary5, Account4, Transfer5, Transaction9, Transaction10); // From Account 3
      SendMoney(Account3, Beneficiary6, Account5, Transfer6, Transaction11, Transaction12);
      SendMoney(Account3, Beneficiary7, Account8, Transfer7, Transaction13, Transaction14);

      SendMoney(Account4, Beneficiary8, Account3, Transfer8, Transaction15, Transaction16); // From Account 4
      SendMoney(Account4, Beneficiary9, Account8, Transfer9, Transaction17, Transaction18);

      SendMoney(Account5, Beneficiary10, Account1, Transfer10, Transaction19, Transaction20); // From Account 5
      SendMoney(Account5, Beneficiary11, Account2, Transfer11, Transaction21, Transaction22);

      return this;
   }

   public void SendMoney(
      Account accountDebit,
      Beneficiary beneficiary,
      Account accountCredit,
      Transfer transfer,
      Transaction transactionDebit,
      Transaction transactionCredit
   ){
      accountDebit.Add(transfer, beneficiary);

      transactionDebit.Amount = -transfer.Amount;
      transactionDebit.Account = accountDebit;
      transactionDebit.AccountId = accountDebit.Id;
      transactionDebit.Transfer = transfer;
      transactionDebit.TransferId = transfer.Id;
      
      if(beneficiary.Iban != accountCredit.Iban)
         throw new Exception("Error in Seed SendMoney");
      transactionCredit.Amount = transfer.Amount;
      transactionCredit.Account = accountCredit;
      transactionCredit.AccountId = accountCredit.Id;
      transactionCredit.Transfer = transfer;
      transactionCredit.TransferId = transfer.Id;

      transactionDebit.Account.Add(transactionDebit, transfer);
      transactionCredit.Account.Add(transactionCredit, transfer);
   }

   public void PrepareTest1(){ 
      Owner1.Add(Account1);
      Owner5.Add(Account6);
      // AccountDebit
      Account1.Add(Beneficiary1);
      // Account1.Add(Transfer1, Beneficiary1);
      // Account1.Add(Transaction1, Transfer1);
      // // AccountCredit
      // Account6.Add(Transaction2, Transfer1);
   }
   
   public void DoTransfer1(){
      Owner1.Add(Account1);
      Owner5.Add(Account6);
      // AccountDebit
      Account1.Add(Beneficiary1);
      
      SendMoney(Account1, Beneficiary1, Account6,
         Transfer1, Transaction1, Transaction2); 
   }
   
   public void PrepareExample1WithReverse(DateTime reverseDate){
      Owner1.Add(Account1);
      Owner5.Add(Account6);
      // AccountDebit
      Account1.Add(Beneficiary1);
      Account1.Add(Transfer1, Beneficiary1);
      Account1.Add(Transaction1, Transfer1);
      // AccountCredit
      Account6.Add(Transaction2, Transfer1);

      Transfer transfer = new(){
         Id = new Guid("90010000-0000-0000-0000-000000000000"),
         Description = "Reverse " + Transfer1.Description,
         Date = reverseDate,
         Amount = -Transfer1.Amount
      };

      Transaction transactionDebit = new(){
         Id = new Guid("90001000-0000-0000-0000-000000000000"),
         Amount = Transfer1.Amount
      };
      Transaction transactionCredit = new(){
         Id = new Guid("90002000-0000-0000-0000-000000000000"),
         Amount = -Transfer1.Amount
      };
      
      // Reverse (Debit) - Lastschrift
      Account1.Add(transfer, Beneficiary1);
      Account1.Add(transactionDebit, transfer);
      // Reverse (Credit)  - Gutschrift     
      Account6.Add(transactionCredit, transfer);
   }

   public void Example1(){
      Owner1.Add(Account1);
      Owner5.Add(Account6);
      Account1.Add(Beneficiary1); // beneficiary 1 for account 1
      SendMoney(Account1, Beneficiary1, Account6,
         Transfer1, Transaction1, Transaction2); // From Account 1
   }

   public void Example2(){
      Owner1.Add(Account1); // owner 1 with 2 accounts 1+2
      Owner1.Add(Account2);
      Owner5.Add(Account6); // owner 5 with 2 accounts 6+7
      Owner5.Add(Account7);
      Account1.Add(Beneficiary1); // beneficiary 1+2 for account 1
      Account1.Add(Beneficiary2);
      Account2.Add(Beneficiary3); // beneficiary 3+4 for account 2
      Account2.Add(Beneficiary4);
      SendMoney(Account1, Beneficiary1, Account6,
         Transfer1, Transaction1, Transaction2); // From Account 1
      SendMoney(Account1, Beneficiary2, Account7,
         Transfer2, Transaction3, Transaction4);
   }

}