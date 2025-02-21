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

[ApiVersion("3.0")]
[Route("banking/v{version:apiVersion}")]
[ApiController]
public class OwnersController(
   IOwnersRepository ownersRepository,
   IAccountsRepository accountsRepository,
   ITransactionsRepository transactionRepository,
   IDataContext dataContext,
   IMapper mapper,
   ILogger<OwnersController> logger
): ControllerBase {
   
   /// <summary>
   /// Get all owners 
   /// </summary>
   /// <returns>IEnumerable{OwnerDto?}</returns>
   /// <response code="200">Ok: Owner with given id returned</response>
   [HttpGet("owners")]
   [Authorize(Roles = "webtech-admin")]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(StatusCodes.Status200OK)]
   public async Task<ActionResult<OwnerDto?>> GetOwners() {
      var owners = await ownersRepository.SelectAsync();
      return Ok(mapper.Map<IEnumerable<OwnerDto>>(owners));
   }
   
   /// <summary>
   /// Get an owner by Id
   /// </summary>
   /// <param name="id">id of the owner</param>
   /// <returns>OwnerDto?</returns>
   /// <response code="200">Ok: Owner with given id returned</response>
   /// <response code="404">NotFound: Owner with given id not found</response>
   [HttpGet("owners/{id:guid}")]
   [Authorize(Roles = "webtech-user,webtech-admin")]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesResponseType(StatusCodes.Status404NotFound)]
   public async Task<ActionResult<OwnerDto?>> GetOwnerById(
      [FromRoute] Guid id
   ) {
      logger.LogDebug("GetOwnerById() id={id}", id.As8());
      return await ownersRepository.FindByIdAsync(id) switch {
         // return owner as Dto
         { } owner => Ok(mapper.Map<OwnerDto>(owner)),
         // return not found
         null => NotFound("Owner with given Id not found")
      };
   }

   /// <summary>
   /// Exists an owner for the given UserName
   /// </summary>
   /// <param name="userName"></param>
   /// <returns></returns>
   [HttpGet("owners/exists")]
   [Authorize(Roles = "webtech-user,webtech-admin")]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(StatusCodes.Status200OK)]
   public async Task<IActionResult> ResourceExists(
      [FromQuery] string userName
   ) {
      var exists = await ownersRepository.ExistsByUserNameAsync(userName);
      return Ok(exists);
   }

   
   /// <summary>
   /// Get an owner by UserName
   /// </summary>
   /// <param name="userName">userName of the owner</param>
   /// <returns>OwnerDto?</returns>
   /// <response code="200">Ok: Owner with given userName returned</response>
   /// <response code="404">NotFound: Owner with given userName not found</response>
   [HttpGet("owners/username")]
   [Authorize(Roles = "webtech-user,webtech-admin")]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesResponseType(StatusCodes.Status404NotFound)]
   public async Task<ActionResult<OwnerDto?>> GetOwnerByUserName(
      [FromQuery] string userName
   ) {
      logger.LogDebug("GetOwnerByUserName() username={1}",userName);
      return await ownersRepository.FindByUserNameAsync(userName) switch {
         // return owner as Dto
         { } owner => Ok(mapper.Map<OwnerDto>(owner)),
         // return not found
         null => NotFound("Owner with given UserName not found")
      };
   }
   
   /// <summary>
   /// Get an owner by UserId
   /// </summary>
   /// <param name="userId">userId of the owner</param>
   /// <returns>OwnerDto?</returns>
   /// <response code="200">Ok: Owner with given userId returned</response>
   /// <response code="404">NotFound: Owner with given userid not found</response>
   [HttpGet("owners/userid")]
   [Authorize(Roles = "webtech-user,webtech-admin")]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesResponseType(StatusCodes.Status404NotFound)]
   public async Task<ActionResult<OwnerDto?>> GetOwnerByUserId(
      [FromQuery] string userId
   ) {
      logger.LogDebug("GetOwnerByUserId() userid={1}",userId);
      return await ownersRepository.FindByUserIdAsync(userId) switch {
         // return owner as Dto
         { } owner => Ok(mapper.Map<OwnerDto>(owner)),
         // return not found
         null => NotFound("Owner with given UserId not found")
      };
   }

   
   /// <summary>
   /// List owners by Name with SQL like %name%  
   /// </summary>
   /// <param name="name">query: ?name=complete name</param>
   /// <returns>IEnumerable{OwnerDto}</returns>
   /// <response code="200">Ok: Owners with given name returned</response>
   /// <response code="404">NotFound: Owner with given name not found</response>
   [HttpGet("owners/name")]
   [Authorize(Roles = "webtech-user,webtech-admin")]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesResponseType(StatusCodes.Status404NotFound)]
   public async Task<ActionResult<IEnumerable<OwnerDto>>> GetOwnersByName(
      [FromQuery] string name
   ) {
      logger.LogDebug("ListOwnersByName {1}",name);

      // Find owner by SQL like %name%
      var owners = await ownersRepository.LikeNameByAsync(name);

      // if owners are found return them
      if (owners.Any()) return Ok(mapper.Map<IEnumerable<OwnerDto>>(owners));
      // else return not found
      return NotFound("Owners with given FirstName not found");
   }

   /// <summary>
   /// Create an owner. 
   /// </summary>
   /// <param name="ownerDto">OwnerDto to create</param>
   /// <returns>OwnerDto, the created resource</returns>
   /// <response code="201">Created: Owner is created</response>
   /// <response code="409">Conflict: Owner with given id already exists.</response>
   /// <response code="500">Server internal error.</response>
   [HttpPost("owners")]
   [Authorize(Roles = "webtech-user,webtech-admin")]
   [Consumes(MediaTypeNames.Application.Json)]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(StatusCodes.Status201Created)]
   [ProducesResponseType(StatusCodes.Status409Conflict)]
   public async Task<ActionResult<OwnerDto>> CreateOwner(
      [FromBody] OwnerDto ownerDto
   ) {
      logger.LogDebug("CreateOwner");
      
      // validate input
      var (error, actionResult) = await ValidateOwnerAsync(ownerDto);
      if (error) return actionResult;
      if((actionResult?.Result as OkObjectResult)?.Value is not OwnerDto value) 
         return BadRequest("Error in ValidateOwner");
      ownerDto = value;
      
      // map ownerDto to owner
      var owner = mapper.Map<Owner>(ownerDto);

      // save to ownersRepository and write to database 
      ownersRepository.Add(owner);
      await dataContext.SaveAllChangesAsync();
   
      // https://ochzhen.com/blog/created-createdataction-createdatroute-methods-explained-aspnet-core
      // Request == null in unit tests
      var path = Request == null 
         ? $"/banking/v2/owners/{owner.Id}" 
         : $"{Request.Path}/{owner.Id}";
      var uri = new Uri(path, UriKind.Relative);
      ownerDto = mapper.Map<OwnerDto>(owner);
      return Created(uri, ownerDto);

      // createdAtRoute 
      // var route = Request.RouteValues;
      // var createdResource = new { Id = owner.Id, Version = "1.0" };
      // var routeValues = new { id = createdResource.Id, version = createdResource.Version };
      // return CreatedAtRoute(nameof(GetOwnerById), routeValues, createdResource);
      
      // createdAtAction
      // return CreatedAtAction(
      //     actionName: nameof(GetOwnerById),
      //     routeValues: new { id = owner.Id },
      //     value: _mapper.Map<OwnerDto>(owner)
      //);
   }
   
   /// <summary>
   /// Updates an owner
   /// </summary>
   /// <param name="id">id of the owner</param>
   /// <param name="updOwnerDto">Parameter to update as OwnerDto</param>
   /// <returns>OwnerDto, the updated resource</returns>
   [HttpPut("owners/{id:guid}")] 
   [Authorize(Roles = "webtech-user,webtech-admin")]
   [Consumes(MediaTypeNames.Application.Json)]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesResponseType(StatusCodes.Status400BadRequest)]
   [ProducesResponseType(StatusCodes.Status404NotFound)]
   [ProducesDefaultResponseType]
   public async Task<ActionResult<OwnerDto>> UpdateOwner(
      [FromRoute] Guid id,
      [FromBody]  OwnerDto updOwnerDto
   ) {
      logger.LogDebug("UpdateOwner() id={id} updOwnerDto={updOwnerDto}", id.As8(), updOwnerDto.FirstName);
      
      var updOwner = mapper.Map<Owner>(updOwnerDto);

      // check if Id in the route and body match, else return bad request (400)
      if(id != updOwner.Id) 
         return BadRequest("UpdateOwner: Id in the route and body do not match.");
      
      // check if owner with given Id exists, else return not found (404)
      var owner = await ownersRepository.FindByIdAsync(id);
      if (owner == null)
         return NotFound("UpdateOwner: Owner with given id not found.");

      // Update person
      owner.Update(updOwner);
      
      // save to repository 
      await ownersRepository.UpdateAsync(owner);
      // write to database
      await dataContext.SaveAllChangesAsync();

      // return updated owner
      return Ok(mapper.Map<OwnerDto>(updOwner)); 
   }

   /// <summary>
   /// Delete an owner
   /// </summary>
   /// <param name="id">id of the the owner</param>
   [HttpDelete("owners/{id:guid}")]
   [Authorize(Roles = "webtech-admin")]
   [ProducesResponseType(StatusCodes.Status204NoContent)]
   [ProducesResponseType(StatusCodes.Status404NotFound)]
   public async Task<IActionResult> DeleteOwner(
      [FromRoute] Guid id
   ) {
      logger.LogDebug("DeleteOwner {id}", id.As8());

      // check if owner with given Id exists
      var owner = await ownersRepository.FindByIdJoinAsync(id, true);
      if (owner == null)
         return NotFound("DeleteOwner: Owner with given id not found.");

      // remove all transactions for all accounts of the owner
      // transactions are removed first, because there is not cascading delete
      foreach (var acc in owner.Accounts) {
         // get account with all references
         // i.e. join Owner, Beneficiaries, Transfers, Transactions
         var account = await accountsRepository.FindByIdJoinAsync(acc.Id, true, true, true, true);
         if(account == null) 
            return NotFound("DeleteOwner: Account with given id not found.");
         // get all transactions for account
         var transactions = 
            await transactionRepository.FilterByAccountIdAsync(account.Id, null);
         foreach (var transaction in transactions)
            transactionRepository.Remove(transaction);
         // write changes to database, i.e. remove transactions
         await dataContext.SaveAllChangesAsync();
      }

      // remove in repository 
      ownersRepository.Remove(owner);
      // write to database
      await dataContext.SaveAllChangesAsync();

      // return no content
      return NoContent(); 
   }
   
   #region local methods
   private async Task<(bool error, ActionResult<OwnerDto> result)> ValidateOwnerAsync(OwnerDto ownerDto) {

      // check if ownerDto.Id is empty
      if (ownerDto.Id == Guid.Empty)
         ownerDto = ownerDto with { Id = Guid.NewGuid() };

      // check if ownerDto.Id already exists
      if (await ownersRepository.FindByIdAsync(ownerDto.Id) != null)
         return (true, Conflict("CreateOwner: Owner with given id already exists."));
      
      // check if ownerDto.FirstName is too short or too long
      if (ownerDto.FirstName.Length < 3)
         return (true, BadRequest("FirstName is too short (min 3 chars)."));
      if (ownerDto.FirstName.Length > 128)
         return (true, BadRequest("FirstName is too long (max 128 chars)."));

      try {
         if (ownerDto.Email != null) {
            var mail = new System.Net.Mail.MailAddress(ownerDto.Email);
         }
      } 
      catch {
         return (true, BadRequest("Email is not valid."));
      }
      return (false, Ok(ownerDto));
   }

   #endregion
}