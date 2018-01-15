
using System.Drawing;
using TentacleSlicers.interfaces;
using Point = TentacleSlicers.general.Point;

namespace TentacleSlicers.hud
{
    /// <summary>
    /// Représente un composant affiché à l'écran en position absolue, dessiné et actualisé.
    /// </summary>
    public abstract class HudComponent : IDrawable, ITickable
    {
        public static readonly string ImagePath = graphics.SpriteHandler.ImagePath + "hud/";

        protected Point Position { get; }

        /// <summary>
        /// Crée le composant à la position indiquée.
        /// </summary>
        /// <param name="position"> Le centre du composant </param>
        protected HudComponent(Point position)
        {
            Position = position;
        }

        public abstract void Draw(Point shift, Graphics g);
        public abstract void Tick(int ms);
    }
}