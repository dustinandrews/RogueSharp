using System;
using System.Collections.Generic;
using RogueSharp.Algorithms;
using RogueSharp.Random;

namespace RogueSharp.MapCreation
{
    /// <summary>
    /// The CaveMapCreationStrategy creates a Map of the specified type by using a cellular automata algorithm for creating a cave-like map.
    /// </summary>
    /// <seealso href="http://www.roguebasin.com/index.php?title=Cellular_Automata_Method_for_Generating_Random_Cave-Like_Levels">Cellular Automata Method from RogueBasin</seealso>
    /// <typeparam name="T">The type of IMap that will be created</typeparam>
    public class CaveMapCreationStrategy<T> : IMapCreationStrategy<T> where T : class, IMap, new()
   {
      private readonly int _width;
      private readonly int _height;
      private readonly int _fillProbability;
      private readonly int _totalIterations;
      private readonly int _cutoffOfBigAreaFill;
      private readonly IRandom _random;
      private T _map;

      /// <summary>
      /// Constructs a new CaveMapCreationStrategy with the specified parameters
      /// </summary>
      /// <param name="width">The width of the Map to be created</param>
      /// <param name="height">The height of the Map to be created</param>
      /// <param name="fillProbability">Recommend int between 40 and 60. Percent chance that a given cell will be a floor when randomizing all cells.</param>
      /// <param name="totalIterations">Recommend int between 2 and 5. Number of times to execute the cellular automata algorithm.</param>
      /// <param name="cutoffOfBigAreaFill">Recommend int less than 4. The iteration number to switch from the large area fill algorithm to a nearest neighbor algorithm</param>
      /// <param name="random">A class implementing IRandom that will be used to generate pseudo-random numbers necessary to create the Map</param>
      public CaveMapCreationStrategy( int width, int height, int fillProbability, int totalIterations, int cutoffOfBigAreaFill, IRandom random)
      {
         _width = width;
         _height = height;
         _fillProbability = fillProbability;
         _totalIterations = totalIterations;
         _cutoffOfBigAreaFill = cutoffOfBigAreaFill;
         _random = random;
         _map = new T();
      }

      /// <summary>
      /// Constructs a new CaveMapCreationStrategy with the specified parameters
      /// </summary>
      /// <param name="width">The width of the Map to be created</param>
      /// <param name="height">The height of the Map to be created</param>
      /// <param name="fillProbability">Recommend int between 40 and 60. Percent chance that a given cell will be a floor when randomizing all cells.</param>
      /// <param name="totalIterations">Recommend int between 2 and 5. Number of times to execute the cellular automata algorithm.</param>
      /// <param name="cutoffOfBigAreaFill">Recommend int less than 4. The iteration number to switch from the large area fill algorithm to a nearest neighbor algorithm</param>
      /// <remarks>Uses DotNetRandom as its RNG</remarks>
      public CaveMapCreationStrategy( int width, int height, int fillProbability, int totalIterations, int cutoffOfBigAreaFill )
      {
         _width = width;
         _height = height;
         _fillProbability = fillProbability;
         _totalIterations = totalIterations;
         _cutoffOfBigAreaFill = cutoffOfBigAreaFill;
         _random = Singleton.DefaultRandom;
         _map = new T();
      }

      /// <summary>
      /// Creates a new IMap of the specified type.
      /// </summary>
      /// <remarks>
      /// The map will be generated using cellular automata. First each cell in the map will be set to a floor or wall randomly based on the specified fillProbability.
      /// Next each cell will be examined a number of times, and in each iteration it may be turned into a wall if there are enough other walls near it.
      /// Once finished iterating and examining neighboring cells, any isolated map regions will be connected with paths.
      /// </remarks>
      /// <returns>An IMap of the specified type</returns>
      public T CreateMap()
      {
         _map.Initialize( _width, _height );

         RandomlyFillCells();

         for ( int i = 0; i < _totalIterations; i++ )
         {
            if ( i < _cutoffOfBigAreaFill )
            {
               CellularAutomataBigAreaAlgorithm();
            }
            else if ( i >= _cutoffOfBigAreaFill )
            {
               CellularAutomataNearestNeighborsAlgorithm();
            }
         }

         _map = MapHelper.ConnectOrphanedSections(_map);

         return _map;
      }

      private void RandomlyFillCells()
      {
         _map = MapHelper.RandomlyFillCells(_map, _fillProbability, _random);
      }

      private void CellularAutomataBigAreaAlgorithm()
      {
         T updatedMap = _map.Clone<T>();

         foreach ( ICell cell in _map.GetAllCells() )
         {
            if ( IsBorderCell( cell ) )
            {
               continue;
            }
            if ( ( CountWallsNear( cell, 1 ) >= 5 ) || ( CountWallsNear( cell, 2 ) <= 2 ) )
            {
               updatedMap.SetCellProperties( cell.X, cell.Y, false, false );
            }
            else
            {
               updatedMap.SetCellProperties( cell.X, cell.Y, true, true );
            }
         }

         _map = updatedMap;
      }

      private void CellularAutomataNearestNeighborsAlgorithm()
      {
         T updatedMap = _map.Clone<T>();

         foreach ( ICell cell in _map.GetAllCells() )
         {
            if ( IsBorderCell( cell ) )
            {
               continue;
            }
            if ( CountWallsNear( cell, 1 ) >= 5 )
            {
               updatedMap.SetCellProperties( cell.X, cell.Y, false, false );
            }
            else
            {
               updatedMap.SetCellProperties( cell.X, cell.Y, true, true );
            }
         }

         _map = updatedMap;
      }

      private bool IsBorderCell(ICell cell)
      {
         return MapHelper.IsBorderCell(_map, cell);
      }

      private int CountWallsNear(ICell cell, int distance)
      {
         return MapHelper.CountWallsNear(_map, cell, distance);
      }
   }
}
