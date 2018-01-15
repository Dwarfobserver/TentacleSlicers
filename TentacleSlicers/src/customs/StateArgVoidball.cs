using System.Collections.Generic;
using TentacleSlicers.actors;
using TentacleSlicers.general;

namespace TentacleSlicers.customs
{
    /// <summary>
    /// Stocke des données pour le fonctionnement de l'effet CthulhuVoidBall lorsqu'il est déclenché :
    /// Permet d'envoyer en arc de cercle une série de projectiles et de les accélérer.
    /// </summary>
    public class StateArgVoidball
    {
        public Point BeginPoint;
        public Point EndingPoint;
        public List<Missile> Voidballs;
        public int MsElapsed;
    }
}