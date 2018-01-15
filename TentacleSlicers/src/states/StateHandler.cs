
using System;
using System.Collections.Generic;
using System.Drawing;
using TentacleSlicers.actors;
using TentacleSlicers.interfaces;
using Point = TentacleSlicers.general.Point;

namespace TentacleSlicers.states
{
    /// <summary>
    /// Gère tous les états d'un acteur, et permet de les créer à partir de StateData.
    /// </summary>
    public class StateHandler : IDrawable, ITickable
    {
        private readonly ControlledActor _owner;
        private List<State> _states;

        /// <summary>
        /// Initialise le StateHandler.
        /// </summary>
        /// <param name="owner"> L'acteur associé aux effets </param>
        public StateHandler(ControlledActor owner)
        {
            _owner = owner;
            _states = new List<State>();
        }

        /// <summary>
        /// Dessine les états.
        /// </summary>
        /// <param name="shift"> La position de l'acteur dansa le monde </param>
        /// <param name="g"> L'objet permettant de dessiner </param>
        public void Draw(Point shift, Graphics g)
        {
            foreach (var state in _states)
            {
                state.Draw(shift, g);
            }
        }

        /// <summary>
        /// Actualise les états. Déclenche l'effet d'expiration puis supprime les états qui sont terminés.
        /// </summary>
        /// <param name="ms"> Le temps écoulé en millisecondes </param>
        public void Tick(int ms)
        {
            var needToActualise = false;
            var states = new List<State>(_states);

            foreach (var t in states)
            {
                t.Tick(ms);
            }
            foreach (var state in _states)
            {
                if (state.IsFinished()) needToActualise = true;
            }

            if (!needToActualise) return;
            // Traitement des états terminés

            var newStates = new List<State>();
            states = new List<State>(_states);

            foreach (var t in states)
            {
                if (t.IsFinished())
                {
                    t.Expires();
                }
                else
                {
                    newStates.Add(t);
                }
            }
            _states = newStates;
        }

        /// <summary>
        /// Ajoute un état à l'acteur.
        /// </summary>
        /// <param name="data"> Les données pour créer l'état </param>
        /// <param name="arg"> Le paramètre optionnel </param>
        public void CreateState(StateData data, object arg)
        {
            _states.Add(new State(_owner, data, arg));
        }

        /// <summary>
        /// Supprime tous les états affectant l'acteur. Utilisé notamment pour obtenir une sauvegarde dont les
        /// statistiques des personnages ne sont pas altérées par des effets temporaires.
        /// </summary>
        public void Clean()
        {
            var states = new List<State>(_states);
            foreach (var t in states)
            {
                t.Expires();
            }
            _states = new List<State>();
        }
    }
}