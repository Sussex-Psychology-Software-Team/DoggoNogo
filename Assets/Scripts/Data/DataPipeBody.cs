using UnityEngine;

[System.Serializable]
public class DataPipeBody {
    public string experimentID;
    public string filename;
    public string data;

    public DataPipeBody(Data dataObject, string experimentID) {
        this.experimentID = experimentID; //VSyXogVR8oTS
        filename = Utility.GenerateRandomId(10) + ".json";
        data = JsonUtility.ToJson(dataObject);
    }

    public string DataPipeJSON(){
        return JsonUtility.ToJson(this);
    }
}
