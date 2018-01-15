
using TentacleSlicers.collisions;
using TentacleSlicers.general;
using TentacleSlicers.graphics;

namespace TentacleSlicers.actors
{
    /// <summary>
    /// Spécifie la class Actor pour créer des projectiles dont la direction et la vitesse sont fixées à l'avance et
    /// dispose d'une durée de vie au-delà de laquelle le projectile est supprimé.
    /// </summary>
    public class Missile : Actor
    {
        private readonly BoundedDouble _lifeTime;

        /// <summary>
        /// Crée un projectile avec les caractéristiques spécifiées.
        /// Il est crée avec la même faction que l'acteur qui l'a lancé et depuis l'extrémité de cet acteur
        /// correspondant à son orientation actuelle.
        /// Le projectile dispose par défaut d'un résidu prolongeant sa trajectoire avec son animation de mort
        /// lorsqu'il atteint sa durée de vie.
        /// </summary>
        /// <param name="owner"> L'acteur à l'origine du projectile </param>
        /// <param name="target"> La cible, qui va définir l'orientation du projectile </param>
        /// <param name="speed"> La vitesse en pixels par seconde du projectile </param>
        /// <param name="lifeTime"> La durée de vie du projectile en secondes </param>
        public Missile(Actor owner, Point target, int speed, double lifeTime) : base(owner.Muzzle())
        {
            _lifeTime = new BoundedDouble(lifeTime, -1);
            Speed = speed;
            Orientation = owner.Position.GetOrientation(target);
            Faction = owner.Faction;
            ResiduData = new ResiduData(this, AnimationType.Death, true);
        }

        /// <summary>
        /// Crée un projectile avec toutes les données fournies par la classe MissileData.
        /// Il est crée avec la même faction que l'acteur qui l'a lancé et depuis l'extrémité de cet acteur
        /// correspondant à son orientation actuelle.
        /// Le projectile d'un résidu prolongeant sa trajectoire avec son animation de mort lorsqu'il atteint sa durée
        /// de vie.
        /// Il possède une hitbox immatérielle et va mourir lorsqu'il rencontre un acteur (avec une hitobx non
        /// immatérielle )avec lequel il n'est pas ami et créer un résidu avec l'animation de type Explodes, si elle
        /// est disponible.
        /// Si cet acteur est un ennemi vivant, les dégâts indiqués dans MissileData lui seront infligés, modifiés par
        /// le SpellPowerRatio() de l'acteur à l'origine du projecile. De plus, si le lanceur du pojectile est vivant
        /// et possède un ratio de vampirisme non nul, il sera soigné d'après ce ratio en fonction des dégâts infligés.
        /// </summary>
        /// <param name="owner"> L'acteur à l'origine du projectile </param>
        /// <param name="target"> La cible, qui va définir l'orientation du projectile </param>
        /// <param name="data"> Les différents paramètres du missile </param>
        public Missile(Actor owner, Point target, MissileData data) : base(owner.Muzzle())
        {
            _lifeTime = new BoundedDouble(data.Range / (double) data.Speed, -1);
            Speed = data.Speed;
            Orientation = owner.Position.GetOrientation(target);
            SpeedVector = Orientation * Speed;
            Faction = owner.Faction;

            ResiduData = new ResiduData(this, AnimationType.Death, true);

            SpriteHandler = new AnimationHandler(this, data.Animations);

            var damages = data.Damage;
            var livingOwner = owner as LivingActor;
            var heal = livingOwner?.VampirismRatio() ?? 0.0;

            // Création de la hitbox immatérielle en prenant en compte SpellPowerRatio() et VampirismRatio()
            SetCollision(new ActorCollision(this, new Rectangle(data.Size, data.Size), HitboxType.Immaterial,
                actor =>
                {
                    if (actor == owner || actor.Collision.HitboxType == HitboxType.Immaterial) return;
                    var player = actor as LivingActor;
                    if (EnnemyWith(actor) && player != null)
                    {
                        player.Damages(damages * owner.SpellPowerRatio());
                        if (heal != 0.0) livingOwner?.Heals(damages * heal);
                    }
                    if (!FriendWith(actor))
                    {
                        ResiduData = new ResiduData(this, AnimationType.Explodes, false);
                        Kill();
                    }
                })
            );
        }

        /// <summary>
        /// Verrouille la fonction SetCollision, car elle n'est pas destinée à être réécrite.
        /// </summary>
        /// <param name="actorCollision"> La nouvelle collision de l'acteur </param>
        public sealed override void SetCollision(ActorCollision actorCollision)
        {
            base.SetCollision(actorCollision);
        }

        /// <summary>
        /// Actualise l'acteur et le déplace suivant sa vitesse constante et le temps écoulé.
        /// Ensuite, actualise sa durée de vie et le retire du monde si celle-ci est écoulée, en créant un Residu si le
        /// projectile possède une animation de type Death.
        /// </summary>
        /// <param name="ms"> Le temps écoulé en millisecondes </param>
        public override void Tick(int ms)
        {
            base.Tick(ms);
            SpeedVector = Orientation * Speed;
            Move(SpeedVector * ((double)ms / 1000));
            _lifeTime.Tick(ms);
            if (_lifeTime.IsEmpty()) Kill();
        }
    }
}