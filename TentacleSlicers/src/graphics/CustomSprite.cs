using System.Drawing;
using System.Drawing.Imaging;
using TentacleSlicers.interfaces;
using Point = TentacleSlicers.general.Point;

namespace TentacleSlicers.graphics
{
    /// <summary>
    /// Encapsule les images a dessiner pour centraliser l'affichage de tous les sprites dans la fonction DrawBitmap.
    /// La taille relative et l'opacité de ces sprites peuvent être modifiés de l'extérieur.
    /// </summary>
    public abstract class CustomSprite : IDrawable
    {
        public int HeightShift { get; }
        public float Opacity;
        public double WidthRatio;
        public double HeightRatio;

        /// <summary>
        /// Initialise les paramètres du sprite avec la hauteur supplémentaire indiquée.
        /// Par défaut, son opacité est maximale et sa taille n'est pas modifiée.
        /// </summary>
        /// <param name="heightShift"> Le décalage en pixels vers le haut </param>
        protected CustomSprite(int heightShift)
        {
            HeightShift = heightShift;
            Opacity = 1;
            WidthRatio = 1;
            HeightRatio = 1;
        }

        /// <summary>
        /// Si l'opacité n'est pas nulle, dessine le sprite dans le rectangle indiqué, modifié par la taille relative
        /// et l'opacité courante. Le sprite est dessiné au-dessus du point donné.
        /// </summary>
        /// <param name="bitmap"> Le sprite à dessiner </param>
        /// <param name="shift"> La position du sprite </param>
        /// <param name="g"> L'objet permettant de dessiner </param>
        /// <param name="r"> Le rectangle dans lequel est dessiné le sprite </param>
        protected void DrawBitmap(Bitmap bitmap, Point shift, Graphics g, Rectangle r)
        {
            if (Opacity == 0) return;
            r = new Rectangle((int) (r.X * WidthRatio), (int) (r.Y * HeightRatio),
                              (int) (r.Width * WidthRatio), (int) (r.Height * HeightRatio));
            shift = shift - new Point(bitmap.Width * WidthRatio / 2, bitmap.Height * HeightRatio - HeightShift);
            var destRect = new Rectangle((int) shift.X + r.X, (int) shift.Y + r.Y, r.Width, r.Height);
            if (Opacity == 1)
            {
                g.DrawImage(bitmap, destRect, r, GraphicsUnit.Pixel);
            }
            else
            {
                var matrix = new ColorMatrix {Matrix33 = Opacity};
                var attributes = new ImageAttributes();
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                g.DrawImage(bitmap, destRect, r.X, r.Y, r.Width, r.Height, GraphicsUnit.Pixel, attributes);
            }
        }

        /// <summary>
        /// Dessine le sprite en appelant dans les classes filles la fonction DrawBitmap.
        /// </summary>
        /// <param name="shift"> La position du sprite </param>
        /// <param name="g"> L'objet permettant de dessiner </param>
        public abstract void Draw(Point shift, Graphics g);
    }
}