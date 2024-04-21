using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessPiece
{
    public override IEnumerable<Vector3Int> ValidMoves(bool xray = false) {
        int moveDirection = 1;
        if(PieceColor == PColor.Black) {
            moveDirection = -1;
        }
        Vector3Int target;
        foreach(int sign in plusminus) {
            target = new Vector3Int(sign, moveDirection) + Position;
            if(board.GetPiece(target)?.PieceColor == (PColor)(1 - (int) PieceColor)) {
                yield return target;
            }
        }
        target = Position;
        for(int i = 0; i < 2; ++i) {
            target.y += moveDirection;
            if(board.GetPiece(target) != null)
                break;

            yield return target;
            if(moved)
                break;
        }
    }
}
