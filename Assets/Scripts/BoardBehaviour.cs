using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardBehaviour : MonoBehaviour
{
    public GameObject[,] squares; 
    private Transform planeTransform;
    private float squareSize;
    public int BoardSize = 8; 

    public void InitializeBoard()
    {
        if (this.gameObject != null)
        {
            planeTransform = this.transform.Find("Plane");
            if (planeTransform != null)
            {
                squareSize = planeTransform.localScale.x * 10 / BoardSize; 
            }
            else
            {
                Debug.LogError("Plane non trovato come figlio di Board!");
            }
        }
        else
        {
            Debug.LogError("Board non assegnato!");
        }

        squares = new GameObject[BoardSize, BoardSize];

        for (int x = 0; x < BoardSize; x++)
        {
            for (int y = 0; y < BoardSize; y++)
            {
                // Crea un nuovo GameObject piano
                GameObject newSquare = GameObject.CreatePrimitive(PrimitiveType.Plane);

                // Aggiungi tag alla casella
                newSquare.tag = Constants.SQUARE_TAG;

                // Aggiungi componente alla casella
                BoardSquare boardSquare = newSquare.AddComponent<BoardSquare>();
                boardSquare.Position = new Vector2(x, y);

                // Imposta la scala del piano in base alla dimensione della casella
                newSquare.transform.localScale = new Vector3(squareSize / 10f, 1f, squareSize / 10f);

                // Calcola la posizione della casella rispetto al piano
                Vector3 squarePosition = new Vector3(
                    x * squareSize + planeTransform.position.x - planeTransform.localScale.x * 5 + squareSize / 2,
                    planeTransform.position.y + 0.0001f,
                    y * squareSize + planeTransform.position.z - planeTransform.localScale.z * 5 + squareSize / 2
                );

                // Posiziona la casella nella posizione corretta
                newSquare.transform.position = squarePosition;

                // Assegna un nome alla casella basato sulla posizione
                newSquare.name = $"Square_{x}_{y}";

                // Rimuovi o disabilita il MeshRenderer per rendere il piano invisibile
                MeshRenderer renderer = newSquare.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    renderer.enabled = false;
                }

                // Imposta il nuovo piano come figlio del board
                newSquare.transform.parent = this.transform;

                // Salva la casella nell'array per futuri riferimenti
                squares[x, y] = newSquare;
            }
        }
        if (planeTransform != null)
        {
            Destroy(planeTransform.gameObject);
        }
    }

    public GameObject GetSquare(int x, int y)
    {
        if (x >= 0 && x < BoardSize && y >= 0 && y < BoardSize)
        {
            return squares[x, y];
        }
        else
        {
            return null;
        }
    }
}
