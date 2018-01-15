
using System.Collections.Generic;
using System.Drawing;
using TentacleSlicers.actors;
using TentacleSlicers.graphics;
using TentacleSlicers.maps;
using TentacleSlicers.spells;
using Point = TentacleSlicers.general.Point;

namespace TentacleSlicers.customs
{
    /// <summary>
    /// Classe statique qui rescence les sorts du jeu.
    /// </summary>
    public class Spells
    {
        private Spells() {}

        /// <summary>
        /// Utilisé pour charger au lancement du programme les spell data.
        /// Afin de conserver la comptabilité avec les sauvegardes précédentes, les nouveaux sorts doivent être
        /// rajoutés à la suite de ceux déjà présents dans la fonction.
        /// </summary>
        public static void Load()
        {
            GreenCultistAttack();
            OrangeCultistAttack();
            OrangeCultistFireball();

            PlayerAttack();
            RegenerationBuff();
            VampirismBuff();
            StrengthBuff();
            InvincibilityBuff();
            Holyball();
            Impact();
            Arrow();
            Bomb();
            Dash();
        }

        // Attaque des joueurs

        private static SpellData _playerAttack;
        /// <summary>
        /// Retourne l'attaque au corps-à-corps des joueurs.
        /// </summary>
        /// <returns> Le sort </returns>
        public static SpellData PlayerAttack()
        {
            if (_playerAttack == null)
            {
                _playerAttack = new AttackSpellData(50, 50, 500, false);
            }
            return _playerAttack;
        }

        // Liste des sorts des powerups

        private static SpellData _holyball;
        /// <summary>
        /// Retourne le sort HolyBall.
        /// Celui-ci possède 3 charges et permet de lancer un unique projectile après une canalisation.
        /// </summary>
        /// <returns> Le sort </returns>
        public static SpellData Holyball()
        {
            if (_holyball == null)
            {
                _holyball = new MissileSpellData(Actors.Holyball(), 3, 450, false, 3,
                    new Bitmap(Spell.IconPath + "holyball.png"));
            }
            return _holyball;
        }

        private static SpellData _arrow;
        public const int ArrowTimeValue = 700;
        /// <summary>
        /// Retourne le sort Arrow.
        /// Celui-ci possède 2 charges qui déclenchent avec un léger décalage le lancement de deux projectiles dans
        /// la même direction. L'utilisateur peut se déplacer pendant sa canalisation.
        /// </summary>
        /// <returns> Le sort </returns>
        public static SpellData Arrow()
        {
            if (_arrow == null)
            {
                _arrow = new SpellData((owner, target) =>
                {
                    owner.CreateState(States.ArrowBuff, new StateArgArrow { Orientation = owner.Orientation });
                }, AnimationType.Cast, 4, ArrowTimeValue, 0, true, 2, 4242, new Bitmap(Spell.IconPath + "arrow.png"));
            }
            return _arrow;
        }

        private static SpellData _impact;
        /// <summary>
        /// Retourne le sort Impact.
        /// Celui-ci déclenche en face de l'utilisateur, après un délai, une explosion qui blesse les ennemis pris dans
        /// son rayon et les empêche d'attaquer ou de se déplacer pendant un court instant.
        /// </summary>
        /// <returns> Le sort </returns>
        public static SpellData Impact()
        {
            if (_impact == null)
            {
                _impact = new SpellData((owner, target) =>
                {
                    new Explosion(owner.Muzzle(120), (LivingActor) owner, Actors.Impact());
                }, AnimationType.Cast, 5, 600, 0.33, false, 1, 4242, new Bitmap(Spell.IconPath + "impact.png"));
            }
            return _impact;
        }

        private static SpellData _bomb;
        /// <summary>
        /// Retourne le sort Bomb.
        /// Celui-ci permet de lancer, après une longue canalisation, un gros projectile qui explose lors du premier
        /// contact ou lorsqu'il expire et blesse les ennemis dans une grande zone autour de lui, et les empêche de
        /// bouger pendant un court instant.
        /// </summary>
        /// <returns> Le sort </returns>
        public static SpellData Bomb()
        {
            if (_bomb == null)
            {
                _bomb = new SpellData((owner, target) =>
                {
                    new Bomb(owner, owner.Muzzle());
                }, AnimationType.Cast, 6, 800, 1, false, 1, 4242, new Bitmap(Spell.IconPath + "bomb.png"));
            }
            return _bomb;
        }

        private static SpellData _dash;
        public const int DashSpeedValue = 200;
        public const int DashArmorValue = 100000;
        /// <summary>
        /// Retourne le sort Dash.
        /// Celui-ci, à l'activation, accélère et rend son utilisateur invincible pendant un court instant et l'entoure
        /// d'une aura qui va ensuite brûler tous les ennemis à proximité durant l'effet, ce qui va leur faire perdre
        /// de la vie pendant quelques secondes.
        /// </summary>
        /// <returns> Le sort </returns>
        public static SpellData Dash()
        {
            if (_dash == null)
            {
                _dash = new SpellData((owner, target) =>
                {
                    owner.Speed += DashSpeedValue;
                    ((PlayerCharacter) owner).Stats.Armor += DashArmorValue;
                    owner.LockCasts();
                    owner.CreateState(States.DashBuff, new List<LivingActor>());
                }, AnimationType.Stand, 7, 0, 1, true, 1, 4242, new Bitmap(Spell.IconPath + "dash.png"));
            }
            return _dash;
        }

