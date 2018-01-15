using TentacleSlicers.general;

namespace TentacleSlicers.collisions
{
    /// <summary>
    /// Permet la transmission simple d'un rectangle et de son centre.
    /// </summary>
    public struct ShiftedRectangle
    {
        public Point Position { get; }
        public Rectangle Rectangle { get; }

        /// <summary>
        /// Stocke le rectangle et le point passés en argument.
        /// </summary>
        /// <param name="position"> Le centre du rectangle </param>
        /// <param name="rectangle"> Le rectangle indiqué </param>
        public ShiftedRectangle(Point position, Rectangle rectangle)
        {
            Position = position;
            Rectangle = rectangle;
        }
    }
}