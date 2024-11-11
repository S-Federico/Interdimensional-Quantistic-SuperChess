using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

/*
Azioni richieste al player, in sequenza:
1. Hover su statistiche 
2. Click su pezzo per vedere mosse
3. Muovere Pezzo
4. Attaccare Pezzo nemico
5. Hover e click su consumabile
6. Hover e click su manuale
7. Ruotare camera
8. Posizionare pezzo di riserva

StoryBeats:
1. Spawn pezzo bianco 
2. -
3. -
4. Spawn pezzo bianco e pezzo nemico vicini
5. Spawn consumabile
6. Spawn manuale
7. -
8. Spawn pezzo di riserva
*/
public class TutorialManager : MonoBehaviour
{
    public GameObject consumable;
    public GameObject manual;
    public GameObject opponentPiecePrefab;
    public GameObject initialPlayerPiecePrefab;
    public GameObject fightPlayerPiecePrefab;
    public GameObject extraPiecePrefab;
    public BoardManager boardManager;
    public DialogController dialogController;
    void Start()
    {
        consumable.SetActive(false);
        manual.SetActive(false);
    }

    void OnEnable() {
        dialogController.OnNewLineHandler += NextBeat;
        dialogController.OnFinishDialogueHandler += OnFinishDialoguePressed;
    }

    void OnDisable() {
        dialogController.OnNewLineHandler -= NextBeat;
        dialogController.OnFinishDialogueHandler -= OnFinishDialoguePressed;
    }

    private void OnFinishDialoguePressed()
    {
        GameManager.Instance.LoadScene(Constants.Scenes.MENU);
    }

    void Update()
    {
    }

    public void NextBeat(int newBeat)
    {
        switch (newBeat)
        {
            case 1:
                FirstStoryBeat();
                break;
            case 3:
                TutorialBeatFight();
                break;
            case 4:
                TutorialBeatConsumable();
                break;
            case 6:
                TutorialBeatManual();
                break;
            case 8:
                TutorialBeatExtraPieces();
                break;
        }
    }
    public void FirstStoryBeat()
    {
        PlacePiece((7, 4), initialPlayerPiecePrefab);
    }
    public void TutorialBeatFight()
    {
        RemovePiece(initialPlayerPiecePrefab);
        PlacePiece((5, 4), fightPlayerPiecePrefab);
        PlacePiece((4, 4), opponentPiecePrefab);
    }
    public void TutorialBeatConsumable()
    {
        RemovePiece(fightPlayerPiecePrefab);
        if(opponentPiecePrefab!=null){
            RemovePiece(opponentPiecePrefab);
        }
        consumable.SetActive(true);
    }
    public void TutorialBeatManual()
    {
        manual.SetActive(true);
    }
    public void TutorialBeatExtraPieces()
    {
        PlacePiece((-1, -1), extraPiecePrefab);
    }
    private void PlacePiece((int x, int y) position, GameObject piecePrefab)
    {
        Vector3 targetPosition = boardManager.GetSquare(position.x, position.y).transform.position;
        piecePrefab.transform.position = targetPosition;
        PieceStatus pieceStatus = piecePrefab.GetComponent<PieceStatus>();
        pieceStatus.Position = new Vector2(position.x, position.y);
        if (pieceStatus.Position.x >= 0)
            boardManager.Pieces[position.x, position.y] = pieceStatus;
    }
    private void RemovePiece(GameObject piece)
    {
        if (piece != null)
        {
            boardManager.Pieces[(int)piece.GetComponent<PieceStatus>().Position.x, (int)piece.GetComponent<PieceStatus>().Position.y] = null;
            Destroy(piece);
        }
    }
}
