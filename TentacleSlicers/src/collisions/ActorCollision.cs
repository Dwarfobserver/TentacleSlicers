using System;
using System.Collections.Generic;
using TentacleSlicers.actors;
using TentacleSlicers.maps;

namespace TentacleSlicers.collisions
{
    /// <summary>
    /// Déclenche les effets de la collision d'un acteur et le place dans le gestionnaire de collisions.
    /// </summary>
    public class ActorCollision : CollisionFunctions
    {
        public Rectangle Hitbox { get; }
        public HitboxType HitboxType { get; }
        private readonly Actor _owner;
        private readonly Action<Actor> _collsision;
        private readonly List<Square> _squares;

        /// <summary>
        /// Place l'acteur associé dans le gestionnaire de collisions.
        /// </summary>
        /// <param name="owner"> L'acteur associé </param>
        /// <param name="rectangle"> La taille de la hitbox de l'acteur </param>
        /// <param name="hitboxType"> Le type de hitbox de l'acteur </param>
        /// <param name="collision"> L'éventuel effet déclenché lors de la collision de l'acteur </param>
        public ActorCollision(Actor owner, Rectangle rectangle, HitboxType hitboxType,
            Action<Actor> collision = null)
        {
            Hitbox = rectangle;
            HitboxType = hitboxType;
            _owner = owner;
            if (collision == null)
            {
                _collsision = actor => { };
            }
            else
            {
                _collsision = collision;
            }
            _squares = SquaresHandler.GetAndUpdateSquares(_owner, Hitbox);
        }

        /// <summary>
        /// Recrée l'ActorCollision d'un acteur. Utilisé lorsque celui-ci est ressussité, pour restaurer sa collision.
        /// </summary>
        /// <param name="clone"> L'ActorCollision à récupérer </param>
        public ActorCollision(ActorCollision clone)
        {
            Hitbox = clone.Hitbox;
            HitboxType = clone.HitboxType;
            _owner = clone._owner;
            _collsision = clone._collsision;
            _squares = SquaresHandler.GetAndUpdateSquares(_owner, Hitbox);
        }

        /// <summary>
        /// Appelé à chaque fois que l'acteur est déplacé (même lorsqu'il est replacé, contrairement à la fonction
        /// UpdateAndCollide()) afin de le stocker correctement dans le gestionnaire de collisions.
        /// Aucune action n'est faite si l'acteur est en collision avec un ou plusieurs autres acteurs.
        /// </summary>
        public void Update()
        {
            SquaresHandler.UpdateSquares(_owner, _squares);
        }

        /// <summary>
        /// Appelé lorsque l'acteur se déplace afin de l'actualiser dans le gestionnaire de collision et appliquer tous
        /// les possibles effets de collision.
        /// Pour chaque acteur entré en collision, si celui-ci n'est pas mort, les effets de collision sont appliqués à
        /// cet acteur et les effets de collision de cet acteur sont appliqué à l'acteur associé.
        /// Enfin, selon le type de hitbox de chaque acteur, un des deux acteurs peut être replacé.
        /// En cas de deux types de hitbox de même type (non immaterielles ou unmovables), c'est l'acteur en cours de
        /// déplacement qui est replacé.
        /// </summary>
        public void UpdateAndCollide()
        {
            Update();
            var actors = SquaresHandler.GetActors(_squares);
            var collidingActors = KeepCollidingActors(actors, _owner.Position, Hitbox);

            foreach (var actor in collidingActors)
            {
                if (_owner.IsDead() || actor.IsDead() || _owner == actor) continue;
                var handler = actor.Collision;

                // Partie où les effets des collisions sont invoqués
                _collsision.Invoke(actor);
                handler._collsision.Invoke(_owner);

                // Partie où les acteurs, si besoin, sont replacés
                if (HitboxType == HitboxType.Immaterial || handler.HitboxType == HitboxType.Immaterial) continue;
                if (HitboxType <= handler.HitboxType)
                {
                    _owner.Replaces(Hitbox.Replaces(handler.Hitbox, _owner.Position, actor.Position));
                }
                else
                {
                    actor.Replaces(handler.Hitbox.Replaces(Hitbox, actor.Position, _owner.Position));
                }
            }
        }

        /// <summary>
        /// Appelé lors de la destruction de la hitbox de l'acteur ou de la mort de l'acteur lui-même afin de le
        /// retirer du gestionnaire de collisions.
        /// </summary>
        public void Dispose()
        {
            SquaresHandler.DisposeActor(_owner, _squares);
        }
    }
}