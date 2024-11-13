[System.Serializable]
public class GameData {
    public Metadata metadata;
    public List<Trial> level1;
    public GameStats gameStats;

    public GameData() {
        metadata = new Metadata();
        level1 = new List<Trial>();
        gameStats = new GameStats();
    }

    public void AddNewTrial(double isi) {
        level1.Add(new Trial(level1.Count + 1, isi));
        GameEvents.TrialCompleted(new TrialResult { TrialNumber = level1.Count });
    }

    public Trial CurrentTrial() => level1.Count > 0 ? level1[^1] : null;
}