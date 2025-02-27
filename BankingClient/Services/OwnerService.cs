using System.Text.Json;
using BankingClient.Core;
using BankingClient.Core.Dto;
using Microsoft.AspNetCore.Authorization;
namespace BankingClient.Services;

public class OwnerService(
   WebServiceOptions<OwnerService> options
): BaseWebService<OwnerService>(options), IOwnerService {

   // Get all owners 
   [Authorize(Policy = "AdminPolicy")]
   public async Task<ResultData<IEnumerable<OwnerDto>?>> GetAll() =>
      await GetAllAsync<OwnerDto>($"owners");
   
   // Get owner by Id
   [Authorize(Policy = "CombinedPolicy")]
   public async Task<ResultData<OwnerDto?>> GetById(Guid ownerId) =>
      await GetAsync<OwnerDto>($"owners/{ownerId}");

   // Get owner by userId
   [Authorize(Policy = "CombinedPolicy")]
   public async Task<ResultData<OwnerDto?>> GetByUserId(string userId) =>
      await GetAsync<OwnerDto>($"owners/userid/?userid={userId}");
   
   // Get owner by username
   [Authorize(Policy = "CombinedPolicy")]
   public async Task<ResultData<OwnerDto?>> GetByUserName(string userName) =>
      await GetAsync<OwnerDto>($"owners/username/?username={userName}");
   
   // Get owner by name
   public async Task<ResultData<IEnumerable<OwnerDto>?>> GetByName(string name) =>
      await GetAsync<IEnumerable<OwnerDto>>($"owners/name/?name={name}");
   
   // Post (create) an owner
   [Authorize(Policy = "CombinedPolicy")]
   public async Task<ResultData<OwnerDto?>> Post(OwnerDto ownerDto) =>
      await PostAsync("owners", ownerDto);
   
   // Put (update) an owner
   [Authorize(Policy = "CombinedPolicy")]
   public async Task<ResultData<OwnerDto?>> Put(OwnerDto ownerDto) =>
      await PutAsync($"owners/{ownerDto.Id}", ownerDto);

   // Delete an owner
   [Authorize(Policy = "AdminPolicy")]
   public async Task<ResultData<object?>> Delete(Guid ownerId) =>
      await DeleteAsync($"owners/{ownerId}");

   // Exists owner by username
   [Authorize(Policy = "CombinedPolicy")]
   public async Task<bool> ExistsByUserName(string userName) {
      try {
         var response = await _httpClient.GetAsync($"owners/exists/?username={userName}", _cancellationToken);
         response.EnsureSuccessStatusCode();
         var content = await response.Content.ReadAsStringAsync();
         var result = JsonSerializer.Deserialize<bool>(content, _jsonOptions);
         return result;
         
      } catch (Exception ex) {
         _logger.LogError(ex, "Error checking if owner exists by username");
         return false;
      }
   }
}