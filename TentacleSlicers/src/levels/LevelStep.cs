
using TentacleSlicers.actors;
using TentacleSlicers.customs;
using TentacleSlicers.general;
using TentacleSlicers.interfaces;

namespace TentacleSlicers.levels
{
    /// <summary>
    /// Définit une vague du niveau, avec l'apparition de plusieurs mobs identiques à un point. Permet de s'ajouter à
    /// la vague précédente si le délai de ce LevelStep est nul.
    /// </summary>
    public class LevelStep : ITickable
    {
        private const int MsDelay = 1000;

        private readonly Point _spawn;
        private readonly MobData _mobType;
        private readonly int _msBegin;
        private int _nbMobs;
        private int _currentMs;
        private bool _spawnig;

        /// <summary>
        /// Crée un spawn de plusieurs mobs du type donné pour l'endroit donné, après un délai dans le déroulement du
        /// niveau. Si ce délai est nul, alors cette étape fera parti de la même vague que l'étape précédente.
        /// </summary>
        /// <param name="secondsTimer"> Le délai pour lancer l'étape, en secondes </param>
        /// <param name="mobType"> Les données servant à créer les mobs invoqués </param>
        /// <param name="nbMob"> Le nombre de mobs à invoquer </param>
        /// <param name="spawn"> L'endroit où sont invoqués les mobs </param>
        public LevelStep(int secondsTimer, MobData mobType, int nbMob, Point spawn)
        {
            _spawn = spawn;
            _mobType = mobType;
            _nbMobs = nbMob;
            _msBegin = secondsTimer * 1000;
            _currentMs = 0;
            _spawnig = secondsTimer == 0;
        }

        /// <summary>
        /// Retourne en secondes le temps restant avant le déclenchement de l'étape.
        /// </summary>
        /// <returns> Le temps restant en secondes </returns>
        public double Cooldown()
        {
            return (_msBegin - _currentMs) / 1000.0;
        }

        /// <summary>
        /// Indique si l'étape a commencé à invoquer ses mobs.
        /// </summary>
        /// <returns> Vrai si l'étape a commencé </returns>
        public bool HasBegun()
        {
            return _spawnig;
        }

        /// <summary>
        /// indique si l'étape a fini d'invoquer ses mmobs.
        /// </summary>
        /// <returns> Vrai si l'étape est finie </returns>
        public bool IsFinished()
        {
            return _nbMobs == 0 && _spawnig;
        }

        /// <summary>
        /// Actualise l'étape, soit en diminuant le temps restant soit en réduisant le délai pour l'apparition du
        /// prochain mob. Si l'un de ces compteurs est consommé, un nouveau mob est invoqué.
        /// </summary>
        /// <param name="ms"> Le temps écoulé en millisecondes </param>
        public void Tick(int ms)
        {
            if (IsFinished()) return;

            _currentMs += ms;
            if (!_spawnig)
            {
                if ( _currentMs <= _msBegin) return;
                _currentMs -= _msBegin;
                _spawnig = true;
                Trigger();
            }
            else if (_currentMs > MsDelay)
            {
                _currentMs -= MsDelay;
                Trigger();
            }
        }

        /// <summary>
        /// Crée un effet de portail, crée un mob et décrémente le nombre de mobs à invoquer.
        /// </summary>
        private void Trigger()
        {
            if (_nbMobs <= 0) return;
            new Residu(_spawn, Sprites.Portal.DefaultAnimation);
            new Mob(_spawn, _mobType);
            --_nbMobs;
        }
    }
}