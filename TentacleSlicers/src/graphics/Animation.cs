using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TentacleSlicers.graphics
{
    /// <summary>
    /// Charge les images d'un dossier et les associe à un type d'animation.
    /// Les images du dossier doivent être nommées par leur ordre (de 0 au nombre d'images - 1) et au format png.
    /// De plus, le dossier doit contenir les sous-dossiers top, bottom, left et right (avec le même nombre d'images)
    /// si l'animation dépend d'une orientation.
    /// </summary>
    public class Animation
    {
        public static readonly int NbTypes = Enum.GetValues(typeof(AnimationType)).Length;
        private const int DefaultFrameLength = 200;

        public int AnimLength { get; set; }
        public AnimationType Type { get; }
        private readonly BasicSprite[,] _sprites;
        private readonly int _nbFrames;
        private readonly bool _hasSideSprites;

        /// <summary>
        /// Charge et organise l'animation contenue dans le dossier indiqué : récupère le type de l'animation, puis
        /// vérifie si l'animation dépend ou non de son orientation puis charge toutes les images du dossier.
        /// </summary>
        /// <param name="folder"> Le dossier de l'animation </param>
        /// <param name="heightShift"> Le décalage en hauteur des sprites </param>
        public Animation(string folder, int heightShift)
        {
            // Récupération du type de l'animation d'après le nom du dossier (en lui ajoutant une majuscule au début)
            var strs = folder.Split('/');
            var preType = strs[strs.Length - 1];
            var preType2 = "" + char.ToLower(preType[0]);
            for (var i = 1; i < preType.Length; ++i)
            {
                preType2 += preType[i];
            }
            Type = (AnimationType) Enum.Parse(typeof(AnimationType), preType2, true);

            var folders = Directory.EnumerateDirectories(folder).ToArray();

            // L'animation est répartie en quatre sous-dossiers, selon son orientation
            if (folders.Length == 4)
            {
                folder += "/";
                _hasSideSprites = true;
                _nbFrames = Directory.EnumerateFiles(folder + "bottom", "*.png",
                        SearchOption.TopDirectoryOnly).Count();
                _sprites = new BasicSprite[_nbFrames, 4];
                FillSprites(folder + "bottom", 0, heightShift);
                FillSprites(folder + "left", 1, heightShift);
                FillSprites(folder + "top", 2, heightShift);
                FillSprites(folder + "right", 3, heightShift);
            }
            // L'animation ne dépend pas de son orientation
            else
            {
                _hasSideSprites = false;
                _nbFrames = Directory.EnumerateFiles(folder, "*.png",
                    SearchOption.TopDirectoryOnly).Count();
                _sprites = new BasicSprite[_nbFrames, 1];
                FillSprites(folder, 0, heightShift);
            }
            AnimLength = DefaultFrameLength * _nbFrames;
        }

        /// <summary>
        /// Retourne le sprite correspondant au pourcentage de progression de l'animation, aninsi que son orientation.
        /// </summary>
        /// <param name="percent"> Le pourcentage de progression </param>
        /// <param name="orientation"> L'orientation </param>
        /// <returns> Le sprite correspondant </returns>
        public BasicSprite GetSprite(double percent, general.Point orientation)
        {
            return _sprites[(int)(_nbFrames * percent), GetIndice(orientation)];
        }

        /// <summary>
        /// Charges les images d'un dossier ne contenant que des images, numérotées à partir de zéro.
        /// Associe à ces images à des orientations, via un indice récupéré par la fonction GetIndice.
        /// </summary>
        /// <param name="folder"> Le dossier contenant les images </param>
        /// <param name="num"> Indice correspondant aux images </param>
        /// <param name="heightShift"> Le décalage en hauteur du sprite </param>
        private void FillSprites(string folder, int num, int heightShift)
        {
            var files = Directory.EnumerateFiles(folder, "*.png", SearchOption.TopDirectoryOnly);
            foreach (var f in files)
            {
                var i = int.Parse(Regex.Match(f, @"\d+").Value);
                _sprites[i, num] = new BasicSprite(f, heightShift);
            }
        }

        /// <summary>
        /// Associe un indice à une orientation donnée, selon l'existence des sous-dossiers.
        /// </summary>
        /// <param name="orientation"> L'orientation </param>
        /// <returns> L'indice associé à l'orientation </returns>
        private int GetIndice(general.Point orientation)
        {
            if (!_hasSideSprites) return 0;
            // Right
            if (orientation.X > Math.Abs(orientation.Y) + 0.001) return 3;
            // Left
            if (orientation.X < - (Math.Abs(orientation.Y) + 0.001)) return 1;
            // Top / Bottom
            return orientation.Y < 0 ? 2 : 0;
        }
    }
}