
using System.Collections.Generic;
using System.Linq;
using TentacleSlicers.interfaces;

namespace TentacleSlicers.levels
{
    /// <summary>
    /// Définit le déroulement d'un niveau via une file d'étapes, qui constituent des vagues. Chaque étape constitue
    /// une nouvelle vague si son délai n'est pas nul, sinon elle s'ajoute à la vague courante.
    /// Le score total du niveau est calculé.
    /// </summary>
    public class LevelScript : ITickable
    {
        private readonly Queue<LevelStep> _waitingSteps;
        private List<LevelStep> _currentSteps;

        public int Wave { get; private set; }
        public double Cooldown { get; private set; }

        /// <summary>
        /// Initialise le numéro de la vague courante à zéro.
        /// </summary>
        public LevelScript()
        {
            _waitingSteps = new Queue<LevelStep>();
            _currentSteps = new List<LevelStep>();
            Wave = 0;
            Cooldown = 0;
        }

        /// <summary>
        /// Ajoute à la fin de la file l'étape donnée.
        /// </summary>
        /// <param name="step"> La nouvelle étape </param>
        public void AddStep(LevelStep step)
        {
            _waitingSteps.Enqueue(step);
        }

        /// <summary>
        /// Indique si toutes les étapes sont déclenchées et finies.
        /// </summary>
        /// <returns> Vrai si toutes les étapes sont finies </returns>
        public bool IsFinished()
        {
            return _waitingSteps.Count == 0 && _currentSteps.Count == 0;
        }

        /// <summary>
        /// Actualise toutes les étapes qui ont commencé mais pas fini l'invocation de leurs mobs, et retire celles qui
        /// ont terminé.
        /// Ensuite, actualise la prochaine étape qui doit commencer. Si elle début, lance les prochaines tant qu'elles
        /// ont un délai nul, afin de lancer la nouvelle vague de façon homogène.
        /// </summary>
        /// <param name="ms"> Le temps écoulé, en millisecondes </param>
        public void Tick(int ms)
        {
            if (IsFinished()) return;

            // Actualise toutes les étapes qui ont commenncé mais n'ont pas fini d'invoquer des mobs
            if (_currentSteps.Count != 0)
            {
                var needToActualise = false;
                foreach (var step in _currentSteps)
                {
                    step.Tick(ms);
                    if (step.IsFinished()) needToActualise = true;
                }
                if (!needToActualise) return;
                var newSteps = new List<LevelStep>();
                newSteps.AddRange(_currentSteps.Where(currentStep => !currentStep.IsFinished()));
                _currentSteps = newSteps;
            }

            // Si il y a des étapes qui n'ont pas commencé
            if (_waitingSteps.Count != 0)
            {
                var nextStep = _waitingSteps.Peek();
                Cooldown = nextStep.Cooldown();
                nextStep.Tick(ms);
                var nextWave = false;
                var dequeing = nextStep.HasBegun();
                // La boucle permet de défiler en même temps les étapes qui suivent, si elles ont un délai nul
                while (dequeing)
                {
                    nextWave = true;
                    _waitingSteps.Dequeue();
                    _currentSteps.Add(nextStep);
                    if (_waitingSteps.Count == 0)
                    {
                        dequeing = false;
                    }
                    else
                    {
                        nextStep = _waitingSteps.Peek();
                        dequeing = nextStep.HasBegun();
                    }
                }
                if (nextWave) ++Wave;
            }
            else
            {
                Cooldown = 0;
            }
        }
    }
}