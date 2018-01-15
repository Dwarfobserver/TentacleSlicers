
using TentacleSlicers.general;

namespace TentacleSlicers.customs
{
    /// <summary>
    /// Stocke des données pour le fonctionnement de l'effet CthulhuMeteor lorsqu'il est déclenché :
    /// Permet de projeter le joueur touché par rapport au point d'impact du météore.
    /// </summary>
    public class StateArgMeteor
    {
        public Point Vector;
        public int MsElapsed;
    }
}