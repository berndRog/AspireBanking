using BankingClient.Core.Dto;
namespace BankingClient.Core;

public interface IOwnerService {
    Task<ResultData<IEnumerable<OwnerDto>?>> GetAll();
    Task<ResultData<OwnerDto?>> GetById(Guid ownerId);
    Task<ResultData<OwnerDto?>> GetByUserName(string userName);
    Task<ResultData<IEnumerable<OwnerDto>?>> GetByName(string name);
    Task<ResultData<OwnerDto?>> GetOwnerByUserId(string userId);
    Task<ResultData<OwnerDto?>> Post(OwnerDto ownerDto);
    Task<ResultData<OwnerDto?>> Put(OwnerDto ownerDto);
    Task<ResultData<object?>>   Delete(Guid ownerId);
    
    Task<bool> ExistsByUserName(string userName);
}