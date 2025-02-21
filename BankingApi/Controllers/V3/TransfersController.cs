using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Asp.Versioning;
using AutoMapper;
using BankingApi.Core;
using BankingApi.Core.DomainModel.Entities;
using BankingApi.Core.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
namespace BankingApi.Controllers.V2;
[Route("banking/v{version:apiVersion}")]
[ApiVersion("3.0")]
[ApiController]
public class TransfersController(
   IUseCasesTransfer useCasesTransfer,
   IAccountsRepository accountsRepository,
   ITransfersRepository transfersRepository,
   IMapper mapper,  
   ILogger<TransfersController> logger
) : ControllerBase {
   
   /// <summary>
   /// List transfers of an account with accountId 
   /// </summary>
   /// <returns>IEnumerable{TransferDto}; </returns>
   /// <response code="200">Ok. Tranfers returned</response>
   [HttpGet("accounts/{accountId:guid}/transfers")]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(StatusCodes.Status200OK)]
   public async Task<ActionResult<IEnumerable<TransferDto>>> GetTransfersByAccountId(
      [FromRoute] Guid accountId
   ){
      logger.LogDebug("Get by accountId {AccountId}", accountId);
      var account = await accountsRepository.FindByIdAsync(accountId);
      if(account == null)
         return BadRequest("Bad request: accountId does not exist.");

      var transfers =
         await transfersRepository.SelectByAccountIdAsync(accountId);
      
      return Ok(mapper.Map<IEnumerable<TransferDto>>(transfers));
   }

   /// <summary>
   /// Get the transfer by Id. 
   /// </summary>
   /// <param name="id">id of the account</param>
   /// <returns>TransferDto?</returns>
   /// <response code="200">Ok: Transfer with given id returned</response>
   /// <response code="404">NotFound: Transfer with given id not found</response>
   [HttpGet("transfers/{id:guid}")]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesResponseType(StatusCodes.Status404NotFound)]
   [ProducesDefaultResponseType]
   public async Task<ActionResult<TransferDto>> GetTransferById(
      [FromRoute] Guid id
   ){
      logger.LogDebug("Get {id}", id);
      return await transfersRepository.FindByIdAsync(id) switch {
         { } beneficiary => Ok(mapper.Map<TransferDto>(beneficiary)),
         null => NotFound("Transfer with given id not found.")
      };
   }

   /// <summary>
   /// Create a transfer and the two transactions (debit/credit)
   /// </summary>
   /// <param name="accountId">sender</param>
   /// <param name="transferDto"></param>
   /// <returns>TransferDto</returns>
   /// <response code="201">Created: Transfer is created</response>
   /// <response code="400">Bad request: accountId does not exits.</response>
   /// <response code="409">Conflict: Transfer with given id already exists.</response>
   [HttpPost("accounts/{accountId:guid}/transfers")]
   [Consumes(MediaTypeNames.Application.Json)]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(StatusCodes.Status201Created)]
   [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
   [ProducesDefaultResponseType]
   public async Task<ActionResult<TransferDto>> SendMoney(
      [FromRoute] Guid accountId,  // sender
      [FromBody] TransferDto transferDto
   ){
      logger.LogDebug("SendMoney");
      var transfer = mapper.Map<Transfer>(transferDto);
      
      // send money, i.e. create transfer and two transactions
      var resultData = await useCasesTransfer.SendMoneyAsync(accountId, transfer);
      
      // return errors
      if (resultData is Error<Transfer> && resultData.Status is not null) {
         return resultData.Status switch {
            400 => BadRequest(resultData.Message),
            404 => NotFound(resultData.Message),
            409 => Conflict(resultData.Message),
            _ => StatusCode(500, resultData.Message)
         };
      }
      
      var path = Request == null
         ? $"/banking/v2/transfers/{transferDto.Id}"
         : $"{Request.Path}";
      var uri = new Uri(path, UriKind.Relative);
      return Created(uri, transferDto);
   }

   /// <summary>
   /// Create a reverse transfer and the two transactions (debit/credit)
   /// </summary>
   /// <param name="originalTransferId">id of the original transfer</param>
   /// <param name="reverseTransferDto">parameter of the reverse transfer</param>
   /// <returns>TransferDto</returns>
   [HttpPost("accounts/{accountId:guid}/transfers/reverse/{originalTransferId:guid}")]
   [Consumes(MediaTypeNames.Application.Json)]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(StatusCodes.Status201Created)]
   [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
   [ProducesDefaultResponseType]
   public async Task<ActionResult<TransferDto>> ReverseMoney(
      [FromRoute] Guid accountId,
      [FromRoute] Guid originalTransferId,
      [FromBody] TransferDto reverseTransferDto
   ){
      logger.LogDebug("ReverseMoney");
      
      var originalTransfer = await transfersRepository.FindByIdAsync(originalTransferId);
      if (originalTransfer == null)
         return NotFound("Original transfer not found.");
      
      var reverseTransfer = new Transfer(
         id: Guid.NewGuid(),
         date: DateTime.UtcNow,
         description: "Reverse transfer",
         amount: -originalTransfer.Amount,
         beneficiary:  originalTransfer.Beneficiary,
         account: originalTransfer.Account
      );
      
      // reverse money, i.e. create reverse transfer and two transactions
      var resultData = await useCasesTransfer.ReverseMoneyAsync(originalTransferId, reverseTransfer);

      // return errors
      if (resultData is Error<Transfer> && resultData.Status is not null) {
         return resultData.Status switch {
            400 => BadRequest(resultData.Message),
            404 => NotFound(resultData.Message),
            409 => Conflict(resultData.Message),
            _ => StatusCode(500, resultData.Message)
         };
      }
      
      var path = Request == null
         ? $"/banking/v2/transfers/{reverseTransferDto.Id}"
         : $"{Request.Path}";
      var uri = new Uri(path, UriKind.Relative);
      return Created(uri, reverseTransferDto);
   }
}