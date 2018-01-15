
using System.Collections.Generic;
using TentacleSlicers.actors;
using TentacleSlicers.general;

namespace TentacleSlicers.collisions
{
    /// <summary>
    /// Facilite le test de collision des segments en stockant le résultat des collisions d'un segment crée entre deux
    /// points :
    /// CollidingActors stocke la liste des acteurs touchés, triés par leur distance depuis la source du segment (le
    /// premier acteur est le plus proche du segment),
    /// Impacts stocke pour chaque acteur touché le premier point d'impact où le segment touche l'acteur.
    /// </summary>
    public class RayonCaster : CollisionFunctions
    {
        public List<Actor> CollidingActors { get; }
        public List<Point> Impacts { get; }

        /// <summary>
        /// Crée le rayon et récupère les acteurs touchés dans le gestionnaire de collisions.
        /// Trie ensuite les acteurs selon leur distance depuis la source du segment (le premier acteur est le plus
        /// proche du segment).
        /// Enfin, stocke pour chaque acteur le point où le segment touche l'acteur (en partant de la source).
        /// </summary>
        /// <param name="position"> La source du segment </param>
        /// <param name="target"> La destination du segment </param>
        public RayonCaster(Point position, Point target)
        {
            var segment = new Segment(target - position);
            // Récupération des acteurs
            CollidingActors = GetCollidingActors(position, segment);
            // Tri des acteurs
            CollidingActors.Sort(Comparer<Actor>.Create(
                (a1, a2) =>
                {
                    var d1 = position.Length(a1.Position);
                    var d2 = position.Length(a2.Position);
                    if (d1 > d2)
                    {
                        return 1;
                    }
                    return -1;
                })
            );
            // Récupération de l'impact du segment sur chaque acteur.
            Impacts = new List<Point>();
            foreach (var actor in CollidingActors)
            {
                Impacts.Add(segment.Impact(actor.Collision.Hitbox, position, actor.Position));
            }
        }

        /// <summary>
        /// Utilisé lorsque deux acteurs sont à chaque bout du segment.
        /// Indique si le segment relie les deux acteurs sans toucher un autre acteur.
        /// </summary>
        /// <returns> Vrai si les deux acteurs sont les seuls touchés par le segment </returns>
        public bool DirectSight()
        {
            return CollidingActors.Count <= 2;
        }
    }
}
