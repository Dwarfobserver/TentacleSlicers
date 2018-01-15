
using TentacleSlicers.graphics;
using TentacleSlicers.spells;

namespace TentacleSlicers.actors
{
    /// <summary>
    /// Permet la création facilitée de mobs.
    /// </summary>
    public class MobData
    {
        public int Life;
        public int Speed;
        public AnimationPack Animations;
        public int Size;
        public SpellData[] Spells;
        public double LootRate;
        public int ScoreValue;
    }
}