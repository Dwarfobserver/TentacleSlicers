
using System;
using System.Collections.Generic;
using TentacleSlicers.actors;
using TentacleSlicers.collisions;
using TentacleSlicers.graphics;
using TentacleSlicers.states;

namespace TentacleSlicers.customs
{
    /// <summary>
    /// Classe statique qui rescence les states du jeu.
    /// </summary>
    public class States
    {
        private States() {}

        public static readonly StateData ArrowBuff = new StateData(Spells.ArrowTimeValue + 50, (owner, ms, arg) =>
        {
            // Vérifie si assez de temps s'est écoulé pour créer une flèche
            var data = (StateArgArrow) arg;
            data.CurrentMs += ms;
            if (data.CurrentMs <= Spells.ArrowTimeValue / 2) return;
            data.CurrentMs -= Spells.ArrowTimeValue / 2;
            new Missile(owner, owner.Position + data.Orientation, Actors.Arrow());
        });

        public static readonly StateData ImpactDebuff = new StateData(1000, null, (actor, arg) =>
        {
            actor.UnlockCasts();
            actor.UnlockMoves();
        }, new BasicSprite(State.ImagePath + "impact.png"));

        public static readonly StateData BombDebuff = new StateData(1500, null, (actor, arg) =>
        {
            actor.UnlockMoves();
        }, new BasicSprite(State.ImagePath + "bomb.png"));

        private static readonly Circle DashHitbox = new Circle(85);
        public static readonly StateData DashBuff = new StateData(1500, (owner, ms, arg) =>
        {
            // Brûle tous les ennemis proches pas encore affectés
            var victims = (List<LivingActor>) arg;
            var actors = CollisionFunctions.GetCollidingActors(owner.Position, DashHitbox);
            foreach (var actor in actors)
            {
                var livingActor = actor as LivingActor;
                if (livingActor != null && owner.EnnemyWith(livingActor) && !victims.Contains(livingActor))
                {
                    victims.Add(livingActor);
                    livingActor.CreateState(DashDeBuff, owner);
                }
            }
        }, (owner, arg) =>
        {
            ((PlayerCharacter) owner).Stats.Armor -= Spells.DashArmorValue;
            owner.Speed -= Spells.DashSpeedValue;
            owner.UnlockCasts();
        }, new BasicSprite(State.ImagePath + "dash buff.png"));

        public static readonly StateData DashDeBuff = new StateData(3000, (actor, ms, arg) =>
        {
            // Inflige des dégâts et soigne le lanceur, suivant ses ratios
            var livingActor = (LivingActor) actor;
            var caster = (LivingActor) arg;
            var damages = caster.SpellPowerRatio() * 50 * ms / 3000;
            livingActor.Damages(damages);
            caster.Heals(damages * caster.VampirismRatio());
        }, null, new BasicSprite(State.ImagePath + "dash debuff.png"));

        public static readonly StateData RegenerationBuff = new StateData(2000, (owner, ms, arg) =>
        {
            var player = (PlayerCharacter) owner;
            player.Heals(ms * Spells.RegenerationBuffValue * player.Stats.SpellPowerRatio() / 2000.0);
        }, null, new BasicSprite(State.ImagePath + "couronne green.png"));

        public static readonly StateData VampirismBuff = new StateData(2000, null, (owner, arg) =>
        {
            var player = (PlayerCharacter) owner;
            player.Stats.Vampirism -= Spells.VampirismBuffValue;
        }, new BasicSprite(State.ImagePath + "couronne red.png"));

        public static readonly StateData StrengthBuff = new StateData(2000, null, (owner, arg) =>
        {
            var player = (PlayerCharacter) owner;
            player.Stats.Strength -= Spells.StrengthBuffValue;
        }, new BasicSprite(State.ImagePath + "couronne gold.png"));

        public static readonly StateData InvincibilityBuff = new StateData(1500, null, (owner, arg) =>
        {
            var player = (PlayerCharacter) owner;
            player.Stats.Armor -= Spells.InvincibilityBuffValue;
        }, new BasicSprite(State.ImagePath + "couronne blue.png"));

        public static readonly StateData CthulhuChainsInit = new StateData(1000000, (actor, ms, arg) =>
        {
            var caster = (CthulhuGhost) arg;
            if (caster.IsDead())
            {
                actor.CleanStates();
            }
        }, (actor, arg) =>
        {
            actor.Speed = Knight.InitialSpeed;
        } , new BasicSprite(State.ImagePath + "chains init.png"));

        public static readonly StateData CthulhuChainsDebuff = new StateData(10000, null, (actor, arg) =>
        {
            var caster = (CthulhuGhost) arg;
            // Création récursive de l'effet
            if (!caster.IsDead())
            {
                actor.Speed -= (int) (actor.Speed * Spells.CthulhuChainsReductionPercent);
                actor.CreateState(CthulhuChainsDebuff, arg);
            }
        } , new BasicSprite(State.ImagePath + "chains debuff.png"));

        public static readonly StateData CthulhuMeteor = new StateData(500, (actor, ms, arg) =>
        {
            var data = (StateArgMeteor) arg;
            var vector = data.Vector * (500 - data.MsElapsed) * ms / 1000.0;
            data.MsElapsed += ms;
            actor.Move(vector * ms);
        });

        public static readonly StateData CthulhuVoidball = new StateData(2000, (caster, ms, arg) =>
        {
            const int num = Spells.CthulhuVoidballNumber;
            const int msCast = 800;

            var data = (StateArgVoidball) arg;

            foreach (var voidball in data.Voidballs)
            {
                voidball.Speed += (int) (ms / 2.5);
            }
            data.MsElapsed += ms;

            if (data.MsElapsed <= msCast / num ||
                data.Voidballs.Count >= num) return;

            data.MsElapsed -= msCast / num;
            var coef1 = (num - data.Voidballs.Count) / (double) num;
            var coef2 = data.Voidballs.Count / (double) num;
            var target = caster.Position + data.BeginPoint * coef1 + data.EndingPoint * coef2;
            var missile = new Missile(caster, target, Actors.CthulhuVoidball()) {Speed = 0};
            missile.Move(caster.Position - caster.Muzzle());
            data.Voidballs.Add(missile);
        });
    }
}