using System;
using System.Collections.Generic;
using RogueSharp.Random;

namespace RogueSharp.MapCreation
{
   /// <summary>
   /// An implementation of Prim's algorith for random maze generation.
   /// See: https://en.wikipedia.org/wiki/Prim's_algorithm
   /// </summary>
   /// <typeparam name="T"></typeparam>
    public class PrimsMazeMapCreationStrategy<T> : IMapCreationStrategy<T> where T : class, IMap, new()
    {
       int _width;
       int _height;
       T _map;
       IRandom _random;

       Point[] _skipNeighbors = new Point[]{
          new Point(-2, 0),
          new Point(2, 0),
          new Point(0,-2),
          new Point(0,2)
       };

       /// <summary>
       /// Create a new strategy that will create mazes using Prim's Algorith.
       /// </summary>
       /// <param name="width">Even numbers leave uneven padding around the perimeter.</param>
       /// <param name="height">Even numbers leave uneven padding around the perimeter.</param>
       /// <param name="random">Psuedo random number generator.</param>
        public PrimsMazeMapCreationStrategy(int width, int height, IRandom random)
        {
           _width = width;
           _height = height;
           _random = random;
          _map = new T();
        }

         /// <summary>
         /// Return a new maze
         /// </summary>
         /// <returns></returns>
         public T CreateMap()
        {
            _map.Initialize(_width, _height);
            var w = _map.Width;
            // _map.GetCell(x,y) returns a new object every time it's called making it unsuitable
            // for use with list.Add()/list.Remove(). Each cell needs a unique object or value.
            // (y * w) + x creates a unique index.

            // Create "open" list of nodes where the neighbors are not yet exhausted.
            var open = new List<int>();
            open.Add((1*w)+1); // add cell 1,1
            _map.SetCellProperties(1, 1, true, false); // use map settings to mark cell on the "open" list.
            while(open.Count > 0)
            {
               // Pick from the open nodes at random.
               var index = _random.Next(open.Count - 1);
               var current = open[index];
               var currX = current % w;
               var currY = (current - currX) / w;
               var currentCell = _map.GetCell(currX, currY);

               // Get 4 neighbors with a one cell buffer for the walls.
               var possibleNodes = GetSkipNeighbors(currentCell);

               if(possibleNodes.Length > 0)
               {
                  // Randomly select a destination, add it to the open list and clear the path.
                  var node = _random.Next(possibleNodes.Length - 1);
                  var nextCell = possibleNodes[node];
                  var link = GetLinkCell(currentCell, nextCell);
                  _map.SetCellProperties(link.X, link.Y, true, true);
                  _map.SetCellProperties(nextCell.X, nextCell.Y, true, false);
                  open.Add((nextCell.Y * w) + nextCell.X);
               }
               else
               {
                  // There are no more valid links from this node.
                  open.Remove(current);
                  _map.SetCellProperties(currX, currY, true, true);
               }

            }
            return _map;
        }

        ICell GetLinkCell(ICell start, ICell end)
        {
            var deltaX = (end.X - start.X) / 2;
            var deltaY = (end.Y - start.Y) / 2;
            var newX = start.X + deltaX;
            var newY = start.Y + deltaY;
            var linkCell = _map.GetCell(newX, newY);
            return linkCell;
        }

        ICell[] GetSkipNeighbors(ICell cell)
        {
           var results = new List<ICell>();
           foreach(var check in _skipNeighbors)
           {
              var checkX = cell.X + check.X;
              var checkY = cell.Y + check.Y;
              if(checkX >= 0 && checkX < _map.Width && checkY >= 0 && checkY < _map.Height)
              {
                 var addCell = _map.GetCell(checkX, checkY);
                 // Only cells that are !transparent and !walkable are valid next steps.
                 if(!MapHelper.IsBorderCell(_map, addCell) && !addCell.IsTransparent && !addCell.IsWalkable)
                 {
                  results.Add(addCell);
                 }
              }
           }
           return results.ToArray();
        }
    }
}
