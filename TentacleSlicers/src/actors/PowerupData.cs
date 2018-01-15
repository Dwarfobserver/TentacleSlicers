using System;
using TentacleSlicers.graphics;
using TentacleSlicers.spells;

namespace TentacleSlicers.actors
{
    /// <summary>
    /// Stocke les données permettant de créer un powerup, en lui donnant un sprite puis soit un effet, appliqué lors
    /// de son ramassage, soit un sort que le joueur va pouvoir assigner à l'une de ses touches.
    /// </summary>
    public class PowerupData
    {
        public CustomSprite Sprite { get; }
        public SpellData Spell { get; }
        public Action<PlayerCharacter> Effect { get; }

        /// <summary>
        /// Crée des données de powerup destinées à instancier un SpellPowerup.
        /// </summary>
        /// <param name="sprite"> L'image du powerup </param>
        /// <param name="spell"> Le sort donné au joueur </param>
        public PowerupData(CustomSprite sprite, SpellData spell)
        {
            Sprite = sprite;
            Spell = spell;
        }

        /// <summary>
        /// Crée des données de powerup destinées à instancier un PassivePowerup.
        /// </summary>
        /// <param name="sprite"> L'image du powerup </param>
        /// <param name="effect"> L'effet appliqué </param>
        public PowerupData(CustomSprite sprite, Action<PlayerCharacter> effect)
        {
            Sprite = sprite;
            Effect = effect;
        }
    }
}