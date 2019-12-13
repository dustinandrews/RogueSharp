using System.Collections.Generic;
using RogueSharp.Random;

namespace RogueSharp.MapCreation
{
    /// <summary>
    /// Base class for creating mazes
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MazeStrategyBase<T> : IMapCreationStrategy<T> where T : class, IMap, new()
    {
        private int width;
        private int height;
        private T map;
        private IRandom random;

        Point[] _skipNeighbors = new Point[]{
          new Point(-2, 0),
          new Point(2, 0),
          new Point(0,-2),
          new Point(0,2)
       };

        /// <summary>
        /// Map Width
        /// </summary>
        /// <value></value>
        protected int Width { get => width; set => width = value; }

        /// <summary>
        /// Map Height
        /// </summary>
        /// <value></value>
        protected int Height { get => height; set => height = value; }

        /// <summary>
        /// Map instance
        /// </summary>
        /// <value></value>
        protected T Map { get => map; set => map = value; }

        /// <summary>
        /// Psuedo random number generator
        /// </summary>
        /// <value></value>
        protected IRandom Random { get => random; set => random = value; }

        /// <summary>
        /// Create a new strategy that will create mazes using Prim's Algorith.
        /// </summary>
        /// <param name="width">Even numbers leave uneven padding around the perimeter.</param>
        /// <param name="height">Even numbers leave uneven padding around the perimeter.</param>
        /// <param name="random">Psuedo random number generator.</param>
        public MazeStrategyBase(int width, int height, IRandom random)
        {
            Width = width;
            Height = height;
            Random = random;
            Map = new T();
        }

        /// <summary>
        /// Return a new maze
        /// </summary>
        /// <returns></returns>
        public abstract T CreateMap();

        /// <summary>
        /// Given cells that are vertically or horizonatally seperated by one cell, return the one in the middle.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        protected ICell GetLinkCell(ICell start, ICell end)
        {
            var deltaX = (end.X - start.X) / 2;
            var deltaY = (end.Y - start.Y) / 2;
            var newX = start.X + deltaX;
            var newY = start.Y + deltaY;
            var linkCell = Map.GetCell(newX, newY);
            return linkCell;
        }

        /// <summary>
        /// Returns all the valid map cells one hop vertically or horizontally that are not transparent or walkable
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        protected ICell[] GetSkipNeighbors(ICell cell)
        {
            var results = new List<ICell>();
            foreach (var check in _skipNeighbors)
            {
                var checkX = cell.X + check.X;
                var checkY = cell.Y + check.Y;
                if (checkX >= 0 && checkX < Map.Width && checkY >= 0 && checkY < Map.Height)
                {
                    var addCell = Map.GetCell(checkX, checkY);
                    // Only cells that are !transparent and !walkable are valid next steps.
                    if (!MapHelper.IsBorderCell(Map, addCell) && !addCell.IsTransparent && !addCell.IsWalkable)
                    {
                        results.Add(addCell);
                    }
                }
            }
            return results.ToArray();
        }
    }
}
