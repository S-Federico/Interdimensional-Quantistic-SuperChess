using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using Unity.VisualScripting;
using System;
using System.Linq;

public class GameManager : Singleton<GameManager>
{
    public static int MAX_LEVEL = 3;

    public List<ScriptableLevel> levels = null;

    public GameInfo GameInfo;
    private bool isGameOver = false;

    public bool IsGameOver { get => isGameOver; set => isGameOver = value; }
    public bool IsPaused = false;

    public Options Options;

    void Update() {
        
        // Disable updates when game is paused
        Time.timeScale = IsPaused ? 0f : 1f;

        // Stop InputManager if game is paused
        InputManager inputManager = FindAnyObjectByType<InputManager>();
        if (inputManager != null) {
            inputManager.SendMessage(IsPaused ? "OnDisable" : "OnEnable");
        }
    }

    void Awake()
    {
        Debug.Log("Game Manager instantiated!");
        levels = new List<ScriptableLevel>(Resources.LoadAll<ScriptableLevel>("ScriptableObjects/Levels"));

        // Try to load options. If fail, set default
        try
        {
            Options = SaveManager.Instance.LoadOptions() ?? new Options();
        }
        catch (Exception)
        {
            Options = new Options();
        }
    }

    public IEnumerator NewGameCoroutine(string sceneName)
    {
        TransitionLoader.Instance.transition.SetTrigger("Start");
        yield return new WaitForSeconds(TransitionLoader.Instance.GetTransitionTime());

        LoadingScreenManager.Instance.m_LoadingScreenObjct.SetActive(true);
        LoadingScreenManager.Instance.ProgressBar.value = 0;

        // Carica la scena in modo asincrono
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Aspetta fino a quando la scena non è completamente caricata
        while (!asyncLoad.isDone)
        {
            LoadingScreenManager.Instance.ProgressBar.value = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            yield return null;
        }

        var boardManager = GameObject.FindAnyObjectByType<BoardManager>();
        boardManager.LoadBoardFromBoardData();
        boardManager.InitializePiecesPlanes(true);

        // Imposta il progresso al 100% quando il caricamento è completo.
        LoadingScreenManager.Instance.ProgressBar.value = 1f;

        yield return new WaitForSeconds(0.2f);
        LoadingScreenManager.Instance.m_LoadingScreenObjct.SetActive(false);
        TransitionLoader.Instance.transition.SetTrigger("End");
        yield return new WaitForSeconds(TransitionLoader.Instance.GetTransitionTime());
    }

    public IEnumerator PlayTutorialCoroutine(string sceneName)
    {
        TransitionLoader.Instance.transition.SetTrigger("Start");
        yield return new WaitForSeconds(TransitionLoader.Instance.GetTransitionTime());

        LoadingScreenManager.Instance.m_LoadingScreenObjct.SetActive(true);
        LoadingScreenManager.Instance.ProgressBar.value = 0;

        // Carica la scena in modo asincrono
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Aspetta fino a quando la scena non è completamente caricata
        while (!asyncLoad.isDone)
        {
            LoadingScreenManager.Instance.ProgressBar.value = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            yield return null;
        }

        // START change
        var boardManager = GameObject.FindAnyObjectByType<BoardManager>();
        boardManager.LoadBoardFromBoardData();
        boardManager.InitializePiecesPlanes(true);
        // END change

        // Imposta il progresso al 100% quando il caricamento è completo.
        LoadingScreenManager.Instance.ProgressBar.value = 1f;

        yield return new WaitForSeconds(0.2f);
        LoadingScreenManager.Instance.m_LoadingScreenObjct.SetActive(false);
        TransitionLoader.Instance.transition.SetTrigger("End");
        yield return new WaitForSeconds(TransitionLoader.Instance.GetTransitionTime());
    }

    private IEnumerator LoadSceneAndContinue(string sceneName)
    {
        TransitionLoader.Instance.transition.SetTrigger("Start");
        yield return new WaitForSeconds(TransitionLoader.Instance.GetTransitionTime());

        LoadingScreenManager.Instance.m_LoadingScreenObjct.SetActive(true);
        LoadingScreenManager.Instance.ProgressBar.value = 0;

        // Carica la scena in modo asincrono
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Aspetta fino a quando la scena non è completamente caricata
        while (!asyncLoad.isDone)
        {
            LoadingScreenManager.Instance.ProgressBar.value = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            yield return null;
        }

        // La scena è caricata completamente, ora puoi chiamare il metodo per caricare i dati di gioco
        LoadGameFromFile();
        // Imposta il progresso al 100% quando il caricamento è completo.
        LoadingScreenManager.Instance.ProgressBar.value = 1f;

        yield return new WaitForSeconds(0.2f);
        LoadingScreenManager.Instance.m_LoadingScreenObjct.SetActive(false);
        TransitionLoader.Instance.transition.SetTrigger("End");
        yield return new WaitForSeconds(TransitionLoader.Instance.GetTransitionTime());
    }

