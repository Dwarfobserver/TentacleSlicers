
using TentacleSlicers.actors;
using TentacleSlicers.collisions;
using TentacleSlicers.general;
using TentacleSlicers.graphics;

namespace TentacleSlicers.customs
{
    /// <summary>
    /// Projectile qui explose au contact, ce qui inflige des dégâts aux ennemis aux alentours et les immobilise
    /// pendant un court instant.
    /// </summary>
    public class Bomb : Missile
    {
        private static readonly Rectangle Hitbox = new Rectangle(30, 30);
        private static readonly Circle ExplosionHitbox = new Circle(120);
        private const int Range = 700;
        private const int InitialSpeed = 250;
        private const int InitialDamages = 90;

        private readonly LivingActor _owner;
        private bool _hasExplosed;

        /// <summary>
        /// Crée la bombe depuis son lanceur, et indique à sa collision de déclencher l'explosion lorsque la collision
        /// se déclenche avec un objet tangible.
        /// </summary>
        /// <param name="owner"> Celui qui a lancé la bombe </param>
        /// <param name="target"> La cible de la bombe </param>
        public Bomb(ControlledActor owner, Point target) : base(owner, target, InitialSpeed, (double) Range / InitialSpeed)
        {
            _owner = (LivingActor) owner;
            SpriteHandler = new AnimationHandler(this, Sprites.Bomb);
            SetCollision(new ActorCollision(this, Hitbox, HitboxType.Immaterial, actor =>
            {
                if (!FriendWith(actor) && actor.Collision.HitboxType != HitboxType.Immaterial)
                {
                    Kill();
                    Explodes();
                }
            }));
            ResiduData = new ResiduData(this, AnimationType.Death, false);
            _hasExplosed = false;
        }

        /// <summary>
        /// Actualise la bombe et la fait détoner si elle a atteint sa fin de vie.
        /// </summary>
        /// <param name="ms"> Le temps écoulé en millisecondes </param>
        public override void Tick(int ms)
        {
            base.Tick(ms);
            if (IsDead() && !_hasExplosed)
            {
                Explodes();
            }
        }

        /// <summary>
        /// Fait exploser la bombe, ce qui immobilise et blesse les ennemis proches.
        /// </summary>
        private void Explodes()
        {
            _hasExplosed = true;
            var damages = InitialDamages * _owner.SpellPowerRatio();
            var actors = CollisionFunctions.GetCollidingActors(Position, ExplosionHitbox);
            foreach (var actor in actors)
            {
                var livingActor = actor as LivingActor;
                if (EnnemyWith(actor) && livingActor != null)
                {
                    livingActor.Damages(damages);
                    _owner.Heals(damages * _owner.VampirismRatio());
                    livingActor.LockMoves();
                    livingActor.CreateState(States.BombDebuff);
                }
            }
        }
    }
}