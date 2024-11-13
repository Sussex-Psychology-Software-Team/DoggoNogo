// using System;
// using System.Collections.Generic;
//
// namespace Data
// {
//     public interface IGameModel
//     {
//         // Trial Management
//         void ClearTrials();
//         int GetTrialCount();
//         void AddNewTrial(double isi);
//         
//         // Time Management
//         void SetLevelStartTime(DateTime time);
//         void SetLevelEndTime(DateTime time);
//         string GetLevelStartTime();
//         string GetLevelEndTime();
//         
//         // Trial Data Operations
//         void SaveCurrentStimulusData(Dictionary<string, float> stimSpec);
//         void SaveTrialResults(double rt, string type, int score, int total, 
//             double threshold, bool validTrial, int validTrialCount);
//         
//         // State Queries
//         double GetCurrentThreshold();
//         TrialData GetCurrentTrial();
//         
//         // Data Access
//         GameData GetGameData();
//         GameMetadata GetMetadata();
//         
//         // Metadata Operations
//         void InitializeMetadata();
//         void SetExperimentId(string id);
//         void SetParticipantName(string name);
//         
//         // Query Parameters
//         int GetTrialsFromQuery();
//         
//         // Optional: Event notifications
//         event System.Action<TrialData> OnTrialCompleted;
//         event System.Action<GameMetadata> OnMetadataChanged;
//     }
// }