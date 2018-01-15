using System.Drawing;
using Point = TentacleSlicers.general.Point;

namespace TentacleSlicers.interfaces
{
    /// <summary>
    /// A chaque itération de la boucle principale, les classes qui implémentent cette interface doivent être dessinées
    /// en appelant leur fonction Draw, en passant un point de décalage pour les éléments qui se dessinent avec une
    /// position relative, et en passant l'objet permettant de dessiner.
    /// </summary>
    public interface IDrawable
    {
        void Draw(Point shift, Graphics g);
    }
}