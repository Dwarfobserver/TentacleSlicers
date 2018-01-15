
namespace TentacleSlicers.actors
{
    /// <summary>
    /// Stocke les statistiques d'un joueur, qui vont augmenter son efficacité.
    /// Les statistiques peuvent être enregistrées et chargées depuis une chaîne de caractères.
    /// </summary>
    public class PlayerStats
    {
        private const double BaseValue = 100;

        public int Armor;
        public int Strength;
        public int SpellPower;
        public int Vampirism;
        public int CooldownReduction;

        /// <summary>
        /// Crée de nouvelles statistiques, initialisées à zéro.
        /// </summary>
        public PlayerStats()
        {
            Armor = 0;
            Strength = 0;
            SpellPower = 0;
            Vampirism = 0;
            CooldownReduction = 0;
        }

        /// <summary>
        /// Crée de nouvelles statistiques en copiant celles indiquées.
        /// </summary>
        /// <param name="clone"> Les statistiques recopiées </param>
        public PlayerStats(PlayerStats clone)
        {
            Armor = clone.Armor;
            Strength = clone.Strength;
            SpellPower = clone.SpellPower;
            Vampirism = clone.Vampirism;
            CooldownReduction = clone.CooldownReduction;
        }

        /// <summary>
        /// Retourne le coefficient de réduction des dégâts subis.
        /// </summary>
        /// <returns> Le coefficient </returns>
        public double DamagesTakenRatio()
        {
            return BaseValue / (Armor + BaseValue);
        }

        /// <summary>
        /// Retourne le coefficient de réduction des temps de rechargement des sorts.
        /// </summary>
        /// <returns> Le coefficient </returns>
        public double CooldownRatio()
        {
            return BaseValue / (CooldownReduction + BaseValue);
        }

        /// <summary>
        /// Retourne le coefficient d'augmentation des dégâts physiques.
        /// </summary>
        /// <returns> Le coefficient </returns>
        public double PhysicalDamagesRatio()
        {
            return (Strength + BaseValue) / BaseValue;
        }

        /// <summary>
        /// Retourne le coefficient d'augmentation d'efficacité des sorts.
        /// </summary>
        /// <returns> Le coefficient </returns>
        public double SpellPowerRatio()
        {
            return (SpellPower + BaseValue) / BaseValue;
        }

        /// <summary>
        /// Retourne le pourcentage de vie récupérée par rapport aux dégâts infligés.
        /// </summary>
        /// <returns> Le pourcentage </returns>
        public double VampirismRatio()
        {
            return Vampirism / BaseValue;
        }

        /// <summary>
        /// Enregistre les statistiques dans une chaîne de caractère.
        /// </summary>
        /// <returns> La chaîne de caractères </returns>
        public string Save()
        {
            return "" + Armor + " " + Strength + " " + SpellPower + " " + Vampirism + " " + CooldownReduction;
        }

        /// <summary>
        /// Récupère les statistiques sauvegardées dans une chaîne de caractères.
        /// </summary>
        /// <param name="data"> La chaîne de caractères </param>
        /// <returns> Les statistiques </returns>
        public static PlayerStats Load(string data)
        {
            var arg = data.Split(' ');
            var stats = new PlayerStats
            {
                Armor = int.Parse(arg[0]),
                Strength = int.Parse(arg[1]),
                SpellPower = int.Parse(arg[2]),
                Vampirism = int.Parse(arg[3]),
                CooldownReduction = int.Parse(arg[4])
            };
            return stats;
        }
    }
}