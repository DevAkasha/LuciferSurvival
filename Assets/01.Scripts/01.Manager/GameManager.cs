using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public RxVar<int> essenceOfRuin;

    public int EssenceOfRuin
    {
        get => essenceOfRuin.Value;
        set => essenceOfRuin.SetValue(value, this);
    }

    public void AddEssence(int essence)
    {
        EssenceOfRuin += essence;
    }

    public bool ReduceEssence(int essence)
    {
        if (EssenceOfRuin - essence < 0) return false;
            EssenceOfRuin -= essence;
        return true;
    }

    public async void PauseGame(float delayTime)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delayTime), DelayType.DeltaTime, PlayerLoopTiming.Update);
        Time.timeScale = 0f;
        Debug.Log("일시 정지");
    }

    public void PauseReleaseGame()
    {
        Time.timeScale = 1f;
        Debug.Log("일시 정지 해제");
    }
}

