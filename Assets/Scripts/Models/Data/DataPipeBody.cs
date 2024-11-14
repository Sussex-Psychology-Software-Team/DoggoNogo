using UnityEngine;

[System.Serializable]
public class DataPipeBody {
    public string experimentID;
    public string filename;
    public string data;

    public DataPipeBody(Data dataObject, string experimentID, string participantName) {
        this.experimentID = experimentID; //VSyXogVR8oTS
        string pName = "" + participantName == "QUERY VAR NOT FOUND" ? "" : participantName + "_";
        filename = "DoggoNogo_" + pName + Utility.GenerateRandomId(10) + ".json";
        data = JsonUtility.ToJson(dataObject);
    }
}
