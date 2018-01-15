
using TentacleSlicers.graphics;
using TentacleSlicers.hud;
using TentacleSlicers.maps;
using Button = TentacleSlicers.hud.Button;
using Point = TentacleSlicers.general.Point;

namespace TentacleSlicers.windows
{
    /// <summary>
    /// Définit le menu accessible en jeu depuis la touche echap.
    /// Il permet de créer une nouvelle sauvegarde et de retourner au menu principal.
    /// </summary>
    public class GameMenuState : MenuState
    {
        private static readonly BasicSprite Screen = new BasicSprite(ImagePath + "game menu.png");

        /// <summary>
        /// Crée le menu en jeu avec la possibilité de créer une nouvelle sauvegarde, de revenir en jeu ou de retourner
        /// au menu principal.
        /// </summary>
        /// <param name="form"> La fenêtre de l'application </param>
        /// <param name="state"> L'état de la partie </param>
        public GameMenuState(MainForm form, WindowsState state) : base(form, Screen)
        {
            var tooMuchSave = new TooMuchSaveState(Form, this);

            var bSave = new Button(new Point(600, 250), "Sauvegarder", () =>
            {
                if (GameSave.Saves.Count >= GameSave.NbMax)
                {
                    Form.SetState(tooMuchSave);
                }
                else
                {
                    Form.SetState(new CreateSaveState(Form, this));
                }
            });
            var bRetour = new Button(new Point(600, 400), "Retour", () =>
            {
                Form.SetState(state);
                ButtonsHandler.Reset();
            });
            var bMenu = new Button(new Point(600, 550), "Menu principal", () =>
            {
                Form.SetMainMenuState();
            });

            bSave.BottomButton = bRetour;
            bRetour.TopButton = bSave;
            bRetour.BottomButton = bMenu;
            bMenu.TopButton = bRetour;

            ButtonsHandler = new ButtonsHandler(bSave);
            ButtonsHandler.Add(bRetour);
            ButtonsHandler.Add(bMenu);
            ButtonsHandler.LinkToEchap(bRetour);
        }
    }
}