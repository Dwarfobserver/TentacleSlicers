
using System.Drawing;
using Point = TentacleSlicers.general.Point;

namespace TentacleSlicers.graphics
{
    /// <summary>
    /// Dessine un sprite d'un de ses côtés vers le côté opposé, selon un pourcentage de progression : à 0, le sprite
    /// n'est pas dessiné et à 1, le sprite est totalement dessiné.
    /// Il peut être associé à un sprite dessiné derrière lui et qui est toujours dessiné en entier.
    /// Les booléens Vertical et Inverted permettent de changer les côtés de départ et fin pour dessiner le sprite.
    /// </summary>
    public class ProgressingSprite : CustomSprite
    {
        private readonly Bitmap _backgroundSprite;
        private readonly Bitmap _foregroundSprite;
        private readonly double _percentMerge;
        public bool Vertical;
        public bool Inverted;
        public double Percent;

        /// <summary>
        /// Crée le ProgressingSprite avec le ou les sprites donnés, le décalage et la marge qui dessine une partie des
        /// bordures du sprite, quel que soit son pourcentage associé.
        /// Par défaut, le sprite est dessiné de gauche à droite et est associé au pourcentage maximal (1).
        /// </summary>
        /// <param name="foregroundSprite"> Le sprite dessiné en fonction du pourcentage </param>
        /// <param name="backgroundSprite"> Le sprite optionnel dessiné derrière </param>
        /// <param name="percentMerge"> Le pourcentage du sprite toujours dessiné </param>
        /// <param name="shiftHeight"> Le décalage </param>
        public ProgressingSprite(Bitmap foregroundSprite, Bitmap backgroundSprite = null, double percentMerge = 0,
                                 int shiftHeight = 0) :
            base(shiftHeight)
        {
            _backgroundSprite = backgroundSprite;
            _foregroundSprite = foregroundSprite;
            _percentMerge = percentMerge;
            Vertical = false;
            Inverted = false;
            Percent = 1;
        }

        /// <summary>
        /// Dessine le sprite en fond si il existe, puis le sprite en fonction du pourcentage dans le rectangle calculé
        /// selon le pourcentage courant et les booléens Vertical et Inverted, qui déterminent dans quel sens se
        /// dessine le sprite.
        /// </summary>
        /// <param name="shift"> La position du sprite </param>
        /// <param name="g"> L'objet graphique permettant de dessiner </param>
        public override void Draw(Point shift, Graphics g)
        {
            var percent = Percent * (1 - _percentMerge) + _percentMerge / 2;
            int min, max, x1, y1, x2, y2;
            var limit = Vertical ? _foregroundSprite.Height : _foregroundSprite.Width;
            // Détermine le sens pour dessiner le sprite
            if (!Inverted)
            {
                min = 0;
                max = (int) (percent * limit);
            }
            else // TODO Problème de placement si le sprite est redimensionné
            {
                min = (int) ((1 - percent) * limit);
                max = limit;
            }
            // Entre le haut et le bas
            if (Vertical)
            {
                x1 = 0;
                y1 = min;
                x2 = _foregroundSprite.Width;
                y2 = max;
            }
            // Entre la droite et la gauche
            else
            {
                x1 = min;
                y1 = 0;
                x2 = max;
                y2 = _foregroundSprite.Height;
            }

            if (_backgroundSprite != null) DrawBitmap(_backgroundSprite, shift, g,
                new Rectangle(0, 0, _foregroundSprite.Width, _foregroundSprite.Height));
            DrawBitmap(_foregroundSprite, shift, g, new Rectangle(x1, y1, x2, y2));
        }

    }
}