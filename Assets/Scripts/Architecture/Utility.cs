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

}