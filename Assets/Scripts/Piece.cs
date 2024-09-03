using UnityEngine;
public class Piece : MonoBehaviour
{
   public Board Board { get; private set; }
   public TetrominoData Data { get; private set; }
   public Vector3Int Position { get; private set; }
   public Vector3Int[] Cells { get; private set; }
   private int IndexRotation { get; set; }

   public float StepDelay = 1f;
   public float MoveDelay = 0.1f;
   public float LockDelay = 0.5f;

   private float StepTime;
   private float MoveTime;
   private float LockTime;
   
   public void Initialize(Board board,Vector3Int position, TetrominoData data)
   { 
     Board = board;
     Position = position;
     Data = data;
     IndexRotation = 0;
     StepTime = Time.time + StepDelay;
     MoveTime = Time.time + MoveDelay;
     LockTime = 0;

     if (Cells == null) 
     { 
        Cells = new Vector3Int[Data.Cells.Length];
     }

     for (int i = 0; i < Data.Cells.Length; i++)
     { 
        Cells[i] = (Vector3Int)Data.Cells[i];
     }
   }

   private void Update()
   {
     Board.Clear(this);

     LockTime += Time.deltaTime;

     if (Input.GetKeyDown(KeyCode.Q))
     {
        Rotation(-1);
     }
     else if(Input.GetKeyDown(KeyCode.E))
     {
        Rotation(1);
     }
     
     if (Input.GetKeyDown(KeyCode.Space))
     { 
        HardDrop();
     }
     
     if (Time.time > MoveTime)
     {
        HandleMoveInputs();
     }

     if (Time.time > StepTime)
     {
        Step();
     }
     
     Board.Set(this);
   }

   private void HandleMoveInputs()
   {
      if (Input.GetKey(KeyCode.S) || Input.GetKeyDown(KeyCode.S))
      {
         if ( Move(Vector2Int.down))
         {
            StepTime = Time.time + StepDelay;
         }
      }
      if (Input.GetKey(KeyCode.A) || Input.GetKeyDown(KeyCode.A))
      { 
         Move(Vector2Int.left);
      }
      else if (Input.GetKey(KeyCode.D) || Input.GetKeyDown(KeyCode.D))
      { 
         Move(Vector2Int.right);
      }
   }

   private void Step()
   {
      StepTime = Time.time + StepDelay;
      Move(Vector2Int.down);
      if (LockTime >= LockDelay)
      {
         Lock();
      }
   }

   private void Lock()
   {
      Board.Set(this);
      Board.ClearLines();
      Board.SpawnPiece();
   }
   private void HardDrop()
   {
      while (Move(Vector2Int.down))
      {
         continue;
      }
      Lock();
   }
   private bool Move(Vector2Int translation)
   {
      Vector3Int newPosition = this.Position;
      newPosition.x += translation.x;
      newPosition.y += translation.y;

      bool valid = Board.IsValidPosition(this,newPosition);

      if (valid)
      {
        Position = newPosition;
        MoveTime = Time.time + MoveDelay;
        LockTime = 0f;
      }

      return valid;
   }

   private void Rotation(int direction)
   {
      int originRotation = IndexRotation;
      
      IndexRotation += Wrap(IndexRotation + direction, 0,4 );
      ApplyRotationMatrix(direction);

      if (!TestWallKick(IndexRotation,direction))
      {
         IndexRotation = originRotation;
         ApplyRotationMatrix(-direction);
      }
   }

   private bool TestWallKick(int rotetionIndex, int rotationDirection)
   {
      var wallKickIndex = GetWallKickIndex(rotetionIndex, rotationDirection);
   
      for (int i = 0; i < Data.WallKicks.GetLength(1); i++)
      {
         Vector2Int translation = Data.WallKicks[wallKickIndex, i];
         if (Move(translation))
         {
            return true;
         }
      }
   
      return false;
   }

   private int GetWallKickIndex(int rotetionIndex, int rotationDirection)
   {
      int wallKickIndex = rotetionIndex * 2;
      if (rotationDirection < 0)
      {
         wallKickIndex--;
      }

      return Wrap(wallKickIndex, 0, Data.WallKicks.GetLength(0));
   }

   private void ApplyRotationMatrix(int direction)
   {
      float[] matrix = global::Data.RotationMatrix;
      
      for (int i = 0; i < Data.Cells.Length; i++)
      {
         Vector3 cell = Cells[i];
         
         int x, y;
         
         switch (this.Data.TetrominoEnum)
         {
            case Tetromino.I :
            case Tetromino.O :
               cell.x -= 0.5f;
               cell.y -= 0.5f;
               
               x = Mathf.CeilToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
               y = Mathf.CeilToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
               break;
            default: 
               x = Mathf.RoundToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
               y = Mathf.RoundToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
               break;
         }

         Cells[i] = new Vector3Int(x, y, 0);
      }
   }
   private int Wrap(int input,int min, int max)
   {
      if (input < min)
      {
         return max - (min - input) % (max - min);
      }
      else
      {
         return min + (input - min) % (max - min);
      }
   }
}
