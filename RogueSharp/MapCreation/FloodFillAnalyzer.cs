using System.Collections.Generic;

namespace RogueSharp.MapCreation
{
      /// <summary>
      /// Flood Fill utility
      /// </summary>
      public class FloodFillAnalyzer
      {
         private readonly IMap _map;
         private readonly List<MapSection> _mapSections;

         private readonly int[][] _offsets =
         {
            new[] { 0, -1 }, new[] { -1, 0 }, new[] { 1, 0 }, new[] { 0, 1 }
         };

         private readonly bool[][] _visited;

         /// <summary>
         /// Create new analyzer for the given map.
         /// </summary>
         /// <param name="map"></param>
         public FloodFillAnalyzer( IMap map )
         {
            _map = map;
            _mapSections = new List<MapSection>();
            _visited = new bool[_map.Height][];
            for ( int i = 0; i < _visited.Length; i++ )
            {
               _visited[i] = new bool[_map.Width];
            }
         }

         /// <summary>
         /// Get list of map sections based on flood fill
         /// </summary>
         /// <returns></returns>
         public List<MapSection> GetMapSections()
         {
            IEnumerable<ICell> cells = _map.GetAllCells();
            foreach ( ICell cell in cells )
            {
               MapSection section = Visit( cell );
               if ( section.Cells.Count > 0 )
               {
                  _mapSections.Add( section );
               }
            }

            return _mapSections;
         }

         private MapSection Visit( ICell cell )
         {
            Stack<ICell> stack = new Stack<ICell>( new List<ICell>() );
            MapSection mapSection = new MapSection();
            stack.Push( cell );
            while ( stack.Count != 0 )
            {
               cell = stack.Pop();
               if ( _visited[cell.Y][cell.X] || !cell.IsWalkable )
               {
                  continue;
               }
               mapSection.AddCell( cell );
               _visited[cell.Y][cell.X] = true;
               foreach ( ICell neighbor in GetNeighbors( cell ) )
               {
                  if ( cell.IsWalkable == neighbor.IsWalkable && !_visited[neighbor.Y][neighbor.X] )
                  {
                     stack.Push( neighbor );
                  }
               }
            }
            return mapSection;
         }

         private ICell GetCell( int x, int y )
         {
            if ( x < 0 || y < 0 )
            {
               return null;
            }
            if ( x >= _map.Width || y >= _map.Height )
            {
               return null;
            }
            return _map.GetCell( x, y );
         }

         private IEnumerable<ICell> GetNeighbors( ICell cell )
         {
            List<ICell> neighbors = new List<ICell>( 8 );
            foreach ( int[] offset in _offsets )
            {
               ICell neighbor = GetCell( cell.X + offset[0], cell.Y + offset[1] );
               if ( neighbor == null )
               {
                  continue;
               }
               neighbors.Add( neighbor );
            }

            return neighbors;
         }
      }
}
