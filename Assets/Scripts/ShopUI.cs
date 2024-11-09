using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopUI : MonoBehaviour
{
    public Texture2D defaultCursor;
    public Texture2D handCursor;
    public TMP_Text TotalMoneyText;

    private static Texture2D _defaultCursor;
    private static Texture2D _handCursor;

    void Start()
    {
        _defaultCursor = defaultCursor;
        _handCursor = handCursor;
    }
    void Update()
    {
        TotalMoneyText.text = $"Total money: {GameManager.Instance.GameInfo.PlayerInfo.Money}$";
    }

    public void BackToMainMenu()
    {
        GameManager.Instance.RestartGame();
        GameManager.Instance.IsPaused = false;
    }

    public void GoToNextLevel()
    {
        GameManager.Instance.GoToNextLevelFromShop();
    }

    public static void SetCursor(CursorType cursorType)
    {
        switch (cursorType)
        {
            case CursorType.Default:
                Cursor.SetCursor(_defaultCursor, Vector2.zero, CursorMode.Auto);
                break;
            case CursorType.Hand:
                Cursor.SetCursor(_handCursor, Vector2.zero, CursorMode.Auto);
                break;
            default:
                Cursor.SetCursor(_defaultCursor, Vector2.zero, CursorMode.Auto);
                break;
        }
    }
}
