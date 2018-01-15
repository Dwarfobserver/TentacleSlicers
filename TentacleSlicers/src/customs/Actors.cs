
using TentacleSlicers.actors;
using TentacleSlicers.spells;

namespace TentacleSlicers.customs
{
    /// <summary>
    /// Classe statique qui rescence les différents projectiles et mobs du jeu.
    /// </summary>
    public class Actors
    {
        private Actors() {}

        // Projectiles des powerups

        private static MissileData _holyball;
        public static MissileData Holyball()
        {
            if (_holyball == null)
            {
                _holyball = new MissileData
                {
                    Damage = 70,
                    Range = 700,
                    Speed = 400,
                    Size = 20,
                    Animations = Sprites.Holyball
                };
            }
            return _holyball;
        }

        private static MissileData _arrow;
        public static MissileData Arrow()
        {
            if (_arrow == null)
            {
                _arrow = new MissileData
                {
                    Damage = 40,
                    Range = 600,
                    Speed = 600,
                    Size = 15,
                    Animations = Sprites.Arrow
                };
            }
            return _arrow;
        }

        private static ExplosionData _impact;
        public static ExplosionData Impact()
        {
            if (_impact == null)
            {
                _impact = new ExplosionData
                {
                    Delay = 600,
                    Animations = Sprites.Impact,
                    Size = 100,
                    Damage = 75,
                    Effect = (caster, position, actor) =>
                    {
                        actor.LockCasts();
                        actor.LockMoves();
                        actor.CreateState(States.ImpactDebuff);
                    }
                };
            }
            return _impact;
        }

        // Mobs et leur sprojectiles

        private static MobData _orangeCultist;
        public static MobData OrangeCultist()
        {
            if (_orangeCultist == null)
            {
                _orangeCultist = new MobData
                {
                    Life = 100,
                    Speed = 140,
                    Animations = Sprites.OrangeCultist,
                    Size = 40,
                    Spells = new SpellData[2],
                    LootRate = 0.6,
                    ScoreValue = 5
                };
                _orangeCultist.Spells[0] = Spells.OrangeCultistFireball();
                _orangeCultist.Spells[1] = Spells.OrangeCultistAttack();
            }
            return _orangeCultist;
        }

        private static MobData _greenCultist = GreenCultist();
        public static MobData GreenCultist()
        {
            if (_greenCultist == null)
            {
                _greenCultist = new MobData
                {
                    Life = 150,
                    Speed = 180,
                    Animations = Sprites.GreenCultist,
                    Size = 40,
                    Spells = new SpellData[1],
                    LootRate = 0.6,
                    ScoreValue = 4
                };
                _greenCultist.Spells[0] = Spells.GreenCultistAttack();
            }
            return _greenCultist;
        }

        private static MissileData _cultistFireball;
        public static MissileData CultistFireball()
        {
            if (_cultistFireball == null)
            {
                _cultistFireball  = new MissileData
                {
                    Range = 550,
                    Speed = 350,
                    Animations = Sprites.Fireball,
                    Size = 25,
                    Damage = 30
                };
            }
            return _cultistFireball;
        }

        // Projectiles du boss

        private static ExplosionData _lightning1;
        public static ExplosionData Lightning1()
        {
            if (_lightning1 == null)
            {
                _lightning1 = new ExplosionData
                {
                    Delay = 1000,
                    Animations = Sprites.CthulhuLightning1,
                    Size = 75,
                    Damage = 55
                };
            }
            return _lightning1;
        }

        private static ExplosionData _lightning2;
        public static ExplosionData Lightning2()
        {
            if (_lightning2 == null)
            {
                _lightning2 = new ExplosionData
                {
                    Delay = 1000,
                    Animations = Sprites.CthulhuLightning2,
                    Size = 75,
                    Damage = 55
                };
            }
            return _lightning2;
        }

        private static ExplosionData _meteor;
        public static ExplosionData Meteor()
        {
            if (_meteor == null)
            {
                _meteor = new ExplosionData
                {
                    Delay = 900,
                    Animations = Sprites.CthulhuMeteor,
                    Size = 60,
                    Damage = 40,
                    Effect = (caster, position, actor) =>
                    {
                        var direction = position.GetOrientation(actor.Position);
                        var distance = position.Length(actor.Position);
                            actor.CreateState(States.CthulhuMeteor, new StateArgMeteor
                        {
                            Vector = direction * 2 / (distance + 20),
                            MsElapsed = 0
                        });
                    }
                };
            }
            return _meteor;
        }

        private static MissileData _cthulhuVoidball;
        public static MissileData CthulhuVoidball()
        {
            if (_cthulhuVoidball == null)
            {
                _cthulhuVoidball  = new MissileData
                {
                    Range = 150,
                    Speed = 100,
                    Animations = Sprites.CthulhuVoidball,
                    Size = 25,
                    Damage = 30
                };
            }
            return _cthulhuVoidball;
        }
    }
}