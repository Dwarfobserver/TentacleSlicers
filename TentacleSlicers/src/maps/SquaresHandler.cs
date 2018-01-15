
using System.Collections.Generic;
using TentacleSlicers.actors;
using TentacleSlicers.collisions;
using TentacleSlicers.general;

namespace TentacleSlicers.maps
{
    /// <summary>
    /// Le gestionnaire de collisions du jeu. Il découpe l'espace jouable en cases, qui contiennent chacune les acteurs
    /// qui sont en collision avec elles.
    /// Pour cela, lors du test d'une collision d'une forme à une certaine position, le gestionnaire de collisions va
    /// inclure la forme dans un rectangle et va indiquer les acteurs qui sont suspectibles d'être en collision avec la
    /// forme, en retournant les acteurs référencées par les cases en collision avec le rectangle incluant la forme.
    /// Ainsi, cette méthode limite les nombres d'acteurs à tester pour voir si ils entrent en collision avec la forme.
    /// </summary>
    public class SquaresHandler
    {
        private readonly Square[,] _squares;
        private readonly int _width;
        private readonly int _height;

        /// <summary>
        /// Crée le gestionnaire de collision avec une grille de Square aux dimensions indiquées.
        /// Cette grille doit recouvrir l'espace jouable.
        /// </summary>
        /// <param name="width"> La largeur de la grille </param>
        /// <param name="height"> Le hauteur de la grille </param>
        public SquaresHandler(int width, int height)
        {
            _width = width;
            _height = height;
            _squares = new Square[width, height];
            for (var i = 0; i < width; ++i)
            {
                for (var j = 0; j < height; ++j)
                {
                    _squares[i , j] = new Square();
                }
            }
        }

        /// <summary>
        /// Retourne tous les Square qui sont en collision avec un rectangle positionné dans le monde.
        /// </summary>
        /// <param name="data"> Le rectangle positionné </param>
        /// <returns> La liste de Squares </returns>
        private List<Square> GetSquares(ShiftedRectangle data)
        {
            var demiSize = new Point(data.Rectangle.Width / 2, data.Rectangle.Height / 2);
            var min = (data.Position - demiSize) / Square.Size;
            var max = (data.Position + demiSize) / Square.Size;
            var squares = new List<Square>();
            for (var i = (int) min.X; i <= (int) max.X; ++i)
            {
                if (i < 0 || i >= _width) continue;
                for (var j = (int) min.Y; j <= (int) max.Y; ++j)
                {
                    if (j < 0 || j >= _height) continue;
                    squares.Add(_squares[i, j]);
                }
            }
            return squares;
        }

        /// <summary>
        /// Retourne la liste d'acteurs (sans doublons) en collision avec les Squares passés en paramètre.
        /// </summary>
        /// <param name="squares"> La liste de Squares </param>
        /// <returns> La liste d'acteurs </returns>
        public List<Actor> GetActors(List<Square> squares)
        {
            var actors = new List<Actor>();
            foreach (var square in squares)
            {
                square.AddActorsTo(actors);
            }
            return actors;
        }

        /// <summary>
        /// Retourne la liste d'acteurs (sans doublons) proches du rectangle positionné passé en paramètre.
        /// </summary>
        /// <param name="data"> Le rectangle positionné </param>
        /// <returns> La liste d'acteurs </returns>
        public List<Actor> GetActors(ShiftedRectangle data)
        {
            return GetActors(GetSquares(data));
        }

        /// <summary>
        /// Retourne la liste de Squares en collision avec l'acteur passé en paramètres.
        /// Ces squares ajoutent l'acteur à leur liste d'acteurs.
        /// </summary>
        /// <param name="actor"> L'acteur </param>
        /// <param name="hitbox"> La hitbox de l'acteur </param>
        /// <returns> La liste de Squares </returns>
        public List<Square> GetAndUpdateSquares(Actor actor, Rectangle hitbox)
        {
            var squares = GetSquares(new ShiftedRectangle(actor.Position, hitbox));
            foreach (var square in squares)
            {
                square.Actors.Add(actor);
            }
            return squares;
        }

        /// <summary>
        /// Actualise la liste passée en paramètre pour qu'elle n'ait que les Squares en collision avec l'acteur.
        /// Les anciens Squares ne référencent plus l'acteur et les nouveaux le référence.
        /// </summary>
        /// <param name="actor"> L'acteur </param>
        /// <param name="squares"> la liste à actualiser </param>
        public void UpdateSquares(Actor actor, List<Square> squares)
        {
            var newSquares = GetSquares(new ShiftedRectangle(actor.Position, actor.Collision.Hitbox));
            foreach (var square in squares)
            {
                square.Actors.Remove(actor);
            }
            squares.Clear();
            foreach (var square in newSquares)
            {
                square.Actors.Add(actor);
                squares.Add(square);
            }
        }

        /// <summary>
        /// Appelé lorsque l'acteur est mort, pour qu'il soit enlevé des squares qu'il occupait.
        /// </summary>
        /// <param name="actor"> L'acteur mort </param>
        /// <param name="squares"> La liste des squares qu'occupe l'acteur </param>
        public void DisposeActor(Actor actor, List<Square> squares)
        {
            foreach (var square in squares)
            {
                square.Actors.Remove(actor);
            }
        }

        /// <summary>
        /// Indique les acteurs et leurs positions contenus dans chaque Square.
        /// </summary>
        /// <returns> Le contenu de chaque Square du gestionnarie de collisions </returns>
        public override string ToString()
        {
            var strings = new string[_height];
            for (var j = 0; j < _height; j++)
            {
                var str = j + " : ";
                for (var i = 0; i < _width; i++)
                {
                    str += "(" + _squares[i, j] + ")";
                }
                strings[j] = str;
            }
            var finalStr = "SquareHandler :";
            for (var i = 0; i < _height; ++i)
            {
                finalStr += "\n" + strings[i];
            }
            return finalStr;
        }
    }
}