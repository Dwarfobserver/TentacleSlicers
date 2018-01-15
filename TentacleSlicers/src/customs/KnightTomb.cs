using TentacleSlicers.actors;
using TentacleSlicers.graphics;
using TentacleSlicers.maps;

namespace TentacleSlicers.customs
{
    /// <summary>
    /// Définit la tombe d'un joueur. Elle possède une barre de vie qui se vide lorsqu'un joueur vivant se situe à
    /// proxitimté. Lorsqu'elle est vidée, la tombe fait revivre le joueur mort. Si aucun joueur n'est près de la
    /// tombe, elle se régénère.
    /// </summary>
    public sealed class KnightTomb : LivingActor
    {
        private const int Range = 250;
        private const double PvRegenerated = 0.5;
        private const int LoosingLoading = 10;
        private const int Loading = 40;

        private readonly Knight _owner;

        /// <summary>
        /// Crée la tombe du joueur mort, sans hitbox et de faction neutre, avec son animation.
        /// Par défaut, la vie de la tombe se régénère.
        /// </summary>
        /// <param name="owner"> Le joueur mort </param>
        /// <param name="num"> Le numéro du joueur, pour établir sa couleur </param>
        public KnightTomb(Knight owner, int num) : base(owner.Position, 100, true)
        {
            Faction = Faction.Neutral;
            _owner = owner;
            Life.Regeneration = LoosingLoading;
            SpriteHandler = num == 0 ? new AnimationHandler(this, Sprites.BlueKnightTomb) :
                                       new AnimationHandler(this, Sprites.PinkKnightTomb);
        }

        /// <summary>
        /// Si la barre de vie de la tombe est écoulée, alors le joueur mort est ressuscité.
        /// </summary>
        /// <param name="value"> les point sde vie enlevés à la tombe </param>
        public override void Damages(double value)
        {
            base.Damages(value);
            if (IsDead())
            {
                _owner.Revive();
                _owner.Damages(_owner.LifeMax() * PvRegenerated);
            }
        }

        /// <summary>
        /// Vérifie si des joueurs vivants sont à proximité de la tombe et la régénère si c'est le cas.
        /// </summary>
        /// <param name="ms"> Le temps écoulé en millisecondes </param>
        public override void Tick(int ms)
        {
            base.Tick(ms);

            var players = World.GetWorld().GetAlivePlayers();
            var i = 0;
            var nearPlayer = false;
            while (i < players.Count && !nearPlayer)
            {
                // Joueur vivant à proximité
                if (players[i].Position.Length(Position) < Range)
                {
                    nearPlayer = true;
                }
                ++i;
            }
            if (nearPlayer)
            {
                Damages((Loading + LoosingLoading) * ms / 1000.0);
            }
        }
    }
}