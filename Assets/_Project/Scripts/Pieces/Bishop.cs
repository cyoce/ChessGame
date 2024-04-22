using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Slider
{
    public override int value() => 3;
    public override IEnumerable<Vector3Int> ValidMoves(bool xray = false, bool control = false) {
        foreach(int r in plusminus) {
            foreach(int f in plusminus) {
                foreach(Vector3Int target in ScanMoves(new Vector3Int(f, r), xray, control)) {
                    yield return target;
                }
            }
        }
    }
}
