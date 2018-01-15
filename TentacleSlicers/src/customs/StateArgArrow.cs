using TentacleSlicers.general;

namespace TentacleSlicers.customs
{
    /// <summary>
    /// Stocke des données pour le fonctionnement de l'effet Arrow lorsqu'il est déclenché :
    /// Permet de conserver la direction des projectiles envoyés et de garder un compteur pour envoyer les projectiles
    /// à intervalles régulières.
    /// </summary>
    public class StateArgArrow
    {
        public Point Orientation;
        public int CurrentMs;
    }
}