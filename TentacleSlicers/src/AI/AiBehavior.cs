
using System.Collections.Generic;
using TentacleSlicers.actors;
using TentacleSlicers.collisions;
using TentacleSlicers.general;
using TentacleSlicers.interfaces;
using TentacleSlicers.maps;

namespace TentacleSlicers.AI
{
    /// <summary>
    /// Définit les destinations et une possible cible en ligne de vue directe pour un mob associé.
    /// </summary>
    public class AiBehavior : ITickable
    {
        private const double RefreshRatio = 0.5;

        public PlayerCharacter Target { get; private set; }
        public bool TargetNeedDirectSight;

        private readonly Mob _mob;
        private int _maxMs;
        private int _currentMs;

        /// <summary>
        /// Initialise le comportement en lui demandant de se mettre à jour lorsque le mob associé sera actualisé.
        /// Le paramètre TargetNeedDirectSight esst mis à vrai pour indiquer que la cible n'est indiquée que si le mob
        /// a une ligne de vue directe sur elle.
        /// </summary>
        /// <param name="mob"> Le mob associé </param>
        public AiBehavior(Mob mob)
        {
            TargetNeedDirectSight = true;
            Target = null;
            _mob = mob;

            // Update est déclenché à la prochaine frame
            _currentMs = 0;
            _maxMs = 1;
        }

        /// <summary>
        /// Incrémente un timer interne. Si celui-ci dépasse un certain seuil, le comportement est mis à jour.
        /// Le seuil à dépasser est d'autant plus grand si les adversaires du mob sont éloignés.
        /// Le seuil contient également une part d'aléatoire, le faisant aller du simple au double en terme de durée.
        /// </summary>
        /// <param name="ms"> Le temps écoulé en millisecondes </param>
        public void Tick(int ms)
        {
            _currentMs += ms;
            if (_currentMs <= _maxMs) return;
            var actualisationDistance = Update();
            _currentMs = 0;
            _maxMs = (int) ((actualisationDistance + 100.0) / RefreshRatio);
            _maxMs -= World.Random.Next(_maxMs / 2);
        }

        /// <summary>
        /// Récupère les joueurs vivants du monde.
        /// Si au moins l'un d'entre eux est en ligne de vue directe, le mob aura en destination et en cible le joueur
        /// vivant visible directement le plus près de lui.
        /// Sinon, le mob n'a aucune cible et effectue l'a star pour obtenir un chemin vers le joueur le plus près (non
        /// visible).
        /// </summary>
        /// <returns> La distance entre le mob et le joueur le plus proche afin d'établir le prochain seuil de mise à
        /// jour </returns>
        private double Update()
        {
            Target = null;
            var players = World.GetWorld().GetAlivePlayers();

            // Prend la distance minimale
            var distanceMin = 500000.0;
            var nearestPlayerIndice = 0;
            var indice = 0;
            while (indice < players.Count)
            {
                if (players[indice].IsDead()) continue;
                var distance = _mob.Position.Length(players[indice].Position);
                if (distance < distanceMin)
                {
                    distanceMin = distance;
                    nearestPlayerIndice = indice;
                }
                indice++;
            }
            var actualistaionDistance = distanceMin;
            if (distanceMin >= 500000.0)
            {
                _mob.SetTargets(new List<Point>());
                return actualistaionDistance;
            }

            // Teste si le mob a une ligne de vue sur un des joueurs
            var seeingNobody = true;
            indice = 0;
            distanceMin = 500000;
            var nearestIndirectPlayerIndice = 0;
            while (indice < players.Count && seeingNobody)
            {
                if (players[indice].IsDead()) continue;
                var rayon = new RayonCaster(_mob.Position, players[indice].Position);
                if (rayon.DirectSight())
                {
                    seeingNobody = false;
                }
                else
                {
                    var distance = _mob.Position.Length(players[indice].Position);
                    if (distance < distanceMin)
                    {
                        distanceMin = distance;
                        nearestIndirectPlayerIndice = indice;
                    }
                }
                indice++;
            }
            // Il existe une ligne de vue directe
            if (!seeingNobody)
            {
                Target = players[nearestPlayerIndice];
                _mob.SetTarget(players[nearestPlayerIndice].Position);
            }
            // Le mob ne voit personne
            else
            {
                if (!TargetNeedDirectSight) Target = players[nearestPlayerIndice];
                _mob.SetTargets(new AStar(_mob, players[nearestIndirectPlayerIndice]).Targets);
            }
            return actualistaionDistance;
        }
    }
}