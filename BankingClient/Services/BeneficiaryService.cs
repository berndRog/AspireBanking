using System.Text.Json;
using BankingClient.Core;
using BankingClient.Core.Dto;
using Microsoft.AspNetCore.Authorization;
namespace BankingClient.Services;

public class BeneficiaryService(
    WebServiceOptions<BeneficiaryService> options
): BaseWebService<BeneficiaryService>(options), IBeneficiaryService {
    
    // GET: All beneficiaries of an account with given accountid string /banking/v2/accounts/{accountId}/beneficiaries
    public async Task<ResultData<IEnumerable<BeneficiaryDto>?>> GetAllBeneficiariesByAccount(Guid accountId) =>
        await GetAllAsync<BeneficiaryDto>($"accounts/{accountId}/beneficiaries");
    
    // GET a beneficiary by Id 
    public async Task<ResultData<BeneficiaryDto?>> GetBeneficiaryById(Guid beneficiaryId) =>
        await GetAsync<BeneficiaryDto>($"beneficiaries/{beneficiaryId}");

    // POST: Insert a beneficiary
    public async Task<ResultData<BeneficiaryDto?>> PostBeneficiary(Guid accountId, BeneficiaryDto beneficiaryDto) =>
        await PostAsync<BeneficiaryDto>($"accounts/{accountId}/beneficiaries", beneficiaryDto);
    
}