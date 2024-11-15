using System.Threading.Tasks;

// Interface for data operations - unsure
public interface IDataService
{
    Task<bool> SaveData(GameData data);
}