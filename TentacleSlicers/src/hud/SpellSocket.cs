
using System;
using System.Drawing;
using TentacleSlicers.actors;
using TentacleSlicers.graphics;
using TentacleSlicers.spells;
using Point = TentacleSlicers.general.Point;

namespace TentacleSlicers.hud
{
    /// <summary>
    /// Affiche un emplacement de sort d'un joueur. Si cet emplacement est pris, affiche le nombre de charges du sort
    /// correspondant et son rechargement.
    /// </summary>
    public class SpellSocket : HudComponent
    {
        private static readonly BasicSprite SocketSprite = new BasicSprite(ImagePath + "SpellSocket.png");
        private static readonly BasicSprite Bloom = new BasicSprite(ImagePath + "SpellSocketBloom.png");
        private const int HeightShift = - 10;

        private readonly PlayerCharacter _owner;
        private readonly int _numSpell;
        private Spell _currentSpell;
        private BasicSprite _backgroundSprite;
        private ProgressingSprite _foregroundSprite;

        /// <summary>
        /// Crée le composant en indiquant le joueur et son numéro de sort associé.
        /// </summary>
        /// <param name="position"> Le centre du composant </param>
        /// <param name="owner"> Le joueur associé </param>
        /// <param name="numSpell"> Le numéro du sort associé </param>
        public SpellSocket(Point position, PlayerCharacter owner, int numSpell) : base(position)
        {
            _owner = owner;
            _numSpell = numSpell;
            Update();
        }

        /// <summary>
        /// Affiche l'emplacement de sort. Puis, si le numéro de sort du joueur surveillé correspond à un sort, affiche
        /// son icône en fond, affiche de haut en bas l'icône plus lumineuse indiquant le chargement du sort, et écrit
        /// le nombre de charges disponibles.
        /// La couleur du nombre change si aucune n'est disponible, (orange) si il y en a un nombre non maximal (jaune)
        /// ou si le nombre maximal est atteint, (vert) auquel cas l'icône lumineuse est donc affichée intégralement.
        /// </summary>
        /// <param name="shift"> Le point de décalage, inutilisée </param>
        /// <param name="g"> L'objet permettant de dessiner </param>
        public override void Draw(Point shift, Graphics g)
        {
            SocketSprite.Draw(Position, g);

            var p1 = Position + new Point(0, HeightShift);
            _backgroundSprite?.Draw(p1, g);
            _foregroundSprite?.Draw(p1, g);

            if (_owner.IsChoosingSpell())
            {
                Bloom.Draw(Position, g);
                return;
            }

            if (_currentSpell == null) return;
            var p2 = Position - new Point(0, 20);
            var nbCharges = _currentSpell.CurrentCharges;
            var color = Color.Orange;
            if (nbCharges == 0)
            {
                color = Color.Red;
            }
            else if (nbCharges == _currentSpell.MaxChares)
            {
                color = Color.GreenYellow;
            }
            new Text(nbCharges.ToString(), 16, color).Draw(p2, g);
        }

        /// <summary>
        /// Actualise les données observées du sort.
        /// </summary>
        private void Update()
        {
            var spell = _owner.GetSpell(_numSpell);
            if (spell != null && spell != _currentSpell)
            {
                _currentSpell = spell;
                _backgroundSprite = new BasicSprite(spell.Icon)
                {
                    Opacity = 0.5f
                };
                _foregroundSprite = new ProgressingSprite(spell.Icon)
                {
                    Vertical = true
                };
            }
            if (_currentSpell != null)
            {
                _foregroundSprite.Percent = _currentSpell.CooldownRatio();
            }
        }

        /// <summary>
        /// Actualise les données observées du sort.
        /// </summary>
        /// <param name="ms"> le temps écoulé en millisecondes </param>
        public override void Tick(int ms)
        {
            Update();
        }
    }
}