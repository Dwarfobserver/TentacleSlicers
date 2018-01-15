
using System.Collections.Generic;
using System.Windows.Forms;
using TentacleSlicers.actors;
using TentacleSlicers.interfaces;
using TentacleSlicers.windows;

namespace TentacleSlicers.inputs
{
    /// <summary>
    /// Reçoit les inputs de la fenêtre en jeu pour les traiter.
    /// </summary>
    public class InputHandler : IController
    {
        public static readonly Keys[] MovingKeys =
        {
            Keys.Down,
            Keys.Up,
            Keys.Left,
            Keys.Right
        };

        public static readonly int[] MovingCodes =
        {
            115,
            122,
            113,
            100
        };

        public static readonly int EnterCode = 13;
        public static readonly int EscapeCode = 27;

        private readonly IController[] _controllers;

        /// <summary>
        /// Crée l'InputHandler avec les joueurs, la fenêtre de l'application et le menu en jeu à déclencher.
        /// A chque joueur est assigné un PlayerController, qui va récupérer les évènements qui passent par
        /// l'InputHandler.
        /// Si un évènement n'est pas traité après les PlayerController, un GameController le reçoit pour faire les
        /// actions qui ne concernent pas la gestion d'un personnage.
        /// </summary>
        /// <param name="players"> Les joueurs, qui sont associés à un PlayerController </param>
        /// <param name="form"> La fenêtre de l'application </param>
        /// <param name="escapeState"> Le menu en jeu, déclenché par le GameController </param>
        public InputHandler(IReadOnlyList<PlayerCharacter> players, MainForm form, WindowsState escapeState)
        {
            _controllers = new IController[players.Count + 1];

            for (var i = 0; i < players.Count; ++i)
            {
                _controllers[i] = new PlayerController(players[i], i);
            }
            _controllers[players.Count] = new GameController(form, escapeState);
        }

        /// <summary>
        /// Délègue l'évènement aux PlayerController puis au GameController, tant qu'il n'est pas traité par l'un d'eux.
        /// </summary>
        /// <param name="e"> L'objet détaillant l'évènement </param>
        /// <returns> Vrai si aucune action n'a été déclenchée </returns>
        public bool KeyDown(KeyEventArgs e)
        {
            var fetching = true;
            var indice = 0;
            while (fetching && indice < _controllers.Length)
            {
                fetching = _controllers[indice].KeyDown(e);
                indice++;
            }
            return fetching;
        }

        /// <summary>
        /// Délègue l'évènement aux PlayerController puis au GameController, tant qu'il n'est pas traité par l'un d'eux.
        /// </summary>
        /// <param name="e"> L'objet détaillant l'évènement </param>
        /// <returns> Vrai si aucune action n'a été déclenchée </returns>
        public bool KeyPressed(KeyPressEventArgs e)
        {
            var fetching = true;
            var indice = 0;
            while (fetching && indice < _controllers.Length)
            {
                fetching = _controllers[indice].KeyPressed(e);
                indice++;
            }
            return fetching;
        }

        /// <summary>
        /// Délègue l'évènement aux PlayerController puis au GameController, tant qu'il n'est pas traité par l'un d'eux.
        /// </summary>
        /// <param name="e"> L'objet détaillant l'évènement </param>
        /// <returns> Vrai si aucune action n'a été déclenchée </returns>
        public bool KeyUp(KeyEventArgs e)
        {
            var fetching = true;
            var indice = 0;
            while (fetching && indice < _controllers.Length)
            {
                fetching = _controllers[indice].KeyUp(e);
                indice++;
            }
            return fetching;
        }
    }
}