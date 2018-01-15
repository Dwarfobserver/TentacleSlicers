
using System;
using System.Collections.Generic;
using TentacleSlicers.actors;
using TentacleSlicers.interfaces;

namespace TentacleSlicers.spells
{
    /// <summary>
    /// Gère d'un acteur et permet de les créer, à partir de SpellData et en l'associant à un numéro.
    /// </summary>
    public class SpellHandler : ITickable
    {
        private readonly ControlledActor _owner;
        private readonly Dictionary<int, Spell> _spells;

        /// <summary>
        /// Initialise le Spellhandler sans sorts.
        /// </summary>
        /// <param name="owner"> Le possesseur des sorts </param>
        public SpellHandler(ControlledActor owner)
        {
            _owner = owner;
            _spells = new Dictionary<int, Spell>();
        }

        /// <summary>
        /// Actualise les sorts.
        /// </summary>
        /// <param name="ms"> Le temps écoulé en millisecondes </param>
        public void Tick(int ms)
        {
            foreach (var spell in _spells)
            {
                spell.Value.Tick(ms);
            }
        }

        /// <summary>
        /// Crée un sort pour le possesseur du Spellhandler, avec le numéro associé.
        /// </summary>
        /// <param name="numSpell"> Le numéro du sort </param>
        /// <param name="data"> Les données du sort </param>
        public void CreateSpell(int numSpell, SpellData data)
        {
            _spells[numSpell] = new Spell(_owner, data);
        }

        /// <summary>
        /// Crée un sort pour le joueur du Spellhandler, avec le numéro associé et les statistiques du joueur.
        /// Celles-ci permettent notemment de réduire le temps de rechargement du sort.
        /// </summary>
        /// <param name="numSpell"> Le numéro du sort </param>
        /// <param name="data"> Les données du sort </param>
        /// <param name="stats"> les statistiques du joueur </param>
        public void CreateSpell(int numSpell, SpellData data, PlayerStats stats)
        {
            _spells[numSpell] = new Spell(_owner, data, stats);
        }

        /// <summary>
        /// Appelé lorsque le joueur modifie sa caractéristique CooldownReduction pour mettre à jour le temps de
        /// rechargement de ses sorts.
        /// </summary>
        /// <param name="stats"> Les statistiques du joueur </param>
        public void ActualiseSpells(PlayerStats stats)
        {
            foreach (var spell in _spells)
            {
                spell.Value.ActualiseCooldown(stats);
            }
        }

        /// <summary>
        /// Retourne le sort qui fut assigné au numéro indiqué lors de sa création.
        /// Si il n'existe pas, renvoie null.
        /// </summary>
        /// <param name="numSpell"> Le numéro du sort </param>
        /// <returns> Le sort correspondant au numéro </returns>
        public Spell GetSpell(int numSpell)
        {
            return _spells.ContainsKey(numSpell) ? _spells[numSpell] : null;
        }

        /// <summary>
        /// Retourne le nombre de sorts du possesseur.
        /// </summary>
        /// <returns> Le nombre de sorts </returns>
        public int GetNbSpells()
        {
            return _spells.Count;
        }
    }
}