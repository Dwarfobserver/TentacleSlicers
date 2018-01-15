using System.Collections.Generic;
using TentacleSlicers.general;
using TentacleSlicers.levels;
using TentacleSlicers.maps;

namespace TentacleSlicers.customs
{
    /// <summary>
    /// Classe statique qui rescence les étapes des niveaux du jeu.
    /// </summary>
    public class LevelSteps
    {
        private LevelSteps() {}

        public static List<LevelStep> CreateSteps(List<Point> spawns) // Fix rapide : choix entre 3 modèles de niveaux
        {
            var idLevel = World.Random.Next(3);
            if (idLevel == 0)
            {
                var steps = new List<LevelStep>
                {
                    new LevelStep(3, Actors.OrangeCultist(), 2, spawns[World.Random.Next(spawns.Count)]),
                    new LevelStep(15, Actors.GreenCultist(), 2, spawns[World.Random.Next(spawns.Count)]),
                    new LevelStep(15, Actors.OrangeCultist(), 3, spawns[World.Random.Next(spawns.Count)]),
                    new LevelStep(15, Actors.GreenCultist(), 3, spawns[World.Random.Next(spawns.Count)]),
                    new LevelStep(12, null, 0, null)
                };
                return steps;
            }
            else if (idLevel == 1)
            {
                var tmp1 = World.Random.Next(spawns.Count);
                var steps = new List<LevelStep>
                {
                    new LevelStep(3, Actors.OrangeCultist(), 2, spawns[tmp1]),
                    new LevelStep(0, Actors.GreenCultist(), 2, spawns[tmp1]),
                    new LevelStep(30, Actors.GreenCultist(), 1, spawns[World.Random.Next(spawns.Count)]),
                    new LevelStep(0, Actors.GreenCultist(), 1, spawns[World.Random.Next(spawns.Count)]),
                    new LevelStep(0, Actors.GreenCultist(), 1, spawns[World.Random.Next(spawns.Count)]),
                    new LevelStep(18, Actors.OrangeCultist(), 3, spawns[World.Random.Next(spawns.Count)]),
                    new LevelStep(12, null, 0, null)
                };
                return steps;
            }
            else
            {
                var steps = new List<LevelStep>
                {
                    new LevelStep(3, Actors.GreenCultist(), 1, spawns[World.Random.Next(spawns.Count)]),
                    new LevelStep(4, Actors.OrangeCultist(), 1, spawns[World.Random.Next(spawns.Count)]),
                    new LevelStep(5, Actors.GreenCultist(), 1, spawns[World.Random.Next(spawns.Count)]),
                    new LevelStep(5, Actors.OrangeCultist(), 1, spawns[World.Random.Next(spawns.Count)]),
                    new LevelStep(15, Actors.GreenCultist(), 3, spawns[World.Random.Next(spawns.Count)]),
                    new LevelStep(18, Actors.OrangeCultist(), 3, spawns[World.Random.Next(spawns.Count)]),
                    new LevelStep(12, null, 0, null)
                };
                return steps;
            }
        }
    }
}
