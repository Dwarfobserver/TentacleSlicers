
using System.Drawing;
using TentacleSlicers.actors;
using TentacleSlicers.graphics;
using TentacleSlicers.interfaces;
using Point = TentacleSlicers.general.Point;

namespace TentacleSlicers.states
{
    /// <summary>
    /// Objet temporaire qui affecte son possesseur lors de son actualisation et / ou lorsqu'il expire. Il peut aussi
    /// afficher une image jusqu'à son expiration.
    /// </summary>
    public class State : IDrawable, ITickable
    {
        public static readonly string ImagePath = SpriteHandler.ImagePath + "states/";

        private readonly ControlledActor _owner;
        private readonly StateData _data;
        private readonly object _arg;

        private int _currentMs;

        /// <summary>
        /// Crée l'état avec les données fournies et en l'associant à un acteur.
        /// </summary>
        /// <param name="owner"> L'acteur associé </param>
        /// <param name="data"> les données </param>
        /// <param name="arg"> Le paramètre optionnel </param>
        public State(ControlledActor owner, StateData data, object arg)
        {
            _owner = owner;
            _data = data;
            _arg = arg;
        }

        /// <summary>
        /// Affiche l'image de l'état, si elle existe.
        /// </summary>
        /// <param name="shift"> La position où est dessiné l'acteur associé </param>
        /// <param name="g"> L'objet permettant de dessiner </param>
        public void Draw(Point shift, Graphics g)
        {
            _data.Sprite?.Draw(shift, g);
        }

        /// <summary>
        /// Actualise l'état et déclenche son effet d'actualisation.
        /// </summary>
        /// <param name="ms"> Le temps écoulé en millisecondes </param>
        public void Tick(int ms)
        {
            _currentMs += ms;
            _data.Effect?.Invoke(_owner, ms, _arg);
        }

        /// <summary>
        /// Indique si l'état est terminé.
        /// </summary>
        /// <returns> Vrai si l'état est terminé </returns>
        public bool IsFinished()
        {
            return _currentMs >= _data.MaxMs;
        }

        /// <summary>
        /// Déclenche l'effet d'expiration de l'état, si il existe.
        /// </summary>
        public void Expires()
        {
            _data.ExpiringEffect?.Invoke(_owner, _arg);
        }
    }
}