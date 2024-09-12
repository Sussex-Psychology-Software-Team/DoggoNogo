using System.Collections.Generic;

[System.Serializable]
public class Data {
    public Metadata metadata;
    public List<Trial> trials;

    public Data() {
        metadata = new Metadata();
        trials = new List<Trial>();
    }

    public void newTrial(double isi) {
        trials.Add(new Trial(trials.Count + 1, isi));
    }

    public Trial currentTrial() {
        return trials[trials.Count - 1];
    }

    public Trial lastTrial() {
        return trials[trials.Count - 2];
    }

    public void ClearTrials() {
        trials.Clear();
    }
}
