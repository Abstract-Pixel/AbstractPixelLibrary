using AbstractPixel.Utility;
using AbstractPixel.Utility.Save;
using System;
using UnityEngine;


[Serializable]
public class LeaderboardData
{
    public int bestTime;
    public int leaderboardCount;
    public int leaderboardIndex;

    public LeaderboardData(int bestTime, int leaderboardCount, int leaderboardIndex)
    {
        this.bestTime = bestTime;
        this.leaderboardCount = leaderboardCount;
        this.leaderboardIndex = leaderboardIndex;
    }
}
[Saveable(SaveCategory.Game)]
public class FakeoBest : MonoBehaviour,ISaveable<LeaderboardData>
{
    [SerializeField] LeaderboardData example;

    public LeaderboardData CaptureData()
    {
        return example;
    }

    public void RestoreData(LeaderboardData _loadedData)
    {
        example = _loadedData;
    }
}


