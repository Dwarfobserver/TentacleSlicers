
using System;
using TentacleSlicers.actors;
using TentacleSlicers.general;

namespace TentacleSlicers.graphics
{
    /// <summary>
    /// Etend la classe SpriteHandler pour associer plusieurs animations à un acteur.
    /// Met à disposition de nouvelles fonctions pour forcer le lancement d'un nouvelle animation, et pour indiquer le
    /// type d'animation à jouer lorsque l'animation prioritaire courante est terminée.
    /// Le paramètre AnimationType définit le type d'animation non prioritaire.
    /// </summary>
    public class AnimationHandler : SpriteHandler
    {
        public AnimationType AnimationType;

        private readonly AnimationPack _animations;
        private Animation _currentAnimation;
        private int _currentMs;
        private bool _loop;
        private int _maxMs;

        /// <summary>
        /// Crée le gestionnaire d'animations avec comme sprite par défaut le premier sprite de l'animation par défaut
        /// (généralement de type Stand) du pack d'animations fourni. Peut également récupérer un comportement pour
        /// l'opacité de l'animation.
        /// Par défaut, l'animation de type Birth est lancée une unique fois puis le type de l'animation par défaut du
        /// pack d'animations est mis en type d'animation par défaut.
        /// </summary>
        /// <param name="owner"> L'acteur assossié </param>
        /// <param name="animations"> Les animations de l'acteur </param>
        /// <param name="transparency"> Le paramaètre d'opacité optionnel </param>
        public AnimationHandler(Actor owner, AnimationPack animations, TransparencyHandler transparency = null) :
            base(owner, animations.DefaultAnimation.GetSprite(0, Point.Null.GetOrientation()), transparency)
        {
            _animations = animations;
            PlayAnimation(AnimationType.Birth, false);
            AnimationType = animations.DefaultAnimation.Type;
        }

        /// <summary>
        /// Crée le gestionnaire d'animations avec comme sprite par défaut le premier sprite de l'animation donnée.
        /// Peut également récupérer un comportement pour l'opacité de l'animation.
        /// L'animation est lancée une unique fois ou en boucle avec la durée indiquée, sinon celle par défaut.
        /// </summary>
        /// <param name="owner"> L'acteur associé </param>
        /// <param name="animation"> L'animation à jouer </param>
        /// <param name="loop"> Indique si l'animation est joueé en boucle </param>
        /// <param name="msLength"> Fournit une longueur de l'animation optionnelle </param>
        /// <param name="transparency"> Le paramètre d'opacité optionnel </param>
        public AnimationHandler(Actor owner, Animation animation, bool loop, int msLength = -1,
                                TransparencyHandler transparency = null) :
            base(owner, animation.GetSprite(0, owner.Orientation), transparency)
        {
            _animations = new AnimationPack(animation);
            PlayAnimation(animation, loop, msLength);
        }

        /// <summary>
        /// Actualise l'animation courante.
        /// Si l'animation courante est lancée en boucle alors que le type d'animation à jouer par défaut est différent,
        /// lance en boucle l'animation correspondant à ce type d'animation.
        /// Si l'animation est terminée, soit elle doit être jouée en boucle et est relancée, soit elle doit être jouée
        /// une unique fois.
        /// Dans ce cas, si le type d'animation à jouer par défaut est différent, une nouvelle animation correspondant
        /// à ce type est lancée en boucle, sinon l'animation est mise en pause.
        /// </summary>
        /// <param name="ms"></param>
        public override void Tick(int ms)
        {
            if (_loop && _currentAnimation.Type != AnimationType)
            {
                PlayAnimation(AnimationType, true);
            }
            CurrentSprite = _currentAnimation.GetSprite((double) _currentMs / _maxMs, Owner.Orientation);
            _currentMs += ms;
            if (_currentMs < _maxMs) return;
            if (_loop)
            {
                _currentMs -= _maxMs;
            }
            else if (AnimationType == _currentAnimation.Type)
            {
                _currentMs = _maxMs - 1;
            }
            else
            {
                PlayAnimation(AnimationType, true);
            }
        }

        /// <summary>
        /// Appelle PlayAnimation avec l'animation correspondant au type indiqué.
        /// Si les animations fournies ne comportent pas le type indiqué, l'animation lancée correspond à l'animation
        /// par défaut des animations fournies.
        /// </summary>
        /// <param name="type"> Le type de l'animation à jouer </param>
        /// <param name="loop"> Si l'aniamtion doit être jouée en boucle </param>
        /// <param name="msLength"> La longueur de l'animation </param>
        public void PlayAnimation(AnimationType type, bool loop, int msLength = -1)
        {
            PlayAnimation(_animations.GetAnimation(type), loop, msLength);
        }

        /// <summary>
        /// Interrompt l'animation courante pour jouer l'animation indiquée, avec la longueur indiquée (sinon, utilise
        /// la longueur par défaut de l'animation fournie).
        /// Le type d'animation par défaut est mis égal au type de l'animation fournie.
        /// Si l'animation doit être jouée en boucle, elle n'est pas prioritaire et changer la valeur du type à jouer
        /// par défaut va interrompre l'animation en boucle.
        /// Sinon, l'animation est prioritaire et ne peut être interrompue qu'avec un autre appel à cette fonction.
        /// </summary>
        /// <param name="animation"> L'animation à jouer </param>
        /// <param name="loop"> Si l'aniamtion doit être jouée en boucle </param>
        /// <param name="msLength"> La longueur de l'animation </param>
        public void PlayAnimation(Animation animation, bool loop, int msLength = -1)
        {
            _currentAnimation = animation;
            AnimationType = _currentAnimation.Type;
            if (msLength == -1) msLength = _currentAnimation.AnimLength;
            _maxMs = Math.Max(msLength, 1);
            _currentMs = 0;
            _loop = loop;
        }

        /// <summary>
        /// Retourne l'animation correspondant au type d'animation indiqué.
        /// Si l'animation n'existe pas, retourne l'animation par défaut, auquel cas le type de l'animation renvoyée
        /// sera différent du type demandé.
        /// </summary>
        /// <param name="type"> Le type d'animation </param>
        /// <returns> L'animation correspondant au type requis </returns>
        public Animation GetAnimation(AnimationType type)
        {
            return _animations.GetAnimation(type);
        }

        /// <summary>
        /// Renvoie l'animation jouée actuellement.
        /// </summary>
        /// <returns> L'animation courante </returns>
        public Animation CurrentAnimation()
        {
            return _currentAnimation;
        }

        /// <summary>
        /// Indique si l'animation est finie et mise en pause.
        /// </summary>
        /// <returns> Vrai si l'animtaion est finie et mise en pause </returns>
        public bool IsFinished()
        {
            return !_loop && _currentMs == _maxMs - 1;
        }
    }
}