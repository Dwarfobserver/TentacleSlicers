
using System.Collections.Generic;
using TentacleSlicers.actors;
using TentacleSlicers.general;
using TentacleSlicers.maps;

namespace TentacleSlicers.collisions
{
    /// <summary>
    /// Classe mère des classes qui testent les collisions entre les différentes formes.
    /// Donne accès à une fonction pour récupérer les acteurs en collision avec une forme donnée, et une fonction pour
    /// ses classe filles qui garde parmi une liste d'acteurs ceux qui sont en collision avec une forme donnée.
    /// </summary>
    public abstract class CollisionFunctions
    {
        public static SquaresHandler SquaresHandler;

        /// <summary>
        /// Récupère les acteurs proches de la forme dans le gestionnaire de collision et renvoie ceux qui sont en
        /// collision avec la forme.
        /// </summary>
        /// <param name="position"> La position de la forme </param>
        /// <param name="form"> La forme indiquée </param>
        /// <returns> les acteurs en collision avec la forme </returns>
        public static List<Actor> GetCollidingActors(Point position, Form form)
        {
            var actors = SquaresHandler.GetActors(form.IncludingRectangle(position));
            return KeepCollidingActors(actors, position, form);
        }

        /// <summary>
        /// Parcourt le sacteurs donnés pour renvoyer uniquement ceux qui sont en collision avec la forme à sa position
        /// indiquée.
        /// </summary>
        /// <param name="actors"> La collection d'acteurs à tester </param>
        /// <param name="position"> La position de la forme </param>
        /// <param name="form"> La forme indiquée </param>
        /// <returns> les acteurs, parmi ceux donnés, qui sont en collision avec la forme </returns>
        protected static List<Actor> KeepCollidingActors(List<Actor> actors, Point position, Form form)
        {
            var collidingActors = new List<Actor>();
            foreach (var actor in actors)
            {
                var handler = actor.Collision;
                if (handler != null && form.Collides(handler.Hitbox, position, actor.Position))
                {
                    collidingActors.Add(actor);
                }
            }
            return collidingActors;
        }
    }
}
