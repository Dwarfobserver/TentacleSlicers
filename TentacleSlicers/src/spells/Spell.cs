
using System;
using System.Drawing;
using TentacleSlicers.actors;
using TentacleSlicers.general;
using TentacleSlicers.graphics;
using TentacleSlicers.interfaces;
using TentacleSlicers.states;
using Point = TentacleSlicers.general.Point;

namespace TentacleSlicers.spells
{
    /// <summary>
    /// Un sort (ou compétence) peut être utilisée par un acteur, qui va alors l'incanter pour déclencher son effet.
    /// Un sort peut posséder beaucoup de caractéristiques, données par la classe SpellData qui lui est fournie lors de
    /// son instanciation.
    /// </summary>
    public class Spell : ITickable
    {
        public static readonly string ImagePath = SpriteHandler.ImagePath + "spells/";
        public static readonly string IconPath = SpriteHandler.ImagePath + "spells/icons/";

        private const int DefaultMs = -1;

        public int Range => _spellData.Range;
        public Bitmap Icon => _spellData.Icon;
        public long Id => _spellData.Id;
        public int MaxChares => _spellData.NbCharges;

        public int CurrentCharges { get; private set; }

        private readonly SpellData _spellData;
        private readonly ControlledActor _owner;
        private readonly BoundedDouble _cooldown;
        protected StateData StateData;
        private int _currentMs;
        private Point _target;

        /// <summary>
        /// Crée le sort en lui assignant son possesseur et les données indiquées.
        /// Le sort est crée avec une charge.
        /// </summary>
        /// <param name="owner"> Le possesseur du sort </param>
        /// <param name="spellData"> Les données du sort </param>
        public Spell(ControlledActor owner, SpellData spellData)
        {
            _owner = owner;
            _spellData = spellData;
            StateData = new StateData(_spellData.CastTimeMs, null, (actor, arg) =>
            {
                actor.UnlockCasts();
                if (!_spellData.CanMove) actor.UnlockMoves();
            });
            _currentMs = DefaultMs;
            _cooldown = new BoundedDouble(_spellData.Cooldown, -1) {Value = 0};
        }

        /// <summary>
        /// Crée le sort en lui assignant son joueur et les données indiquées.
        /// Le sort est crée avec une charge.
        /// La statistique de CooldownReduction du joueur réduit le temps de recharge du sort.
        /// </summary>
        /// <param name="owner"> Le possesseur du sort </param>
        /// <param name="spellData"> Les données du sort </param>
        /// <param name="stats"> Les statistiques du joueur </param>
        public Spell(ControlledActor owner, SpellData spellData, PlayerStats stats)
        {
            _owner = owner;
            _spellData = spellData;
            StateData = new StateData(_spellData.CastTimeMs, null, (actor, arg) =>
            {
                actor.UnlockCasts();
                if (!_spellData.CanMove) actor.UnlockMoves();
            });
            _currentMs = DefaultMs;
            _cooldown = new BoundedDouble(_spellData.Cooldown * stats.CooldownRatio(), -1) {Value = 0};
        }

        /// <summary>
        /// Fait progresser le chargement du sort (si il n'a pas atteint son nombre de charges maximal) et son
        /// incantation si le possesseur du sort est en train de le lancer. Si le temps d'incantation écoulé est
        /// suffisant, déclenche l'effet du sort.
        /// </summary>
        /// <param name="ms"> Le temps écoule en millisecondes </param>
        public void Tick(int ms)
        {
            _cooldown.Tick(ms);
            if (_cooldown.IsEmpty() && CurrentCharges < _spellData.NbCharges)
            {
                CurrentCharges++;
                if (CurrentCharges < _spellData.NbCharges)
                {
                    _cooldown.Value = _cooldown.ValueMax;
                }
            }
            if (_currentMs == DefaultMs) return;
            // Si le sort est en cours d'incantation
            _currentMs += ms;
            if (_currentMs <= _spellData.CastTimeMs * _spellData.PercentCast) return;
            if (CurrentCharges == _spellData.NbCharges) _cooldown.Value = _cooldown.ValueMax;
            CurrentCharges--;
            _spellData.Effect.Invoke(_owner, _target);
            _currentMs = DefaultMs;
        }

        /// <summary>
        /// Met à jour le temps de rechargement du jour avec les statistiques de son joueur.
        /// </summary>
        /// <param name="stats"> Les statistiques du joueur </param>
        public void ActualiseCooldown(PlayerStats stats)
        {
            var currentRatio = _cooldown.Value / _cooldown.ValueMax;
            _cooldown.ValueMax = _spellData.Cooldown * stats.CooldownRatio();
            _cooldown.Value = _cooldown.ValueMax * currentRatio;
        }

        /// <summary>
        /// Indique si le sort peut être lancé par son possesseur.
        /// </summary>
        /// <param name="target"> La cible du sort </param>
        /// <returns> Vrai si le sort peut être lancé </returns>
        public virtual bool Castable(Point target)
        {
            return !_owner.CastsLocked() && CurrentCharges >= 1;
        }

        /// <summary>
        /// Lorsque le sort est lancé, l'incantation de son possesseur commence, ce qui lance le type d'animation
        /// correspondant et lui interdit de lancer d'autres sort sdurant son incantation. Selon les données du sort,
        /// il est également rendu immobile.
        /// </summary>
        /// <param name="target"> La cible du sort </param>
        public virtual void Cast(Point target)
        {
            _currentMs = 0;
            _target = target;
            _owner.Orientation = _owner.Position.GetOrientation(target);

            _owner.LockCasts();
            if (!_spellData.CanMove) _owner.LockMoves();
            _owner.CreateState(StateData);
            (_owner.SpriteHandler as AnimationHandler)?.
                PlayAnimation(_spellData.AnimationType, false, _spellData.CastTimeMs);
        }

        /// <summary>
        /// Retourne le pourcentage de chargement du sort, entre 0 et 1.
        /// Plus la valeur est grande et plus le sort est chargé.
        /// </summary>
        /// <returns> Le pourcentage de chargement du sort </returns>
        public double CooldownRatio()
        {
            return 1 - _cooldown.Value / _cooldown.ValueMax;
        }
    }
}