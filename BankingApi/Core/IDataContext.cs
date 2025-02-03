using System.Threading.Tasks;
namespace BankingApi.Core;

public interface IDataContext {
   Task<bool> SaveAllChangesAsync(); 
   void       ClearChangeTracker();
   void       LogChangeTracker(string text);
}