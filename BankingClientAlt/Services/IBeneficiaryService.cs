using BankingClient.Dto;
namespace BankingClient.Services;

public interface IBeneficiaryService
{
    Task<IEnumerable<BeneficiaryDto>?> GetAllBeneficiariesByAccount(Guid accountId);
    Task<BeneficiaryDto?> PostBeneficiary(Guid accountId, BeneficiaryDto beneficiaryDto);
    Task<BeneficiaryDto?> GetBeneficiaryById(Guid beneficiaryId);
}