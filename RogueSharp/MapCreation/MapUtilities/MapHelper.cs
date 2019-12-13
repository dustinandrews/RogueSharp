using System;
using System.Collections.Generic;
using RogueSharp.Random;
using RogueSharp.Algorithms;

namespace RogueSharp.MapCreation
{
   /// <summary>
   /// Generic Cellular Automata functions
   /// </summary>
   public static class MapHelper
   {
         /// <summary>
         /// Runs one cellular automata generation with the provided born/survive rules
         /// </summary>
         /// <typeparam name="T"></typeparam>
         public static T RunGeneration<T>(T map, HashSet<int> born, HashSet<int> survive ) where T : class, IMap, new()
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
      public static int CountWallsNear<T>(T map, ICell cell, int distance ) where T : class, IMap, new()
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

      /// <summary>
      /// Randomly fill cells at the desired density
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="map">Map to base filled map on, will not be altered</param>
      /// <param name="fillProbability">0-100 chance of each cell being filled.</param>
      /// <param name="random">Pseudo-random number generator</param>
      public static T RandomlyFillCells<T>(T map, int fillProbability, IRandom random) where T : class, IMap, new()
      {
         var outmap = map.Clone<T>();
         foreach ( ICell cell in map.GetAllCells() )
         {
            if ( IsBorderCell( map, cell ) )
            {
               outmap.SetCellProperties( cell.X, cell.Y, false, false );
            }
            else if ( random.Next( 1, 100 ) < fillProbability )
            {
               outmap.SetCellProperties( cell.X, cell.Y, true, true );
            }
            else
            {
               outmap.SetCellProperties( cell.X, cell.Y, false, false );
            }
         }
         return outmap;
      }

      /// <summary>
      /// Resize map via nearest neighbor scaling to new map size.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      public static T ScaleUp<T>(T map, int scale) where T: class, IMap, new()
      {
         if(scale < 2)
         {
            throw new ArgumentException("Scale factor must be greater than 1.");
         }
         var newHeight = map.Height * scale;
         var newWidth = map.Width * scale;
         var outmap = new T();
         outmap.Initialize(newWidth, newHeight);
         var heightRatio = (double) map.Height / newHeight;
         var widthRatio = (double) map.Width / newWidth;
         foreach( ICell cell in outmap.GetAllCells() )
         {
            var baseX = (int)(widthRatio * cell.X);
            var baseY = (int)(heightRatio * cell.Y);
            var baseCell = map.GetCell(baseX, baseY);
            outmap.SetCellProperties(cell.X, cell.Y, baseCell.IsTransparent, baseCell.IsTransparent);
         }

         return outmap;
      }

      /// <summary>
      /// Connects any orphaned sections of the map together.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      public static T ConnectOrphanedSections<T>(T map)where T: class, IMap, new()
      {
         var floodFillAnalyzer = new FloodFillAnalyzer( map );
         var returnMap = map.Clone<T>();
         List<MapSection> mapSections = floodFillAnalyzer.GetMapSections();
         var unionFind = new UnionFind( mapSections.Count );
         while ( unionFind.Count > 1 )
         {
            for ( int i = 0; i < mapSections.Count; i++ )
            {
               int closestMapSectionIndex = FindNearestMapSection( mapSections, i, unionFind );
               MapSection closestMapSection = mapSections[closestMapSectionIndex];
               IEnumerable<ICell> tunnelCells = map.GetCellsAlongLine( mapSections[i].Bounds.Center.X, mapSections[i].Bounds.Center.Y,
                  closestMapSection.Bounds.Center.X, closestMapSection.Bounds.Center.Y );
               ICell previousCell = null;
               foreach ( ICell cell in tunnelCells )
               {
                  returnMap.SetCellProperties( cell.X, cell.Y, true, true );
                  if ( previousCell != null )
                  {
                     if ( cell.X != previousCell.X || cell.Y != previousCell.Y )
                     {
                        returnMap.SetCellProperties( cell.X + 1, cell.Y, true, true );
                     }
                  }
                  previousCell = cell;
               }
               unionFind.Union( i, closestMapSectionIndex );
            }
         }
         return returnMap;
      }

      /// <summary>
      /// Get the closest sections index.
      /// </summary>
      /// <param name="mapSections"></param>
      /// <param name="mapSectionIndex"></param>
      /// <param name="unionFind"></param>
      /// <returns></returns>
      public static int FindNearestMapSection( IList<MapSection> mapSections, int mapSectionIndex, UnionFind unionFind )
      {
         MapSection start = mapSections[mapSectionIndex];
         int closestIndex = mapSectionIndex;
         int distance = int.MaxValue;
         for ( int i = 0; i < mapSections.Count; i++ )
         {
            if ( i == mapSectionIndex )
            {
               continue;
            }
            if ( unionFind.Connected( i, mapSectionIndex ) )
            {
               continue;
            }
            int distanceBetween = DistanceBetween( start, mapSections[i] );
            if ( distanceBetween < distance )
            {
               distance = distanceBetween;
               closestIndex = i;
            }
         }
         return closestIndex;
      }

      /// <summary>
      /// Gets Manhattan distance between map sections.
      /// </summary>
      /// <param name="startMapSection"></param>
      /// <param name="destinationMapSection"></param>
      /// <returns></returns>
      public static int DistanceBetween( MapSection startMapSection, MapSection destinationMapSection )
      {
         return Math.Abs( startMapSection.Bounds.Center.X - destinationMapSection.Bounds.Center.X ) + Math.Abs( startMapSection.Bounds.Center.Y - destinationMapSection.Bounds.Center.Y );
      }
   }
}