    public IEnumerator Load(string sceneName)
    {
        TransitionLoader.Instance.transition.SetTrigger("Start");
        yield return new WaitForSeconds(TransitionLoader.Instance.GetTransitionTime());

        // Carica la scena in modo asincrono
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Aspetta fino a quando la scena non è completamente caricata
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        TransitionLoader.Instance.transition.SetTrigger("End");
        yield return new WaitForSeconds(TransitionLoader.Instance.GetTransitionTime());
    }

    public void NewGame(GameInfo gameInfo)
    {
        this.GameInfo = gameInfo;

        StartCoroutine(NewGameCoroutine(Constants.Scenes.SAMPLE_SCENE));
    }

    public void ContinueGame(GameInfo gameInfo)
    {
        this.GameInfo = gameInfo;
        StartCoroutine(LoadSceneAndContinue(Constants.Scenes.SAMPLE_SCENE));
    }

    public void RestartGame()
    {
        // When game restarts, load the main menu scene
        StartCoroutine(Load(Constants.Scenes.MENU));
        isGameOver = false;
    }

    public void LoadScene(string scene){
        StartCoroutine(Load(scene));
    }

    public void SaveGameToFile(GameInfo gameInfo)
    {
        SaveManager.Instance.Save(gameInfo, gameInfo.ProfileName);
    }

    public void SaveGameToFile()
    {
        var boardManager = GameObject.FindAnyObjectByType<BoardManager>();
        if (boardManager == null)
        {
            Debug.LogError("BoardManager non trovato!");
            return;
        }

        // Chiama GetBoardData() e controlla se il risultato è valido
        BoardData boardStatus = boardManager.GetBoardData();
        this.GameInfo.BoardData = boardStatus;
        if (boardStatus == null)
        {
            Debug.LogError("boardConfig è null!");
            return;
        }

        // Salvo i pezzi usati attualmente dall'opponent nel file di salvataggio
        OpponentManager opponentManager = FindAnyObjectByType<OpponentManager>();
        if (opponentManager != null)
        {
            GameInfo.OpponentInfo.CurrentlyUsedExtraPieces = new List<PieceData>();
            opponentManager.pieces.ForEach(p => GameInfo.OpponentInfo.CurrentlyUsedExtraPieces.Add(p.GetPieceData()));
        }

        // Salvo i pezzi usati attualmente dal player nel file di salvataggio
        PlayerManager playerManager = FindAnyObjectByType<PlayerManager>();
        if (playerManager != null)
        {
            GameInfo.PlayerInfo.CurrentlyUsedExtraPieces = new List<PieceData>();
            playerManager.pieces.ForEach(p => GameInfo.PlayerInfo.CurrentlyUsedExtraPieces.Add(p.GetPieceData()));
        }


        // string json = JsonConvert.SerializeObject(boardStatus, Formatting.Indented); // Usa JSON.NET
        SaveManager.Instance.Save(GameInfo, this.GameInfo.ProfileName);
    }

    public void LoadGameFromFile()
    {
        GameInfo = SaveManager.Instance.Load<GameInfo>(GameInfo.ProfileName);

        var boardManager = GameObject.FindAnyObjectByType<BoardManager>();
        if (boardManager == null)
        {
            Debug.LogError("BoardManager non trovato!");
            return;
        }

        boardManager.BuildFromData(GameInfo.BoardData);

        var playerManager = GameObject.FindAnyObjectByType<PlayerManager>();
        if (playerManager == null)
        {
            Debug.LogError("BoardManager non trovato!");
            return;
        }
        playerManager.BuildFromData(GameInfo.PlayerInfo);
    }

    internal void GameOver(PieceColor? winner)
    {
        this.isGameOver = true;
        GameInfo.Winner = winner;

        // Give money only if game is currently running to prevent giving again money
        if (GameInfo.GameState == GameState.RUNNING && GameInfo.Winner == PieceColor.White)
        {
            this.GameInfo.PlayerInfo.Money += MoneyWonFromCurrentRound;
        }

        // Signal that now game is over
        GameInfo.GameState = GameState.GAME_OVER;

        SaveGameToFile();
    }

