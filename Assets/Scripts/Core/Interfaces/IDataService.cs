using System.Threading.Tasks;

public interface IDataService
{
    Task<bool> SaveData(GameData data);
}