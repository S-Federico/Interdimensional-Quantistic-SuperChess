using System.Collections.Generic;
using System.Collections;
using UnityEngine;
public class GameInfo
{
    public string ProfileName; //Game1.save
    public int currentLevel = 1;
    public int currentStage = 1;

    public PlayerInfo PlayerInfo;

    public bool HasSaveFile()
    {
        return true;
    }
}