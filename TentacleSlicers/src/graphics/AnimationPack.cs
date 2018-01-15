
using System.IO;

namespace TentacleSlicers.graphics
{
    /// <summary>
    /// Stocke et organise une ou plusieurs animations, en ne gardant qu'une animation par type d'animation.
    /// </summary>
    public class AnimationPack
    {
        private readonly Animation[] _animations;
        public Animation DefaultAnimation { get; }

        /// <summary>
        /// Crée un pack d'animation à partir d'une animtion, qui est définie comme l'aniamtion par défaut du pack.
        /// </summary>
        /// <param name="animation"> L'animation </param>
        public AnimationPack(Animation animation)
        {
            _animations = new Animation[Animation.NbTypes];
            _animations[(int) animation.Type] = animation;
            DefaultAnimation = animation;
        }

        /// <summary>
        /// Crée (donc charge) toutes les animations dans le dossier indiqué et les assossie à leur type.
        /// La permière animation chargée (dans l'ordre de déclaration des types d'animations) définit l'animation par
        /// défaut du pack, donc généralement stand.
        /// </summary>
        /// <param name="folder"> Le dossier à charger </param>
        /// <param name="heightShift"> Le décalage en hauteur des sprites </param>
        public AnimationPack(string folder, int heightShift = 0)
        {
            folder += "/";
            _animations = new Animation[Animation.NbTypes];
            for (var i = 0; i < Animation.NbTypes; ++i)
            {
                // Teste si le type d'animation courante correspond à un dossier existant
                var type = ((AnimationType) i).ToString();
                var type2 = "" + char.ToLower(type[0]);
                for (var j = 1; j < type.Length; ++j)
                {
                    type2 += type[j];
                }
                var str = folder + type2;
                if (!Directory.Exists(str)) continue;
                // Crée l'animation et la définit comme animation par défaut si il n'en existe pas encore une
                _animations[i] = new Animation(str, heightShift);
                if (DefaultAnimation == null) DefaultAnimation = _animations[i];
            }
        }

        /// <summary>
        /// Retourne l'animaton du type demandé si elle existe, sinon l'animation par défaut.
        /// </summary>
        /// <param name="type"> Le type demandé </param>
        /// <returns> L'animation correspondante ou par défaut </returns>
        public Animation GetAnimation(AnimationType type)
        {
            return _animations[(int) type] ?? DefaultAnimation;
        }

        /// <summary>
        /// Ajoute une animation au pack. Si une animation de même type était déjà stockée, elle est écrasée par la
        /// nouvelle animation.
        /// </summary>
        /// <param name="animation"> la nouvelle animation </param>
        public void AddAnimation(Animation animation)
        {
            _animations[(int) animation.Type] = animation;
        }
    }
}