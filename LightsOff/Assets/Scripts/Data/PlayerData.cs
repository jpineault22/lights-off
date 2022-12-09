[System.Serializable]
public class PlayerData
{
    public int CurrentLevelNumber { get; set; }

    public PlayerData(int pCurrentLevelNumber)
    {
        CurrentLevelNumber = pCurrentLevelNumber;
    }
}
