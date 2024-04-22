using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Slider
{
    public override int value() => 5;
    public override IEnumerable<Vector3Int> ValidMoves(bool xray=false, bool control = false) {
        Vector3Int[] directions = { Vector3Int.left, Vector3Int.right, Vector3Int.up, Vector3Int.down };
        foreach(Vector3Int dir in directions){
            foreach(Vector3Int move in ScanMoves(dir, xray, control)) {
                yield return move;
            }
        }
    }
}
