
using TentacleSlicers.graphics;
using TentacleSlicers.hud;
using TentacleSlicers.maps;
using Point = TentacleSlicers.general.Point;
using Button = TentacleSlicers.hud.Button;

namespace TentacleSlicers.windows
{
    /// <summary>
    /// Définit le menu permettant de choisir une sauvegarde.
    /// </summary>
    public class ChooseSaveState : MenuState
    {
        private static readonly BasicSprite Screen = new BasicSprite(ImagePath + "choose save menu.png");

        /// <summary>
        /// Crée l'état avec le bouton retour permettant de retourner au menu principal.
        /// Une grille de boutons est crée, chaque bouton correpondant à une sauvegarde qui peut être chargée pour
        /// entrer en jeu.
        /// </summary>
        /// <param name="form"> La fenêtre de l'application </param>
        /// <param name="mainMenu"> Le menu principal </param>
        public ChooseSaveState(MainForm form, WindowsState mainMenu) : base(form, Screen)
        {
            var bRetour = new Button(new Point(600, 700), "Retour", () =>
            {
                Form.SetState(mainMenu);
            });

            // Remplit de gauche à droite et de haut en bas la grille de 3x3 sauvegardes
            var bSaves = new Button[3, 3];
            for (var num = 0; num < GameSave.Saves.Count; ++num)
            {
                var i = num / 3;
                var j = num % 3;
                var save = GameSave.Saves[num];
                var playerStr = save.NumberOfPlayers == 1 ? "Solo" : "Duo";
                bSaves[i, j] = new Button(new Point(300 + j * 300, 200 + i * 150),
                                      save.Name + '\n' + playerStr + ", niveau " + save.Level, () =>
                {
                    Form.SetState(new GameState(form, save));
                });
            }

            // Relie les boutons de la grille entre eux
            for (var i = 0; i < 3; ++i)
            {
                for (var j = 0; j < 3; ++j)
                {
                    var save = bSaves[i, j];
                    if (save == null) continue;

                    if (i + 1 >= 3 || bSaves[i + 1, j] == null)
                    {
                        save.BottomButton = bRetour;
                    }
                    else
                    {
                        save.BottomButton = bSaves[i + 1, j];
                    }
                    if (i > 0) save.TopButton = bSaves[i - 1, j];
                    if (j > 0) save.LeftButton = bSaves[i, j - 1];
                    if (j < 2 && bSaves[i, j + 1] != null) save.RightButton = bSaves[i, j + 1];
                }
            }
            if (bSaves[0, 1] != null)
            {
                bRetour.TopButton = bSaves[0, 1];
            }
            else if (bSaves[0, 0] != null)
            {
                bRetour.TopButton = bSaves[0, 0];
            }

            ButtonsHandler = new ButtonsHandler(bRetour);
            foreach (var b in bSaves)
            {
                if (b != null) ButtonsHandler.Add(b);
            }
            ButtonsHandler.LinkToEchap(bRetour);
        }
    }
}