using System.Drawing;
using System.Windows.Forms;
using TentacleSlicers.hud;
using TentacleSlicers.inputs;
using TentacleSlicers.maps;
using Point = TentacleSlicers.general.Point;

namespace TentacleSlicers.windows
{
    /// <summary>
    /// Définit l'état en cours de partie. L'actualisation et le dessin sont délégués au World puis au HudHandler, et
    /// les évènements du clavier sont délégués à l'InputHandler.
    /// </summary>
    public class GameState : WindowsState
    {
        private World _world;
        private InputHandler _inputHandler;
        private HudHandler _hudHandler;

        /// <summary>
        /// Crée la partie à partir d'une sauvegarde.
        /// </summary>
        /// <param name="form"> La fenêtre de l'application </param>
        /// <param name="save"> la sauvegarde à charger </param>
        public GameState(MainForm form, GameSave save) : base(form)
        {
            World.Load(save);
            Init();
        }

        /// <summary>
        /// Crée une nouvelle partie avec les paramètres indiqués.
        /// </summary>
        /// <param name="form"> La fenêtre de l'application </param>
        /// <param name="numMap"> Le numéro de la map </param>
        /// <param name="nbPlayers"> Le nombre de joueurs </param>
        public GameState(MainForm form, int numMap, int nbPlayers) : base(form)
        {
            World.Load(numMap, nbPlayers);
            Init();
        }

        /// <summary>
        /// Finalise la création de la partie en générant l'Inputhandler, (pour les évènements au clavier) l'HudHandler
        /// (pour l'interface) avec le menu en jeu et l'état de game over.
        /// </summary>
        private void Init()
        {
            _world = World.GetWorld();
            _inputHandler = new InputHandler(_world.GetPlayers(), Form, new GameMenuState(Form, this));
            _hudHandler = new HudHandler(_world.GetPlayers());
        }

        /// <summary>
        /// Délègue les évènements liés à l'appui d'une touche à l'InputHandler.
        /// </summary>
        /// <param name="e"> L'objet détaillant l'évènement </param>
        /// <returns> Vrai si aucune action n'a été déclenchée </returns>
        public override bool KeyDown(KeyEventArgs e)
        {
            return _inputHandler.KeyDown(e);
        }

        /// <summary>
        /// Délègue les évènements liés à la pression d'une touche à l'InputHandler.
        /// </summary>
        /// <param name="e"> L'objet détaillant l'évènement </param>
        /// <returns> Vrai si aucune action n'a été déclenchée </returns>
        public override bool KeyPressed(KeyPressEventArgs e)
        {
            return _inputHandler.KeyPressed(e);
        }

        /// <summary>
        /// Délègue les évènements liés au relâchement d'une touche à l'InputHandler.
        /// </summary>
        /// <param name="e"> L'objet détaillant l'évènement </param>
        /// <returns> Vrai si aucune action n'a été déclenchée </returns>
        public override bool KeyUp(KeyEventArgs e)
        {
            return _inputHandler.KeyUp(e);
        }

        /// <summary>
        /// Affiche le monde puis l'interface.
        /// </summary>
        /// <param name="shift"> La position de décalage, inutilisée </param>
        /// <param name="g"> L'objet permettant de dessiner </param>
        public override void Draw(Point shift, Graphics g)
        {
            _world.Draw(shift, g);
            _hudHandler.Draw(shift, g);
        }

        /// <summary>
        /// Vérifie si la partie est terminée, auquel cas l'état de game over est passé à l'application.
        /// Sinon, actualise le monde et l'interface.
        /// </summary>
        /// <param name="ms"> Le temps écoulé en millisecondes </param>
        public override void Tick(int ms)
        {
            if (_world.GameOverDelay >= World.MaxGameOverDelay)
            {
                Form.SetState(new GameOverState(Form, World.GetWorld().Score));
                return;
            }
            _world.Tick(ms);
            _hudHandler.Tick(ms);
        }

    }
}