
using TentacleSlicers.interfaces;

namespace TentacleSlicers.general
{
    /// <summary>
    /// Encapsule une valeur (un double) et le garde entre 0 et un maximum indiqué.
    /// Lui rajoute également un facteur de modification en fonction du temps écoulé.
    /// </summary>
    public class BoundedDouble : ITickable
    {
        private double _value;
        private double _valueMax;

        public double Value
        {
            get { return _value; }
            set { ChangeValue(value); }
        }

        public double ValueMax
        {
            get { return _valueMax; }
            set { ChangeValueMax(value); }
        }
        public double Regeneration;

        /// <summary>
        /// Crée la valeur encadrée et l'initialise au maximum donné.
        /// Lui attribue aussi sa régénération (modification en fonction du temps écoulé).
        /// </summary>
        /// <param name="valueMax"> La valeur initiale et maximale </param>
        /// <param name="regeneration"> La régénération de la valeur encadrée </param>
        public BoundedDouble(double valueMax, double regeneration = 0)
        {
            ChangeValueMax(valueMax);
            Regeneration = regeneration;
        }

        /// <summary>
        /// Actualise la valeur encadrée en fonction de sa régénération.
        /// </summary>
        /// <param name="ms"> Le temps écoulé en millisecondes </param>
        public void Tick(int ms)
        {
            if (Regeneration != 0) ChangeValue(_value + ms * (Regeneration / 1000));
        }

        /// <summary>
        /// Indique si la valeur est égale à zéro.
        /// </summary>
        /// <returns> Vrai si la valeur est égale à zéro </returns>
        public bool IsEmpty()
        {
            return _value == 0;
        }

        /// <summary>
        /// Indique si la valeur est égale à son maximum.
        /// </summary>
        /// <returns> Vrai si la valeur est égale à son maximum </returns>
        public bool IsFull()
        {
            return _value == _valueMax;
        }

        /// <summary>
        /// Appelée à chaque fois que la valeur encadrée est modifiée.
        /// </summary>
        /// <param name="newValue"> La nouvelle valeur encadrée </param>
        private void ChangeValue(double newValue)
        {
            _value = newValue;
            if (_value < 0)
            {
                _value = 0;
            }
            else if (_value > _valueMax)
            {
                _value = _valueMax;
            }
        }

        /// <summary>
        /// Change la valeur maximale de la valeur encadrée et la rend égale à cette novuelle valeur maximale.
        /// </summary>
        /// <param name="newMax"> Le nouveau maximum de la valeur encadrée </param>
        private void ChangeValueMax(double newMax)
        {
            _valueMax = newMax;
            ChangeValue(_valueMax);
        }

        /// <summary>
        /// Renvoie la valeur actuelle et sa valeur maximale, ainsi que sa régénération si elle n'est pas nulle.
        /// </summary>
        /// <returns> Le string décrivant la valeur encadrée </returns>
        public override string ToString()
        {
            var str = "";
            if (Regeneration != 0) str = " (" + Regeneration + " per s)";
            return _value + " / " + _valueMax + str;
        }
    }
}