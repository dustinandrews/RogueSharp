using System.Collections.Generic;

namespace RogueSharp.MapCreation
{
   /// <summary>
   /// Chunk of a map
   /// </summary>
   public class MapSection
   {
      private int _top;
      private int _bottom;
      private int _right;
      private int _left;

      /// <summary>
      /// Bounding Rectangle
      /// </summary>
      public Rectangle Bounds => new Rectangle( _left, _top, _right - _left + 1, _bottom - _top + 1 );

      /// <summary>
      /// Cells
      /// </summary>
      /// <value></value>
      public HashSet<ICell> Cells { get; private set; }

      /// <summary>
      /// Create an empty MapSection
      /// </summary>
      public MapSection()
      {
         Cells = new HashSet<ICell>();
         _top = int.MaxValue;
         _left = int.MaxValue;
      }

      /// <summary>
      /// Add a cell
      /// </summary>
      /// <param name="cell"></param>
      public void AddCell( ICell cell )
      {
         Cells.Add( cell );
         UpdateBounds( cell );
      }

      private void UpdateBounds( ICell cell )
      {
         if ( cell.X > _right )
         {
            _right = cell.X;
         }
         if ( cell.X < _left )
         {
            _left = cell.X;
         }
         if ( cell.Y > _bottom )
         {
            _bottom = cell.Y;
         }
         if ( cell.Y < _top )
         {
            _top = cell.Y;
         }
      }

      /// <summary>
      /// ToString
      /// </summary>
      /// <returns></returns>
      public override string ToString()
      {
         return $"Bounds: {Bounds}";
      }
   }
}
