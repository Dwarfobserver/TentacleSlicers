using TentacleSlicers.collisions;
using TentacleSlicers.general;
using TentacleSlicers.graphics;

namespace TentacleSlicers.actors
{
    /// <summary>
    /// Spécifie la classe Actor pour créer une explosion, qui applique son effet et ses dégâts à tous les ennemis de
    /// son lanceur dans sa zone d'effet, puis s'auto-détruit et laisse un résidu de mort.
    /// </summary>
    public class Explosion : Actor
    {
        private readonly ExplosionData _data;
        private readonly LivingActor _owner;

        /// <summary>
        /// Créer l'explosion lancée par l'acteur indiqué à la position donnée, avec les données fournies.
        /// Ainsi, le délai d'explosion, les dégâts de l'explosion (modifiés par les caractéristiques du lanceur), ses
        /// animations Birth et Death ainsi que son éventuel effet appliqué aux ennemis touchés sont définis.
        /// </summary>
        /// <param name="position"> La position de l'explosion </param>
        /// <param name="owner"> Le lanceur de l'explosion </param>
        /// <param name="data"> Les données de l'explosion </param>
        public Explosion(Point position, LivingActor owner, ExplosionData data) : base(position)
        {
            _data = data;
            _owner = owner;
            Faction = _owner.Faction;
            var anim =  new AnimationHandler(this, _data.Animations);
            anim.PlayAnimation(AnimationType.Birth, false, _data.Delay);
            SpriteHandler = anim;
            ResiduData = new ResiduData(this, AnimationType.Death, false);
        }

        /// <summary>
        /// Vérifie si le délai est écoulé, auquel cas déclenche l'explosion, ce qui tue l'actor courant (et laisse son
        /// résidu de mort) et affecte tous les ennemis dans le rayon de l'explosion.
        /// </summary>
        /// <param name="ms"> Le temps écoulé en millisecondes </param>
        public override void Tick(int ms)
        {
            base.Tick(ms);
            if (!((AnimationHandler) SpriteHandler).IsFinished()) return;

            // Si le délai est écoulé
            var damages = _data.Damage * _owner.SpellPowerRatio();
            foreach (var actor in CollisionFunctions.GetCollidingActors(Position, new Circle(_data.Size)))
            {
                var livingActor = actor as LivingActor;
                if (EnnemyWith(actor) && livingActor != null)
                {
                    livingActor.Damages(damages);
                    _owner.Heals(damages * _owner.VampirismRatio());
                    _data.Effect?.Invoke(_owner, Position, livingActor);
                }
            }
            Kill();
        }
    }
}