    public int MoneyWonFromCurrentRound
    {
        get => 2 * GameInfo.currentLevel * GameInfo.currentStage;
    }

    internal void AdvanceLevel()
    {
        GameInfo.currentStage++;
        if (GameInfo.currentStage == MAX_LEVEL + 1)
        {
            GameInfo.currentLevel += 1;
            GameInfo.currentStage = 1;
            GameInfo.Level = GetRandomLevel().Name;
        }

        // Move currentPieces in inventory (player only because enemy will be overridden in other function)
        GameInfo.PlayerInfo.CurrentlyUsedExtraPieces.ForEach(p => {
            // Remove modifiers from piece
            p.Modifiers.Clear();
            p.AppliedModifierPaths.Clear();
            GameInfo.PlayerInfo.ExtraPieces.Add(p);
        });
        GameInfo.PlayerInfo.CurrentlyUsedExtraPieces = new List<PieceData>();
        GameInfo.OpponentInfo.CurrentlyUsedExtraPieces = new List<PieceData>();
    }

    public void GoToNextLevel()
    {
        AdvanceLevel();
        GameInfo gameInfo = GameInfo;

        // To reset game running again
        gameInfo.GameState = GameState.RUNNING;
        gameInfo.Winner = null;
        Debug.Log($"Livello salvato: {gameInfo.Level} e getrandom level ritorna:");
        List<PieceData> enemies = LevelGenerator.Instance.GeneratePieces(GetLevel(gameInfo.Level), PieceColor.Black, gameInfo.currentLevel, gameInfo.currentStage);
        gameInfo.OpponentInfo.ExtraPieces = enemies;
        foreach (var item in gameInfo.BoardData.piecesData)
        {
            if (item != null && item.PieceColor == PieceColor.White && item.PieceType != PieceType.King)
            {
                gameInfo.PlayerInfo.ExtraPieces.Add(item);
            }
        }
        gameInfo.PlayerInfo.CurrentlyUsedExtraPieces.ForEach(p => gameInfo.PlayerInfo.ExtraPieces.Add(p));
        gameInfo.PlayerInfo.CurrentlyUsedExtraPieces = new List<PieceData>();

        BoardManager.MovePiecesFromInventoryToPlanes(gameInfo, 10);
        gameInfo.BoardData = LevelGenerator.Instance.GenerateDefaultBoardData();

        IsGameOver = false;

        SaveGameToFile(gameInfo);
        LoadGameFromFile();
        ContinueGame(gameInfo);
    }

    public void GoToNextLevelFromShop()
    {
        AdvanceLevel();
        GameInfo gameInfo = GameInfo;

        // To reset game running again
        gameInfo.GameState = GameState.RUNNING;
        gameInfo.Winner = null;
        Debug.Log($"Livello salvato: {gameInfo.Level} e getrandom level ritorna:");
        List<PieceData> enemies = LevelGenerator.Instance.GeneratePieces(GetLevel(gameInfo.Level), PieceColor.Black, gameInfo.currentLevel, gameInfo.currentStage);
        gameInfo.OpponentInfo.ExtraPieces = enemies;
        foreach (var item in gameInfo.BoardData.piecesData)
        {
            if (item != null && item.PieceColor == PieceColor.White && item.PieceType != PieceType.King)
            {
                gameInfo.PlayerInfo.ExtraPieces.Add(item);
            }
        }
        gameInfo.PlayerInfo.CurrentlyUsedExtraPieces.ForEach(p => gameInfo.PlayerInfo.ExtraPieces.Add(p));
        gameInfo.PlayerInfo.CurrentlyUsedExtraPieces = new List<PieceData>();

        BoardManager.MovePiecesFromInventoryToPlanes(gameInfo, 10);
        gameInfo.BoardData = LevelGenerator.Instance.GenerateDefaultBoardData();

        IsGameOver = false;

        SaveGameToFile(gameInfo);
        //LoadGameFromFile();
        ContinueGame(gameInfo);
    }

    public ScriptableLevel GetLevel(string name)
    {
        return levels.Find(level => level.Name == name);
    }

    public ScriptableLevel GetRandomLevel()
    {
        int randomIndex = UnityEngine.Random.Range(0, levels.Count);
        return levels[randomIndex];
    }

    public void SaveOptions()
    {
        SaveManager.Instance.SaveOptions(Options);
    }

    internal void PlayTutorial(GameInfo selectedGameInfo)
    {
        StartCoroutine(PlayTutorialCoroutine(Constants.Scenes.SAMPLE_SCENE));
    }
}
