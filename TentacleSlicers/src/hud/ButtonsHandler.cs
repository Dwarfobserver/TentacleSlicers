
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TentacleSlicers.inputs;
using TentacleSlicers.interfaces;
using Point = TentacleSlicers.general.Point;

namespace TentacleSlicers.hud
{
    /// <summary>
    /// Gère et affiche un ou plusieurs boutons en gardant celui qui est focus, et lui redirige les évènements de
    /// touche presée. Permet aussi d'associer un bouton à la touche echap.
    /// </summary>
    public class ButtonsHandler : IDrawable, IKeyPressed
    {
        private readonly List<Button> _buttons;
        private readonly Button _firstButton;
        private Button _focusedButton;
        private Button _echapButton;

        /// <summary>
        /// Crée le ButtonsHandler avec le bouton qui possède le focus au début.
        /// Par défaut, aucun bouton n'est associé à la touche echap.
        /// </summary>
        /// <param name="firstButton"> Le bouton qui possède initialement le focus </param>
        public ButtonsHandler(Button firstButton)
        {
            _buttons = new List<Button>
            {
                firstButton
            };
            _firstButton = firstButton;
            _echapButton = null;

            Reset();
        }

        /// <summary>
        /// Remet le ButtonsHandler à son état d'origine, avec comme bouton focus celui qui fut passé au constructeur.
        /// </summary>
        public void Reset()
        {
            _focusedButton = _firstButton;
            foreach (var b in _buttons) b.SetFocus(false);
            _focusedButton.SetFocus(true);
        }

        /// <summary>
        /// Ajoute un bouton au ButtonsHandler.
        /// </summary>
        /// <param name="button"> Le bouton ajouté </param>
        public void Add(Button button)
        {
            _buttons.Add(button);
        }

        /// <summary>
        /// Lie le bouton donné à la touche echap. Si un bouton y était déjà lié, il est écrasé par le nouveau bouton.
        /// </summary>
        /// <param name="button"> Le bouton lié à la touche echap </param>
        public void LinkToEchap(Button button)
        {
            _echapButton = button;
        }

        /// <summary>
        /// Dessine tous les boutons du ButtonsHandler.
        /// </summary>
        /// <param name="shift"> Le point de décalage </param>
        /// <param name="g"> L'objet permettant de dessiner </param>
        public void Draw(Point shift, Graphics g)
        {
            foreach (var button in _buttons)
            {
                button.Draw(shift, g);
            }
        }

        /// <summary>
        /// Si la touche entrée est appuyée, déclenche l'effet du bouton qui y est lié.
        /// Sinon, délègue l'évènement au bouton ayant le focus et récupère le nouveau bouton focus.
        /// </summary>
        /// <param name="e"> L'objet détaillant l'évènement </param>
        /// <returns> Vrai si aucune action ne correspond à l'évènement </returns>
        public bool KeyPressed(KeyPressEventArgs e)
        {
            if (e.KeyChar == InputHandler.EscapeCode && _echapButton != null)
            {
                _echapButton.Push();
                return false;
            }
            var fetching = _focusedButton.KeyPressed(e);
            _focusedButton = _focusedButton.GetNextFocusedButton();
            return fetching;
        }
    }
}