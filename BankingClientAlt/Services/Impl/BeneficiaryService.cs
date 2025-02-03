using System.Text.Json;
using BankingClient.Dto;
using BankingClient.ErrorHandling;
using BankingClient.Services.Impl;
using Microsoft.AspNetCore.Components;
namespace BankingClient.Services;

public class BeneficiaryService(
    HttpClient httpClient,
    IConfiguration configuration,
    ILogger<BeneficiaryService> logger,
    ResponseErrors responseErrors,
    JsonSerializerOptions jsonOptions
): BaseWebService<BeneficiaryService>(httpClient, configuration, logger, responseErrors, jsonOptions), 
    IBeneficiaryService {
    
    // GET: All beneficiaries of an account with given accountid string /banking/v2/accounts/{accountId}/beneficiaries
    public async Task<IEnumerable<BeneficiaryDto>?> GetAllBeneficiariesByAccount(Guid accountId) =>
        await GetAllAsync<BeneficiaryDto>($"accounts/{accountId}/beneficiaries");
    
    // GET: Beneficiary with given ID /banking/v2/beneficiaries/{id}
    public async Task<BeneficiaryDto?> GetBeneficiaryById(Guid beneficiaryId) =>
        await GetAsync<BeneficiaryDto>($"beneficiaries/{beneficiaryId}");

    // POST: Insert a beneficiary.  /banking/v2/accounts/{accountId}/beneficiaries
    public async Task<BeneficiaryDto?> PostBeneficiary(Guid accountId, BeneficiaryDto beneficiaryDto) =>
        await PostAsync<BeneficiaryDto>($"accounts/{accountId}/beneficiaries", beneficiaryDto);
    
}