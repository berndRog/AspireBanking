using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Asp.Versioning;
using AutoMapper;
using BankingApi.Core;
using BankingApi.Core.DomainModel.Entities;
using BankingApi.Core.Dto;
using BankingApi.Core.Misc;
using BankingApi.Core.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
namespace BankingApi.Controllers.V2;

[ApiVersion("3.0")]
[Route("banking/v{version:apiVersion}")]
[ApiController]
public class 
   
   AccountsController(
   IOwnersRepository ownersRepository,
   IAccountsRepository accountsRepository,
   ITransactionsRepository transactionRepository,
   IDataContext dataContext,
   IMapper mapper,
   ILogger<AccountsController> logger
) : ControllerBase {
   
   
   /// <summary>
   /// Get all accounts 
   /// </summary>
   /// <returns>IEnumerable{OwnerDto?}</returns>
   /// <response code="200">Ok: Owner with given id returned</response>
   [HttpGet("accounts")]
   [Authorize(Roles = "webtech-admin")]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(StatusCodes.Status200OK)]
   public async Task<ActionResult<AccountDto?>> GetAccounts() {
      var accounts = await accountsRepository.SelectAsync();
      return Ok(mapper.Map<IEnumerable<AccountDto>>(accounts));
   }
   
   /// <summary>
   /// Get all accounts of a given ownerId
   /// </summary>
   /// <param name="ownerId"></param>
   /// <returns>IEnumerable{AccountDto}; </returns>
   /// <response code="200">Ok. Accounts returned.</response>
   [HttpGet("owners/{ownerId:Guid}/accounts")]
   [Authorize(Roles = "webtech-user,webtech-admin")]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesDefaultResponseType]
   public async Task<ActionResult<IEnumerable<AccountDto>>> GetAccountsByOwnerId(
      [FromRoute] Guid ownerId
   ) {
      logger.LogDebug("GetAccountsByOwner ownerId={ownerId}", ownerId);
      var accounts = 
         await accountsRepository.SelectByOwnerIdAsync(ownerId);
      return Ok(mapper.Map<IEnumerable<AccountDto>>(accounts));
   }

   /// <summary>
   /// Get an account by id. 
   /// </summary>
   /// <param name="id">id of the account</param>
   /// <returns>AccountDto?</returns>
   /// <response code="200">Ok: Account with given id returned.</response>
   /// <response code="404">NotFound: Account with given id not found.</response>
   [HttpGet("accounts/{id:guid}")]
   [Authorize(Roles = "webtech-user,webtech-admin")]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesResponseType(StatusCodes.Status404NotFound)]
   [ProducesDefaultResponseType]
   public async Task<ActionResult<AccountDto>> GetAccountById(
      [FromRoute] Guid id
   ) {
      logger.LogDebug("GetAccoutById {id}", id.As8());
      return await accountsRepository.FindByIdAsync(id) switch {
         { } account => Ok(mapper.Map<AccountDto>(account)),
         null => NotFound("Account with given id not found")
      };
   }

   /// <summary>
   /// Find an account by Iban. 
   /// </summary>
   /// <param name="iban">part of the iban</param>
   /// <returns>AccountDto?</returns>
   /// <response code="200">Ok: Account with given iban returned.</response>
   /// <response code="400">BadRequest: Iban is not valid.</response>
   /// <response code="404">NotFound: Account with given iban not found.</response>
   [HttpGet("accounts/iban/{iban}")]
   [Authorize(Roles = "webtech-user,webtech-admin")]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesResponseType(StatusCodes.Status400BadRequest)]
   [ProducesResponseType(StatusCodes.Status404NotFound)]
   [ProducesDefaultResponseType]
   public async Task<ActionResult<AccountDto?>> GetAccountByIban(
      [FromRoute] string iban
   ) {
      logger.LogDebug("GetByIban {iban}", iban);
      var checkedIban = Utils.CheckIban(iban);
      if (checkedIban != iban.Replace(" ", "").ToUpper())
         return BadRequest("Iban is not valid.");
      
      return await accountsRepository.FindByAsync(a => a.Iban == checkedIban) switch {
         { } account => Ok(mapper.Map<AccountDto>(account)),
         null => NotFound("Account with given Iban not found")
      };
   }

   /// <summary>
   /// Create an account. 
   /// Account can have an Id, otherwise it will be created
   /// </summary>
   /// <param name="ownerId"></param>
   /// <param name="accountDto"></param>
   /// <returns>AccountDto?</returns>
   /// <response code="201">Created: Account is created</response>
   /// <response code="400">Bad request: ownerId does not exist.</response>
   /// <response code="400">Bad request: ownerId from route does not match ownerId in account.</response>
   /// <response code="409">Conflict: Account with given id already exists.</response>
   [HttpPost("owners/{ownerId:guid}/accounts")]
   [Authorize(Roles = "webtech-user,webtech-admin")]
   [Consumes(MediaTypeNames.Application.Json)]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(StatusCodes.Status201Created)]
   [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
   
   public async Task<ActionResult<AccountDto>> CreateAccount(
      [FromRoute] Guid ownerId,
      [FromBody] AccountDto accountDto
   ) {
      logger.LogDebug("CreateAccount");
      
      // get the owner for the account
      var owner = await ownersRepository.FindByIdAsync(ownerId);
      if(owner == null)
         return BadRequest("Owner for account doesn't exists.");
      // validate input
      var (error, actionResult) = await ValidateAccountAsync(ownerId, accountDto);
      if (error) return actionResult;
      if((actionResult?.Result as OkObjectResult)?.Value is not AccountDto value) 
         return BadRequest("Error in ValidateAndSendMoney");
      accountDto = value;
      
      // map dto to domainModel
      var account = mapper.Map<Account>(accountDto);

      // check if account with given Id already exists
      if (await accountsRepository.FindByIdAsync(account.Id) != null)
         return Conflict("CreateOwner: Account with given id already exists.");

      // update domainModel
      owner.Add(account);

      // save to repository and write to database 
      accountsRepository.Add(account);
      await dataContext.SaveAllChangesAsync();

      // Request == null in unit tests
      var path = Request == null
         ? $"/banking/v2/accounts/{account.Id}"
         : $"{Request.Path}";
      var uri = new Uri(path, UriKind.Relative);
      return Created(uri: uri, value: mapper.Map<AccountDto>(account));
   }
   
   /// <summary>
   /// Delete an account
   /// </summary>
   /// <param name="ownerId">id of the the owner</param>
   /// <param name="id">id of the account</param>
   [HttpDelete("owners/{ownerId:guid}/accounts/{id:guid}")] 
   [Authorize(Roles = "webtech-admin")]
   [ProducesResponseType(StatusCodes.Status204NoContent)]
   [ProducesResponseType(StatusCodes.Status404NotFound)]
   
   public async Task<IActionResult> DeleteAccount(
      [FromRoute] Guid ownerId, 
      [FromRoute] Guid id
   ) {
      logger.LogDebug("DeleteAccount {id}", id.As8());
      
      // check if owner with given Id exists
      var owner = await ownersRepository.FindByIdAsync(ownerId);
      if (owner == null)
         return NotFound("DeleteAccount: Owner not found.");

      var account = await accountsRepository.FindByIdJoinAsync(id,true,true,true,true);
      if (account == null)
         return NotFound("DeleteAccount: Account not found.");
     
      if(account.OwnerId != owner.Id)
         return BadRequest("DeleteAccount: Owner and Account data do not match.");
      
      // get all transactions for account      
      var transactions = await transactionRepository.FilterByAccountIdAsync(account.Id,
         null);

      // remove in repository
      foreach (var transaction in transactions)
         transactionRepository.Remove(transaction);      
      accountsRepository.Remove(account);
      // write to database
      await dataContext.SaveAllChangesAsync();

      // return no content
      return NoContent(); 
   }

   private async Task<(bool error, ActionResult<AccountDto> result)> ValidateAccountAsync(
      Guid ownerId,
      AccountDto accountDto
   ) {
      // check if accountDto.Id is empty
      if (accountDto.Id == Guid.Empty)
         accountDto = accountDto with { Id = Guid.NewGuid() };

      // check if accountDto.Id already exists
      if (await accountsRepository.FindByIdAsync(accountDto.Id) != null)
         return (true, Conflict("Account with given id already exists."));
      
      // check if ownerDto.FirstName is too short or too long
      if (accountDto.Iban.Length < 3)
         return (true, BadRequest("FirstName is too short (min 3 chars)."));

      return (false, Ok(accountDto));
   }

}