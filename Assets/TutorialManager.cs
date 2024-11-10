using System.Collections;
using System.Collections.Generic;
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
    private int dialogueBeat;

    void Start()
    {
        PlacePiece((0, 4), initialPlayerPiecePrefab);
        consumable.SetActive(false);
        //Capire se anche da inattivi fanno cose
        manual.SetActive(false);
    }
    void Update()
    {

    }

    public void TutorialBeatFight()
    {
        RemovePiece(initialPlayerPiecePrefab);
        PlacePiece((4, 4), fightPlayerPiecePrefab);
        PlacePiece((5, 4), opponentPiecePrefab);
    }
    public void TutorialBeatConsumable()
    {
        RemovePiece(fightPlayerPiecePrefab);
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
        piecePrefab = Instantiate(piecePrefab, targetPosition, Quaternion.identity);
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
