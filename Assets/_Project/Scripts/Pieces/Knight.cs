using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : ChessPiece {
    public override IEnumerable<Vector3Int> ValidMoves(bool xray=false) {
        foreach(int dx in new int[]{ 1, 2 }){
            int dy = 3 - dx;
            foreach(int xscale  in plusminus) { 
                foreach(int yscale in plusminus) {
                    Vector3Int output = Position + new Vector3Int(dx * xscale, dy * yscale);
                    if(board.Contains(output)) {
                        yield return output;
                    }
                }
            }
        }
    }
}
