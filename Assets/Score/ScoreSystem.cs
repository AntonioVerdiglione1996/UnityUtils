using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "ScoreSystem", menuName = "ScoreSystem")]
public class ScoreSystem : ScriptableObject
{
    [NonSerialized]
    public int Score;

    public int BestScore;
    public void OnGameOverActive()
    {
        CheckBestScore();
        //serialization call
        SerializerHandler.Save(SerializerHandler.DefaultDirectory, "Score.json", JsonUtility.ToJson(this));
    }
    public void UpdateScore()
    {
        Score++;
    }
    public void ReleaseDataForScore()
    {
        Score = 0;
    }
    public void CheckBestScore()
    {
        int previusBestScore = BestScore;
        BestScore = Score > previusBestScore ? Score : previusBestScore;
    }
}
