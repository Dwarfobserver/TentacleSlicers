
using TentacleSlicers.actors;
using TentacleSlicers.collisions;
using TentacleSlicers.general;
using TentacleSlicers.graphics;

namespace TentacleSlicers.customs
{
    /// <summary>
    /// Spécifie la classe de personnages jouables en donnant des animations et une hitbox au personnage.
    /// De plus, crée une tombe lors de la mort du personnage.
    /// </summary>
    public sealed class Knight : PlayerCharacter
    {
        public const int InitialSpeed = 260;
        private static readonly Rectangle Hitbox = new Rectangle(40, 40);
        private const int InitialPv = 100;

        private readonly int _num;
        private bool _tombCreated;

        /// <summary>
        /// Initialise le personnage joueur avec sa hitbox, son attaque au corps-à-corps, (associée au numéro 0) sa
        /// vitesse, ses points de vie et ses animations.
        /// Il est possible de créer le personnage en lui donnant ses statistiques.
        /// </summary>
        /// <param name="position"> La position de départ du joueur </param>
        /// <param name="num"> Le numéro du joueur, utilis épour déterminer sa couleur </param>
        /// <param name="stats"> Les statistiques optionnelles du joueur </param>
        public Knight(Point position, int num, PlayerStats stats = null) : base(position, InitialPv, stats)
        {
            _num = num;
            CreateSpell(0, Spells.PlayerAttack());
            Speed = InitialSpeed;
            SpriteHandler = num == 0 ? new AnimationHandler(this, Sprites.BlueKnight) :
                                       new AnimationHandler(this, Sprites.PinkKnight);
            SetCollision(new ActorCollision(this, Hitbox, HitboxType.Player));
            ResiduData = null;
        }

        /// <summary>
        /// Lorsque le personnage est tué, il laisse une tombe à son emplacmeent afin de pouvoir le faire revivre.
        /// </summary>
        /// <param name="value"> Dégâts entrants </param>
        public override void Damages(double value)
        {
            base.Damages(value);
            if (IsDead() && !_tombCreated)
            {
                new KnightTomb(this, _num);
                _tombCreated = true;
            }
        }

        /// <summary>
        /// Lors de la résurrection du personnage, son booléen permet à nouveau la création d'une tombe.
        /// </summary>
        public override void Revive()
        {
            base.Revive();
            _tombCreated = false;
        }
    }
}