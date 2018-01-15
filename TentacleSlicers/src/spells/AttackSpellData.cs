
using TentacleSlicers.actors;
using TentacleSlicers.collisions;
using TentacleSlicers.graphics;

namespace TentacleSlicers.spells
{
    /// <summary>
    /// Simplifie les données à fournir pour créer un sort d'attaque au corps-à-corps.
    /// </summary>
    public class AttackSpellData : SpellData
    {

        /// <summary>
        /// Crée un sort qui va infliger des dégâts aux ennemis situés dans un cercle en face du lanceur.
        /// Certaines données sont écrites par défaut :
        /// L'animation du posesseur lors de l'incantation du sort est de type Attack, le sort n'a pas de temps de
        /// recharge, le pourcentage du déclenchement de l'effet est de 1 (ce qui signifie que le sort est lancé à la
        /// fin de son incantation) et la portée du sort équivaut à deux fois la portée indiquée.
        /// Ces données prennent en compte les statistiques de son possesseur.
        /// </summary>
        /// <param name="damages"> Le sdégâts infligés </param>
        /// <param name="range"> La distance de l'acteur au cercle d'impact et le rayon de ce cercle </param>
        /// <param name="castTimeMs"> Le temps d'attaque </param>
        /// <param name="canMove"> Indique si le lanceur peut se déplacer pendant son attaque </param>
        public AttackSpellData(int damages, int range, int castTimeMs, bool canMove) :
            base((caster, target) =>
            {
                target = caster.Muzzle(range);
                var livingCaster = caster as LivingActor;
                var heal = livingCaster?.VampirismRatio() ?? 0.0;
                var actors = CollisionFunctions.GetCollidingActors(target, new Circle(range));
                foreach (var actor in actors)
                {
                    var livingActor = actor as LivingActor;

                    if (livingActor != null && !caster.FriendWith(actor))
                    {
                        livingActor.Damages(damages * caster.PhysicalDamagesRatio());
                        if (heal != 0.0) livingCaster?.Heals(damages * livingCaster.VampirismRatio());
                    }
                }
            }, AnimationType.Attack, 0, castTimeMs, 1, canMove, 2, range * 2)
        {

        }
    }
}
