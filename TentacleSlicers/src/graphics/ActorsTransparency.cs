using System;
using TentacleSlicers.actors;
using TentacleSlicers.collisions;
using TentacleSlicers.general;

namespace TentacleSlicers.graphics
{
    /// <summary>
    /// Indique une opacité qui s'atténue progressivement lorsque des acteurs sont derrière l'acteur indiqué.
    /// L'opacité est minimale lorsque au moins un acteur est juste derrière l'acteur indiqué.
    /// </summary>
    public class ActorsTransparency : TransparencyHandler
    {
        private readonly Actor _owner;
        private readonly float _opacityMin;
        private readonly int _width;
        private readonly int _height;

        /// <summary>
        /// Crée le comportement avec l'acteur associé, une opacité minimale et la taille de la zone de détection de
        /// l'opacité derrière lui.
        /// </summary>
        /// <param name="owner"> L'acteur associé </param>
        /// <param name="opacityMin"> L'opacité minimale atteinte </param>
        /// <param name="width"> La largeur de la zone de détection </param>
        /// <param name="height"> La hauteur de la zone de détection </param>
        public ActorsTransparency(Actor owner, float opacityMin, int width, int height)
        {
            _owner = owner;
            _opacityMin = opacityMin;
            _width = width;
            _height = height;
        }

        /// <summary>
        /// Récupère les acteurs dans la zone de détection derrière l'acteur associé, et utilise celui qui est le plus
        /// centré en bas de la zone de détection pour établir la nouvelle opacité.
        /// </summary>
        /// <param name="ms"> Le temps écoulé en millisecondes </param>
        public override void Tick(int ms)
        {
            var actors = CollisionFunctions.GetCollidingActors(_owner.Position - new Point(0, (double) _height / 2),
                new Rectangle(_width * 2, _height * 2));
            var opacityMin = 1.0f;

            foreach (var actor in actors)
            {
                // Les acteurs neutres sont ignorés
                if (actor.Faction == Faction.Neutral) continue;
                var p = actor.Position;
                var x = Math.Abs(_owner.Position.X - p.X);
                double y;
                if (p.Y > _owner.Position.Y)
                {
                    y = _height + 1;
                }
                else
                {
                    y = Math.Abs(_owner.Position.Y - p.Y);
                }
                if ((x > _width) ||
                    (y > _height)) continue;
                // Minimise l'opacité lorsque l'acteur est au milieu en abscisse et en bas en ordonnée
                var x2 = (float) Math.Min(1, x / _width) * (1 - _opacityMin) + _opacityMin;
                var y2 = (float) Math.Min(1, y / _height) * (1 - _opacityMin) + _opacityMin;
                opacityMin = Math.Min(Math.Max(x2, y2), opacityMin);
            }
            Opacity = opacityMin;
        }
    }
}