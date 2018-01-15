using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TentacleSlicers.actors;
using TentacleSlicers.customs;
using TentacleSlicers.hud;
using TentacleSlicers.interfaces;
using TentacleSlicers.levels;
using TentacleSlicers.spells;
using TentacleSlicers.windows;
using Point = TentacleSlicers.general.Point;

namespace TentacleSlicers.maps
{
    /// <summary>
    /// Le monde est le conteneur de tous les aceurs et de la map générée, et leur délègue les fonctions Tick et Draw.
    /// Il peut être crée à partir d'une sauvegarde, il actualise le déroulement des niveaux et il place la caméra.
    /// </summary>
    public class World : IDrawable, ITickable
    {
        private static World _instance;

        public const int MaxGameOverDelay = 1000;
        public static readonly Random Random = new Random();

        private const double DifficultyLifeRatio = 1.6;
        private const double DifficultyDamagesRatio = 1.6;

        /// <summary>
        /// Charge le monde à partir d'une sauvegarde.
        /// </summary>
        /// <param name="save"> La sauvegarde </param>
        public static void Load(GameSave save)
        {
            _instance = new World(save);
            _instance.InitMap(save.NumMap);
            _instance.InitPlayers(save.NumberOfPlayers, save);
            _instance.NextLevel(false);
        }

        /// <summary>
        /// Initialise le monde avec un numéro de map et un nombre de joueurs.
        /// </summary>
        /// <param name="numMap"> Le numéro de map </param>
        /// <param name="nbPlayers"> Le nombre de joueurs </param>
        public static void Load(int numMap, int nbPlayers)
        {
            _instance = new World();
            _instance.InitMap(numMap);
            _instance.InitPlayers(nbPlayers);
            _instance.NextLevel(true);
        }

        /// <summary>
        /// Retourne le monde actuel.
        /// </summary>
        /// <returns> Le monde </returns>
        public static World GetWorld()
        {
            return _instance;
        }

        public int InitialScore { get; private set; }
        public int Score { get; private set; }
        public int Level { get; private set; }
        public Map Map { get; private set; }

        private readonly SortedSet<Actor> _actors;
        private readonly List<PlayerCharacter> _players;
        private Point _camera;
        private LevelScript[] _levelScript;
        private LevelObserver _levelObserver;
        private int _spellPowerups;
        private bool _levelHasBegun;
        private Mob _boss;

        public int GameOverDelay { get; private set; }

        /// <summary>
        /// Initialise le monde avec le niveau de la sauvegarde si elle existe, sinon 1.
        /// </summary>
        /// <param name="save"> La potentielle sauvegarde </param>
        private World(GameSave save = null)
        {
            _actors = new SortedSet<Actor>();
            _players = new List<PlayerCharacter>();
            Level = save?.Level - 1 ?? 0;
            Score = save?.Score ?? 0;
            InitialScore = Score;
            GameOverDelay = 0;
        }

        /// <summary>
        /// Initialise la map avec le numéro indiqué.
        /// </summary>
        /// <param name="numMap"> Le numéro de la map </param>
        private void InitMap(int numMap)
        {
            Map = new Map(numMap);
        }

        /// <summary>
        /// Initialise les joueurs avec une potentielle sauvegarde et leur nombre.
        /// </summary>
        /// <param name="nbPlayers"> Le nombre de joueurs </param>
        /// <param name="save"> La potentielle sauvegarde </param>
        private void InitPlayers(int nbPlayers, GameSave save = null)
        {
            for (var i = 0; i < nbPlayers; ++i)
            {
                if (save == null)
                {
                    _players.Add(new Knight(Map.PlayerSpawns[i], i));
                }
                else
                {
                    _players.Add(new Knight(Map.PlayerSpawns[i], i, save.Stats[i]));
                    var numSpell = 0;
                    while (numSpell < save.Spells[i].Count)
                    {
                        _players[i].CreateSpell(numSpell, SpellData.GetSpellData(save.Spells[i][numSpell].Id));
                        ++numSpell;
                    }
                }
            }
        }

        /// <summary>
        /// Centre la caméra par rapport aux joueurs vivants.
        /// </summary>
        private void CenterCamera()
        {
            var camera = Point.Null;
            var numPlayersAlive = 0;
            foreach (var player in _players)
            {
                if (player.IsDead()) continue;
                camera += player.Position;
                numPlayersAlive++;
            }
            if (numPlayersAlive > 0)
            {
                camera /= numPlayersAlive;
                camera -= new Point(MainForm.FixedWidth / 2.0, MainForm.FixedHeight / 2.0);
                _camera = camera;
            }
        }

        public void AddScore(int points)
        {
            Score += points;
        }

        /// <summary>
        /// Renvoi le facteur qui augmente les points de vie des mobs, en fonction des niveaux.
        /// </summary>
        /// <returns> Le facteur de point de vie </returns>
        public double LifeRatio()
        {
            return 1 + (Level - 1) * (DifficultyLifeRatio - 1);
        }

        /// <summary>
        /// Renvoi le facteur qui augmente les dégâts des mobs, en fonction des niveaux.
        /// </summary>
        /// <returns> Le facteur de dégâts </returns>
        public double DamagesRatio()
        {
            return 1 + (Level - 1) * (DifficultyDamagesRatio - 1);
        }

        /// <summary>
        /// Renvoie le facteur de vitesse de déplacement et de lancement de sorts des mobs, en fonction des niveaux.
        /// </summary>
        /// <returns> Le facteur de vitesse </returns>
        public double SpeedRatio()
        {
            return 1.1 * (3 + Level) / (4.0 + Level);
        }

