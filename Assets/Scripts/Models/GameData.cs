using System.Collections.Generic;

// Main data container for the game
[System.Serializable]
public class GameData {
    public Metadata metadata = new();
    public List<Trial> level1 = new();
    public GameStats gameStats = new();
    
    // Accessors for the entirety of the level1 array
    public int TrialNumber => level1.Count;
    
    public void AddNewTrial(double isi) {
        level1.Add(new Trial(TrialNumber + 1, isi));
    }

    public Trial CurrentTrial() => level1.Count > 0 ? level1[^1] : null;
    
    public void ClearTrials()
    {
        level1.Clear();
    }
}