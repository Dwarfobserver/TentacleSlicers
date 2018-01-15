using TentacleSlicers.collisions;
using TentacleSlicers.graphics;
using Point = TentacleSlicers.general.Point;
using Rectangle = TentacleSlicers.collisions.Rectangle;

namespace TentacleSlicers.actors
{
    /// <summary>
    /// Représente un utilitaire à ramasser pour les joueurs. Il est immobile et disparaît si le joueur passe dessus.
    /// Il peut appliquer un effet au joueur lorsqu'il est ramassé ou être ensuite assigné à un emplacement de sort
    /// pour que le joueur aie une nouvelle compétence.
    /// </summary>
    public abstract class Powerup : Actor
    {
        public static readonly string ImagePath = SpriteHandler.ImagePath + "powerups/";

        private static readonly Rectangle Hitbox = new Rectangle(30, 30);

        /// <summary>
        /// Fabrique un powerup à partir d'une position et d'un objet stockant les données du powerup à créer.
        /// Selon ces données, instancie un SpellPowerup ou un PassivePowerup.
        /// </summary>
        /// <param name="position"> La position du powerup dans le monde </param>
        /// <param name="data"> Les données du powerup </param>
        /// <returns> Le powerup instancié </returns>
        public static Powerup Create(Point position, PowerupData data)
        {
            if (data.Spell != null)
            {
                return new SpellPowerup(position, data);
            }
            return new PassivePowerup(position, data);
        }

        /// <summary>
        /// Initialise le powerup avec sa position et son sprite associé.
        /// Lui donne également sa collision lui permettant d'être ramassé.
        /// </summary>
        /// <param name="position"> la position du powerup </param>
        /// <param name="sprite"> Le sprite du powerup </param>
        protected Powerup(Point position, CustomSprite sprite) : base(position)
        {
            SetCollision(new ActorCollision(this, Hitbox, HitboxType.Immaterial, actor =>
            {
                var player = actor as PlayerCharacter;
                if (player != null && player.LootPowerup(this)) Kill();
            }));
            SpriteHandler = new SpriteHandler(this, sprite);
        }

        /// <summary>
        /// Scèle la fonction SetCollision parce qu'elle n'est pas destinée à être reécrite.
        /// </summary>
        /// <param name="collision"> La collision assignée à l'acteur </param>
        public sealed override void SetCollision(ActorCollision collision)
        {
            base.SetCollision(collision);
        }

        /// <summary>
        /// Applique les effets du powerup sur son possesseur, si besoin à l'aide d'un paramètre, comme le numéro de
        /// compétence auquel le sort a été associé.
        /// </summary>
        /// <param name="owner"> Le nouveau possesseur du powerup </param>
        /// <param name="arg"> L'argument supplémentaire </param>
        public abstract void Apply(PlayerCharacter owner, int arg);

        /// <summary>
        /// Indique si le powerup est consommé directement après être ramassé. Sinon, il est utilisé à la place de la
        /// prochaine utilisation d'un sort.
        /// </summary>
        /// <returns> Vrai si le powerup est consommé directement après être ramassé </returns>
        public abstract bool IsAutomaticallyConsumed();
    }
}