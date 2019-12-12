using System;
using System.Collections.Generic;

namespace RogueSharp
{
   /// <summary>
   /// Generic Cellular Automata functions
   /// </summary>
   public static class CellularAutomata
   {
         /// <summary>
         /// Runs one cellular automata generation with the provided born/survive rules
         /// </summary>
         /// <typeparam name="T"></typeparam>
         public static T CellularAutomataRunGeneration<T>(T map, HashSet<int> born, HashSet<int> survive ) where T : class, IMap, new()
         {
            T updatedMap = map.Clone<T>();

            foreach ( ICell cell in map.GetAllCells() )
            {
               var isAlive = !cell.IsWalkable;
               var newIsLive = false;
               var count = CountWallsNear( map, cell, 1);

               if( IsBorderCell(map, cell))
               {
                  continue;
               }
               if(isAlive && survive.Contains(count))
               {
                  newIsLive = true;
               }
               else if(!isAlive && born.Contains(count))
               {
                  newIsLive = true;
               }

               var isWalkable = !newIsLive;
               updatedMap.SetCellProperties(cell.X, cell.Y, isWalkable,isWalkable);
            }

            return updatedMap;
         }


      /// <summary>
      /// Returns true for cells along the edge
      /// </summary>
      /// <typeparam name="T"></typeparam>
      public static bool IsBorderCell<T>( T map, ICell cell ) where T : class, IMap, new()
      {
         return cell.X == 0 || cell.X == map.Width - 1
                || cell.Y == 0 || cell.Y == map.Height - 1;
      }

      /// <summary>
      /// Counts the "walls" in the radius around the given cell
      /// </summary>
      /// <typeparam name="T"></typeparam>
      public static int CountWallsNear<T>(T map, ICell cell, int distance )where T : class, IMap, new()
      {
         int count = 0;
         foreach ( ICell nearbyCell in map.GetCellsInSquare( cell.X, cell.Y, distance ) )
         {
            if ( nearbyCell.X == cell.X && nearbyCell.Y == cell.Y )
            {
               continue;
            }
            if ( !nearbyCell.IsWalkable )
            {
               count++;
            }
         }
         return count;
      }
   }
}