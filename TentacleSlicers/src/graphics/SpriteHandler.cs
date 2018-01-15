
using System.Drawing;
using TentacleSlicers.actors;
using TentacleSlicers.interfaces;
using Point = TentacleSlicers.general.Point;

namespace TentacleSlicers.graphics
{
    /// <summary>
    /// Affiche un unique sprite qui peut avoir un comportement pour être transparent.
    /// </summary>
    public class SpriteHandler :IDrawable, ITickable
    {
        public const string ImagePath = "../../resources/images/";

        public TransparencyHandler Transparency { get; }
        protected CustomSprite CurrentSprite { get; set; }
        protected readonly Actor Owner;
        protected readonly CustomSprite DefaultSprite;

        /// <summary>
        /// Créer l'afficheur de sprite avec l'acteur associé, son sprite et un comportement de transparence optionnel.
        /// </summary>
        /// <param name="owner"> L'acteur associé au sprite </param>
        /// <param name="defaultSprite"> Le sprite </param>
        /// <param name="transparency"> Le paramètre d'opacité </param>
        public SpriteHandler(Actor owner, CustomSprite defaultSprite, TransparencyHandler transparency = null)
        {
            Owner = owner;
            DefaultSprite = defaultSprite;
            CurrentSprite = defaultSprite;
            Transparency = transparency;
        }

        /// <summary>
        /// Prend en compte l'opacité du comportement associé (maximale si il n'existe pas) pour dessiner le sprite à
        /// la position indiquée.
        /// </summary>
        /// <param name="shift"> L'extrémité basse du sprite </param>
        /// <param name="g"> l'objet permettant de dessiner </param>
        public void Draw(Point shift, Graphics g)
        {
            if (Transparency != null) CurrentSprite.Opacity = Transparency.Opacity;
            CurrentSprite.Draw(shift, g);
        }

        /// <summary>
        /// Actualise l'opacité si un comportement lui a été assossié.
        /// </summary>
        /// <param name="ms"> Le temps écoulé en millisecondes </param>
        public virtual void Tick(int ms)
        {
            Transparency?.Tick(ms);
        }
    }
}