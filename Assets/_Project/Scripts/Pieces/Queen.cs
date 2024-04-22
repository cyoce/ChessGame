using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Slider
{
    public override int value() => 9;
    public override IEnumerable<Vector3Int> ValidMoves(bool xray = false) {
        for(int i = -1; i <= 1; ++i) {
            for(int j = -1; j <= 1; ++j) {
                if((i | j) == 0) continue;
                foreach(Vector3Int target in ScanMoves(new Vector3Int(i, j), xray)) {
                    yield return target;
                }
            }
        }
    }
}
