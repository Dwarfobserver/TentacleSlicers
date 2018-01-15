using TentacleSlicers.general;

namespace TentacleSlicers.AI
{
    /// <summary>
    /// Classe essentielle à l'a star permettant de détailler une case, notamment en y inscrivant la distance depuis le
    /// lanceur et la distance à vol d'oiseau jusqu'à la cible.
    /// </summary>
    public class Node
    {
        public Point Location { get; set; }
        public bool IsWalkable { get; set; }
        public double G { get; set; }
        public double H { get; set; }
        public double F => G + H;
        public NodeState State { get; set; }
        public Node ParentNode { get; set; }

    }

    public enum NodeState { Untested, Open, Closed }
}