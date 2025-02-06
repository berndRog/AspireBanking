using BankingClient.Core.Dto;
namespace BankingClient.Core;

public interface IBeneficiaryService
{
    Task<ResultData<IEnumerable<BeneficiaryDto>?>> GetByAccount(Guid accountId);
    Task<ResultData<BeneficiaryDto?>> Post(Guid accountId, BeneficiaryDto beneficiaryDto);
    Task<ResultData<BeneficiaryDto?>> GetById(Guid beneficiaryId);
}