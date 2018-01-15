
using System.Drawing;
using TentacleSlicers.collisions;
using TentacleSlicers.general;
using TentacleSlicers.graphics;
using TentacleSlicers.hud;
using Point = TentacleSlicers.general.Point;

namespace TentacleSlicers.actors
{
    /// <summary>
    /// Spécifie la classe ControlledActor pour créer des personnages vivants pouvant être blessés ou soignés, qui sont
    /// retirés du monde lorsque leur vie atteint 0.
    /// Leur barre de vie peut être affichée temporairement au-dessus d'eux lorsque leur niveau de vie est modifiée.
    /// </summary>
    public class LivingActor : ControlledActor
    {
        private static readonly Bitmap EmptyBar = new Bitmap(HudComponent.ImagePath + "MobLife_Empty.png");
        private static readonly Bitmap ColorBar = new Bitmap(HudComponent.ImagePath + "MobLife_Color.png");
        private const double PercentMerge = 0.01;
        private const int FadingMs = 1200;
        private const double LifebarWidth = 2; // Multipliés par la taille de la hitbox de l'acteur
        private const double LifebarHeight = 2;

        protected BoundedDouble Life { get; }
        private readonly SpriteHandler _lifeSprite;
        private readonly ProgressingSprite _lifeBar;

        /// <summary>
        /// Initialise la vie de l'acteur à sa valeur maximale donnée.
        /// Si showingLife est vrai, l'acteur affichera sa barre de vie au-dessus de lui un court instant lorsqu'elle
        /// est modifiée.
        /// </summary>
        /// <param name="position"> La position de l'acteur dans le monde </param>
        /// <param name="lifeMax"> La vie maximale de l'acteur </param>
        /// <param name="showingLife"> Indique si il faut afficher la vie de l'acteur. </param>
        public LivingActor(Point position, int lifeMax, bool showingLife = false) : base(position)
        {
            Life = new BoundedDouble(lifeMax);
            if (!showingLife) return;
            _lifeBar = new ProgressingSprite(ColorBar, EmptyBar, PercentMerge);
            _lifeSprite = new SpriteHandler(this, _lifeBar, new FadingTransparency(FadingMs));
        }

        /// <summary>
        /// Les sorts utilisent ce ratio pour soigner leur possesseur en fonction des dégâts infligés.
        /// Initialement, les acteurs vivants ont un ratio de vampirisme nul.
        /// </summary>
        /// <returns> Le pourcentage de vie rendu au possesseur par rapport aux dégâts infligés </returns>
        public virtual double VampirismRatio()
        {
            return 0;
        }

        /// <summary>
        /// Affiche l'acteur et sa barre de vie si son affichage a été demandé à la création de l'acteur.
        /// </summary>
        /// <param name="shift"> La position relative pour afficher l'acteur </param>
        /// <param name="g"> L'objet permettant de dessiner </param>
        public override void Draw(Point shift, Graphics g)
        {
            base.Draw(shift, g);
            if (_lifeSprite == null) return;
            if (Collision != null) shift = shift + new Point(0, - Collision.Hitbox.Height * LifebarHeight);
            _lifeSprite.Draw(shift + Position, g);
        }

        /// <summary>
        /// La fonction SetCollision() de la classe Actor est légèrement modifiée pour calculer ensuite l'ordonnée
        /// relative à l'acteur pour afficher sa barre de vie, si son affichage a été demandé à la création de
        /// l'acteur.
        /// </summary>
        /// <param name="actorCollision"></param>
        public sealed override void SetCollision(ActorCollision actorCollision)
        {
            base.SetCollision(actorCollision);
            if (_lifeSprite != null) _lifeBar.WidthRatio = actorCollision.Hitbox.Width * LifebarWidth / EmptyBar.Width;
        }

        /// <summary>
        /// Actualise la vie de l'acteur, ce qui peut le tuer si elle est amenée à zéro, et actualise sa barre de vie
        /// si elle avait été demandée à la création de l'acteur.
        /// </summary>
        /// <param name="ms"></param>
        public override void Tick(int ms)
        {
            base.Tick(ms);
            Life.Tick(ms);
            if (_lifeSprite != null)
            {
                _lifeBar.Percent = Life.Value / Life.ValueMax;
                _lifeSprite.Tick(ms);
            }
            if (Life.IsEmpty()) Kill();
        }

        public override void Revive()
        {
            Life.Value += Life.ValueMax;
            base.Revive();
        }

        /// <summary>
        /// Enlève des points de vie à l'acteur et le tue si sa vie est descendue à zéro.
        /// La barre de vie est rendue visible pour un court instant.
        /// </summary>
        /// <param name="value"> Le nombre de points de vie enlevés </param>
        public virtual void Damages(double value)
        {
            Life.Value -= value;
            if (Life.IsEmpty())
            {
                Kill();
            }
            else
            {
                ((FadingTransparency) _lifeSprite?.Transparency)?.Actualise();
            }
        }

        /// <summary>
        /// Rajoute des points de vie à l'acteur, sans que sa vie maximale ne soit dépassée.
        /// La barre de vie est rendue visible pour un court instant.
        /// </summary>
        /// <param name="value"> Le nombre de points de vie rendus </param>
        public virtual void Heals(double value)
        {
            Life.Value += value;
            ((FadingTransparency) _lifeSprite?.Transparency)?.Actualise();
        }

        /// <summary>
        /// Retourne le maximum de points de vie de l'acteur.
        /// </summary>
        /// <returns> Le maximum de points de vie de l'acteur </returns>
        public int LifeMax()
        {
            return (int) Life.ValueMax;
        }

        /// <summary>
        /// Retourne la vie actuelle de l'acteur.
        /// </summary>
        /// <returns> La vie actuelle de l'acteur </returns>
        public double ActualLife()
        {
            return Life.Value;
        }
    }
}