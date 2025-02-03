using BankingClient.Dto;
namespace BankingClient.Services;

public interface IOwnerService
{
    Task<OwnerDto?> GetOwnerById(Guid ownerId);
    Task<OwnerDto?> PostOwner(OwnerDto ownerDto);
}