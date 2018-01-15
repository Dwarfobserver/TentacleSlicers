using System;
using System.Collections.Generic;
using TentacleSlicers.actors;
using TentacleSlicers.maps;
using Point = TentacleSlicers.general.Point;

namespace TentacleSlicers.AI
{
    /// <summary>
    /// Algorithme permettant de trouver le plus court chemin d'un lanceur jusqu'à une cible.
    /// </summary>
    public class AStar
    {
        private const int WallValue = 1000;

        // La liste de points (dans l'ordre) que doit parcourir le caster
        public List<Point> Targets;

        private readonly List<Node> _walkableNodes = new List<Node>();
        private readonly Map _map;
        private readonly Node[,] _nodes;

        /// <summary>
        /// Initialise une grille de Node à partir de la map.
        /// Lance la fonction recursive search qui est le coeur de l'a star.
        /// la liste Targets est la liste des positions décrivant le plus court chemin.
        /// </summary>
        /// <param name="caster"> L'acteur qui souhaite bouger </param>
        /// <param name="target"> L'acteur que l'on cible </param>
        public AStar(Actor caster, Actor target)
        {
            Targets = new List<Point>();

            _map = World.GetWorld().Map;

            var cases = new int[_map.Width, _map.Height];
            _nodes = new Node[_map.Width, _map.Height];
            for (var i = 0; i < _map.Width; ++i)
            {
                for (var j = 0; j < _map.Height; ++j)
                {
                    _nodes[i, j] = new Node();

                    if (_map.WallsMatrix[i, j])
                    {
                        cases[i, j] = WallValue;
                        _nodes[i, j].IsWalkable = false;
                    }
                    else
                    {
                        cases[i, j] = 0;
                        _nodes[i, j].IsWalkable = true;
                    }

                    _nodes[i, j].Location = new Point(i * Map.Size + Map.Size / 2, j * Map.Size + Map.Size / 2);
                    _nodes[i, j].H = _nodes[i, j].Location.Length(target.Position);
                    _nodes[i, j].G = 1000;
                    _nodes[i, j].State = NodeState.Untested;
                }
            }

            GetPointNode(caster.Position).G = 0;
            GetPointNode(caster.Position).ParentNode = null;

            if (!GetPointNode(target.Position).IsWalkable)
            {
                Targets.Add(target.Position);
            }
            else if (GetPointNode(caster.Position) != GetPointNode(target.Position))
            {
                Search(GetPointNode(caster.Position), GetPointNode(target.Position).Location);

                Targets.Reverse();
                try
                {
                    SignificantTargets();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        /// <summary>
        /// Fonction récursive qui indique si le node fait parti du plus court chemin.
        /// </summary>
        /// <param name="currentNode"> Le node dont on souhaite savoir si il fait parti du plus court chemin </param>
        /// <param name="end"> La position de la cible </param>
        /// <returns> Vrai si le node fait parti du plus court chemin </returns>
        private bool Search(Node currentNode, Point end)
        {
            currentNode.State = NodeState.Closed;
            _walkableNodes.Remove(currentNode);
            GetAdjacentWalkableNodes(currentNode);
            var nextNodes = _walkableNodes;
            nextNodes.Sort((node1, node2) => node1.F.CompareTo(node2.F));
            foreach (var nextNode in nextNodes)
            {
                if (nextNode.Location == end)
                {
                    Targets.Add(nextNode.Location);
                    return true;
                }

                if (Search(nextNode, end))
                {
                    if (nextNode.Location.Length(Targets[Targets.Count-1]) <= Map.Size)
                        Targets.Add(nextNode.Location);
                    return true;
                }
            }
            return false;

        }

        /// <summary>
        /// Fonction ajoutant à la liste _walkableNodes les nodes accessibles et adjacents au node courant.
        /// </summary>
        /// <param name="fromNode"> Le node à partir du quel on souhaite ajouter des nodes dans la liste </param>
        private void GetAdjacentWalkableNodes(Node fromNode)
        {

            var nextLocations = GetAdjacentLocations(fromNode.Location);

            foreach (var location in nextLocations)
            {
                var x = (int) location.X;
                var y = (int) location.Y;

                // On reste à l'intérieur de la map
                if (x < 0 || x >= _map.Width*Map.Size || y < 0 || y >= _map.Height*Map.Size)
                    continue;

                var node = GetPointNode(new Point(x,y));
                // On ignore les noeuds infranchissables
                if (!node.IsWalkable)
                    continue;

                // On ignore les noeuds déjà fermés
                if (node.State == NodeState.Closed)
                    continue;

                // Les noeuds ouverts ne sont ajoutés à la liste qui si leur distance G est plus petite avec
                // cette route.
                if (node.State == NodeState.Open)
                {
                    double traversalCost = node.Location.Length(fromNode.Location);
                    double gTemp = fromNode.G + traversalCost;
                    if (gTemp < node.G)
                    {
                        node.ParentNode = fromNode;
                        node.G = gTemp;
                        if (!_walkableNodes.Contains(node))
                            _walkableNodes.Add(node);
                    }
                }
                else
                {
                    // On met le noeud en "ouvert" si il n'a pas été testé
                    node.ParentNode = fromNode;
                    node.State = NodeState.Open;
                    node.G = fromNode.G + node.Location.Length(fromNode.Location);
                    if (!_walkableNodes.Contains(node))
                        _walkableNodes.Add(node);
                }
            }
        }

        /// <summary>
        /// Fonction renvoyant une liste de points représentant les positions adjacentes au node courant.
        /// </summary>
        /// <param name="fromNodeLocation"> Le point dont on souhaite avoir les positions adjacentes </param>
        /// <returns>  La liste des positions adjacentes </returns>
        private List<Point> GetAdjacentLocations(Point fromNodeLocation)
        {
            var adjacentLocations = new List<Point>
            {
                fromNodeLocation + new Point(-Map.Size, 0),
                fromNodeLocation + new Point(Map.Size, 0),
                fromNodeLocation + new Point(0, -Map.Size),
                fromNodeLocation + new Point(0, Map.Size)
            };

            return adjacentLocations;
        }

        /// <summary>
        /// Récupère le node à partir de la position donnée.
        /// </summary>
        /// <param name="point"> Le point dont on souhaite avoir le node </param>
        /// <returns> Le node qui à le point pour centre </returns>
        private Node GetPointNode(Point point)
        {
            var a = (int) Math.Truncate(point.X / Map.Size);
            var b = (int) Math.Truncate(point.Y / Map.Size);
            return _nodes[a, b];
        }

        /// <summary>
        /// Permet de savoir si la position correspond à un coin d'un mur.
        /// </summary>
        /// <param name="p"> Le point dont on souhaite savoir si il est le coin d'un mur </param>
        /// <returns>  Une liste comportant des Integers :
        ///         1 = coin inférieur gauche
        ///         2 = coin superieur gauche
        ///         3 = coin inférieur droit
        ///         4 = coin supérieur droit
        /// </returns>
        private List<int> GetCornerPath(Point p)
        {
            var listRetour = new List<int>();
            if (!GetPointNode(p + new Point(-Map.Size, -Map.Size)).IsWalkable
                && GetPointNode(p + new Point(-Map.Size,0)).IsWalkable
                && GetPointNode(p + new Point(0,-Map.Size)).IsWalkable) listRetour.Add(1);
            if (!GetPointNode(p + new Point(-Map.Size, Map.Size)).IsWalkable
                && GetPointNode(p + new Point(-Map.Size,0)).IsWalkable
                && GetPointNode(p + new Point(0,Map.Size)).IsWalkable) listRetour.Add(2);
            if (!GetPointNode(p + new Point(Map.Size, -Map.Size)).IsWalkable
                && GetPointNode(p + new Point(Map.Size,0)).IsWalkable
                && GetPointNode(p + new Point(0,-Map.Size)).IsWalkable) listRetour.Add(3);
            if (!GetPointNode(p + new Point(Map.Size, Map.Size)).IsWalkable
                && GetPointNode(p + new Point(Map.Size,0)).IsWalkable
                && GetPointNode(p + new Point(0,Map.Size)).IsWalkable) listRetour.Add(4);
            return listRetour;
        }

        /// <summary>
        /// Supprime les positions non indispensables de Targets, afin d'avoir un trajet plus direct.
        /// </summary>
        private void SignificantTargets()
        {
            var significantTargets = new List<Point>();

            for (var i = 0; i < Targets.Count - 2; i++)
            {
                if (GetCornerPath(Targets[i]).Count>0) significantTargets.Add(Targets[i]);
            }
            significantTargets.Add(Targets[Targets.Count-1]);


            Targets.Clear();
            Targets.Add(significantTargets[0]);
            for (var i = 1; i<significantTargets.Count-2; i++)
            {
                var corners = GetCornerPath(significantTargets[i]);
                double ecart = 0, coef = 1000;
                if (significantTargets[i + 1].X - significantTargets[i - 1].X > 0.001)
                {
                    coef = significantTargets[i + 1].Y - significantTargets[i - 1].Y / significantTargets[i + 1].X -
                           significantTargets[i - 1].X;
                    ecart = significantTargets[i - 1].Y - (significantTargets[i + 1].Y -
                                                           significantTargets[i - 1].Y / significantTargets[i + 1].X -
                                                           significantTargets[i - 1].X);
                }

                if (coef > 0)
                {
                    if (significantTargets[i].Y < coef * significantTargets[i].X + ecart && corners.Contains(2)
                        || significantTargets[i].Y > coef * significantTargets[i].X + ecart && corners.Contains(3))
                    {
                        Targets.Add(significantTargets[i]);
                    }
                }
                else
                {
                    if (significantTargets[i].Y < coef * significantTargets[i].X + ecart && corners.Contains(4)
                        || significantTargets[i].Y > coef * significantTargets[i].X + ecart && corners.Contains(1))
                    {
                        Targets.Add(significantTargets[i]);
                    }
                }
            }
            Targets.Add(significantTargets[significantTargets.Count-1]);
        }
    }
}