using System;
using System.Linq;
using System.Collections.Generic;
using RogueSharp.Random;

namespace RogueSharp.MapCreation
{
    /// <summary>
    /// Create a depth first/flood fill maze strategy.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DepthFirstMazeMapCreationStrategy<T> : MazeStrategyBase<T>, IMapCreationStrategy<T> where T : class, IMap, new()
    {
        /// <summary>
        /// New strategy
        /// </summary>
        /// <param name="width">Even numbers leave uneven padding around the perimeter.</param>
        /// <param name="height">Even numbers leave uneven padding around the perimeter.</param>
        /// <param name="random">Psuedo random number generator.</param>
        /// <returns></returns>
        public DepthFirstMazeMapCreationStrategy(int width, int height, IRandom random) : base(width, height, random)
        {
        }

        /// <summary>
        /// Create a new maze
        /// </summary>
        /// <returns></returns>
        public override T CreateMap()
        {
            Map.Initialize(Width, Height);
            var w = Map.Width;
            // _map.GetCell(x,y) returns a new object every time it's called making it unsuitable
            // for use with list.Add()/list.Remove(). Each cell needs a unique object or value.
            // (y * w) + x creates a unique index.

            // Create "open" list of nodes where the neighbors are not yet exhausted.
            var open = new Stack<int>();
            var currentCell = Map.GetCell(1, 1);
            open.Push((currentCell.Y * w) + currentCell.X); // add cell 1,1

            Map.SetCellProperties(1, 1, true, false);

            while (open.Any())
            {
                var unvisited = GetSkipNeighbors(currentCell);
                if (unvisited.Length > 0)
                {
                    // Randomly select a destination, add it to the open list and clear the path.
                    var node = Random.Next(unvisited.Length - 1);
                    var nextCell = unvisited[node];
                    var link = GetLinkCell(currentCell, nextCell);
                    Map.SetCellProperties(link.X, link.Y, true, true);
                    Map.SetCellProperties(nextCell.X, nextCell.Y, true, false);
                    currentCell = nextCell;
                    open.Push((currentCell.Y * w) + currentCell.X);
                }
                else
                {
                    var index = open.Pop();
                    var currX = index % w;
                    var currY = (index - currX) / w;
                    currentCell = Map.GetCell(currX, currY);
                    Map.SetCellProperties(currentCell.X, currentCell.Y, true, true);
                }
            }

            return Map;
        }
    }
}
