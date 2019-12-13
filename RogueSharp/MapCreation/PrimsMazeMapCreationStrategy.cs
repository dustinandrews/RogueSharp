using System.Collections.Generic;
using RogueSharp.Random;

namespace RogueSharp.MapCreation
{
    /// <summary>
    /// An implementation of Prim's algorithm for random maze generation.
    /// See: https://en.wikipedia.org/wiki/Prim's_algorithm
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PrimsMazeMapCreationStrategy<T> : MazeStrategyBase<T>, IMapCreationStrategy<T> where T : class, IMap, new()
    {
        /// <summary>
        /// New Prim's algorithm maze generator
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="random"></param>
        /// <returns></returns>
        public PrimsMazeMapCreationStrategy(int width, int height, IRandom random) : base(width, height, random)
        {
        }


        /// <summary>
        /// Return a new maze
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
            var open = new List<int>();
            open.Add((1 * w) + 1); // add cell 1,1
            Map.SetCellProperties(1, 1, true, false); // use map settings to mark cell on the "open" list.
            while (open.Count > 0)
            {
                // Pick from the open nodes at random.
                var index = Random.Next(open.Count - 1);
                var current = open[index];
                var currX = current % w;
                var currY = (current - currX) / w;
                var currentCell = Map.GetCell(currX, currY);

                // Get 4 neighbors with a one cell buffer for the walls.
                var possibleNodes = GetSkipNeighbors(currentCell);

                if (possibleNodes.Length > 0)
                {
                    // Randomly select a destination, add it to the open list and clear the path.
                    var node = Random.Next(possibleNodes.Length - 1);
                    var nextCell = possibleNodes[node];
                    var link = GetLinkCell(currentCell, nextCell);
                    Map.SetCellProperties(link.X, link.Y, true, true);
                    Map.SetCellProperties(nextCell.X, nextCell.Y, true, false);
                    open.Add((nextCell.Y * w) + nextCell.X);
                }
                else
                {
                    // There are no more valid links from this node.
                    open.Remove(current);
                    Map.SetCellProperties(currX, currY, true, true);
                }

            }
            return Map;
        }
    }
}
