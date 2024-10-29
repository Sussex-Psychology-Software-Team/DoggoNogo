using System.Collections.Generic;

[System.Serializable]
public class Data {
    public Metadata metadata;
    public List<Trial> level1;

    public Data() {
        metadata = new Metadata();
        level1 = new List<Trial>();
    }

    public void NewTrial(double isi) {
        level1.Add(new Trial(level1.Count + 1, isi));
    }

    public Trial CurrentTrial() {
        return level1[level1.Count - 1];
    }

    public void ClearTrials() {
        level1.Clear();
    }
}
