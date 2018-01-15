
using TentacleSlicers.actors;
using TentacleSlicers.graphics;
using TentacleSlicers.spells;

namespace TentacleSlicers.customs
{
    /// <summary>
    /// Classe statique qui rescence les animations du jeu.
    /// </summary>
    public class Sprites
    {
        private Sprites() {}

        // Sprites des joueurs

        public static readonly AnimationPack BlueKnight = LoadBlueKnight();
        private static AnimationPack LoadBlueKnight()
        {
            var animations = new AnimationPack(PlayerCharacter.ImagePath + "blue knight");
            animations.GetAnimation(AnimationType.Run).AnimLength = 600;
            return animations;
        }

        public static readonly AnimationPack BlueKnightTomb = LoadBlueKnightTomb();
        private static AnimationPack LoadBlueKnightTomb()
        {
            var animations = new AnimationPack(PlayerCharacter.ImagePath + "blue tomb");
            animations.GetAnimation(AnimationType.Birth).AnimLength = 1800;
            return animations;
        }

        public static readonly AnimationPack PinkKnight = LoadPinkKnight();
        private static AnimationPack LoadPinkKnight()
        {
            var animations = new AnimationPack(PlayerCharacter.ImagePath + "pink knight");
            animations.GetAnimation(AnimationType.Run).AnimLength = 600;
            return animations;
        }

        public static readonly AnimationPack PinkKnightTomb = LoadPinkKnightTomb();
        private static AnimationPack LoadPinkKnightTomb()
        {
            var animations = new AnimationPack(PlayerCharacter.ImagePath + "pink tomb");
            animations.GetAnimation(AnimationType.Birth).AnimLength = 1800;
            return animations;
        }

        // Sprites des sorts du joueur

        public static readonly AnimationPack Holyball = LoadHolyball();
        private static AnimationPack LoadHolyball()
        {
            var animations = new AnimationPack(Spell.ImagePath + "holyball");
            animations.GetAnimation(AnimationType.Birth).AnimLength = 300;
            animations.GetAnimation(AnimationType.Stand).AnimLength = 300;
            return animations;
        }

        public static readonly AnimationPack Arrow = LoadArrow();
        private static AnimationPack LoadArrow()
        {
            var animations = new AnimationPack(Spell.ImagePath + "arrow");
            return animations;
        }

        public static readonly AnimationPack Impact = LoadImpact();
        private static AnimationPack LoadImpact()
        {
            var animations = new AnimationPack(Spell.ImagePath + "impact", 70);
            animations.GetAnimation(AnimationType.Death).AnimLength = 500;
            return animations;
        }

        public static readonly AnimationPack Bomb = LoadBomb();
        private static AnimationPack LoadBomb()
        {
            var animations = new AnimationPack(Spell.ImagePath + "bomb", 16);
            animations.GetAnimation(AnimationType.Stand).AnimLength = 500;
            animations.GetAnimation(AnimationType.Death).AnimLength = 600;
            return animations;
        }

        // Sprites des mobs

        public static readonly AnimationPack Portal = LoadPortal();
        private static AnimationPack LoadPortal()
        {
            var animations = new AnimationPack(Spell.ImagePath + "portal");
            animations.GetAnimation(AnimationType.Stand).AnimLength = 1300;
            return animations;
        }

        public static readonly AnimationPack Fireball = LoadFireball();
        private static AnimationPack LoadFireball()
        {
            var animations = new AnimationPack(Spell.ImagePath + "fireball");
            animations.GetAnimation(AnimationType.Birth).AnimLength = 300;
            animations.GetAnimation(AnimationType.Stand).AnimLength = 450;
            return animations;
        }

        public static readonly AnimationPack GreenCultist = LoadGreenCultist();
        private static AnimationPack LoadGreenCultist()
        {
            var animations = new AnimationPack(Mob.ImagePath + "green cultist");
            animations.GetAnimation(AnimationType.Death).AnimLength = 1500;
            return animations;
        }

        public static readonly AnimationPack OrangeCultist = LoadOrangeCultist();
        private static AnimationPack LoadOrangeCultist()
        {
            var animations = new AnimationPack(Mob.ImagePath + "orange cultist");
            animations.GetAnimation(AnimationType.Death).AnimLength = 1500;
            return animations;
        }

        // Sprites du boss

        public static readonly AnimationPack CthulhuGhost = LoadCthulhuGhost();
        private static AnimationPack LoadCthulhuGhost()
        {
            var animations = new AnimationPack(Mob.ImagePath + "cthulhu ghost");
            return animations;
        }

        public static readonly AnimationPack CthulhuLightning1 = LoadCthulhuLightning1();
        private static AnimationPack LoadCthulhuLightning1()
        {
            var animations = new AnimationPack(Spell.ImagePath + "lightning A", 16);
            return animations;
        }

        public static readonly AnimationPack CthulhuLightning2 = LoadCthulhuLightning2();
        private static AnimationPack LoadCthulhuLightning2()
        {
            var animations = new AnimationPack(Spell.ImagePath + "lightning B", 16);
            return animations;
        }

        public static readonly AnimationPack CthulhuMeteor = LoadCthulhuMeteor();
        private static AnimationPack LoadCthulhuMeteor()
        {
            var animations = new AnimationPack(Spell.ImagePath + "meteor");
            return animations;
        }

        public static readonly AnimationPack CthulhuVoidball = LoadCthulhuVoidball();
        private static AnimationPack LoadCthulhuVoidball()
        {
            var animations = new AnimationPack(Spell.ImagePath + "voidball");
            animations.GetAnimation(AnimationType.Stand).AnimLength = 300;
            animations.GetAnimation(AnimationType.Death).AnimLength = 300;
            return animations;
        }
    }
}