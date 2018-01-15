using System.Drawing;
using Point = TentacleSlicers.general.Point;

namespace TentacleSlicers.graphics
{
    /// <summary>
    /// Correspond au Bitmap de System.Drawing, dessiné au milieu de la position passée en paramètre de Draw.
    /// </summary>
    public class BasicSprite : CustomSprite
    {
        private readonly Bitmap _sprite;

        /// <summary>
        /// Crée le sprite en enveloppant le bitmap indiqué, avec un décalage optionnel.
        /// </summary>
        /// <param name="bitmap"> Le bitmap enveloppé </param>
        /// <param name="shiftHeight"> Les pixels de décalages pour dessiner le sprite plus haut </param>
        public BasicSprite(Bitmap bitmap, int shiftHeight = 0) :
            base(shiftHeight)
        {
            _sprite = bitmap;
        }

        /// <summary>
        /// Crée le sprite en chargeant le bitmap au fichier indiqué, avec un décalage optionnel.
        /// </summary>
        /// <param name="bitmapPath"> Le bitmap à charger </param>
        /// <param name="shiftHeight"> Les pixels de décalages pour dessiner le sprite plus haut </param>
        public BasicSprite(string bitmapPath, int shiftHeight = 0) :
            base(shiftHeight)
        {
            _sprite = new Bitmap(bitmapPath);
        }

        /// <summary>
        /// Appelle la fonction DrawBitmap par laquelle passe tous les sprites du jeu.
        /// </summary>
        /// <param name="shift"> La position du sprite à dessiner </param>
        /// <param name="g"> L'objet permettant de dessiner </param>
        public override void Draw(Point shift, Graphics g)
        {
            DrawBitmap(_sprite, shift, g, new Rectangle(0, 0, _sprite.Width, _sprite.Height));
        }
    }
}