using Monoliths;
using System;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;

public class LevelProgressObserver : Monolith
{
    private const string LevelInfoKey = "currentLevelResult";

    private GameObject[] _stars;
    private GameObject[] _levelTargets;

    public static event Action OnLevelShouldFinalize;

    public override bool Init()
    {
        base.Init();

        _stars = GameObject.FindGameObjectsWithTag("Star");
        _levelTargets = GameObject.FindGameObjectsWithTag("LevelTargets");

        foreach (var star in _stars)
        {
            var starComponent = star.GetComponent<Collectable>(); 
            if (starComponent == null)
            {
                starComponent = star.AddComponent<Collectable>();
            }
            starComponent.OnCollison += HandleStarAcquired;
        }
                
        foreach (var target in _levelTargets)
        {
            var targetComponent = target.GetComponent<Collectable>(); 
            if (targetComponent == null)
            {
                targetComponent = target.AddComponent<Collectable>();
            }
                targetComponent.OnCollison += HandleTargetAcquired;
        }

        return true;
    }

    private void OnEnable()
    {
        OnLevelShouldFinalize += FinalizeLevel;
    }
    private void OnDisable()
    {
        OnLevelShouldFinalize -= FinalizeLevel;
    }
    private void FinalizeLevel()
    {
        if (!(GameStateMachine.Instance.Current is LevelState levelState)) return;

        levelState.FinalizeLevel();
    }

    ////////////////   ////////////////   ////////////////   ////////////////   ////////////////   ////////////////   ////////////////   ////////////////   ////////////////   ////////////////
    ////////////////   ////////////////   ////////////////   ////////////////   ////////////////   ////////////////   ////////////////   ////////////////   ////////////////   ////////////////
    //                                                          Kiril senpai pls check the abomination that I came up with ... uwu
    ////////////////   ////////////////   ////////////////   ////////////////   ////////////////   ////////////////   ////////////////   ////////////////   ////////////////   ////////////////
    ////////////////   ////////////////   ////////////////   ////////////////   ////////////////   ////////////////   ////////////////   ////////////////   ////////////////   ////////////////

    private void HandleStarAcquired(GameObject star) 
    {
        Data<LevelResult> data = DataBridge.TryGetData<LevelResult>(LevelInfoKey);
        if (!data.IsEmpty) 
        {
            LevelResult levelInfo = data.EncodedData;
            levelInfo.CollectedStars++;
            DataBridge.UpdateData(LevelInfoKey, levelInfo);
        }
    }
    private void HandleTargetAcquired(GameObject levelTarget)  => CheckCompletedLevel();

    private void InitializeLevelInfo() //prob problem here!!
    {
        Data <LevelResult> data = DataBridge.TryGetData<LevelResult>(LevelInfoKey);
        if (!data.IsEmpty) 
        {
            LevelResult levelResult = new LevelResult(0, 1, false);
            DataBridge.UpdateData(LevelInfoKey, levelResult);
        }
    }

    private void CheckCompletedLevel()
    {
        Data<LevelResult> data = DataBridge.TryGetData<LevelResult>(LevelInfoKey);
        if (!data.IsEmpty)
        {
            LevelResult levelInfo = data.EncodedData;
            levelInfo.IsCompleted = true; 
            DataBridge.UpdateData(LevelInfoKey, levelInfo); 
        }

        SaveLevelInfoToFile();
    }

    private void SaveLevelInfoToFile()
    {
        var levelInfo = DataBridge.TryGetData<LevelResult>(LevelInfoKey).EncodedData;
        XmlSerializer serializer = new XmlSerializer(typeof(LevelResult));
        using (TextWriter writer = new StreamWriter(@"LevelResult.xml"))
        {
            serializer.Serialize(writer, levelInfo);   
        }
    }
}

public struct LevelResult
{
    public int CollectedStars;
    public int LevelID;
    public bool IsCompleted;

    public LevelResult(int number, int id, bool completed)
    {
        CollectedStars = number;
        LevelID = id;
        IsCompleted = completed;
    }
}


