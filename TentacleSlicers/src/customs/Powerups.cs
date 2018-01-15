using System.Collections.Generic;
using TentacleSlicers.actors;
using TentacleSlicers.graphics;

namespace TentacleSlicers.customs
{
    /// <summary>
    /// Classe statique qui rescence les powerups du jeu.
    /// Liste également les powerups qui peuvent être donnés à chaque changement de niveau et ceux qui peuvent être
    /// lâchés par les mobs à leur mort.
    /// </summary>
    public class Powerups
    {
        private Powerups() {}

        // Powerup de sorts

        private static List<PowerupData> _spellPowerups;
        public static List<PowerupData> SpellPowerups()
        {
            if (_spellPowerups == null)
            {
                _spellPowerups = new List<PowerupData>
                {
                    Dash,
                    Bomb,
                    Holyball,
                    Impact,
                    Arrow,/*
                    RegenerationBuff,
                    VampirismBuff,
                    StrengthBuff,
                    InvincibilityBuff*/
                };
            }
            return _spellPowerups;
        }

        public static readonly PowerupData Holyball = new PowerupData(
            new BasicSprite(Powerup.ImagePath + "holyball.png"), Spells.Holyball());

        public static readonly PowerupData Arrow = new PowerupData(
            new BasicSprite(Powerup.ImagePath + "arrow.png"), Spells.Arrow());

        public static readonly PowerupData Impact = new PowerupData(
            new BasicSprite(Powerup.ImagePath + "impact.png"), Spells.Impact());

        public static readonly PowerupData Bomb = new PowerupData(
            new BasicSprite(Powerup.ImagePath + "bomb.png"), Spells.Bomb());

        public static readonly PowerupData Dash = new PowerupData(
            new BasicSprite(Powerup.ImagePath + "dash.png"), Spells.Dash());

        public static readonly PowerupData RegenerationBuff = new PowerupData(
            new BasicSprite(Powerup.ImagePath + "couronne green.png"), Spells.RegenerationBuff());

        public static readonly PowerupData VampirismBuff = new PowerupData(
            new BasicSprite(Powerup.ImagePath + "couronne red.png"), Spells.VampirismBuff());

        public static readonly PowerupData StrengthBuff = new PowerupData(
            new BasicSprite(Powerup.ImagePath + "couronne gold.png"), Spells.StrengthBuff());

        public static readonly PowerupData InvincibilityBuff = new PowerupData(
            new BasicSprite(Powerup.ImagePath + "couronne blue.png"), Spells.InvincibilityBuff());

        // Powerup de statistiques

        private static List<PowerupData> _passivePowerups;
        public static List<PowerupData> PassivePowerups()
        {
            if (_passivePowerups == null)
            {
                _passivePowerups = new List<PowerupData>
                {
                    BonusVampirism,
                    BonusCooldownReduction,
                    BonusStrength,
                    BonusArmor,
                    BonusSpellpower,
                    HealFullLife
                };
            }
            return _passivePowerups;
        }

        public static readonly PowerupData BonusVampirism = new PowerupData(
            new BasicSprite(Powerup.ImagePath + "bonus vampirism.png"), character =>
            {
                character.Stats.Vampirism += 5;
            });

        public static readonly PowerupData BonusCooldownReduction = new PowerupData(
            new BasicSprite(Powerup.ImagePath + "bonus cooldown reduction.png"), character =>
            {
                character.Stats.CooldownReduction += 15;
            });

        public static readonly PowerupData BonusStrength = new PowerupData(
            new BasicSprite(Powerup.ImagePath + "bonus strength.png"), character =>

            {
                character.Stats.Strength += 20;
            });

        public static readonly PowerupData BonusArmor = new PowerupData(
            new BasicSprite(Powerup.ImagePath + "bonus armor.png"), character =>
            {
                character.Stats.Armor += 20;
            });

        public static readonly PowerupData BonusSpellpower = new PowerupData(
            new BasicSprite(Powerup.ImagePath + "bonus spell power.png"), character =>
            {
                character.Stats.SpellPower += 15;
            });

        public static readonly PowerupData HealFullLife = new PowerupData(
            new BasicSprite(Powerup.ImagePath + "heal full life.png"), character =>
            {
                character.Heals(character.LifeMax());
            });
    }
}