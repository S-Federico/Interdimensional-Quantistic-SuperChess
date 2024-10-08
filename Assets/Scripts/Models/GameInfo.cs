using System.Collections.Generic;
using System.Collections;
using UnityEngine;
public class GameInfo
{
    public string ProfileName = null; //Game1.save
    public int currentLevel = 1;
    public int currentStage = 1;

    public PlayerInfo PlayerInfo = new PlayerInfo();

    public bool HasSaveFile()
    {
        return SaveManager.Instance.Load<GameInfo>(ProfileName) != null;
    }
    List<Consumable> UnlockedConsumables = new List<Consumable>();

    public BoardData BoardData;
}