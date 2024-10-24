using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
public class GameInfo
{
    public string ProfileName = null; //Game1.save
    public int currentLevel = 1;
    public int currentStage = 1;
    public string Level="DefaultLevel";
    public PlayerInfo PlayerInfo = new PlayerInfo();
    public PlayerInfo OpponentInfo = new PlayerInfo();

    public bool HasSaveFile()
    {
        return SaveManager.Instance.Load<GameInfo>(ProfileName) != null;
    }

    /// <summary>
    /// This method resets values when a new game is started on the profile
    /// </summary>
    internal void Reset()
    {
        this.currentLevel = 1;
        this.currentStage = 1;
        this.PlayerInfo = new PlayerInfo();
        this.OpponentInfo = new PlayerInfo();
        this.BoardData = LevelGenerator.Instance.GenerateDefaultBoardData();
        this.GameState = GameState.RUNNING;
    }

    List<Consumable> UnlockedConsumables = new List<Consumable>();

    public BoardData BoardData;
    public PieceColor? Winner;

    public GameState GameState = GameState.RUNNING;
}