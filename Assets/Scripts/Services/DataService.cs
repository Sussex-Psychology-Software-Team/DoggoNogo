public class DataService : IDataService {
    private readonly string apiUrl = "https://pipe.jspsych.org/api/data/";
    private readonly WebService webService;

    public DataService(WebService webService) {
        this.webService = webService;
    }

    public async Task<bool> SaveData(GameData data) {
        try {
            var dataPipeBody = new DataPipeBody(data, 
                data.metadata.experimentID, 
                data.metadata.participantName);
            
            string json = JsonUtility.ToJson(dataPipeBody);
            return await webService.PostData(apiUrl, json);
        }
        catch (Exception ex) {
            Debug.LogError($"Failed to save data: {ex.Message}");
            return false;
        }
    }
}