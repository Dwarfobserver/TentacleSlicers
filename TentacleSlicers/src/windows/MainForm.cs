using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using TentacleSlicers.hud;
using Point = TentacleSlicers.general.Point;

namespace TentacleSlicers.windows
{
    /// <summary>
    /// La fenêtre graphique de l'application. Crée périodiquement des évènements d'actualisation qui appellent les
    /// fonctions Tick et Draw, et reçoit les évènements liés à l'appui, l'activation ou le relâchement d'une touche du
    /// clavier. Toutes ces fonctions sont déléguées à l'objet qui gère l'état actuel de la fenêtre.
    /// </summary>
    public sealed class MainForm : Form
    {
        public const int FixedWidth = 1200;
        public const int FixedHeight = 800;
        private const int Ips = 60;
        private const double SpeedRatio = 1.2; // Réglage de la vitesse du jeu à la volée

        private Timer _timer;
        private Stopwatch _stopwatch;
        private Bitmap _backBuffer;
        private WindowsState _state;
        private MainMenuState _mainMenuState;

        /// <summary>
        /// Initialise les composants et lie les fonctions aux évènements souhaités de la fenêtre, la centre puis lui
        /// donne des paramètres pour optimiser l'affichage graphique.
        /// </summary>
        public MainForm()
        {
            InitializeComponents();
            CenterToScreen();
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
        }

        /// <summary>
        /// Délègue l'actualisation et les évènements du clavier à l'objet donné.
        /// </summary>
        /// <param name="state"> Le nouvel état de la fenêtre </param>
        public void SetState(WindowsState state)
        {
            _state = state;
        }

        /// <summary>
        /// Redirige la fenêtre sur son menu principal.
        /// </summary>
        public void SetMainMenuState()
        {
            _mainMenuState.Update();
            _state = _mainMenuState;
        }

        /// <summary>
        /// Initialise l'état de la fenêtre à son menu prncipal.
        /// Définit ses bordures, sa taille, son titre et son icône.
        /// Indique que la fenêtre récupère les évènements du clavier, lui donne la fonction de création du backbuffer
        /// pour dessiner et celle pour l'afficher.
        /// Enfin, lance le timer avec la fréquence d'image par seconde désirée qui va actualiser l'application.
        /// </summary>
        private void InitializeComponents()
        {
            _mainMenuState = new MainMenuState(this);
            _state = _mainMenuState;

            FormBorderStyle = FormBorderStyle.FixedDialog;
            Size = new Size(FixedWidth, FixedHeight);
            MinimizeBox = false;
            MaximizeBox = false;
            Text = "Tentacle Slicers";
            Icon = new Icon(HudComponent.ImagePath + "windows/icon.ico");

            _timer = new Timer {Interval = 1000 / Ips};
            _stopwatch = new Stopwatch();

            Load += Event_CreateBackBuffer;
            Paint += Event_Paint;
            KeyDown += Event_KeyDown;
            KeyPress += Event_KeyPressed;
            KeyUp += Event_KeyUp;
            _timer.Tick += Event_Loop;

            _timer.Start();
            _stopwatch.Start();
        }

        /// <summary>
        /// lance l'appel en cascade de la fonction Draw depuis l'état courant de la fenêtre.
        /// </summary>
        private void Draw()
        {
            if (_backBuffer == null) return;
            using (var g = Graphics.FromImage(_backBuffer))
            {
                g.Clear(Color.Black);
                _state.Draw(Point.Null, g);
            }
            Invalidate();
        }

        /// <summary>
        /// Lance l'appel en cascade de la fonction Tick depuis l'état courant de la fenêtre, en calculant le nombre
        /// de millisecondes qui s'est écoulé depuis le dernier appel de Tick.
        /// </summary>
        private void Tick()
        {
            _state.Tick((int) (_stopwatch.ElapsedMilliseconds * SpeedRatio));
            _stopwatch.Restart();
        }

        /// <summary>
        /// Délègue le traitement de l'évènement de l'appui d'une touche du clavier à l'état courant de la fenêtre.
        /// Fonction directement déclenchée lorsque la fenêtre détecte l'appui d'une touche.
        /// </summary>
        /// <param name="sender"> L'objet envoyant l'évènement, inutilisé </param>
        /// <param name="e"> L'objet détaillant l'évènement </param>
        private void Event_KeyDown(object sender, KeyEventArgs e)
        {
            _state.KeyDown(e);
        }

        /// <summary>
        /// Délègue le traitement de l'évènement de l'activation d'une touche du clavier à l'état courant de la fenêtre.
        /// Fonction directement déclenchée lorsque la fenêtre détecte l'activation d'une touche.
        /// </summary>
        /// <param name="sender"> L'objet envoyant l'évènement, inutilisé </param>
        /// <param name="e"> L'objet détaillant l'évènement </param>
        private void Event_KeyPressed(object sender, KeyPressEventArgs e)
        {
            _state.KeyPressed(e);
        }

        /// <summary>
        /// Délègue le traitement de l'évènement du relâchement d'une touche du clavier à l'état courant de la fenêtre.
        /// Fonction directement déclenchée lorsque la fenêtre détecte le relâchement d'une touche.
        /// </summary>
        /// <param name="sender"> L'objet envoyant l'évènement, inutilisé </param>
        /// <param name="e"> L'objet détaillant l'évènement </param>
        private void Event_KeyUp(object sender, KeyEventArgs e)
        {
            _state.KeyUp(e);
        }

        /// <summary>
        /// Affiche le backbuffer de la fenêtre à l'écran. Déclenchée après l'appel en cascade de Draw lors d'une
        /// boucle d'actualisation.
        /// </summary>
        /// <param name="sender"> L'objet envoyant l'évènement, inutilisé </param>
        /// <param name="e"> L'objet détaillant l'évènement </param>
        private void Event_Paint(object sender, PaintEventArgs e)
        {
            if (_backBuffer != null)
            {
                e.Graphics.DrawImageUnscaled(_backBuffer, System.Drawing.Point.Empty);
            }
        }

        /// <summary>
        /// Crée le backbuffer sur lequel la fonction Draw va dessiner. Déclenchée avant l'appel en cascade de Draw
        /// lors d'une boucle d'actualisation.
        /// </summary>
        /// <param name="sender"> L'objet envoyant l'évènement, inutilisé </param>
        /// <param name="e"> L'objet détaillant l'évènement, inutilisé </param>
        private void Event_CreateBackBuffer(object sender, EventArgs e)
        {
            _backBuffer?.Dispose();
            _backBuffer = new Bitmap(FixedWidth, FixedHeight);
        }

        /// <summary>
        /// Boucle d'actualisation comprenant d'abord l'actualisation des données de l'application puis leur affichage.
        /// Déclenchée régulièrement, lors de la réception de l'évènement généré par le timer initialisé avec la
        /// fréquence de rafraîchissement de l'écran voulue.
        /// </summary>
        /// <param name="sender"> L'objet envoyant l'évènement, inutilisé </param>
        /// <param name="e"> L'objet détaillant l'évènement, inutilisé </param>
        private void Event_Loop(object sender, EventArgs e)
        {
            Tick();
            Draw();
        }

        // Fonction modifiée pour permettre une meilleure gestion des deux buffers d'affichage
        protected override CreateParams CreateParams {
            get {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

    }
}