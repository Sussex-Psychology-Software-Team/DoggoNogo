using UnityEngine;

[System.Serializable]
public class DataPipeBody {
    public string experimentID;
    public string filename;
    public string data;

    public DataPipeBody(Data data) {
        experimentID = "VSyXogVR8oTS";
        filename = data.metadata.name + "_" + data.metadata.id + ".json";
        this.data = JsonUtility.ToJson(data);
    }
}
