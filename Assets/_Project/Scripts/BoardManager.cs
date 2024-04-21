using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    [SerializeField] Tilemap squareMap;

    [SerializeField] TileBase darkSquare;
    [SerializeField] TileBase lightSquare;
    ChessPiece[,] pieceBoard = new ChessPiece[8, 8];
    List<ChessPiece>[] captures = { new List<ChessPiece>(), new List<ChessPiece>() };
    ChessPiece moveTarget;

    // Start is called before the first frame update
    void Start()
    {
        //squareMap.SetTile(new Vector3Int(100, 200, 300), lightSquare);
        for(int r = 0; r < 8; ++r) {
            for(int c = 0; c < 8; ++c) {
                squareMap.SetTile(new Vector3Int(r, c, 0), (r + c) % 2 == 0 ? darkSquare : lightSquare);
            }
        }
    }

    // Update is called once per frame
    void Update() {
        if(Input.GetMouseButtonDown(0)) {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int boardPos = squareMap.WorldToCell(worldPos);
            if(Contains(boardPos)) {
                if(moveTarget == null) { // this click has selected a piece
                    moveTarget = pieceBoard[boardPos.x, boardPos.y];
                    if(moveTarget != null)
                        moveTarget.Selected = true;
                } else if(GetPiece(boardPos) == moveTarget || !moveTarget.CanMove(boardPos)){ // user has clicked on the selected piece's current square
                    CancelMove();
                } else { // move selected piece
                    if(GetPiece(boardPos) is ChessPiece p) {
                        capture(p);
                    }
                    moveTarget.Position = boardPos;
                    moveTarget.transform.position = squareMap.CellToWorld(boardPos);
                    moveTarget.Selected = false;
                    moveTarget = null;
                }
                
            } else if(moveTarget != null) {
                CancelMove();
            }
        }
    }
    
    void capture(ChessPiece piece) {
        piece.captured = true;
        int color = (int)piece.PieceColor;
        List<ChessPiece> list = captures[color];
        piece.Position = new Vector3Int(list.Count, 8 - 9 * color);
        list.Add(piece);
    }

    public ChessPiece GetPiece(Vector3Int position) {
        return pieceBoard[position.x, position.y];
    }
    public ChessPiece GetPiece(int file, int rank) {
        return pieceBoard[file, rank];
    }


    public void SetPiece(Vector3Int position, ChessPiece piece) {
        pieceBoard[position.x, position.y] = piece;
    }

    public void SetPiece(int file, int rank,  ChessPiece piece) {
        pieceBoard[file, rank] = piece;
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
