using System;
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
            throw new NotImplementedException();
        }
    }
}
