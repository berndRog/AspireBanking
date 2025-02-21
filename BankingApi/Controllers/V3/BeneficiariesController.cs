using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Asp.Versioning;
using AutoMapper;
using BankingApi.Core;
using BankingApi.Core.DomainModel.Entities;
using BankingApi.Core.Dto;
using BankingApi.Core.Misc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
namespace BankingApi.Controllers.V3;
[Route("banking/v{version:apiVersion}")]
[ApiVersion("3.0")]
[ApiController]
public class BeneficiariesController(
   IOwnersRepository ownersRepository,
   IAccountsRepository accountsRepository,
   IBeneficiariesRepository beneficiariesRepository,
   ITransfersRepository transfersRepository,
   IDataContext dataContext,
   IMapper mapper,
   ILogger<BeneficiariesController> logger
): ControllerBase {

   /// <summary>
   /// List all beneficiaries.
   /// </summary>
   /// <returns>IEnumerable{BeneficiaryDto}</returns>
   /// <response code="200">Ok. Beneficiarys returned.</response>
   [HttpGet("beneficiaries")]
   [Authorize(Roles = "webtech-admin")]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesDefaultResponseType]
   public async Task<ActionResult<IEnumerable<BeneficiaryDto>>> GetAll(){
      var beneficiaries = await beneficiariesRepository.SelectAsync();
      return Ok(mapper.Map<IEnumerable<BeneficiaryDto>>(beneficiaries));
   }
   
   /// <summary>
   /// List beneficiaries of an account by accountId.
   /// </summary>
   /// <param name="accountId">Account Id</param>
   /// <returns>IEnumerable{BeneficiaryDto}</returns>
   /// <response code="200">Ok. Beneficiarys returned.</response>
   /// <response code="400">Bad request: accountId does not exist.</response>
   [HttpGet("accounts/{accountId:Guid}/beneficiaries")]
   [Authorize(Roles = "webtech-user,webtech-admin")]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
   [ProducesDefaultResponseType]
   public async Task<ActionResult<IEnumerable<BeneficiaryDto>>> GetBeneficiariesByAccountId(
      [FromRoute] Guid accountId
   ){
      logger.LogDebug("GetBeneficiariesByAccountId {accountId}", accountId);

      var account = await accountsRepository.FindByIdAsync(accountId);
      if(account == null)
         return BadRequest("Bad request: accountId does not exist.");

      var beneficiaries =
         await beneficiariesRepository.SelectByAccountIdAsync(accountId);

      return Ok(mapper.Map<IEnumerable<BeneficiaryDto>>(beneficiaries));
   }

   /// <summary>
   /// Get a beneficiary by Id. 
   /// </summary>
   /// <param name="id">id of the beneficiary</param>
   /// <returns>BeneficiaryDto?</returns>
   /// <response code="200">Ok: Beneficiary with given id returned</response>
   /// <response code="404">NotFound: Beneficiary with given id not found</response>
   [HttpGet("beneficiaries/{id:guid}")]
   [Authorize(Roles = "webtech-user,webtech-admin")]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(typeof(BeneficiaryDto), StatusCodes.Status200OK)]
   [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
   public async Task<ActionResult<BeneficiaryDto>> GetBeneficiaryById(
      [FromRoute] Guid id
   ){
      logger.LogDebug("GetBeneficiaryById {id}", id.As8());
      return await beneficiariesRepository.FindByIdAsync(id) switch {
         { } beneficiary => Ok(mapper.Map<BeneficiaryDto>(beneficiary)),
         null => NotFound("Beneficiary with given id not found.")
      };
   }

   /// <summary>
   /// Get beneficiary by FirstName and LastName  
   /// </summary>
   /// <param name="firstName">first name</param>
   /// <param name="lastName">last name</param>
   /// <returns>BeneficiaryDto?</returns>
   /// <response code="200">Ok: Beneficiary with given name returned</response>
   /// <response code="404">NotFound: Beneficiary with given name not found</response>
   [HttpGet("beneficiaries/name")]
   [Authorize(Roles = "webtech-user,webtech-admin")]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(typeof(BeneficiaryDto), StatusCodes.Status200OK)]
   [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
   public async Task<ActionResult<BeneficiaryDto>> GetBeneficiariesByName(
      [FromQuery] string firstName,
      [FromQuery] string lastName
   ){
      // Find beneficiaries by first and last name
      var beneficiary = await beneficiariesRepository.FindByOwnersNameAsync(firstName, lastName);

      // if beneficiary is found return them
      if (beneficiary != null)return Ok(mapper.Map<BeneficiaryDto>(beneficiary));
      // else return not found
      return NotFound("Beneficiaries with given names not found");
   }

   /// <summary>
   /// Create a beneficiary. 
   /// Beneficiary can have an Id, otherwise it will be created
   /// </summary>
   /// <param name="accountId">Account Id</param>
   /// <param name="beneficiaryDto">BeneficiaryDto</param>
   /// <returns>AccountDto?</returns>
   /// <response code="201">Created: Beneficiary is created</response>
   /// <response code="400">Bad request: accountId and Beneficiary.AccountId do not match.</response>
   /// <response code="409">Conflict: Beneficiary with given id already exists.</response>
   [HttpPost("accounts/{accountId:Guid}/beneficiaries")]
   [Authorize(Roles = "webtech-user,webtech-admin")]
   [Consumes(MediaTypeNames.Application.Json)]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(typeof(BeneficiaryDto), StatusCodes.Status201Created)]
   [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
  
   public async Task<ActionResult<BeneficiaryDto>> CreateBeneficiary(
      [FromRoute] Guid accountId,
      [FromBody] BeneficiaryDto beneficiaryDto
   ){
      logger.LogDebug("CreateBeneficiary accoungtId={accountId}", accountId);
      
      // set accountId in beneficiaryDto
      beneficiaryDto = beneficiaryDto with { 
         Iban = Utils.CheckIban(beneficiaryDto.Iban), 
         AccountId = accountId
      };
      
      // validate input
      var (error, actionResult) = await ValidateBeneficiaryAsync(accountId, beneficiaryDto);
      if (error) return actionResult;
      if((actionResult.Result as OkObjectResult)?.Value is not BeneficiaryDto value) 
         return BadRequest("Error in ValidateAndSendMoney");
      beneficiaryDto = value;
      
      // map dto to domain model
      var beneficiary = mapper.Map<Beneficiary>(beneficiaryDto);
      
      // update domainModel
      // Debit account
      var account = await accountsRepository.FindByIdAsync(beneficiaryDto.AccountId);
      if(account == null  || account.Id != accountId)
         return BadRequest("Bad request: accountId does not exist.");
      account.Add(beneficiary);

      // save to repository and write to database
      beneficiariesRepository.Add(beneficiary);
      await dataContext.SaveAllChangesAsync();

      // Request == null in unit tests
      var path = Request == null
         ? $"/banking/v2/beneficiaries/{beneficiary.Id}"
         : $"{Request.Path}";
      var uri = new Uri(path, UriKind.Relative);
      return Created(uri: uri, value: mapper.Map<BeneficiaryDto>(beneficiary));
   }
   
   /// <summary>
   /// Delete a beneficiary by Id.
   /// </summary>
   /// <param name="id">given id</param>
   /// <returns>IActionResult</returns>
   /// <response code="204">NoContent: Beneficiary with given id is deleted.</response>
   /// <response code="404">NotFound: Beneficiary with given id not found</response>
   [HttpDelete("accounts/{accountId:guid}/beneficiaries/{id:guid}")]
   [Authorize(Roles = "webtech-admin")]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesResponseType(StatusCodes.Status404NotFound)]
   [ProducesDefaultResponseType]
   public async Task<IActionResult> DeleteBeneficiary(
      [FromRoute] Guid accountId,
      [FromRoute] Guid id
   ){
      logger.LogDebug("DeleteBeneficiary {id}", id);
      
      // Debit account
      var account = await accountsRepository.FindByIdAsync(accountId);
      if(account == null  || account.Id != accountId)
         return BadRequest("Bad request: accountId does not exist.");
      
      // check if beneficiary with given id exists
      var beneficiary = await beneficiariesRepository.FindByIdAsync(id);
      if(beneficiary == null)
         return NotFound("DeleteBeneficiary: Beneficiary with given id not found.");

      if(beneficiary.AccountId != accountId)
         return BadRequest("Bad request: accountId does not match.");

      
      // Load all transfers with the beneficiary to delete
      var transfers = await transfersRepository.SelectByBeneficiaryIdAsync(id);
      foreach(var transfer in transfers) {
         transfer.Remove();
         await transfersRepository.UpdateAsync(transfer);
      }
      
      // save to repository and write to database 
      beneficiariesRepository.Remove(beneficiary);
      await dataContext.SaveAllChangesAsync();

      return NoContent();
   }

   private async Task<(bool error, ActionResult<BeneficiaryDto> result)> ValidateBeneficiaryAsync(
      Guid accountId, 
      BeneficiaryDto beneficiaryDto
   ) {

      if(beneficiaryDto.AccountId != accountId)
         return ( true, BadRequest("Bad request: accountId does not match."));
      
      // Get account by Iban
      if (beneficiaryDto.Iban.Length < 1) 
         return( true, BadRequest("Beneficiary: Iban is empty"));

      // check if beneficiaryDto.Id is empty
      if (beneficiaryDto.Id == Guid.Empty)
         beneficiaryDto = beneficiaryDto with { Id = Guid.NewGuid() };
      // check if beneficiaryDto.Id already exists
      if (await beneficiariesRepository.FindByIdAsync(beneficiaryDto.Id) != null)
         return (true, Conflict("Beneficiary with given id already exists."));
      
      // Get account by Iban
      var accountCredit = await accountsRepository.FindByAsync(a => a.Iban == beneficiaryDto.Iban);
      // check if account with given Iban exists
      if(accountCredit == null)
         return (true, BadRequest("Beneficiary: Credit account with given Iban not found"));
      
      // Get owners name by accout.OwnerId
      var name = beneficiaryDto.FirstName + " " + beneficiaryDto.LastName;
      var owners = await ownersRepository.LikeNameByAsync(name);
      if(!owners.Any())
         return (true, BadRequest("Beneficiary: Owner for credit account not found"));
      
      return (false, Ok(beneficiaryDto));
   }
}