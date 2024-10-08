using System;
[Serializable]

public class SingleGameHistory
{
    public int totalScore;
    public string totalTime;
    public DateTime timeStamp;
    public SingleGameHistory(int totalScore, string totalTime, DateTime timeStamp)
    {
        this.totalScore = totalScore;
        this.totalTime = totalTime;
        this.timeStamp = timeStamp;
    }
    public SingleGameHistory()
    {

    }
    public int GetTotalScore()
    {
        return this.totalScore;
    }

    public void SetTotalScore(int totalScore)
    {
        this.totalScore = totalScore;
    }
    public string GetTotalTime()
    {
        return this.totalTime;
    }
    public void SetTotalTime(string totalTime)
    {
        this.totalTime = totalTime;
    }
    public DateTime GetTimeStamp()
    {
        return this.timeStamp;
    }
    public void SetTimeStamp(DateTime timeStamp)
    {
        this.timeStamp = timeStamp;
    }
}
