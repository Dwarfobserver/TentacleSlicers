
using TentacleSlicers.actors;
using TentacleSlicers.collisions;
using TentacleSlicers.general;
using TentacleSlicers.graphics;
using TentacleSlicers.maps;

namespace TentacleSlicers.customs
{
    /// <summary>
    /// Représente le boss d'un niveau. Il possède beaucoup de points de vie et des attaques très puissantes.
    /// </summary>
    public class CthulhuGhost : Mob
    {
        private static readonly Rectangle Hitbox = new Rectangle(80, 80);
        private const int InitialLife = 400;
        private const int InitialSpeed = 75;
        private const int VoidballsBeginning = 2;
        private const int StormsBeginning = 5;
        private const int ChainsBeginning = 14;

        public bool VoidballsAvailable;
        public bool StormsAvailable;
        public int ChainsStacks;
        private double _secondsElapsed;

        /// <summary>
        /// Crée le boss avec ses points de vie, ses animations et sa hitbox immatérielle.
        /// Il commence avec un sort qui invoque régulièrement des météores.
        /// </summary>
        /// <param name="position"> La position de départ du boss </param>
        public CthulhuGhost(Point position) :
            base(position, InitialLife * (World.GetWorld().GetPlayers().Count + 1) / 2, true)
        {
            SpriteHandler = new AnimationHandler(this, Sprites.CthulhuGhost);
            SetCollision(new ActorCollision(this, Hitbox, HitboxType.Heavy));
            Speed = InitialSpeed;
            Life.Regeneration = 5;
            _secondsElapsed = 0;
            StormsAvailable = false;
            ChainsStacks = -1;
            CreateSpell(0, Spells.CthulhuMeteor());
            Behavior.TargetNeedDirectSight = false;
        }

        /// <summary>
        /// Au fur à et mesure que le temps passe, le boss gagne en puissance.
        /// Au bout de 2 secondes, il gagne des projectiles lancés en arc de cercle.
        /// Au bout de 7 secondes, il invoque le tonerre là où les joueurs se dirigent.
        /// Au bout de 14 secondes, il ralentit de plus en plus tous les joueurs.
        /// </summary>
        /// <param name="ms"> Le temps écoulé en millisecondes </param>
        public override void Tick(int ms)
        {
            base.Tick(ms);
            _secondsElapsed += ms / 1000.0;
            if (_secondsElapsed > VoidballsBeginning && !VoidballsAvailable)
            {
                CreateSpell(1, Spells.CthulhuVoidball());
                VoidballsAvailable = true;
            }
            else if (_secondsElapsed > StormsBeginning && !StormsAvailable)
            {
                CreateSpell(2, Spells.CthulhuLightning());
                StormsAvailable = true;
            }
            else if (_secondsElapsed > ChainsBeginning && ChainsStacks == -1)
            {
                CreateSpell(3, Spells.CthulhuChains());
                ++ChainsStacks;
            }
        }

        /// <summary>
        /// Inflige les dégâts au boss et augmente le score des joueurs si il meurt sute à ces dégâts.
        /// </summary>
        /// <param name="value"> les dégâts subis </param>
        public override void Damages(double value)
        {
            base.Damages(value);
            if (IsDead())
            {
                World.GetWorld().AddScore(20 * World.GetWorld().GetPlayers().Count);
            }
        }
    }
}