
using System.Drawing;
using TentacleSlicers.graphics;
using TentacleSlicers.hud;
using TentacleSlicers.maps;
using Point = TentacleSlicers.general.Point;
using Button = TentacleSlicers.hud.Button;

namespace TentacleSlicers.windows
{
    /// <summary>
    /// Définit l'état sur lequel on arrive une fois que tous les joueurs sont morts.
    /// </summary>
    public class GameOverState : MenuState
    {
        private static readonly BasicSprite Screen = new BasicSprite(ImagePath + "game over menu.png");
        private static readonly Text Title = new Text("VOUS ETES MORT", 30);

        private readonly Text _score;

        /// <summary>
        /// Crée l'état corerspondant au game over avec la possibilité de revenir au menu principal ou de quitter
        /// l'application. Affiche le score atteint par les joueurs.
        /// </summary>
        /// <param name="form"> La fenêtre de l'application </param>
        /// <param name="score"> Le score final </param>
        public GameOverState(MainForm form, int score) : base(form, Screen)
        {
            _score = new Text("Score : " + score, 18);
            if (score > GameSave.HighScore) GameSave.HighScore = score;

            var bMenu = new Button(new Point(450, 700), "Menu principal", () =>
            {
                Form.SetMainMenuState();
            });
            var bClose = new Button(new Point(750, 700), "Quitter", () =>
            {
                Form.Close();
            });

            bMenu.RightButton = bClose;
            bClose.LeftButton = bMenu;

            ButtonsHandler = new ButtonsHandler(bMenu);
            ButtonsHandler.Add(bClose);
        }

        /// <summary>
        /// Affiche le contenu de l'état, puis le texte et le score.
        /// </summary>
        /// <param name="shift"> La position de décalage, inutilisée </param>
        /// <param name="g"> L'objet permettant de dessiner </param>
        public override void Draw(Point shift, Graphics g)
        {
            base.Draw(shift, g);
            Title.Draw(new Point(600, 180), g);
            _score.Draw(new Point(600, 230), g);
        }
    }
}