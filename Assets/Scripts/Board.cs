using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
  public Tilemap Tilemap { get; private set; }
  public TetrominoData[] TerminoDatas;
  public Piece ActivePiece { get; private set; }
  public Vector3Int SpawnPosition;
  public Vector2Int BoardSize = new Vector2Int(16, 20);
  [SerializeField] private TextMeshProUGUI _scoreText;
  private int _score = 100;

  public RectInt Bounds
  {
    get
    {
      Vector2Int position = new Vector2Int(-this.BoardSize.x / 2, -this.BoardSize.y / 2);
      return new RectInt(position, this.BoardSize);
    }
  }

  private void Awake()
  {
    Tilemap = GetComponentInChildren<Tilemap>();
    ActivePiece = GetComponentInChildren<Piece>();
    
    for (int i = 0; i < TerminoDatas.Length; i++)
    {
      TerminoDatas[i].Initialize();
    }
  }

  private void Start()
  {
    SpawnPiece();
  }

  public void SpawnPiece()
  {
    int random = Random.Range(0, TerminoDatas.Length);
    TetrominoData data = TerminoDatas[random];
    
    ActivePiece.Initialize(this, SpawnPosition,data);

    if (IsValidPosition(ActivePiece,SpawnPosition))
    {
      Set(ActivePiece);
    }
    else
    {
      GameOver();
    }
  }

  public void GameOver()
  {
    Tilemap.ClearAllTiles();
  }

  public void Set(Piece piece)
  {
    for (int i = 0; i < piece.Cells.Length; i++)
    {
      Vector3Int tilePosition = piece.Cells[i] + piece.Position;
      Tilemap.SetTile(tilePosition, piece.Data.Tiles);
    }
  }

  public void Clear(Piece piece)
  {
    for (int i = 0; i < piece.Cells.Length; i++)
    {
      Vector3Int tilePosition = piece.Cells[i] + piece.Position;
      Tilemap.SetTile(tilePosition, null);
    }
  }
  
  public bool IsValidPosition(Piece piece, Vector3Int position)
  {
    RectInt bounds = Bounds;
    
    for (int i = 0; i < piece.Cells.Length; i++)
    {
      Vector3Int tilePos = piece.Cells[i] + position;

      if (!bounds.Contains((Vector2Int)tilePos))
      {
        return false;
      }
      if (Tilemap.HasTile(tilePos))
      {
        return false;
      }
    }
    return true;
  }
  public void ClearLines()
  {
    RectInt bounds = Bounds;
    int row = bounds.yMax;

    while (row >= bounds.yMin)
    {
      if (IsLineFull(row))
      {
        LineClear(row);
        _score += 100;
        _scoreText.SetText(_score.ToString());
      }
      else
      {
        row--;
      }
    }
  }

  private void LineClear(int row)
  {
    RectInt bounds = Bounds;
    for (int col = bounds.xMin; col < bounds.xMax; col++)
    {
      Vector3Int position = new Vector3Int(col, row, 0);
      Tilemap.SetTile(position, null);
    }

    while (row < bounds.yMax)
    {
      for (int col = bounds.xMin; col < bounds.xMax; col++)
      {
        Vector3Int position = new Vector3Int(col, row + 1, 0);
        TileBase above = Tilemap.GetTile(position);

        position = new Vector3Int(col, row, 0);
        Tilemap.SetTile(position, above);
      }
      row++;
    }
  }
  public bool IsLineFull(int row)
  {
    RectInt bounds = Bounds;

    for (int col = bounds.xMin; col < bounds.xMax; col++)
    {
      Vector3Int position = new Vector3Int(col, row, 0);

      if (!Tilemap.HasTile(position))
      {
        return false;
      }
    }
    return true;
  }
}
