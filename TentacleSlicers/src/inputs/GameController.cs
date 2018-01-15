
using System.Windows.Forms;
using TentacleSlicers.interfaces;
using TentacleSlicers.windows;

namespace TentacleSlicers.inputs
{
    /// <summary>
    /// Gère les évènements en jeu qui en sont pas traités par les controlleurs des joueurs.
    /// </summary>
    public class GameController : IController
    {
        private readonly MainForm _form;
        private readonly WindowsState _state;

        /// <summary>
        /// Crée le GameController associé le menu en jeu à déclencher et la fenêtre de l'application.
        /// </summary>
        /// <param name="form"> La fenêtre de l'application </param>
        /// <param name="escapeState"> Le menu en jeu </param>
        public GameController(MainForm form, WindowsState escapeState)
        {
            _form = form;
            _state = escapeState;
        }

        /// <summary>
        /// Retourne vrai : ne traite pas les évènements enregistrant l'appui d'une touche.
        /// </summary>
        /// <param name="e"> L'objet détaillant l'évènement </param>
        /// <returns> Vrai </returns>
        public bool KeyDown(KeyEventArgs e)
        {
            return true;
        }

        /// <summary>
        /// Vérifie si la touche appuyée n'est pas la touche echap, auquel cas lance le menu de jeu.
        /// </summary>
        /// <param name="e"> L'objet détaillant l'évènement </param>
        /// <returns> Vrai si la touche echap n'a pas été pressée </returns>
        public bool KeyPressed(KeyPressEventArgs e)
        {
            var fetching = true;

            if (e.KeyChar == InputHandler.EscapeCode)
            {
                fetching = false;
                _form.SetState(_state);
            }

            return fetching;
        }

        /// <summary>
        /// Retourne vrai : ne traite pas les évènements enregistrant le relâchement d'une touche.
        /// </summary>
        /// <param name="e"> L'objet détaillant l'évènement </param>
        /// <returns> Vrai </returns>
        public bool KeyUp(KeyEventArgs e)
        {
            return true;
        }
    }
}