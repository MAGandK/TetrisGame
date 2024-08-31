using UnityEngine;
using UnityEngine.Tilemaps;

public class Ghost : MonoBehaviour
{
   public Tile GhostTiles;
   public Board MainBoard;
   public Piece TrackingPiece;

   public Tilemap Tilemaps { get; private set; }
   public Vector3Int[] Cells { get; private set; }
   public Vector3Int GhostPosition { get; private set; }

   private void Awake()
   {
      Tilemaps = GetComponentInChildren<Tilemap>();
      Cells = new Vector3Int[4];
   }

   private void LateUpdate()
   {
      Clear();
      Copy();
      Drop();
      Set();
   }

   private void Clear()
   {
      for (int i = 0; i < Cells.Length; i++)
      {
         Vector3Int tilePosition = Cells[i] + GhostPosition;
         Tilemaps.SetTile(tilePosition, null);
      }
   }

   private void Copy()
   {
      for (int i = 0; i < Cells.Length; i++)
      {
         Cells[i] = TrackingPiece.Cells[i];
      }
   }

   private void Drop()
   {
      Vector3Int pos = TrackingPiece.Position;
      
      int current = pos.y;
      int bottom = -MainBoard.BoardSize.y / 2 - 1;
      
      MainBoard.Clear(TrackingPiece);

      for (int row = current; row >= bottom; row--)
      {
         pos.y = row;

         if (MainBoard.IsValidPosition(TrackingPiece, pos))
         {
            GhostPosition = pos;
         }
         else
         {
            break;
         }
      }
      
      MainBoard.Set(TrackingPiece);
   }

   private void Set()
   {
      for (int i = 0; i < Cells.Length; i++)
      {
         Vector3Int tilePos = Cells[i] + GhostPosition;
         Tilemaps.SetTile(tilePos, GhostTiles);
      }
   }
}
