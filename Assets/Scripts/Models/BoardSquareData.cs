using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AI;

public class BoardSquareData : IEquatable<BoardSquareData>
{
    [JsonIgnore] public Vector2 Position;
    public List<string> ManualsModifierNames = new List<string>();

    public BoardSquareData(Vector2 Position, List<string> ManualsModifierPaths)
    {
        this.Position = Position;
        this.ManualsModifierNames = ManualsModifierPaths;
    }

    public bool Equals(BoardSquareData other)
    {
        return other != null
        && Position == other.Position
        && ManualsModifierNames == other.ManualsModifierNames;
    }

    

    public static BoardSquareData FromStatus(BoardSquare boardSquare) {
        if (boardSquare == null) return null;

        List<string> manualModifiersPaths = new List<string>();
        if (boardSquare.ManualsModifiers != null) {
        boardSquare.ManualsModifiers.ForEach(m => {
            if (m != null) manualModifiersPaths.Add(m.name);
        });
        }
        
        return new BoardSquareData(boardSquare.Position, manualModifiersPaths);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Position, ManualsModifierNames);
    }
}
