
using TentacleSlicers.graphics;
using TentacleSlicers.hud;
using TentacleSlicers.maps;
using Point = TentacleSlicers.general.Point;
using Button = TentacleSlicers.hud.Button;

namespace TentacleSlicers.windows
{
    /// <summary>
    /// Définit un état juste avant de lancer une nouvelle partie pour définir le nombre de joueurs et la map sur
    /// laquelle jouer.
    /// </summary>
    public class NewGameState : MenuState
    {
        private static readonly BasicSprite Screen = new BasicSprite(ImagePath + "new game menu.png");

        /// <summary>
        /// Crée l'état avec une paire de boutons par map pour créer la nouvelle partie avec la map et le nombre de
        /// joueurs souhaité.
        /// </summary>
        /// <param name="form"> La fenêtre de l'application </param>
        /// <param name="state"> L'état précédent de l'application </param>
        public NewGameState(MainForm form, WindowsState state) : base(form, Screen)
        {
            var maps = Map.MapNames();
            var b = new Button[2, maps.Count];

            // Création des boutons
            var bRetour = new Button(new Point(600, 700), "Retour", () =>
            {
                Form.SetState(state);
            });
            for (var j = 0; j < maps.Count; ++j)
            {
                var numMap = j;
                b[0, j] = new Button(new Point(450, 200 + 150 * j), maps[j] + "\n(Duo)", () =>
                {
                    Form.SetState(new GameState(Form, numMap, 2));
                });
                b[1, j] = new Button(new Point(750, 200 + 150 * j), maps[j] + "\n(Solo)", () =>
                {
                    Form.SetState(new GameState(Form, numMap, 1));
                });
            }

            // Liaison des boutons entre eux
            for (var j = 0; j < maps.Count; ++j)
            {
                if (j < maps.Count - 1)
                {
                    b[0, j].BottomButton = b[0, j + 1];
                    b[1, j].BottomButton = b[1, j + 1];
                }
                else
                {
                    b[0, j].BottomButton = bRetour;
                    b[1, j].BottomButton = bRetour;
                }
                if (j > 0)
                {
                    b[0, j].TopButton = b[0, j - 1];
                    b[1, j].TopButton = b[1, j - 1];
                }
                b[0, j].RightButton = b[1, j];
                b[1, j].LeftButton = b[0, j];
            }
            bRetour.TopButton = b[0, 0];

            // Création du ButtonHandler
            ButtonsHandler = new ButtonsHandler(bRetour);
            foreach (var button in b)
            {
                ButtonsHandler.Add(button);
            }
            ButtonsHandler.LinkToEchap(bRetour);
        }

    }
}