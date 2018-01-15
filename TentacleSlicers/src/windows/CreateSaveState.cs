
using System.Drawing;
using System.Windows.Forms;
using TentacleSlicers.graphics;
using TentacleSlicers.inputs;
using TentacleSlicers.maps;
using Point = TentacleSlicers.general.Point;

namespace TentacleSlicers.windows
{
    /// <summary>
    /// Définit l'état permettant au(x) joueur(s) d'entrer le nom de la nouvelle sauvegarde et de la valider, ou de
    /// retourner au menu en jeu.
    /// </summary>
    public class CreateSaveState : WindowsState
    {
        private const int MaxSizeName = 20;
        private static readonly BasicSprite Screen = new BasicSprite(ImagePath + "new save menu.png");
        private static readonly Text TextTop = new Text("Nom de la sauvegarde :", 25);
        private static readonly Text TextBottom = new Text("Echap pour annuler, Entrée pour valider");

        private readonly WindowsState _state;
        private string _name;
        private Text _nameText;

        /// <summary>
        /// Crée l'état avec le menu en jeu vers lequel retourner et le texte à afficher à la fenêtre.
        /// </summary>
        /// <param name="form"> La fenêtre de l'application </param>
        /// <param name="state"> L'état précédent </param>
        public CreateSaveState(MainForm form, WindowsState state) : base(form)
        {
            _state = state;
            _name = "";
            _nameText = new Text("", 20);
        }

        /// <summary>
        /// Permet de valider la sauvegarde ou de revenir à l'état précédent.
        /// Permet également d'entrer le prochain caractère ou d'effacer le dernier pour donner le nom de la sauvegarde.
        /// Les caractères autorisés sont les espaces, les lettres minuscules et majuscules et les chiffres.
        /// </summary>
        /// <param name="e"> L'objet détaillant l'évènement </param>
        /// <returns> Vrai si aucune action n'a été déclenchée </returns>
        public override bool KeyPressed(KeyPressEventArgs e)
        {
            var fetching = true;
            // Annuler la sauvegarde
            if (e.KeyChar == InputHandler.EscapeCode)
            {
                fetching = false;
                Form.SetState(_state);
            }
            // Finaliser la sauvegarde
            else if (e.KeyChar == InputHandler.EnterCode)
            {
                fetching = false;
                GameSave.Add(_name);
                Form.SetState(_state);
            }
            // Effacer la dernière lettre
            else if (e.KeyChar == 8 && _name.Length > 0)
            {
                fetching = false;
                _name = _name.Substring(0, _name.Length - 1);
            }
            // Caractères autorisés
            else if (((e.KeyChar >= 97 && e.KeyChar <= 124) || // Minuscules
                      (e.KeyChar >= 65 && e.KeyChar <= 90) ||  // Majuscules
                     (e.KeyChar >= 48 && e.KeyChar <= 57) ||   // Chiffres (du pavé numérique)
                     e.KeyChar == 32) && _name.Length < MaxSizeName) // Espace
            {
                fetching = false;
                _name += e.KeyChar;
            }
            _nameText = new Text(_name, 30);

            return fetching;
        }

        /// <summary>
        /// Affiche le nom courant de la sauvegarde et le texte environnant.
        /// </summary>
        /// <param name="shift"> La position de décalage, inutiilisée </param>
        /// <param name="g"> l'objet permettant de dessiner </param>
        public override void Draw(Point shift, Graphics g)
        {
            Screen.Draw(MenuState.ScreenPoint, g);
            TextTop.Draw(new Point(600, 200), g);
            _nameText.Draw(new Point(600, 280), g);
            TextBottom.Draw(new Point(600, 600), g);
        }
    }
}