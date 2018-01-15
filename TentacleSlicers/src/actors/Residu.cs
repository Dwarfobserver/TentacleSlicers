
using TentacleSlicers.general;
using TentacleSlicers.graphics;

namespace TentacleSlicers.actors
{
    /// <summary>
    /// Un résidu spécifie et simplifie la classe Actor pour jouer une unique animation puis se retirer soi-même du
    /// monde.
    /// </summary>
    public class Residu : Actor
    {
        /// <summary>
        /// Crée une animation à la positoin spécifiée, avec si besoin un vecteur de vitesse constant.
        /// </summary>
        /// <param name="position"> La position initiale de l'animation </param>
        /// <param name="animation"> L'animation à jouer </param>
        /// <param name="speed"> Le vecteur de vitesse de l'animation </param>
        public Residu(Point position, Animation animation, Point speed = null) : base(position)
        {
            SpeedVector = speed ?? Point.Null;
            SpriteHandler = new AnimationHandler(this, animation, false);
        }

        /// <summary>
        /// Déplace si nécessaire le Residu avant d'appeler la fonction Tick de la classe mère.
        /// Puis, vérifie si l'animation est terminée. Si c'est le cas, supprime le Residu du monde.
        /// </summary>
        /// <param name="ms"> Le temps écoulé en millisecondes </param>
        public override void Tick(int ms)
        {
            Move(SpeedVector * ((double)ms / 1000));
            base.Tick(ms);
            if (((AnimationHandler) SpriteHandler).IsFinished())
            {
                Kill();
            }
        }
    }
}