
namespace TentacleSlicers.graphics
{
    /// <summary>
    /// Indique une opacité qui diminue progressivement. La fonction Actualise remet l'opacité au maximum.
    /// </summary>
    public class FadingTransparency : TransparencyHandler
    {
        private readonly int _fadingMs;
        private int _currentMs;

        /// <summary>
        /// Crée le comportement d'opacité avec la durée nécessaire pour que l'opacité descende à 1.
        /// </summary>
        /// <param name="fadingMs"> Le temps en millisecondes pour que l'opacité revienne à zéro </param>
        public FadingTransparency(int fadingMs)
        {
            _currentMs = -1;
            _fadingMs = fadingMs;
        }

        /// <summary>
        /// Actualise le compteur pour réduire l'opacité, si elle n'est pas déjà nulle.
        /// </summary>
        /// <param name="ms"> Le temps écoulé en millisecondes </param>
        public override void Tick(int ms)
        {
            if (_currentMs == -1)
            {
                Opacity = 0;
                return;
            }
            _currentMs += ms;
            if (_currentMs >= _fadingMs)
            {
                _currentMs = -1;
            }
            else
            {
                Opacity = (_fadingMs - _currentMs) / (float) _fadingMs;
            }
        }

        /// <summary>
        /// Remet l'opacité au maximum, qui va diminuer progressivemetn jusqu'à zéro.
        /// </summary>
        public void Actualise()
        {
            _currentMs = 0;
        }
    }
}