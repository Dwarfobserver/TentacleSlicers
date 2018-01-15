using System;
using System.Collections.Generic;
using TentacleSlicers.actors;
using TentacleSlicers.collisions;
using TentacleSlicers.general;

namespace TentacleSlicers.maps
{
    public sealed class WallTransparencyHitbox : Actor
    {
        public static readonly int Height = (int) (Map.Size * 2.5);
        public static readonly int Width = Map.Size;
        public static readonly float OpacityMin = 0.5f;

        private static readonly Rectangle Hitbox = new Rectangle(Wall.Hitbox.Width * 2, Wall.Hitbox.Height * 2);

        public float Opacity { get; private set; }

        private readonly Wall _owner;
        private readonly List<Actor> _nearActors;

        public WallTransparencyHitbox(Wall owner) : base(owner.Position - new Point(0, Height / 2.0))
        {
            _owner = owner;
            _nearActors = new List<Actor>();
            SetCollision(new ActorCollision(this, Hitbox, HitboxType.Immaterial, actor =>
            {
                // Les acteurs neutres sont ignorés
                if (actor.Faction == Faction.Neutral) return;

                if (!_nearActors.Contains(actor)) _nearActors.Add(actor);
            }));
        }

        public override void Tick(int ms)
        {
            base.Tick(ms);
            Opacity = 1f;
            var i = 0;
            while (i < _nearActors.Count)
            {
                var actor = _nearActors[i];
                var p = actor.Position;
                var x = Math.Abs(_owner.Position.X - p.X);
                double y;
                if (p.Y > _owner.Position.Y)
                {
                    y = Height + 1;
                }
                else
                {
                    y = Math.Abs(_owner.Position.Y - p.Y);
                }
                if (x > Width || y > Height)
                {
                    _nearActors.Remove(actor);
                }

                // Minimise l'opacité lorsque l'acteur est au milieu en abscisse et en bas en ordonnée
                var x2 = (float) Math.Min(1, x / Width) * (1 - OpacityMin) + OpacityMin;
                var y2 = (float) Math.Min(1, y / Height) * (1 - OpacityMin) + OpacityMin;
                Opacity = Math.Min(Math.Max(x2, y2), Opacity);
                ++i;
            }
        }
    }
}