        private static SpellData _regenerationBuff;
        public const int RegenerationBuffValue = 30;
        /// <summary>
        /// Retourne le sort RegenerationBuff.
        /// Celui-ci, à l'activation, soigne son utilisateur sur une petite période.
        /// </summary>
        /// <returns> Le sort </returns>
        public static SpellData RegenerationBuff()
        {
            if (_regenerationBuff == null)
            {
                _regenerationBuff = new SpellData((caster, target) =>
                {
                    ((PlayerCharacter) caster).CreateState(States.RegenerationBuff);
                }, AnimationType.Stand, 5, 0, 0, true, 1, 0, new Bitmap(Spell.IconPath + "couronne green.png"));
            }
            return _regenerationBuff;
        }

        private static SpellData _vampirismBuff;
        public const int VampirismBuffValue = 50;
        /// <summary>
        /// Retourne le sort VampirismBuff.
        /// Celui-ci, à l'activation, augmente énormément le score de vampirisme de son utilisateur pendant un court
        /// instant.
        /// </summary>
        /// <returns> Le sort </returns>
        public static SpellData VampirismBuff()
        {
            if (_vampirismBuff == null)
            {
                _vampirismBuff = new SpellData((caster, target) =>
                {
                    var player = (PlayerCharacter) caster;
                    player.Stats.Vampirism += VampirismBuffValue;
                    player.CreateState(States.VampirismBuff);
                }, AnimationType.Stand, 5, 0, 0, true, 1, 0, new Bitmap(Spell.IconPath + "couronne red.png"));
            }
            return _vampirismBuff;
        }

        private static SpellData _strengthBuff;
        public const int StrengthBuffValue = 100;
        /// <summary>
        /// Retourne le sort StrengthBuff.
        /// Celui-ci, à l'activation, augmente énormément la force physique de son utilisateur pendant un court instant.
        /// </summary>
        /// <returns> Le sort </returns>
        public static SpellData StrengthBuff()
        {
            if (_strengthBuff == null)
            {
                _strengthBuff = new SpellData((caster, target) =>
                {
                    var player = (PlayerCharacter) caster;
                    player.Stats.Strength += StrengthBuffValue;
                    player.CreateState(States.StrengthBuff);
                }, AnimationType.Stand, 5, 0, 0, true, 1, 0, new Bitmap(Spell.IconPath + "couronne gold.png"));
            }
            return _strengthBuff;
        }

        private static SpellData _invincibilityBuff;
        public const int InvincibilityBuffValue = 100000000;
        /// <summary>
        /// Retourne le sort StrengthBuff.
        /// Celui-ci, à l'activation, rend son utilisateur invincible pendant un court instant.
        /// </summary>
        /// <returns> Le sort </returns>
        public static SpellData InvincibilityBuff()
        {
            if (_invincibilityBuff == null)
            {
                _invincibilityBuff = new SpellData((caster, target) =>
                {
                    var player = (PlayerCharacter) caster;
                    player.Stats.Armor += InvincibilityBuffValue;
                    player.CreateState(States.InvincibilityBuff);
                }, AnimationType.Stand, 5, 0, 0, true, 1, 0, new Bitmap(Spell.IconPath + "couronne blue.png"));
            }
            return _invincibilityBuff;
        }

        // Liste des sorts des mobs

        private static SpellData _greenCultistAttack;
        /// <summary>
        /// Retourne l'attaque au corps-à-corps des cultistes verts.
        /// L'attaque au corps-à-corps des cultistes verts et plutôt rapide et inflige de bons dégâts.
        /// </summary>
        /// <returns> Le sort </returns>
        public static SpellData GreenCultistAttack()
        {
            if (_greenCultistAttack == null)
            {
                _greenCultistAttack = new AttackSpellData(25, 50, 550, false);
            }
            return _greenCultistAttack;
        }

        private static SpellData _orangeCultistAttack;
        /// <summary>
        /// Retourne l'attaque au corps-à-corps des cultistes oranges.
        /// L'attaque au corps-à-corps des cultistes oranges est plus lente et moins efficace que celle des cultistes
        /// verts.
        /// </summary>
        /// <returns> Le sort </returns>
        public static SpellData OrangeCultistAttack()
        {
            if (_orangeCultistAttack == null)
            {
                _orangeCultistAttack = new AttackSpellData(15, 50, 800, false);
            }
            return _orangeCultistAttack;
        }

        private static SpellData _orangeCultistFireball;
        /// <summary>
        /// Retourne le sort d'atatque à ditance des cultistes oranges.
        /// Cette attaque consiste en un projectile (une boule de feu) projeté dans la direction de la cible du
        /// cultiste après une canalisation.
        /// </summary>
        /// <returns> Le sort </returns>
        public static SpellData OrangeCultistFireball()
        {
            if (_orangeCultistFireball == null)
            {
                _orangeCultistFireball = new MissileSpellData(Actors.CultistFireball(), 1.2, 700, false);
            }
            return _orangeCultistFireball;
        }

