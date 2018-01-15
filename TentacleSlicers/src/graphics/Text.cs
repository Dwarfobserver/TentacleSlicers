using System.Drawing;
using System.Drawing.Text;
using TentacleSlicers.interfaces;
using Point = TentacleSlicers.general.Point;

namespace TentacleSlicers.graphics
{
    /// <summary>
    /// Dessine du texte en police Arial centré sur la position indiquée.
    /// </summary>
    public class Text : IDrawable
    {
        private const string FontPath = "../../resources/fonts/";
        private const int DefaultSize = 16;
        private static readonly Brush DefaultBrush = new SolidBrush(Color.White);
        private static FontFamily _fontFamily;

        private readonly string _text;
        private readonly Font _font;
        private readonly Brush _brush;

        /// <summary>
        /// Charge une police de caractères stockée dans le dossier fonts.
        /// </summary>
        private static void LoadFontFamily()
        {
            var myFonts = new PrivateFontCollection();
            myFonts.AddFontFile(FontPath + "Minecraft.ttf");
            myFonts.AddFontFile(FontPath + "Perfect DOS VGA 437.ttf");
            _fontFamily = myFonts.Families[0];
        }

        /// <summary>
        /// Crée l'objet avec le texte, la taille et la couleur (blanche par défaut) donnés.
        /// La police d'affichage est Arial.
        /// </summary>
        /// <param name="text"> Le texte à afficher </param>
        /// <param name="size"> La taille du texte </param>
        /// <param name="color"> La couleur du texte </param>
        public Text(string text, int size = DefaultSize, Color color = default(Color))
        {
            if (_fontFamily == null) LoadFontFamily();
            _text = text;
            // ReSharper disable once AssignNullToNotNullAttribute
            _font = new Font(_fontFamily, size);
            _brush = color == default(Color) ? DefaultBrush : new SolidBrush(color);
        }

        /// <summary>
        /// Affiche le texte avec comme centre le point donné.
        /// </summary>
        /// <param name="shift"> Le centre du texte </param>
        /// <param name="g"> L'objet permettant de dessiner </param>
        public void Draw(Point shift, Graphics g)
        {
            var dim = g.MeasureString(_text, _font);
            var x = (float) shift.X - dim.Width / 2;
            var y = (float) shift.Y - dim.Height / 2;
            g.DrawString(_text, _font, _brush, x, y);
        }
    }
}