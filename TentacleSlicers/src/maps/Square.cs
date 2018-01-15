
using System.Collections.Generic;
using System.Linq;
using TentacleSlicers.actors;

namespace TentacleSlicers.maps
{
    /// <summary>
    /// Une case du gestionnaire de collisions qui stocke les acteurs qui sont en collision avec cette case.
    /// </summary>
    public class Square
    {
        public const int Size = Map.Size * 3;

        public List<Actor> Actors { get; }

        /// <summary>
        /// Initialise la case avec aucun acteur à l'intérieur.
        /// </summary>
        public Square()
        {
            Actors = new List<Actor>();
        }

        /// <summary>
        /// Ajoute à la liste passée en paramètre les acteurs du Square, en évitant les doublons.
        /// </summary>
        /// <param name="actors"> La liste à compléter </param>
        public void AddActorsTo(List<Actor> actors)
        {
            foreach (var actor in Actors)
            {
                if (!actors.Contains(actor))
                {
                    actors.Add(actor);
                }
            }
        }

        /// <summary>
        /// Décrit le contenu de la case en listant ses acteurs et leurs positions.
        /// </summary>
        /// <returns> Les acteurs en collision avec la case </returns>
        public override string ToString()
        {
            return Actors.Aggregate("Square : ", (current, actor) => current + actor.Position + " ");
        }
    }
}