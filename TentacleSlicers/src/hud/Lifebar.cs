
using System.Drawing;
using TentacleSlicers.actors;
using TentacleSlicers.graphics;
using Point = TentacleSlicers.general.Point;

namespace TentacleSlicers.hud
{
    /// <summary>
    /// Indique la vie actuelle possédée par l'acteur associé.
    /// </summary>
    public class Lifebar : HudComponent
    {
        private static readonly Bitmap EmptyBar = new Bitmap(ImagePath + "Lifebar_empty.png");
        private static readonly Bitmap ColorBar = new Bitmap(ImagePath + "Lifebar_color.png");
        private const double PercentMerge = 0.04;

        public static readonly int Width = EmptyBar.Width;
        public static readonly int Height = EmptyBar.Height;

        private readonly ProgressingSprite _sprite;
        private readonly PlayerCharacter _owner;

        /// <summary>
        /// Crée la barre de vie qui représente l'état de santé du joueur donné.
        /// </summary>
        /// <param name="position"> Le centre de la barre de vie </param>
        /// <param name="owner"> Le joueur associé </param>
        /// <param name="inverted"> Indique si la barre de vie s'écoule de droite à gauche ou dans l'autre sens </param>
        public Lifebar(Point position, PlayerCharacter owner, bool inverted) : base(position)
        {
            _sprite = new ProgressingSprite(ColorBar, EmptyBar, PercentMerge) {Inverted = inverted};
            _owner = owner;
        }

        /// <summary>
        /// Affiche la barre de vie.
        /// </summary>
        /// <param name="shift"> La position de décalage, inutilisée </param>
        /// <param name="g"> L'objet permettant de dessiner </param>
        public override void Draw(Point shift, Graphics g)
        {
            _sprite.Draw(shift + Position, g);
        }

        /// <summary>
        /// Actualise le pourcentage de vie à afficher.
        /// </summary>
        /// <param name="ms"> le temps écoulé en millisecondes </param>
        public override void Tick(int ms)
        {
            _sprite.Percent = _owner.ActualLife() / _owner.LifeMax();
        }
    }
}