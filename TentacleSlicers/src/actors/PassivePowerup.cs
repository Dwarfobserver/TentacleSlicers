using System;
using TentacleSlicers.general;

namespace TentacleSlicers.actors
{
    /// <summary>
    /// Un type de powerup qui aplique un effet au joueur qui le ramasse. Il est donc directement consommé.
    /// Il efface les états du joueur qui le prend.
    /// </summary>
    public class PassivePowerup : Powerup
    {
        private readonly Action<PlayerCharacter> _effect;

        /// <summary>
        /// Crée le powerup avec la position et les données indiquées.
        /// Appelé par la méthode statique Powerup.Create.
        /// </summary>
        /// <param name="position"> La position du powerup </param>
        /// <param name="data"> Les données du powerup </param>
        public PassivePowerup(Point position, PowerupData data) : base(position, data.Sprite)
        {
            _effect = data.Effect;
        }

        /// <summary>
        /// L'effet appliqué par le PassivePowerup est un effet concernant son possesseur.
        /// </summary>
        /// <param name="owner"> Le possesseur du powerup </param>
        /// <param name="arg"> L'argument optionnel, inutilisé par ce type de powerup </param>
        public override void Apply(PlayerCharacter owner, int arg)
        {
            owner.CleanStates();
            _effect.Invoke(owner);
        }

        /// <summary>
        /// Le powerup est consommé dès son ramassage.
        /// </summary>
        /// <returns> Vrai si le powerup est consommé à son ramassage </returns>
        public override bool IsAutomaticallyConsumed()
        {
            return true;
        }
    }
}