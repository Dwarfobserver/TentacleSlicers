using TentacleSlicers.general;

namespace TentacleSlicers.collisions
{
    /// <summary>
    /// Représente une forme géométrique qui peut entrer en collision avec les hitbox rectangulaires des acteurs.
    /// </summary>
    public abstract class Form
    {
        /// <summary>
        /// Indique si la forme courante à la position p1 entre en collision avec le rectangle donné à la position p2.
        /// </summary>
        /// <param name="hitbox"></param>
        /// <param name="p1"> La position de la forme courante </param>
        /// <param name="p2"> Le centre du rectangle </param>
        /// <returns> Vrai si la forme courante et le rectangle donné sont en collision </returns>
        public abstract bool Collides(Rectangle hitbox, Point p1, Point p2);

        /// <summary>
        /// Retourne un rectangle qui niclut la forme courante, pour la position donnée.
        /// </summary>
        /// <param name="position"> La position de la forme courante </param>
        /// <returns> Le rectangle incluant la forme courante, avec son centre </returns>
        public abstract ShiftedRectangle IncludingRectangle(Point position);
    }
}