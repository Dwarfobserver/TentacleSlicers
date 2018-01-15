using TentacleSlicers.general;
using TentacleSlicers.spells;

namespace TentacleSlicers.actors
{
    /// <summary>
    /// Un type de powerup qui va permettre au joueur qui l'a ramassé de l'assigner à une touche de sort, ce qui va lui
    /// assigner à cet emplacement une nouvelle compétence.
    /// </summary>
    public class SpellPowerup : Powerup
    {
        private readonly SpellData _spellData;

        /// <summary>
        /// Crée le powerup avec la position et les données indiquées.
        /// Appelé par la méthode statique Powerup.Create.
        /// </summary>
        /// <param name="position"> La position du powerup </param>
        /// <param name="data"> Les données du powerup </param>
        public SpellPowerup(Point position, PowerupData data) : base(position, data.Sprite)
        {
            _spellData = data.Spell;
        }

        /// <summary>
        /// L'effet appliqué par le SpellPowerup est la création d'un sort pour son possesseur avec le numéro indiqué.
        /// </summary>
        /// <param name="owner"> Le possesseur du powerup </param>
        /// <param name="arg"> Le numéro du sort </param>
        public override void Apply(PlayerCharacter owner, int arg)
        {
            owner.CreateSpell(arg, _spellData);
        }

        /// <summary>
        /// Le powerup est consommé lorsqu'il est assigné à une touche et non directement à son ramassage.
        /// </summary>
        /// <returns> Vrai si le powerup est consommé à son ramassage </returns>
        public override bool IsAutomaticallyConsumed()
        {
            return false;
        }
    }
}