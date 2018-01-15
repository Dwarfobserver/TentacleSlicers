
using System.Drawing;
using TentacleSlicers.graphics;
using TentacleSlicers.hud;
using TentacleSlicers.maps;
using Point = TentacleSlicers.general.Point;
using Button = TentacleSlicers.hud.Button;

namespace TentacleSlicers.windows
{
    /// <summary>
    /// Définit le menu principal.
    /// C'est l'état initial de l'application.
    /// </summary>
    public class MainMenuState : MenuState
    {
        private static readonly BasicSprite Screen = new BasicSprite(ImagePath + "main menu.png");

        private Text _highScore;

        /// <summary>
        /// Crée l'état via la fonction Update.
        /// </summary>
        /// <param name="form"> La fenêtre de l'application </param>
        public MainMenuState(MainForm form) : base(form, Screen)
        {
            Update();
        }

        /// <summary>
        /// Initialisateur appelé par le constructeur et la fenêtre lors de l'actualisation du menu.
        /// Permet notamment de recréer le menu des sauvegardes, au cas où elles ont changé, et d'actualiser le
        /// highscore.
        /// </summary>
        public void Update()
        {
            // Charge les sorts et les sauvegardes
            GameSave.Load();

            _highScore = new Text("High Score : " + GameSave.HighScore, 18);

            var savesMenu = new ChooseSaveState(Form, this);

            var b1 = new Button(new Point(300, 700), "Nouvelle partie", () =>
            {
                Form.SetState(new NewGameState(Form, this));
            });
            var b2 = new Button(new Point(600, 700), "Charger partie", () =>
            {
                Form.SetState(savesMenu);
            });
            var b3 = new Button(new Point(900, 700), "Quitter", () =>
            {
                Form.Close();
            });

            b1.RightButton = b2;
            b2.RightButton = b3;
            b3.LeftButton = b2;
            b2.LeftButton = b1;

            ButtonsHandler = new ButtonsHandler(b1);
            ButtonsHandler.Add(b2);
            ButtonsHandler.Add(b3);
            ButtonsHandler.LinkToEchap(b3);
        }

        /// <summary>
        /// Affiche le contenu de l'état ainsi que le highscore.
        /// </summary>
        /// <param name="shift"> La position de décalage, inutilisée </param>
        /// <param name="g"> L'objet permettant de dessiner </param>
        public override void Draw(Point shift, Graphics g)
        {
            base.Draw(shift, g);
            _highScore.Draw(new Point(600, 50), g);
        }
    }
}