using System.Text.Json;
using BankingClient.Dto;
using BankingClient.ErrorHandling;
using BankingClient.Services.Impl;
using Microsoft.AspNetCore.Components;
namespace BankingClient.Services;

public class OwnerService(
   HttpClient httpClient,
   IConfiguration configuration,
   ILogger<OwnerService> logger,
   ResponseErrors responseErrors,
   JsonSerializerOptions jsonOptions
) : BaseWebService<OwnerService>(httpClient, configuration, logger, responseErrors, jsonOptions),
   IOwnerService {
   
   // Get Owner /banking/v2/owners/{ownerId}: Find the owner with the given ID
   public async Task<OwnerDto?> GetOwnerById(Guid ownerId) =>
      await GetAsync<OwnerDto>($"/owners/{ownerId}");

   // Post Owner /banking/v2/owners: Create an owner
   public async Task<OwnerDto?> PostOwner(OwnerDto ownerDto) =>
      await PostAsync<OwnerDto>("owners", ownerDto);
}