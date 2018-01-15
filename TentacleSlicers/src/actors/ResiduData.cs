
using TentacleSlicers.general;
using TentacleSlicers.graphics;

namespace TentacleSlicers.actors
{
    /// <summary>
    /// Utilisé lors de la mort d'un acteur pour vérifier si il peut créer un résidu, et pour fournir le cas échéant
    /// les paramètres du résidu.
    /// </summary>
    public class ResiduData
    {
        public Animation Animation => GetAnimation();
        public Point SpeedVector => GetSpeedVector();

        private readonly Actor _owner;
        private readonly AnimationType _type;
        private readonly bool _keepOwnerMovement;

        /// <summary>
        /// Initialise les paramètres du résidu de la mort de l'acteur donné avec les informations indiquées.
        /// </summary>
        /// <param name="owner"> L'acteur qui va créer le résidu à sa mort </param>
        /// <param name="type"> Le type de l'animation du résidu </param>
        /// <param name="keepOwnerMovement"> Indique si le résidu conserve la vitesse de l'acteur à sa mort </param>
        public ResiduData(Actor owner, AnimationType type, bool keepOwnerMovement)
        {
            _owner = owner;
            _type = type;
            _keepOwnerMovement = keepOwnerMovement;
        }

        /// <summary>
        /// Vérifie si le résidu peut être crée, en fonction de l'acteur et du type d'animation demandé.
        /// </summary>
        /// <returns> Vrai si le résidu peut être crée </returns>
        public bool IsValidData()
        {
            return (_owner.SpriteHandler as AnimationHandler)?.GetAnimation(_type)?.Type == _type;
        }

        /// <summary>
        /// retourne l'animation du résidu, sans vérifications.
        /// </summary>
        /// <returns> L'animation du résidu </returns>
        private Animation GetAnimation()
        {
            return ((AnimationHandler) _owner.SpriteHandler).GetAnimation(_type);
        }

        /// <summary>
        /// Retourne le vecteur de vitesse du résidu.
        /// </summary>
        /// <returns> Le vecteur de vitesse </returns>
        private Point GetSpeedVector()
        {
            return _keepOwnerMovement ? _owner.SpeedVector : Point.Null;
        }
    }
}