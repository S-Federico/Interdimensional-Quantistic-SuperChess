using System;
using System.Collections;
using System.Collections.Generic;
using Array2DEditor;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using UnityEngine;

public static class Utility
{
    public static int[,] ConvertA2DintToIntMatrix(Array2DInt array2DInt)
    {
        int[,] result = new int[array2DInt.GridSize.x, array2DInt.GridSize.y];
        if (array2DInt != null)
        {
            for (int i = 0; i < array2DInt.GridSize.x; i++)
            {
                for (int j = 0; j < array2DInt.GridSize.y; j++)
                {
                    result[i, j] = array2DInt.GetCell(j, i);
                }
            }
        }
        return result;
    }

    public static List<T> SelectCurrentMatchPieces<T>(int n, List<T> pieces)
    {
        List<T> avaiblePieces = new List<T>();

        // Se non ci sono abbastanza pezzi li usiamo tutti
        if (n > pieces.Count)
        {
            avaiblePieces = new List<T>(pieces);
            return avaiblePieces;
        }

        List<T> tempList = new List<T>(pieces);

        for (int i = 0; i < n; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, tempList.Count);
            avaiblePieces.Add(tempList[randomIndex]);

            // Rimuovi il pezzo dalla lista temporanea per evitare ripetizioni
            tempList.RemoveAt(randomIndex);
        }
        return avaiblePieces;
    }

}