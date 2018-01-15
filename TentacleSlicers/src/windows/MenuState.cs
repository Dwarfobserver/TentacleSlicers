using System.Drawing;
using System.Windows.Forms;
using TentacleSlicers.graphics;
using TentacleSlicers.hud;
using Point = TentacleSlicers.general.Point;

namespace TentacleSlicers.windows
{
    /// <summary>
    /// Spécifie la classe WindowsState pour dessiner l'image de fond de l'écran et gérer l'éventuel gestionnaire de
    /// boutons en lui délégant les entrées et en affichant ses boutons.
    /// </summary>
    public abstract class MenuState : WindowsState
    {
        public static readonly Point ScreenPoint = new Point(MainForm.FixedWidth / 2.0, MainForm.FixedHeight);

        protected ButtonsHandler ButtonsHandler;
        private readonly BasicSprite _screen;

        /// <summary>
        /// Crée l'état avec la fenêtre et l'image de fond associées.
        /// </summary>
        /// <param name="form"> La fenêtre </param>
        /// <param name="screen"> L'image de fonc </param>
        protected MenuState(MainForm form, BasicSprite screen) : base(form)
        {
            _screen = screen;
        }

        /// <summary>
        /// Délègue l'évènement d'une touche pressée aux boutons.
        /// </summary>
        /// <param name="e"> L'objet détaillant l'évènement </param>
        /// <returns> Vrai si aucune action n'a été déclencheé </returns>
        public override bool KeyPressed(KeyPressEventArgs e)
        {
            return ButtonsHandler == null || ButtonsHandler.KeyPressed(e);
        }

        /// <summary>
        /// Dessine l'image de fond puis affiche les boutons.
        /// </summary>
        /// <param name="shift"> La position de décalage, inutilisée </param>
        /// <param name="g"> L'objet permettant de dessiner </param>
        public override void Draw(Point shift, Graphics g)
        {
            _screen.Draw(ScreenPoint, g);
            ButtonsHandler?.Draw(shift, g);
        }
    }
}