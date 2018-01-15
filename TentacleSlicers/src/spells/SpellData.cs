using System;
using System.Collections.Generic;
using System.Drawing;
using TentacleSlicers.actors;
using TentacleSlicers.graphics;
using Point = TentacleSlicers.general.Point;

namespace TentacleSlicers.spells
{
    /// <summary>
    /// Donne les informations communes et nécessaires à tous les sorts.
    /// Les classes filles de SpellData permettent de simplifier et de réduire le nombre de paramètres à donner.
    /// Chaque SpellData reçoit un indentifiant unique qui sert à le charger et à le sauvegarder grâce aux sauvegardes.
    /// </summary>
    public class SpellData
    {
        private static long _nextId;
        private const int DefaultRange = 5000;

        private static readonly Dictionary<long, SpellData> SpellDatas = new Dictionary<long, SpellData>();

        /// <summary>
        /// Retourne le SpellData qui possède l'identifiant demandé.
        /// </summary>
        /// <param name="id"> L'identifiant du SpellData </param>
        /// <returns> Le SpellData </returns>
        public static SpellData GetSpellData(long id)
        {
            return SpellDatas[id];
        }

        public Action<ControlledActor, Point> Effect { get; }
        public AnimationType AnimationType { get; }
        public double Cooldown { get; }
        public int CastTimeMs { get; }
        public double PercentCast { get; }
        public bool CanMove { get; }
        public int NbCharges { get; }
        public int Range { get; }
        public Bitmap Icon { get; }
        public long Id { get; }

        /// <summary>
        /// Crée les données du sort, et y associe un indentifiant.
        /// </summary>
        /// <param name="effet"> L'effet du sort </param>
        /// <param name="animationType"> Le type d'animation à jouer </param>
        /// <param name="cooldown"> Le temps de rechargemetn du sort </param>
        /// <param name="castTimeMs"> Le temps de lancement du sort </param>
        /// <param name="percentCast"> A quel moment de l'incantation le sort est déclenché (entre 0 et 1) </param>
        /// <param name="canMove"> Indique si le lanceur peut bouger pendant le lancement du sort </param>
        /// <param name="nbCharges"> Le nombre de charges du sort (minimum 1) </param>
        /// <param name="range"> La portée du sort, utilisée par les mobs </param>
        /// <param name="icon"> L'icône du sort, qui est affichée des les slots de sorts des joueurs </param>
        public SpellData(Action<ControlledActor, Point> effet, AnimationType animationType, double cooldown,
            int castTimeMs, double percentCast, bool canMove, int nbCharges = 1, int range = DefaultRange,
            Bitmap icon = null)
        {
            Effect = effet;
            AnimationType = animationType;
            Cooldown = cooldown;
            CastTimeMs = castTimeMs;
            PercentCast = Math.Max(0, Math.Min(0.99, percentCast));
            CanMove = canMove;
            NbCharges = nbCharges;
            Range = range;
            Icon = icon;

            Id = ++_nextId;
            SpellDatas[Id] = this;
        }
    }
}