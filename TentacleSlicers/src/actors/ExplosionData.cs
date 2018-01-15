using System;
using TentacleSlicers.general;
using TentacleSlicers.graphics;

namespace TentacleSlicers.actors
{
    /// <summary>
    /// Permet la création facilitée d'explosions.
    /// </summary>
    public class ExplosionData
    {
        public int Delay;
        public AnimationPack Animations;
        public int Size;
        public int Damage;
        public Action<LivingActor, Point, LivingActor> Effect;
    }
}