using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.Assertions;

public class Slider : ChessPiece
{
    public IEnumerable<Vector3Int> ScanMoves(Vector3Int direction, bool xray = false, bool control=false) {
        Assert.AreNotEqual(direction, Vector3Int.zero);
        Vector3Int target = Position;
        while(true) {
            target += direction;
            if(!board.Contains(target))
                yield break;

            ChessPiece captured = board.GetPiece(target);
            if(!xray && captured?.PieceColor == PieceColor)
                yield break;

            yield return target;

            if(!xray && captured != null)
                yield break;
        }
    }
}
