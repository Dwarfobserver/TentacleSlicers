
using System;
using System.Drawing;
using System.Windows.Forms;
using TentacleSlicers.graphics;
using TentacleSlicers.inputs;
using TentacleSlicers.interfaces;
using Point = TentacleSlicers.general.Point;

namespace TentacleSlicers.hud
{
    /// <summary>
    /// Bouton personnalisé donnant accès aux boutons définis adjacents lors de l'utilisation des touches z, q, s et d.
    /// Hormis son constructeur, toutes ses fonctions sont appelées par le ButtonsHandler qui le possède.
    /// </summary>
    public class Button : HudComponent, IKeyPressed
    {
        private static readonly BasicSprite ButtonSprite = new BasicSprite(ImagePath + "Button.png");
        private static readonly BasicSprite ButtonFocusSprite = new BasicSprite(ImagePath + "Button_Focus.png");

        public Button BottomButton;
        public Button TopButton;
        public Button LeftButton;
        public Button RightButton;

        private Button _nextButton;

        private bool _hasFocus;
        private readonly Text _text;
        private readonly Action _clicked;

        /// <summary>
        /// Crée le bouton avec la position et le nom donnés.
        /// L'action donnée est déclenchée lorsqu'il est pressé.
        /// Par défaut, il n'est lié à aucun bouton : pour cela, il faut indiquer les boutons situés autour de lui en
        /// les assignant à ses attributs BottomButton, TopButton, LeftButton et RightButton.
        /// </summary>
        /// <param name="position"> Le centre du bouton </param>
        /// <param name="text"> Le texte du bouton </param>
        /// <param name="clicked"> L'action déclenchée lors du clic du bouton </param>
        public Button(Point position, string text, Action clicked) : base(position)
        {
            BottomButton = this;
            TopButton = this;
            LeftButton = this;
            RightButton = this;

            _nextButton = this;

            _hasFocus = false;
            _text = new Text(text);
            _clicked = clicked;
        }

        /// <summary>
        /// Appelé par le ButtonsHandler afin d'établir le premier focus.
        /// </summary>
        public void SetFocus(bool hasFocus)
        {
            _hasFocus = hasFocus;
        }

        /// <summary>
        /// Appelé par le ButtonsHandler après l'utilisation d'une flèche de déplacement pour avoir le bouton suivant.
        /// </summary>
        /// <returns> Le prochain bouton </returns>
        public Button GetNextFocusedButton()
        {
            var temp = _nextButton;
            _nextButton = this;
            return temp;
        }

        /// <summary>
        /// Appelé pour activer l'effet du bouton lorsqu'il est pressé.
        /// </summary>
        public void Push()
        {
            _clicked.Invoke();
        }

        /// <summary>
        /// Dessine le bouton puis son texte. Si le bouton est en focus, son dessin est modifié.
        /// </summary>
        /// <param name="shift"> Le point de référence, pas utilisé car le bouton est en position absolue </param>
        /// <param name="g"> L'objet qui permet de dessiner </param>
        public override void Draw(Point shift, Graphics g)
        {
            if (_hasFocus)
            {
                ButtonFocusSprite.Draw(Position, g);
            }
            else
            {
                ButtonSprite.Draw(Position, g);
            }
            _text.Draw(Position - new Point(0, 70), g);
        }

        /// <summary>
        /// Si une touche de déplacement (z, q, s ou d) a été presée, le focus esst passé au bouton correspondant.
        /// Si la touche entrée a été pressée, l'action du bouton est déclenchée.
        /// </summary>
        /// <param name="e"> L'objet détaillant l'évènement </param>
        /// <returns> Vrai si aucune action ne correspond à l'évènement </returns>
        public bool KeyPressed(KeyPressEventArgs e)
        {
            var fetching = true;
            var indice = 0;
            while (fetching && indice < InputHandler.MovingCodes.Length)
            {
                if (e.KeyChar == InputHandler.MovingCodes[indice])
                {
                    fetching = false;

                    switch (InputHandler.MovingKeys[indice])
                    {
                        case Keys.Down:
                            _nextButton = BottomButton;
                            break;
                        case Keys.Up:
                            _nextButton = TopButton;
                            break;
                        case Keys.Left:
                            _nextButton = LeftButton;
                            break;
                        case Keys.Right:
                            _nextButton = RightButton;
                            break;
                    }
                    _hasFocus = false;
                    _nextButton._hasFocus = true;
                }
                else if (e.KeyChar == InputHandler.EnterCode)
                {
                    fetching = false;
                    Push();
                }
                indice++;
            }
            return fetching;
        }

        /// <summary>
        /// Le temps écoulé ne modifie pas le bouton.
        /// </summary>
        /// <param name="ms"> Le temps écoulé en millisecondes </param>
        public override void Tick(int ms) {}
    }
}