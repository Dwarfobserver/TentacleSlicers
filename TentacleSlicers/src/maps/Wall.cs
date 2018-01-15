using System.Drawing;
using TentacleSlicers.actors;
using TentacleSlicers.collisions;
using TentacleSlicers.graphics;
using Point = TentacleSlicers.general.Point;
using Rectangle = TentacleSlicers.collisions.Rectangle;

namespace TentacleSlicers.maps
{
    /// <summary>
    /// Spéficie la classe Actor pour représenter un mur, de la taille d'une case de la map.
    /// Il peut être invisible ou avoir un comportement transparent, lorsque des acteurs passent derrière lui.
    /// </summary>
    public sealed class Wall : Actor
    {
        public static readonly Rectangle Hitbox = new Rectangle(Map.Size - Merge, Map.Size - Merge);

        private const double Merge = 0.01;

        /// <summary>
        /// Crée le mur à la position indiquée. Il est possible de donner un sprite au mur et d'indiquer si le mur
        /// possède un comportement de transparence, qui se déclenche lorsque des acteurs passent derrière le mur.
        /// </summary>
        /// <param name="position"> La position du mur </param>
        /// <param name="sprite"> La sprite du mur </param>
        /// <param name="transparency"> Indique si le mur peut devenir transparent </param>
        public Wall(Point position, Bitmap sprite = null, bool transparency = false) : base(position)
        {
            SetCollision(new ActorCollision(this, Hitbox, HitboxType.Unmovable, actor => { }));
            if (sprite != null)
            {
                var t = transparency ? new WallTransparency(this) : null;
                SpriteHandler = new SpriteHandler(this, new BasicSprite(sprite), t);
            }
        }
    }
}
