using System;
using TentacleSlicers.actors;
using TentacleSlicers.graphics;

namespace TentacleSlicers.states
{
    /// <summary>
    /// Fournit les données permettant de créer un état pour n'importe quel ControlledActor.
    /// </summary>
    public class StateData
    {
        public Action<ControlledActor, int, object> Effect { get; }
        public int MaxMs { get; }
        public Action<ControlledActor, object> ExpiringEffect { get; }
        public CustomSprite Sprite { get; }

        /// <summary>
        /// Initialise les données avec la durée de l'état. Les effets d'actualisation, d'expiration ou encore l'image
        /// de l'état sont optionnels. Un objet supplémentaire peut être passé pour stocker des informations.
        /// </summary>
        /// <param name="maxMs"> Le temps écoulé en millisecondes </param>
        /// <param name="effect"> L'effet d'actualisation </param>
        /// <param name="expiringEffect"> L'effet d'expiration </param>
        /// <param name="sprite"> L'image de l'effet </param>
        public StateData(int maxMs, Action<ControlledActor, int, object> effect = null,
            Action<ControlledActor, object> expiringEffect = null, CustomSprite sprite = null)
        {
            Effect = effect;
            MaxMs = maxMs;
            ExpiringEffect = expiringEffect;
            Sprite = sprite;
        }
    }
}