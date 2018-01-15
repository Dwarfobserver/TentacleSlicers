
using System.Drawing;
using TentacleSlicers.graphics;
using TentacleSlicers.spells;
using TentacleSlicers.states;
using Point = TentacleSlicers.general.Point;

namespace TentacleSlicers.actors
{
    /// <summary>
    /// Spécifie la classe Actor pour les personnages en jeu, des joueurs ou de l'ordinateur.
    /// Implémente leur déplacements et états.
    /// </summary>
    public abstract class ControlledActor : Actor
    {
        private const AnimationType IdleAnimation = AnimationType.Stand;
        private const AnimationType MovingAnimation = AnimationType.Run;

        protected readonly SpellHandler SpellHandler;
        private readonly StateHandler _stateHandler;
        private int _blockingMoves;
        private int _blockingCasts;

        /// <summary>
        /// Initialise les gestionnaires de sorts et d'états.
        /// Par défaut, l'acteur peut bouger et lancer des sorts.
        /// </summary>
        /// <param name="position"> La position de l'acteur dans le monde </param>
        protected ControlledActor(Point position) : base(position)
        {
            _blockingMoves = 0;
            _blockingCasts = 0;
            SpellHandler = new SpellHandler(this);
            _stateHandler = new StateHandler(this);
            ResiduData = new ResiduData(this, AnimationType.Death, false);
        }

        /// <summary>
        /// Affiche l'acteur selon la classe mère et ajoute les possibles affichages de ses états courants.
        /// </summary>
        /// <param name="shift"> La position relative pour afficher l'acteur </param>
        /// <param name="g"> L'objet permettant de dessiner </param>
        public override void Draw(Point shift, Graphics g)
        {
            base.Draw(shift, g);
            _stateHandler.Draw(Bottom() + shift, g);
        }

        /// <summary>
        /// Actualise les sorts et les états de l'acteur puis appelle la fonction Tick() de sa classe mère.
        /// Ensuite, si l'acteur possède un AnmationHandler, on lui assigne comme type d'animation par défaut la valeur
        /// IdleAnimation si il est immobile, MovingAnimtaion sinon.
        /// Enfin, si les mouvements de l'acteur ne sont pas bloqués, on le déplace suivant son vecteur de vitesse et
        /// le temps écoulé.
        /// </summary>
        /// <param name="ms"> Le temps écoulé en millisecondes </param>
        public override void Tick(int ms)
        {
            SpellHandler.Tick(ms);
            _stateHandler.Tick(ms);
            base.Tick(ms);
            var handler = SpriteHandler as AnimationHandler;
            if (SpeedVector.IsNull() && handler != null)
            {
                handler.AnimationType = IdleAnimation;
            }
            else if (!SpeedVector.IsNull() && _blockingMoves == 0)
            {
                if (handler != null) handler.AnimationType = MovingAnimation;
                Orientation = SpeedVector.GetOrientation();
                Move(SpeedVector * ((double)ms / 1000));
            }
        }

        /// <summary>
        /// Bloque les mouvements de l'acteur.
        /// Pour que l'effet soit temporaire, il faut appeler UnlockMoves() lorsque l'on souhaite débloquer l'acteur.
        /// </summary>
        public void LockMoves()
        {
            _blockingMoves++;
        }

        /// <summary>
        /// Empêche l'acteur de lancer des sorts.
        /// Pour que l'effet soit temporaire, il faut appeler UnlockCasts() lorsque l'on souhaite débloquer l'acteur.
        /// </summary>
        public void LockCasts()
        {
            _blockingCasts++;
        }

        /// <summary>
        /// Appelé après LockMoves() afin de permetre à nouveau à l'acteur de se déplacer.
        /// </summary>
        public void UnlockMoves()
        {
            _blockingMoves--;
        }

        /// <summary>
        /// Appelé après LockCasts() afin de permetre à nouveau à l'acteur de lancer des sorts.
        /// </summary>
        public void UnlockCasts()
        {
            _blockingCasts--;
        }

        /// <summary>
        /// Indique si les mouvements de l'acteur sont bloqués.
        /// </summary>
        /// <returns> Vrai si l'acteur ne peut pas se déplacer </returns>
        public bool MovesBlocked()
        {
            return _blockingMoves != 0;
        }

        /// <summary>
        /// Indique si l'acteur peut lancer des sorts.
        /// </summary>
        /// <returns> Vrai si l'acteur ne peut pas lancer de sorts </returns>
        public bool CastsLocked()
        {
            return _blockingCasts != 0;
        }

        /// <summary>
        /// Ajoute un sort à l'acteur et l'associe à un indice (positif), de préférence proche de 0.
        /// </summary>
        /// <param name="numSpell"> Le numéro associé au sort </param>
        /// <param name="data"> Les informations pour créer le sort </param>
        public virtual void CreateSpell(int numSpell, SpellData data)
        {
            SpellHandler.CreateSpell(numSpell, data);
        }

        /// <summary>
        /// Retourne le sort de l'acteur associé au numéro fourni lors de sa création.
        /// </summary>
        /// <param name="numSpell"> L'indice du sort </param>
        /// <returns> Le sort associé </returns>
        public Spell GetSpell(int numSpell)
        {
            return SpellHandler.GetSpell(numSpell);
        }

        /// <summary>
        /// Retourne le nombre de sorts que possède l'acteur au maximum.
        /// La valeur renvoyée - 1 donne l'indice du dernier sort de l'adcteur, mais il n'est pas certifié que les
        /// indices inférieurs référencent un sort.
        /// </summary>
        /// <returns> Le nombre de sorts de l'acteur </returns>
        public int GetNbSpells()
        {
            return SpellHandler.GetNbSpells();
        }

        /// <summary>
        /// Ajoute un état à l'acteur. Utilisé par exemple pour l'empêcher de lancer des sorts, de se déplacer ou pour
        /// augmenter temporairement ses caractéristiques.
        /// </summary>
        /// <param name="data"> Les informations concernant l'état à créer </param>
        /// <param name="arg"> L'agument optionnel pour créer l'état </param>
        public void CreateState(StateData data, object arg = null)
        {
            _stateHandler.CreateState(data, arg);
        }

        /// <summary>
        /// Supprime tous les effets présents en déclenchant leur effet d'expiration.
        /// Utilisé notamment pour récupérer les caractéristiques du joueur à sauvegarder, afin de ne pas comptabiliser
        /// des caractéristiques temporaires.
        /// </summary>
        public void CleanStates()
        {
            _stateHandler.Clean();
        }
    }
}