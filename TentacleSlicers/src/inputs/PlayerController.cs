
using System.Windows.Forms;
using TentacleSlicers.actors;
using TentacleSlicers.interfaces;

namespace TentacleSlicers.inputs
{
    /// <summary>
    /// Associe les touches du clavier aux actions du personnage d'un joueur (de déplacement et de sort).
    /// </summary>
    public class PlayerController : IController
    {
        private readonly PlayerCharacter _character;
        private readonly Keys[] _movingKeys;
        private readonly int[] _spellingCodes;

        /// <summary>
        /// Associe à chaque joueur quatres touches de déplacement et trois touches de sort.
        /// Les touches associées dépendent de son numéro.
        /// </summary>
        /// <param name="character"> Le joueur associé </param>
        /// <param name="numPlayer"> Le numéro du joueur </param>
        public PlayerController(PlayerCharacter character, int numPlayer)
        {
            _character = character;
            _movingKeys = new Keys[4];
            _spellingCodes = new int[3];

            switch (numPlayer)
            {
                case 0:
                    _movingKeys[0] = Keys.S;
                    _movingKeys[1] = Keys.Z;
                    _movingKeys[2] = Keys.Q;
                    _movingKeys[3] = Keys.D;

                    _spellingCodes[0] = 116; // T
                    _spellingCodes[1] = 121; // Y
                    _spellingCodes[2] = 117; // U
                    break;
                case 1:
                    _movingKeys[0] = Keys.Down;
                    _movingKeys[1] = Keys.Up;
                    _movingKeys[2] = Keys.Left;
                    _movingKeys[3] = Keys.Right;

                    _spellingCodes[0] = 55; // NumPad7
                    _spellingCodes[1] = 56; // NumPad8
                    _spellingCodes[2] = 57; // NumPad9
                    break;
            }
        }

        /// <summary>
        /// Vérifie si le joueur est vivant et si la touche enfoncée correspond à une de ses touches de déplacement.
        /// Puis, déclenche l'action associée à l'appui de la touche de déplacement du joueur.
        /// </summary>
        /// <param name="e"> L'objet détaillant l'évènement </param>
        /// <returns> Vrai si aucune action n'a eté déclenchée </returns>
        public bool KeyDown(KeyEventArgs e)
        {
            if (_character.IsDead()) return true;
            var fetching = true;
            var indice = 0;
            while (fetching && indice < _movingKeys.Length)
            {
                if (e.KeyCode == _movingKeys[indice])
                {
                    fetching = false;

                    switch (InputHandler.MovingKeys[indice])
                    {
                        case Keys.Down:
                            _character.KeyDown_Down();
                            break;
                        case Keys.Up:
                            _character.KeyDown_Up();
                            break;
                        case Keys.Left:
                            _character.KeyDown_Left();
                            break;
                        case Keys.Right:
                            _character.KeyDown_Right();
                            break;
                    }
                }
                else
                {
                    indice++;
                }
            }
            return fetching;
        }

        /// <summary>
        /// Vérifie si le joueur est vivant et si la touche pressée correspond à une de ses touches de sort.
        /// Puis, appelle la fonction qui demande à lancer le sort avec l'indice du sort correspondant.
        /// </summary>
        /// <param name="e"> L'objet détaillant l'évènement </param>
        /// <returns> Vrai si aucune action n'a eté déclenchée </returns>
        public bool KeyPressed(KeyPressEventArgs e)
        {
            if (_character.IsDead()) return true;
            var fetching = true;
            var indice = 0;
            while (fetching && indice < _spellingCodes.Length)
            {
                if (e.KeyChar == _spellingCodes[indice])
                {
                    fetching = false;
                    _character.KeyPressed_Spell(indice);
                }
                else
                {
                    indice++;
                }
            }
            return fetching;
        }

        /// <summary>
        /// Vérifie si le joueur est vivant et si la touche relâchée correspond à une de ses touches de déplacement.
        /// Puis, déclenche l'action associée au relâchement de la touche de déplacement du joueur.
        /// </summary>
        /// <param name="e"> L'objet détaillant l'évènement </param>
        /// <returns> Vrai si aucune action n'a eté déclenchée </returns>
        public bool KeyUp(KeyEventArgs e)
        {
            if (_character.IsDead()) return true;
            var fetching = true;
            var indice = 0;
            while (fetching && indice < _movingKeys.Length)
            {
                if (e.KeyCode == _movingKeys[indice])
                {
                    fetching = false;

                    switch (InputHandler.MovingKeys[indice])
                    {
                        case Keys.Down:
                            _character.KeyUp_Down();
                            break;
                        case Keys.Up:
                            _character.KeyUp_Up();
                            break;
                        case Keys.Left:
                            _character.KeyUp_Left();
                            break;
                        case Keys.Right:
                            _character.KeyUp_Right();
                            break;
                    }
                }
                else
                {
                    indice++;
                }
            }
            return fetching;
        }
    }
}