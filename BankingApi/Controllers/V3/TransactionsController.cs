using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Asp.Versioning;
using AutoMapper;
using BankingApi.Core;
using BankingApi.Core.Dto;
using BankingApi.Core.Misc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
namespace BankingApi.Controllers.V3;
[Route("banking/v{version:apiVersion}")]
[ApiVersion("3.0")]
[ApiController]
public class TransactionsController(
   IAccountsRepository accountsRepository,
   ITransactionsRepository transactionsRepository,
   IMapper mapper,
   ILogger<TransactionsController> logger 
): ControllerBase {
   
   /// <summary>
   /// List transactions of an account by accountId and time intervall start to end.
   /// </summary>
   /// <param name="accountId">AccountId</param>
   /// <param name="start">IsoTimeStamp (string)</param>
   /// <param name="end">IsoTimeStamp (string)</param>
   /// <returns>IList{TransactionDto}; </returns>
   /// <response code="200">Ok. Transactions returned</response>
   /// <response code="400">Bad request: accountId doesn't exists.</response>*/
   [HttpGet("accounts/{accountId:guid}/transactions/filter")]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesResponseType(StatusCodes.Status400BadRequest)]
   public async Task<ActionResult<IEnumerable<TransactionDto>>> GetTransactionsByAccountId(
      [FromRoute] Guid accountId,
      [FromQuery] string start,
      [FromQuery] string end
   ){
      logger.LogDebug("GetTransactionsByAccountId {1} {2} {3}", accountId, start, end);

      var account = await accountsRepository.FindByIdAsync(accountId);
      if(account == null)
         return BadRequest("Bad request: accountId does not exist.");

      try {
         var dateTimeStart =
            DateTime.ParseExact(start, "o", null, System.Globalization.DateTimeStyles.RoundtripKind);
         var dateTimeEnd = 
            DateTime.ParseExact(end, "o", null, System.Globalization.DateTimeStyles.RoundtripKind);
         var transactions =
            await transactionsRepository.FilterByAccountIdAsync(
               accountId,
               t => t.Date >= dateTimeStart && t.Date <= dateTimeEnd 
            );
         return Ok(mapper.Map<IEnumerable<TransactionDto>>(transactions));
      }
      catch {
         return BadRequest($"Transaction: Fehler Zeitstempel start:{start} end:{end}");
      }
   }

   // /// <summary>
   // /// Get the transactions by transferid.
   // /// </summary>
   // /// <returns>IList{TransactionDto}; </returns>
   // /// <response code="200">Ok. Transactions returned</response>
   // /// <response code="400">Bad request: transferId doesn't exists.</response>
   // [HttpGet("transactions/transfer/{transferId:guid}")]
   // [Produces(MediaTypeNames.Application.Json)]
   // [ProducesResponseType(StatusCodes.Status200OK)]
   // [ProducesResponseType(StatusCodes.Status400BadRequest)]
   // [ProducesDefaultResponseType]
   // public async Task<ActionResult<IEnumerable<TransactionDto>>> Get(
   //    [FromRoute] Guid transferId
   // ){
   //    logger.LogDebug("Get");
   //
   //    var transfer = await transfersRepository.FindByIdAsync(transferId);
   //    if(transfer == null)
   //       return BadRequest("Bad request: transferId does not exist.");
   //
   //    var transactions = await transactionsRepository.SelectByTransferIdAsync(transferId);
   //    return Ok(mapper.Map<IEnumerable<TransactionDto>>(transactions));
   // }

   /// <summary>
   /// Get the transaction by Id 
   /// </summary>
   /// <param name="id">id:Guid of the transaction</param>
   /// <returns>TransactionDto?</returns>
   /// <response code="200">Ok: Transaction with given id returned.</response>
   /// <response code="404">NotFound: Transaction with given id not found.</response>
   [HttpGet("transactions/{id:guid}")]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesResponseType(StatusCodes.Status404NotFound)]

   public async Task<ActionResult<TransactionDto>> GetById(
      [FromRoute] Guid id
   ){
      logger.LogDebug("Get {id}", id);
      var transaction = await transactionsRepository.FindByIdAsync(id);
      if(transaction == null) return NotFound("Transaction with given id not found.");
      return Ok(mapper.Map<TransactionDto>(transaction));
   }
}