        /// <summary>
        /// Retourne un composant de l'interface ayant accès aux données du niveau en train de se dérouler.
        /// </summary>
        /// <param name="position"> La position du composant </param>
        /// <returns> Le composant de l'interface </returns>
        public LevelObserver GetLevelObserver(Point position)
        {
            return _levelObserver ?? (_levelObserver = new LevelObserver(position, _levelScript[0]));
        }

        /// <summary>
        /// Passe au niveau suivant. Recrée les étapes du niveaux, rénitialise le composant d'interface, et sauvegarde
        /// les statistiques des joueurs, en effaçant leurs effets temporaires.
        /// Si des powerups sont crées, attend qu'ils soient consommés pour passer au niveau suivant.
        /// </summary>
        /// <param name="createPowerups"> Indique si un SpellPowerup par joueur est crée </param>
        private void NextLevel(bool createPowerups)
        {
            InitialScore = Score;

            ++Level;
            _levelHasBegun = false;
            _levelObserver = null;
            _levelScript = new LevelScript[_players.Count];
            for (var i = 0; i < _players.Count; ++i)
            {
                _levelScript[i] = new LevelScript();
            }
            var steps = new List<LevelStep>[2];
            for (var i = 0; i < _players.Count; ++i)
            {
                steps[i] =  LevelSteps.CreateSteps(Map.Gateways);
            }
            for (var i = 0; i < _players.Count; ++i)
            {
                foreach (var step in steps[i])
                {
                    _levelScript[i].AddStep(step);
                }
            }

            // Création des powerups
            if (createPowerups) for (var i = 0; i < _players.Count; ++i)
            {
                var powerups = Powerups.SpellPowerups();
                Powerup.Create(Map.BossSpawn + new Point(_spellPowerups * 100 - 50, 0),
                    powerups[Random.Next(powerups.Count)]);
                ++_spellPowerups;
            }
        }

        /// <summary>
        /// Ajoute un acteur au monde, qui va appeler ses fonctions Tick et Draw.
        /// Lorsqu'un acteur s'est déplacé, il est retiré puis replacé dans le monde car les acteurs sont triés en
        /// fonction de leur ordonnée, pour les dessiner dans l'ordre.
        /// </summary>
        /// <param name="actor"> L'acteur ajouté au monde </param>
        public void AddActor(Actor actor)
        {
            _actors.Add(actor);
        }

        /// <summary>
        /// Retire un acteur du monde, parce qu'il est mort ou parce qu'il va se déplacer et être remis dans le monde.
        /// </summary>
        /// <param name="actor"> L'acteur retiré du monde </param>
        public void RemoveActor(Actor actor)
        {
            _actors.Remove(actor);
        }

        /// <summary>
        /// Retire un spell powerup, ce qui peut permettre le lancement du niveau.
        /// </summary>
        public void RemoveSpellPowerup()
        {
            --_spellPowerups;
        }

        /// <summary>
        /// Retourne le liste des joueurs.
        /// </summary>
        /// <returns> La liste des joueurs </returns>
        public List<PlayerCharacter> GetPlayers()
        {
            return _players;
        }

        /// <summary>
        /// Retourne la liste des joueurs en vie.
        /// </summary>
        /// <returns> la liste des joueurs vivants </returns>
        public List<PlayerCharacter> GetAlivePlayers()
        {
            return _players.Where(player => !player.IsDead()).ToList();
        }

        /// <summary>
        /// Dessine les acteurs du monde, triés par ordonnée croissante (ils sont dessinés de haut en bas).
        /// </summary>
        /// <param name="shift"> La position de décalage, inutilisée </param>
        /// <param name="g"> L'objet permettant de dessiner </param>
        public void Draw(Point shift, Graphics g)
        {
            Map.Draw(-_camera, g);
            foreach (var actor in _actors)
            {
                actor.Draw(-_camera, g);
            }
        }

        /// <summary>
        /// Vérifie si tous les acteurs sont morts.
        /// Sinon, centre la caméra, fait avancer le niveau et actualise tous les acteurs.
        /// Si le niveau est fini et que le boss est mort, passe au niveau suivant, et le lance lorsque tous les
        /// powerups de sort sont consommés.
        /// </summary>
        /// <param name="ms"> Le temps écoulé en millisecondes </param>
        public void Tick(int ms)
        {
            // Game over
            if (GetAlivePlayers().Count == 0)
            {
                GameOverDelay += ms;
                return;
            }
            // Centre la caméra
            CenterCamera();
            // Fait avancer le niveau
            if (_levelHasBegun)
            {
                foreach (var script in _levelScript)
                {
                    script.Tick(ms);
                }
            }
            // Vérifie si tous les powerups sont consommés
            else if (_spellPowerups == 0)
            {
                var nobodyChoosingSpell = true;
                foreach (var player in GetAlivePlayers())
                {
                    if (player.IsChoosingSpell()) nobodyChoosingSpell = false;
                }
                // Lancement du niveau
                if (nobodyChoosingSpell)
                {
                    _levelHasBegun = true;
                    foreach (var player in GetPlayers())
                    {
                        player.Heals(player.LifeMax());
                        player.CleanStates();
                        player.SaveStats();
                    }
                }
            }
            // Actualise les acteurs
            foreach (var actor in _actors.ToList())
            {
                if (!actor.IsDead()) actor.Tick(ms);
            }
            // Passe au niveau suivant lorsque le boss est mort
            if (_levelScript[0].IsFinished())
            {
                if (_boss == null)
                {
                    _boss = new CthulhuGhost(Map.BossSpawn);
                }
                else if (_boss.IsDead())
                {
                    _boss = null;
                    NextLevel(true);
                }
            }
        }
    }
}