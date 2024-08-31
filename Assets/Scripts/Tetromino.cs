using UnityEngine;
using UnityEngine.Tilemaps;

public enum Tetromino 
{
    I = 1,
    J = 2,
    L = 3,
    O = 4,
    S = 5,
    T = 6, 
    Z = 7
}

[System.Serializable]
public struct TetrominoData
{
    public Tetromino TetrominoEnum;
    public Tile Tiles;
    public Vector2Int[] Cells;
    public Vector2Int[,] WallKicks { get; private set; }

    public void Initialize()
    {
        Cells = Data.Cells[TetrominoEnum];
        WallKicks = Data.WallKicks[TetrominoEnum];
    }
}