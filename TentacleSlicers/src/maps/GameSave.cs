
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TentacleSlicers.actors;
using TentacleSlicers.spells;

namespace TentacleSlicers.maps
{
    /// <summary>
    /// Stocke les sauvegardes existantes et permet de créer et charger des sauvegardes.
    /// Les sauvegardes conservent l'état de la partie du début du niveau.
    /// La création d'une sauvegarde est détaillé dans le dossier resources/saves avec le fichier reamde.
    /// </summary>
    public class GameSave
    {
        public const string SavesPath = "../../resources/saves/";
        public const string HighScorePath = SavesPath + "highscore";
        public const int NbMax = 9;
        public static List<GameSave> Saves = LoadSaves();
        private const string DefaultName = "Sans nom";

        public static int HighScore
        {
            get { return _highScore; }
            set
            {
                File.WriteAllText(HighScorePath, "" + value);
                _highScore = value;
            }
        }
        private static int _highScore;

        /// <summary>
        /// Actualise les sauvegardes stockées.
        /// </summary>
        public static void Load()
        {
            Saves = LoadSaves();
            using (var sr = new StreamReader(HighScorePath))
            {
                _highScore = int.Parse(sr.ReadLine());
            }
        }

        /// <summary>
        /// Charge tous les sorts pour pouvoir les retrouver depuis l'identifiant stocé dans les sauvegardes.
        /// Puis, charge tous les fichiers du dossier saves et les retourne sous forme de GameSave.
        /// </summary>
        /// <returns> La nouvelle liste de sauvegardes stockées </returns>
        private static List<GameSave> LoadSaves()
        {
            customs.Spells.Load();

            var saves = Directory.EnumerateFiles(SavesPath, "*.save").ToArray();
            var names = new List<GameSave>(saves.Length);

            var i = 0;
            while (i < saves.Length)
            {
                names.Add(new GameSave(saves[i]));
                ++i;
            }
            return names;
        }

        /// <summary>
        /// Enregistre la sauvegarde correspondant a l'état de la partie au début du niveau et lui associe le nom donné.
        /// Sauvegarde donc le numéro de carte, le niveau, le nom et les statistiques et sorts de chaque joueur.
        /// </summary>
        /// <param name="name"> Le nom de la sauvegarde </param>
        public static void Add(string name)
        {
            var world = World.GetWorld();
            var players = world.GetPlayers();
            if (name == "") name = DefaultName;
            var save = name + '\n' +
                       world.Map.Num + '\n' +
                       world.Level + '\n' +
                       world.InitialScore + '\n' +
                       players.Count;
            var i = 0;
            while (i < players.Count)
            {
                save += '\n' + players[i].SavableStats.Save() + '\n';
                for (var j = 0; j < players[i].GetNbSpells(); ++j)
                {
                    save += players[i].GetSpell(j).Id + " ";
                }
                ++i;
            }

            File.WriteAllText(SavesPath + Saves.Count + ".save", save);
        }

        public string Name { get; }
        public int NumMap { get; }
        public int Level { get; }
        public int Score { get; }
        public int NumberOfPlayers { get; }
        public List<PlayerStats> Stats { get; }
        public List<List<SpellData>> Spells { get; }

        /// <summary>
        /// Retire les informations du fichier de sauvegarde donné pour obtenir le nom de la sauvegarde, le numéro de
        /// la carte, le niveau et score de la partie et, pour chaque joueur, ses statistiques et ses sorts.
        /// </summary>
        /// <param name="filePath"> Le fichier de sauvegarde </param>
        private GameSave(string filePath)
        {
            var file = new StreamReader(filePath);
            var line = file.ReadLine();

            // Debute par le nom de la sauvegarde
            Name = line;

            Stats = new List<PlayerStats>();
            Spells = new List<List<SpellData>>();

            var numLine = 0;
            var numPlayer = 0;
            while ((line = file.ReadLine()) != null)
            {
                ++numLine;
                // Ligne 1 : numéro de la map
                if (numLine == 1)
                {
                    NumMap = int.Parse(line);
                }
                // Ligne 2 : niveau de la sauvegarde
                else if (numLine == 2)
                {
                    Level = int.Parse(line);
                }
                // Ligne 3 : score initial des joueurs
                else if (numLine == 3)
                {
                    Score = int.Parse(line);
                }
                // Ligne 3 : nombre de joueurs
                else if (numLine == 4)
                {
                    NumberOfPlayers = int.Parse(line);
                    for (var i = 0; i < NumberOfPlayers; ++i)
                    {
                        Spells.Add(new List<SpellData>());
                    }
                }
                // Permière ligne des données d'un joueur : ses statistiques
                else if ((numLine - 5) % 2 == 0)
                {
                    Stats.Add(PlayerStats.Load(line));
                }
                // Seconde ligne des données d'un joueur : ses sorts
                else if ((numLine - 5) % 2 == 1)
                {
                    var args = line.Split(' ');
                    foreach (var id in args)
                    {
                        if (id != "") Spells[numPlayer].Add(SpellData.GetSpellData(long.Parse(id)));
                    }
                    ++numPlayer;
                }
            }
            file.Close();
        }
    }
}