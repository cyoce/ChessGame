using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ChessPiece : MonoBehaviour
{
    public BoardManager board;
    SpriteRenderer rendy;
    
    public PColor _color;
    public PColor PieceColor {
        get => _color;
        set {
            switch(value) {
                case PColor.White:
                    rendy.color = new Color(0.6f, 0.5f, 0.4f);
                    break;
                case PColor.Black:
                    rendy.color = Color.black;
                    break;
            }
        }
    }

    public bool moved;
    [SerializeField] public Vector3Int _position;
    public Vector3Int Position {
        get => _position;
        set {
            if(_position == value && _initialized)
                Debug.Log("Warning: " + this + " Moving in place");
            if(!captured) {
                board.SetPiece(Position, null);
                _position = value;
                board.SetPiece(value, this);
            }
            transform.position = board.WorldPos(value);
            moved = true;
        }
    }
    public int rank {
        get => Position.y;
    }

    public int file {
        get => Position.x;
    }

    private bool _selected;
    public bool Selected { 
        get => _selected; 
        set {
            _selected = value;
            transform.localScale = Vector3.one * (value ? 1.2f : 1);
        }
    }

    private bool _initialized = false;
    virtual public int value() => 1;
    // Start is called before the first frame update
    void Start() {
        rendy = GetComponent<SpriteRenderer>();

        Position = _position;

        PieceColor = _color;

        _initialized = true;

        moved = false;
    }

    public bool captured = false;

    static protected int[] plusminus = { 1, -1 };
    virtual public bool CanMove(Vector3Int target, bool xray=false) { // override for perf boost
        if(captured) return false;
        foreach(Vector3Int candidate in ValidMoves(xray)) {
            if(candidate == target) return true;
        }
        return false;
    }

    virtual public IEnumerable<Vector3Int> ValidMoves(bool xray=false) { // override per piece
        yield break;
    }

    virtual public IEnumerable<Vector3Int> ControlSquares() {
        return ValidMoves(false);
    }
}

public enum PColor {
    White=0,
    Black=1
}