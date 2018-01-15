
using System.Drawing;
using TentacleSlicers.actors;
using TentacleSlicers.graphics;

namespace TentacleSlicers.spells
{
    /// <summary>
    /// Simplifie les données à fournir pour créer un sort permettant de lancer un projectile.
    /// </summary>
    public class MissileSpellData : SpellData
    {

        /// <summary>
        /// Crée les données du sort avec comme effet l'invocation du missile depuis son possesseur et vers sa cible.
        /// Certaines données sont écrites par défaut :
        /// L'animation du posesseur lors de l'incantation du sort est de type Cast, le pourcentage du déclenchement de
        /// l'effet est de 1 (ce qui signifie que le sort est lancé à la fin de son incantation) et la portée du sort
        /// équivaut à la portée du missile invoqué.
        /// Ces données prennent en compte les statistiques de son possesseur.
        /// </summary>
        /// <param name="missile"> Le projectile invoqué </param>
        /// <param name="cooldown"> Le temps de rechargemetn du sort </param>
        /// <param name="castTimeMs"> Le temps de lancement du sort </param>
        /// <param name="canMove"> Indique si le lanceur peut bouger pendant le lancement du sort </param>
        /// <param name="nbCharges"> Le nombre de charges du sort (minimum 1) </param>
        /// <param name="icon"> L'icône du sort, qui est affichée des les slots de sorts des joueurs </param>
        public MissileSpellData(MissileData missile, double cooldown, int castTimeMs, bool canMove,
            int nbCharges = 1, Bitmap icon = null) : base((owner, target) =>
            {
                new Missile(owner, target, missile);
            }, AnimationType.Cast, cooldown, castTimeMs, 1, canMove, nbCharges, missile.Range, icon)
        {
        }
    }
}