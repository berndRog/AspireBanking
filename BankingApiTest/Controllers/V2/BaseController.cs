using System;
using AutoMapper;
using BankingApi.Controllers.V2;
using BankingApi.Controllers.V3;
using BankingApi.Core;
using BankingApi.Persistence;
using BankingApiTest.Di;
using BankingApiTest.Di.V2;
using BankingApiTest.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
namespace BankingApiTest.Controllers.V2;
[Collection(nameof(SystemTestCollectionDefinition))]
public class BaseControllerTest {

   protected readonly OwnersController _ownersController;
   protected readonly AccountsController _accountsController;
   protected readonly BeneficiariesController _beneficiariesController;
   protected readonly TransfersController _transfersController;
   protected readonly TransactionsController _transactionsController;

   protected readonly IOwnersRepository _ownersRepository;
   protected readonly IAccountsRepository _accountsRepository;
   protected readonly IBeneficiariesRepository _beneficiariesRepository;
   protected readonly ITransfersRepository _transfersRepository;
   protected readonly ITransactionsRepository _transactionsRepository;
   protected readonly IDataContext _dataContext;
   
   protected readonly ArrangeTest _arrangeTest;
   protected readonly IMapper _mapper;
   protected readonly Seed _seed;

   protected BaseControllerTest() {
      IServiceCollection serviceCollection = new ServiceCollection();
      serviceCollection.AddPersistenceTest();
      serviceCollection.AddControllersTest();
      var serviceProvider = serviceCollection.BuildServiceProvider()
         ?? throw new Exception("Failed to build Serviceprovider");

      var dbContext = serviceProvider.GetRequiredService<DataContext>()
         ?? throw new Exception("Failed to create an instance of DataContext");
      dbContext.Database.EnsureDeleted();
      dbContext.Database.EnsureCreated();
      
      _ownersController = serviceProvider.GetRequiredService<OwnersController>()
         ?? throw new Exception("Failed to create an instance of OwnersController");
      _accountsController = serviceProvider.GetRequiredService<AccountsController>()
         ?? throw new Exception("Failed to create an instance of AccountsController");
      _beneficiariesController = serviceProvider.GetRequiredService<BeneficiariesController>()
         ?? throw new Exception("Failed to create an instance of BeneficiariesController");
      _transfersController = serviceProvider.GetRequiredService<TransfersController>()
         ?? throw new Exception("Failed to create an instance of TransfersController");
      _transactionsController = serviceProvider.GetRequiredService<TransactionsController>()
         ?? throw new Exception("Failed to create an instance of TransactionsController");
      
      _ownersRepository = serviceProvider.GetRequiredService<IOwnersRepository>()
         ?? throw new Exception("Failed to create an instance of IOwnersRepository");
      _accountsRepository = serviceProvider.GetRequiredService<IAccountsRepository>()
         ?? throw new Exception("Failed to create an instance of IAccountsRepository");
      _beneficiariesRepository = serviceProvider.GetRequiredService<IBeneficiariesRepository>()
         ?? throw new Exception("Failed to create an instance of IBeneficiariesRepository");
      _transfersRepository = serviceProvider.GetRequiredService<ITransfersRepository>()
         ?? throw new Exception("Failed to create an instance of ITransfersRepository");
      _transactionsRepository = serviceProvider.GetRequiredService<ITransactionsRepository>()
         ?? throw new Exception("Failed to create an instance of ITransactionsRepository");
      _dataContext = serviceProvider.GetRequiredService<IDataContext>() 
         ?? throw new Exception("Failed to create an instance of IDataContext");
      _arrangeTest = serviceProvider.GetRequiredService<ArrangeTest>()
         ?? throw new Exception("Failed create an instance of ArrangeTest");
      _mapper = serviceProvider.GetRequiredService<IMapper>();
      _seed = new Seed();
   }
}