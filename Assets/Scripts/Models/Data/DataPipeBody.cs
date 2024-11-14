using UnityEditor.Playables;
using UnityEngine;

[System.Serializable]
public class DataPipeBody
{
    public string experimentID;
    public string filename;
    public string data;

    public DataPipeBody(GameData dataObject, string experimentID, string participantName)
    {
        this.experimentID = experimentID;
        string pName = string.IsNullOrEmpty(participantName) || participantName == "QUERY VAR NOT FOUND" 
            ? "" 
            : participantName + "_";
        filename = $"DoggoNogo_{pName}{Utility.GenerateRandomId(10)}.json";
        data = JsonUtility.ToJson(dataObject);
    }
}