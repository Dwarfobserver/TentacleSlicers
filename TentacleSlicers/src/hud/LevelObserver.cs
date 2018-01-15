
using System.Drawing;
using TentacleSlicers.graphics;
using TentacleSlicers.levels;
using TentacleSlicers.maps;
using TentacleSlicers.windows;
using Point = TentacleSlicers.general.Point;

namespace TentacleSlicers.hud
{
    /// <summary>
    /// Indique le numéro du niveau actuel, de la vague courante et le temps restant avant la prochaine vague.
    /// </summary>
    public class LevelObserver : HudComponent
    {
        private readonly LevelScript _level;
        private Text _cooldown;

        /// <summary>
        /// Crée le composant en lui donnnant le LevelScript à observer pour récupérer la vague courante et le temps
        /// restant.
        /// </summary>
        /// <param name="position"> Le centre du composant </param>
        /// <param name="level"> Le LevelScript observé </param>
        public LevelObserver(Point position, LevelScript level) : base(position)
        {
            _level = level;
            Update();
        }

        /// <summary>
        /// Indique si ce LevelObsever est devenu obsolète, c'est-à-dire si un novueau niveau a débuté.
        /// </summary>
        /// <returns> Vrai si il faut changer de LevelObserver </returns>
        public bool NeedToBeChanged()
        {
            return _level.IsFinished();
        }

        /// <summary>
        /// Affiche le numéro de la vague courante, et écrit en-dessous le temps restant avant la prochaine.
        /// </summary>
        /// <param name="shift"> La position de décalage, inutilisée </param>
        /// <param name="g"> L'objet permettant de dessiner </param>
        public override void Draw(Point shift, Graphics g)
        {
            new Text("Niveau " + World.GetWorld().Level +
                     " - Vague courante : " + _level.Wave).Draw(Position, g);
            _cooldown.Draw(Position + new Point(0, 35), g);
        }

        /// <summary>
        /// Actualise les données du composant d'après le LevelScript fourni.
        /// </summary>
        /// <param name="ms"> Le temps écoulé en millisecondes </param>
        public override void Tick(int ms)
        {
            Update();
        }

        /// <summary>
        /// Actualise les données du composant d'après le LevelScript fourni.
        /// </summary>
        private void Update()
        {
            _cooldown = new Text(_level.Cooldown.ToString("N1"), 22);
        }
    }
}