
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using TentacleSlicers.collisions;
using TentacleSlicers.interfaces;
using Point = TentacleSlicers.general.Point;
using Rectangle = System.Drawing.Rectangle;

namespace TentacleSlicers.maps
{
    /// <summary>
    /// Représente les données chargées par les fichiers .map, qui n'évolueront pas au cours d'une partie :
    /// Les textures de la map, ses murs et collisions, ses deux points d'apparition pour les joueurs et ceux de leurs
    /// adversaires.
    /// La création d'une map est détaillée dans le dossier resources/maps avec le fichier readme.
    /// </summary>
    public class Map : IDrawable
    {
        public const string MapPath = "../../resources/maps/";
        public const string Terrains = "terrains/";
        public const string Backgrounds = "backgrounds/";
        public const int Size = 92;
        private static readonly Bitmap[] Walls = LoadWalls();

        private static Dictionary<int, string> _mapNames;
        /// <summary>
        /// Retourne le numéro de chaque map avec son nom associé.
        /// </summary>
        /// <returns> Le numéro avec le nom de chaque map </returns>
        public static Dictionary<int, string> MapNames()
        {
            if (_mapNames == null)
            {
                _mapNames = new Dictionary<int, string>();

                var maps = Directory.GetFiles(MapPath, "*.map", SearchOption.TopDirectoryOnly);
                foreach (var map in maps)
                {
                    using (var file = new StreamReader(map))
                    {
                        var line = file.ReadLine();
                        var strs = map.Split('/');
                        var num = int.Parse("" + strs[strs.Length - 1][0]);
                        _mapNames[num] = line.Split(' ')[0];
                    }
                }
            }
            return _mapNames;
        }

        /// <summary>
        /// Charge les images des murs contenus dans le dossier maps/terrains.
        /// Les images doivent être au format png et numérotées de 1 à 9 maximum.
        /// </summary>
        /// <returns> Les images des murs </returns>
        private static Bitmap[] LoadWalls()
        {
            var nbWalls = Directory.GetFiles(MapPath + Terrains, "*.png", SearchOption.TopDirectoryOnly).Length;
            var walls = new Bitmap[nbWalls + 1];
            for (var i = 1; i < nbWalls + 1; ++i)
            {
                walls[i] = new Bitmap(MapPath + Terrains + i + ".png");
            }
            return walls;
        }

        public int Num { get; }
        public string Name { get; }
        public int Width { get; }
        public int Height { get; }
        public bool[,] WallsMatrix { get; }
        public List<Point> PlayerSpawns { get; }
        public List<Point> Gateways { get; }
        public Point BossSpawn { get; }
        private readonly Bitmap _background;

        /// <summary>
        /// Charge le fichier .map du dossier resources/maps ayant le numéro indiqué.
        /// Enregistre le nom, la taille et l'image de fond de la map.
        /// Puis, parcourt les cases de la map pour enregistrer les positions d'apparition des joueurs, des mobs, des
        /// boss et des murs (invisibles ou non). Lors du parcourt, les murs sont crées dans le monde.
        /// </summary>
        /// <param name="num"> Le numéro de la map à charger </param>
        public Map(int num)
        {
            // Ouvre le fichier correspondant au num demandé
            var file = new StreamReader(MapPath + num + ".map");
            var line = file.ReadLine();
            var elements = line.Split(' ');

            Num = num;

            // Enregistre l'entête avec le nom et les dimensions de la map.
            Name = elements[0];
            Width = int.Parse(elements[1]);
            Height = int.Parse(elements[2]);

            // Initialise les Squares et y donne l'accès dans les collisions handler
            const int coef = (int) ((Square.Size + 0.1) / Size);
            var indices = new Point(Width - 1, Height - 1) / coef;
            CollisionFunctions.SquaresHandler = new SquaresHandler((int) indices.X + 2, (int) indices.Y + 2);

            WallsMatrix = new bool[Width, Height];
            _background = new Bitmap(MapPath + Backgrounds + num + ".png");
            var j = 0;
            PlayerSpawns = new List<Point>();
            Gateways = new List<Point>();
            while((line = file.ReadLine()) != null)
            {
                elements = line.Split(' ');
                for (var i = 0; i < Width; i++)
                {
                    var c = elements[i][0];
                    // x = case vide
                    if (c == 'x') continue;
                    var p = new Point(i * Size + Size/2, j * Size + Size/2);
                    // p = spawns des joueurs
                    if (c == 'p')
                    {
                        PlayerSpawns.Add(p);
                    }
                    // g = spawns des mobs
                    else if (c == 'g')
                    {
                        Gateways.Add(p);
                    }
                    // f = spawn du boss
                    else if (c == 'f')
                    {
                        BossSpawn = p;
                    }
                    // [int] = mur avec le sprite associé si num != 0
                    else
                    {
                        WallsMatrix[i, j] = true;
                        var numWall = (int) char.GetNumericValue(c);
                        var sprite = numWall > 0 ? Walls[numWall] : null;
                        var transparency = j > 0 && WallsMatrix[i, j - 1] == false;
                        new Wall(p, sprite, transparency);
                    }
                }
                j++;
            }
            file.Close();
        }

        /// <summary>
        /// Dessine l'image de fond de la map. L'image doit être dessinée en premier pour que les autres acteurs ou
        /// composants s'affichent par dessus.
        /// </summary>
        /// <param name="shift"> La position de décalage </param>
        /// <param name="g"> L'objet permettant de dessiner </param>
        public void Draw(Point shift, Graphics g)
        {
            g.DrawImage(_background, new Rectangle(shift, _background.Size));
        }

    }
}