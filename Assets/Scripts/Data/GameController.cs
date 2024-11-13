// using UnityEngine;
// using System;
// using System.Collections.Generic;
// using Debug = UnityEngine.Debug;
//
// // Singleton Controller for Unity Scene Management
// public class GameController : MonoBehaviour 
// {
//     public static GameController Instance { get; private set; }
//
//     [SerializeField] private bool dontDestroyOnLoad = true;
//     
//     private IGameModel _gameModel;
//     private DataService _dataService;
//
//     private void Awake() 
//     {
//         // Unity singleton pattern
//         if (Instance != null && Instance != this) 
//         {
//             Destroy(gameObject);
//             return;
//         }
//         
//         Instance = this;
//         if (dontDestroyOnLoad)
//         {
//             DontDestroyOnLoad(gameObject);
//         }
//         
//         Initialize();
//     }
//     
//     private void Initialize()
//     {
//         _gameModel = new GameModel();
//         _dataService = gameObject.AddComponent<DataService>(); // DataService needs MonoBehaviour for Coroutines
//     }
//     
//     public int GetTrialCount()
//     {
//         return _gameModel.GetTrialCount();
//     }
//     
//     public void OnLevel1Start() 
//     {
//         _gameModel.ClearTrials();
//         _gameModel.SetLevelStartTime(DateTime.Now);
//     }
//     
//     public void OnStimuliShown(Dictionary<string, float> stimSpec) 
//     {
//         if(_gameModel == null) 
//         {
//             Debug.LogError("GameModel not initialized!");
//             return;
//         }
//         _gameModel.SaveCurrentStimulusData(stimSpec);
//     }
//     
//     public void OnTrialComplete(double rt, string type, int score, int total, double threshold, bool validTrial, int validTrialCount) 
//     {
//         _gameModel.SaveTrialResults(rt, type, score, total, threshold, validTrial, validTrialCount);
//         Debug.Log($"Trial completed: Score {score}/{total}");
//     }
//     
//     public void OnLevelComplete()
//     {
//         _gameModel.SetLevelEndTime(DateTime.Now);
//         _dataService.SendData(_gameModel.GetGameData());
//     }
//
//     public double GetCurrentThreshold()
//     {
//         return _gameModel.GetCurrentThreshold();
//     }
// }