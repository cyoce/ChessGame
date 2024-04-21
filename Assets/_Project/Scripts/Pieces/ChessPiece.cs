using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ChessPiece : MonoBehaviour
{
    protected BoardManager board;
    TMP_Text text;
    
    [SerializeField] protected PColor _color;
    public PColor PieceColor {
        get => _color;
        set {
            switch(value) {
                case PColor.White:
                    text.color = Color.white;
                    text.outlineColor = Color.black;
                    text.outlineWidth = 0.2f;
                    break;
                case PColor.Black:
                    text.color = Color.black;
                    text.outlineColor = Color.white;
                    text.outlineWidth = 0.1f;
                    break;
            }
        }
    }

    [SerializeField] protected Vector3Int _position;
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
            text.fontStyle = value ? FontStyles.Bold : FontStyles.Normal;
        }
    }

    private bool _initialized = false;
    // Start is called before the first frame update
    void Start() {
        Debug.Log(name);
        board = GetComponentInParent<BoardManager>();
        text = GetComponentInChildren<TMP_Text>();

        Position = _position;

        PieceColor = _color;

        _initialized = true;
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
}

public enum PColor {
    White=0,
    Black=1
}