
using TentacleSlicers.graphics;
using TentacleSlicers.hud;
using Point = TentacleSlicers.general.Point;
using Button = TentacleSlicers.hud.Button;

namespace TentacleSlicers.windows
{
    /// <summary>
    /// Définit un petit état qui remplace celui permettant de créer une nouvelle sauvegarde si le nombre maximal de
    /// sauvegardes est déjà atteint.
    /// </summary>
    public class TooMuchSaveState : MenuState
    {
        private static readonly BasicSprite Screen = new BasicSprite(ImagePath + "new save menu.png");

        /// <summary>
        /// Crée l'état avec un unique bouton menant vers l'état précédent.
        /// </summary>
        /// <param name="form"> La fenêtre de l'application </param>
        /// <param name="state"> L'état précédent </param>
        public TooMuchSaveState(MainForm form, WindowsState state) : base(form, Screen)
        {
            ButtonsHandler = new ButtonsHandler(new Button(new Point(450, 400), "Nombre maximal de sauvegardes atteint",
                () =>
            {
                Form.SetState(state);
            }));
        }
    }
}