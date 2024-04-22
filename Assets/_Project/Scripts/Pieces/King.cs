using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class King : ChessPiece
{
    public override IEnumerable<Vector3Int> ValidMoves(bool xray = false) {
        for(int i=-1; i<=1; ++i) {
            for(int j=-1; j<=1; ++j) {
                if(i == 0 && j == 0) continue;
                Vector3Int target = new Vector3Int(i, j) + Position;
                if(board.Contains(target) && board.GetPiece(target)?.PieceColor != PieceColor) {
                    yield return target;
                }
            }
        }
    }
}