        // Liste des sorts du boss


        private static SpellData _cthulhuLightning;
        /// <summary>
        /// Retourne un sort d'attaque à distance du boss.
        /// Celui-ci inflige des dégâts dans une zone après un certain délai.
        /// </summary>
        /// <returns> Le sort </returns>
        public static SpellData CthulhuLightning()
        {
            if (_cthulhuLightning == null)
            {
                _cthulhuLightning = new SpellData((owner, arg) =>
                {
                    var players = World.GetWorld().GetPlayers();
                    var numPlayer = World.Random.Next(2);
                    if (numPlayer >= players.Count) return;
                    var player = players[numPlayer];
                    var target = player.Position + player.SpeedVector * 0.3;

                    var numLightning = World.Random.Next(2);
                    if (numLightning == 0)
                    {
                        new Explosion(target, (LivingActor) owner, Actors.Lightning1());
                    }
                    else
                    {
                        new Explosion(target, (LivingActor) owner, Actors.Lightning2());
                    }
                }, AnimationType.Stand, 1.6, 0, 0, true, 1, 4000);
            }
            return _cthulhuLightning;
        }

        private static SpellData _cthulhuChains;
        public const double CthulhuChainsReductionPercent = 0.05;
        /// <summary>
        /// Retourne un sort d'entrave à distance du boss.
        /// Celui-ci ralentit de plus en plus les joueurs, où qu'ils soient.
        /// </summary>
        /// <returns> Le sort </returns>
        public static SpellData CthulhuChains()
        {
            if (_cthulhuChains == null)
            {
                _cthulhuChains = new SpellData((owner, arg) =>
                {
                    var caster = (CthulhuGhost) owner;
                    var players = World.GetWorld().GetPlayers();
                    if (caster.ChainsStacks == 0)
                    {
                        foreach (var player in players)
                        {
                            player.Speed -= 40;
                            player.CreateState(States.CthulhuChainsInit, owner);
                        }
                    }
                    else
                    {
                        foreach (var player in players)
                        {
                            player.Speed -= (int) (player.Speed * CthulhuChainsReductionPercent);
                            player.CreateState(States.CthulhuChainsDebuff, owner);
                        }
                    }
                    ++caster.ChainsStacks;
                }, AnimationType.Stand, 4.2, 0, 0, true, 1, 4000);
            }
            return _cthulhuChains;
        }

        private static SpellData _cthulhuMeteor;
        public const int CthulhuMeteorDispersion = 300;
        /// <summary>
        /// Retourne un sort d'entrave à distance du boss.
        /// Celui-ci ralentit de plus en plus les joueurs, où qu'ils soient.
        /// </summary>
        /// <returns> Le sort </returns>
        public static SpellData CthulhuMeteor()
        {
            if (_cthulhuMeteor == null)
            {
                _cthulhuMeteor = new SpellData((owner, arg) =>
                {
                    var players = World.GetWorld().GetPlayers();
                    var numPlayer = World.Random.Next(players.Count);
                    var player = players[numPlayer];
                    var shiftX = World.Random.Next(CthulhuMeteorDispersion * 2) - CthulhuMeteorDispersion;
                    var shiftY = World.Random.Next(CthulhuMeteorDispersion * 2) - CthulhuMeteorDispersion;
                    var position = player.Position + new Point(shiftX, shiftY);
                    new Explosion(position, (CthulhuGhost) owner, Actors.Meteor());
                }, AnimationType.Stand, 0.65, 0, 0, true, 1, 4000);
            }
            return _cthulhuMeteor;
        }

        private static SpellData _cthulhuVoidball;
        public const double CthulhuVoidballDispersion = 0.5;
        public const int CthulhuVoidballNumber = 6;
        /// <summary>
        /// Retourne un sort d'entrave à distance du boss.
        /// Celui-ci ralentit de plus en plus les joueurs, où qu'ils soient.
        /// </summary>
        /// <returns> Le sort </returns>
        public static SpellData CthulhuVoidball()
        {
            if (_cthulhuVoidball == null)
            {
                const double coef = CthulhuVoidballDispersion / 2;
                _cthulhuVoidball = new SpellData((owner, target) =>
                {
                    var o = owner.Orientation;
                    var p1 = o + new Point(o.Y, -o.X);
                    var p2 = o + new Point(-o.Y, o.X);
                    owner.CreateState(States.CthulhuVoidball, new StateArgVoidball
                    {
                        BeginPoint =  new Point(p1 * (0.5 + coef) + p2 * (0.5 - coef)),
                        EndingPoint = new Point(p1 * (0.5 - coef) + p2 * (0.5 + coef)),
                        Voidballs = new List<Missile>(),
                        MsElapsed = 0
                    });
                }, AnimationType.Stand, 3.8, 0, 0, true, 1, 4000);
            }
            return _cthulhuVoidball;
        }
    }
}