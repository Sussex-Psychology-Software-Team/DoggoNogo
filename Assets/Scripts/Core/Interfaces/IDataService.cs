public interface IDataService {
    Task<bool> SaveData(GameData data);
    Task<GameData> LoadData();
    void ClearData();
}d