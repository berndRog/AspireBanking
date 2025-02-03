using BankingClient.Core.Dto;
namespace BankingClient.Core;

public interface IBeneficiaryService
{
    Task<ResultData<IEnumerable<BeneficiaryDto>?>> GetAllBeneficiariesByAccount(Guid accountId);
    Task<ResultData<BeneficiaryDto?>> PostBeneficiary(Guid accountId, BeneficiaryDto beneficiaryDto);
    Task<ResultData<BeneficiaryDto?>> GetBeneficiaryById(Guid beneficiaryId);
}