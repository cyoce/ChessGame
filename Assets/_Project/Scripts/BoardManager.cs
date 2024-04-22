using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class BoardManager : MonoBehaviour
{
    [SerializeField] ChessPiece pawn;
    [SerializeField] ChessPiece rook;
    [SerializeField] ChessPiece knight;
    [SerializeField] ChessPiece bishop;
    [SerializeField] ChessPiece king;
    [SerializeField] ChessPiece queen;

    [SerializeField] Tilemap squareMap;

    [SerializeField] TileBase darkSquare;
    [SerializeField] TileBase lightSquare;

    [SerializeField] TMP_Text whiteMoveLabel;
    [SerializeField] TMP_Text blackMoveLabel;

    ChessPiece[,] pieceBoard = new ChessPiece[8, 8];
    List<ChessPiece>[] captures = { new List<ChessPiece>(), new List<ChessPiece>() };
    List<ChessPiece>[] pieces = { new List<ChessPiece>(), new List<ChessPiece>() };
    ChessPiece moveTarget;

    bool playing = true;

    PColor? computer = PColor.Black;

    PColor _turn = PColor.White;
    public PColor Turn {
        get => _turn;
        set {
            _turn = value;
            whiteMoveLabel.gameObject.SetActive(_turn == PColor.White);
            blackMoveLabel.gameObject.SetActive(_turn == PColor.Black);
        }
    }

    // Start is called before the first frame update
    void Start() {
        for(int r = 0; r < 8; ++r) {
            for(int c = 0; c < 8; ++c) {
                squareMap.SetTile(new Vector3Int(r, c, 0), (r + c) % 2 == 0 ? darkSquare : lightSquare);
            }
        }

        foreach(int color in new int[] {0, 1}) {
            int file;
            file = 0;
            ChessPiece[] backRank = { rook, knight, bishop, queen, king, bishop, knight, rook };
            foreach(ChessPiece next in backRank) {
                AddPiece(next, file++, color * 7, color);
            }

            for(file = 0; file < 8; ++file) {
                AddPiece(pawn, file, 1 + 5 * color, color);
            }
        }

        blackMoveLabel.outlineWidth = 0.1f;
        blackMoveLabel.color = Color.black;
        Turn = PColor.White;
        
    }

    // Update is called once per frame
    void Update() {
        // TEMP BIND
        if(Input.GetKeyDown("s")) computer = PColor.Black;
        if(Input.GetKeyDown("a")) computer = PColor.White;
        if(Input.GetKeyDown(KeyCode.Space)) computer = null;
        if(Input.GetKeyDown("q")) {
            for(int i = 8; i-->0;) {
                string s = "";
                for(int j = 0; j < 8; ++j) {
                    s += GetPiece(j, i) switch {
                        Pawn p => "p",
                        King k => "k",
                        Queen q => "q",
                        Bishop b => "b",
                        Rook r => "r",
                        Knight n => "n",
                        _ => "-"
                    } + " ";
                }

                Debug.Log(s);
            }
        }
        if(!playing) return;
        if(Turn == computer) {
            BotMove();
            return;
        }
        if(Input.GetMouseButtonDown(0)) {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int boardPos = squareMap.WorldToCell(worldPos);
            if(Contains(boardPos)) {
                if(moveTarget == null) { // this click has selected a piece
                    moveTarget = pieceBoard[boardPos.x, boardPos.y];
                    if(moveTarget?.PieceColor == Turn) {
                        moveTarget.Selected = true;
                    } else {
                        moveTarget = null;
                    }
                    
                } else if(GetPiece(boardPos) == moveTarget || !moveTarget.CanMove(boardPos)){ // user has clicked on the selected piece's current square
                    CancelMove();
                } else { // move selected piece
                    MovePiece(boardPos);
                }
                
            } else if(moveTarget != null) {
                CancelMove();
            }
        }
    }

    void BotMove() {
        int tIdx = (int) Turn;
        ChessPiece topMover = null;
        Vector3Int topMove = new Vector3Int();
        float highScore = float.MinValue;
        foreach(ChessPiece mover in pieces[tIdx]) {
            Debug.Log("moving " + mover);
            Vector3Int origin = new Vector3Int (mover._position.x, mover._position.y);
            SetPiece(origin, null);
            foreach(Vector3Int move in mover.ValidMoves()) {
                Debug.Log(": " + move);
                ChessPiece capped = SetPiece(move, mover);
                if(capped is King k) {
                    Debug.Log("King!");
                    highScore = float.MaxValue;
                    topMover = mover;
                    topMove = move;
                    SetPiece(k._position, k);
                    break;
                }
                mover._position = move;
                sbyte[,] control = new sbyte[8, 8];
                for(int i=0; i<2; ++i) {
                    sbyte incrementer = (sbyte)(i == tIdx ? 1 : -1);
                    foreach(ChessPiece controller in pieces[i]) {
                        if(controller == capped) continue;
                        
                        foreach(Vector3Int pos in controller.ControlSquares()) {
                            control[pos.x, pos.y] += incrementer;
                        }
                    }
                }
                float score = 0;
                for(int r=0; r<8; ++r) {
                    for(int f=0; f<8; ++f) {
                        float c = control[f, r];
                        ChessPiece king = pieces[c > 0 ? 1 - tIdx : tIdx][4];
                        float dist = Manhattan(king.Position, mover.Position);
                        float mult = 1;
                        if(GetPiece(f, r) is ChessPiece p) {
                            mult += p.value();
                        }
                        mult += 10 / math.pow(1 + dist, 2);
                        score += c * mult;
                    }
                }
                Debug.Log(score);
                if(score > highScore) {
                    highScore = score;
                    topMover = mover;
                    topMove = move;
                }
                
                
                mover._position = origin;
                
                SetPiece(move, capped);
            }
            SetPiece(origin, mover);
        }

        if(topMover == null) {
            Debug.Log("Error: failed to find move");
            computer = null;
            return;
        }
        moveTarget = topMover;
        MovePiece(topMove);
    }

    int Manhattan(Vector3Int v1, Vector3Int v2) {
        Vector3Int r = v1 - v2;
        return math.abs(r.x) + math.abs(r.y);
    }

    void MovePiece(Vector3Int pos) {
        Debug.Log(moveTarget);
        Debug.Log(moveTarget?.Position + " => " + pos);
        if(GetPiece(pos) is ChessPiece p) {
            Capture(p);
        }
        moveTarget.Position = pos;
        //moveTarget.transform.position = squareMap.CellToWorld(boardPos);
        moveTarget.Selected = false;
        moveTarget = null;

        if(playing)
            Turn = 1 - Turn;
    }

    void AddPiece(ChessPiece piece, int file, int rank, int color) {
        piece._position = new Vector3Int(file, rank);
        piece.board = this;
        piece._color = (PColor)color;
        piece = Instantiate(piece);
        pieces[color].Add(piece);
    }
    
    void Capture(ChessPiece piece) {
        piece.captured = true;
        int color = (int)piece.PieceColor;
        List<ChessPiece> list = captures[color];
        piece.Position = new Vector3Int(list.Count, 8 - 9 * color);
        list.Add(piece);
        if(piece is King k)
            GameOver();
    }

    public void GameOver() {
        playing = false;
        (Turn == PColor.White ? whiteMoveLabel : blackMoveLabel).text = Turn + " Wins!";
    }

    public ChessPiece GetPiece(Vector3Int position) {
        return pieceBoard[position.x, position.y];
    }
    public ChessPiece GetPiece(int file, int rank) {
        return pieceBoard[file, rank];
    }


    public ChessPiece SetPiece(Vector3Int position, ChessPiece piece) {
        return SetPiece(position.x, position.y, piece);
    }

    public ChessPiece SetPiece(int file, int rank,  ChessPiece piece) {
        ChessPiece temp = pieceBoard[file, rank];
        pieceBoard[file, rank] = piece;
        return temp;
    }


    public bool Contains(Vector3Int boardPos) {
        return boardPos.x >= 0 && boardPos.x < 8 && boardPos.y >= 0 && boardPos.y < 8;
    }

    public void CancelMove() {
        moveTarget.Selected = false;
        moveTarget = null;
    }

    public Vector3 WorldPos(Vector3Int position) {
        return squareMap.CellToWorld(position);
    }
}
