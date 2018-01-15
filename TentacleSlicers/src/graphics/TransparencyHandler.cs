
using TentacleSlicers.interfaces;

namespace TentacleSlicers.graphics
{
    /// <summary>
    /// Met à disposition un paramètre d'opacité actualisé par la fonction Tick, dont le comprtement est décrit dans
    /// les classes filles.
    /// </summary>
    public abstract class TransparencyHandler : ITickable
    {
        public float Opacity { get; protected set; }

        /// <summary>
        /// Actualise l'opacité.
        /// </summary>
        /// <param name="ms"> Le temps écoulé en millisecondes </param>
        public abstract void Tick(int ms);
    }
}