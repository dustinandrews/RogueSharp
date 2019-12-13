using System;
using System.Collections.Generic;
using RogueSharp.Random;

namespace RogueSharp.MapCreation
{
   /// <summary>
   /// Alternative Cave Map generator based on https://jeremykun.com/2012/07/29/the-cellular-automaton-method-for-cave-generation/
   /// </summary>
   /// <typeparam name="T"></typeparam>
    public class CaveMap2CreationStrategy<T> : IMapCreationStrategy<T> where T : class, IMap, new()
    {
      int _initialIterations = 10;
      HashSet<int> firstPassBorn = new HashSet<int>(){6,7,8};
      HashSet<int> firstPassSurvive = new HashSet<int>(){3,4,5,6,7,8};

      HashSet<int> smoothPassBorn = new HashSet<int>(){5,6,7,8};
      HashSet<int> smoothPassSurvive = new HashSet<int>(){5,6,7,8,};

      int _width;
      int _height;
      int _fillProbability;
      IRandom _random;
      T _map;

      /// <summary>
      /// Cellular Cave map creation based on https://jeremykun.com/2012/07/29/the-cellular-automaton-method-for-cave-generation/
      /// </summary>
      /// <param name="width">Map width</param>
      /// <param name="height">Map height</param>
      /// <param name="fillProbability">Density of inital grid. Recomended 50-70.</param>
      /// <param name="random">Psuedo random number generator</param>
      public CaveMap2CreationStrategy(int width, int height, int fillProbability, IRandom random)
      {
         _width = width;
         _height = height;
         _fillProbability = fillProbability;
         _random = random;
         _map = new T();
      }

      /// <summary>
      /// Create new Cave Map. Start with a 1/2 scale rough version, scale up and smooth
      /// </summary>
      /// <returns></returns>
      public T CreateMap()
      {
         _map.Initialize( _width / 2, _height / 2 );
         _map = MapHelper.RandomlyFillCells(_map, _fillProbability, _random);
         for(int i = 0; i < _initialIterations; i++)
         {
            _map = MapHelper.RunGeneration(_map, firstPassBorn, firstPassSurvive);
         }
         _map = MapHelper.ScaleUp(_map, 2);
         _map = MapHelper.RunGeneration(_map, smoothPassBorn, smoothPassSurvive);
         _map = MapHelper.ConnectOrphanedSections(_map);

         return _map;
      }
    }
}
