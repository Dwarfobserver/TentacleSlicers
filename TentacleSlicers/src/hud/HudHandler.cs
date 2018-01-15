using System.Collections.Generic;
using System.Drawing;
using TentacleSlicers.actors;
using TentacleSlicers.interfaces;
using TentacleSlicers.maps;
using TentacleSlicers.windows;
using Point = TentacleSlicers.general.Point;

namespace TentacleSlicers.hud
{
    /// <summary>
    /// Gère les composants affichés à l'écran en position absolue.
    /// Récupère des données du monde pour afficher l'état des joueurs et la progression du niveau.
    /// </summary>
    public class HudHandler : IDrawable, ITickable
    {
        private const int LifeBarY = MainForm.FixedHeight - 40;
        private const int SpellSocketY = LifeBarY - 26;

        private readonly List<HudComponent> _components;
        private LevelObserver _levelObserver;

        /// <summary>
        /// Crée les composants à afficher en jeu, c'est-à-dire la vie et les sorts des joueurs ainsi qu'un composant
        /// qui suit l'avancée du niveau, qui est instancié par le monde (World).
        /// </summary>
        /// <param name="characters"> Les joueurs </param>
        public HudHandler(IReadOnlyList<PlayerCharacter> characters)
        {
            _components = new List<HudComponent>();

            UpdateLevelObserver();

            _components.Add(new Lifebar(new Point(Lifebar.Width / 2 + 20, LifeBarY), characters[0], true));
            _components.Add(new SpellSocket(new Point(80, SpellSocketY), characters[0], 1));
            _components.Add(new SpellSocket(new Point(164, SpellSocketY), characters[0], 2));

            if (characters.Count == 1) return;

            _components.Add(new Lifebar(new Point(
                MainForm.FixedWidth - Lifebar.Width / 2 - 30, LifeBarY), characters[1], false));
            _components.Add(new SpellSocket(new Point(MainForm.FixedWidth - 174, SpellSocketY), characters[1], 1));
            _components.Add(new SpellSocket(new Point(MainForm.FixedWidth - 90, SpellSocketY), characters[1], 2));
        }

        /// <summary>
        /// Crée un nouvel LevelObserver et actualise avec la liste des composants gérés.
        /// </summary>
        private void UpdateLevelObserver()
        {
            if (_levelObserver != null) _components.Remove(_levelObserver);
            _levelObserver = World.GetWorld().GetLevelObserver(new Point(MainForm.FixedWidth / 2.0, 20));
            _components.Add(_levelObserver);
        }

        /// <summary>
        /// Dessine tous les composants.
        /// </summary>
        /// <param name="shift"> La position de décalage, inutilisée par les composants </param>
        /// <param name="g"> L'objet permettant de dessiner </param>
        public void Draw(Point shift, Graphics g)
        {
            foreach (var component in _components)
            {
                component.Draw(shift, g);
            }
        }

        /// <summary>
        /// Actualise tous les composants.
        /// Vérifie si le LevelObserver doit être changé.
        /// </summary>
        /// <param name="ms"> Le temps écoulé en milliesecondes </param>
        public void Tick(int ms)
        {
            foreach (var component in _components)
            {
                component.Tick(ms);
            }
            if (_levelObserver.NeedToBeChanged())
            {
                UpdateLevelObserver();
            }
        }
    }
}