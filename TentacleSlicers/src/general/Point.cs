
using System;

namespace TentacleSlicers.general
{
    /// <summary>
    /// Permet de stocker deux valuers flottantes (deux doubles).
    /// Ces données sont rendues plus facilement manipualables grâce aux différenes fonctions de la classe.
    /// </summary>
    public class Point
    {
        public static Point Null = new Point(0, 0);
        public double X { get; private set; }
        public double Y { get; private set; }

        /// <summary>
        /// Crée le point d'après les coordonnées données
        /// </summary>
        /// <param name="x"> L'abscisse du point </param>
        /// <param name="y"> L'ordonnée du point </param>
        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Copie le point donné.
        /// </summary>
        /// <param name="p"> Le point à copier </param>
        public Point(Point p)
        {
            X = p.X;
            Y = p.Y;
        }

        /// <summary>
        /// Indique si les deux coordonnées du point sont nulles.
        /// </summary>
        /// <returns> Vrai si les deux coordonnées du point sont égales à zéro </returns>
        public bool IsNull()
        {
            return X == 0 && Y == 0;
        }

        /// <summary>
        /// Retourne la distance euclidienne en pixels entre le point courant et le point donné.
        /// </summary>
        /// <param name="p"> Le point donné </param>
        /// <returns> La distance euclidienne entre les deux points </returns>
        public double Length(Point p)
        {
            return Math.Sqrt((X - p.X) * (X - p.X) + (Y - p.Y) * (Y - p.Y));
        }

        /// <summary>
        /// Retourne le point contenant le cosinus et le sinus de l'angle du segment allant du point courant au point
        /// donné.
        /// </summary>
        /// <param name="target"> Le point déterminant la cible du segment </param>
        /// <returns> Le cosinus et le sinus de l'angle du segment formé par les deux points </returns>
        public Point GetOrientation(Point target)
        {
            var orientation = new Point(Null);
            var p = target - this;
            if (p.X == 0)
            {
                orientation.X = 0;
                if (p.Y > 0)
                {
                    orientation.Y = 1;
                }
                else
                {
                    orientation.Y = -1;
                }
            }
            else
            {
                var a = Math.Atan2(p.Y, p.X);
                orientation.X = Math.Cos(a);
                orientation.Y = Math.Sin(a);
            }
            return orientation;
        }

        /// <summary>
        /// Retourne le point contenant le cosinus et le sinus de l'angle du segment allant de (0, 0) vers le point
        /// courant.
        /// </summary>
        /// <returns> Le cosinus et le sinus de l'angle du segment entre (0, 0) et le point courant </returns>
        public Point GetOrientation()
        {
            return Null.GetOrientation(this);
        }

        /// <summary>
        /// Retourne un point dont les coordonnées sont la somme des deux points indiqués.
        /// </summary>
        /// <param name="p1"> Point à additionner </param>
        /// <param name="p2"> Point à additionner </param>
        /// <returns> Le point correspondant à la somme des deux points </returns>
        public static Point operator +(Point p1, Point p2)
        {
            return new Point(p1.X + p2.X, p1.Y + p2.Y);
        }

        /// <summary>
        /// Retourne un point dont les coordonnées sont la soustraction de p2 à p1.
        /// </summary>
        /// <param name="p1"> Point à soustraire </param>
        /// <param name="p2"> Point soustrayant le premier point </param>
        /// <returns> Le point correspondant à la soustraction de p2 à p1 </returns>
        public static Point operator -(Point p1, Point p2)
        {
            return new Point(p1.X - p2.X, p1.Y - p2.Y);
        }

        /// <summary>
        /// Retourne un point dont les cordonnées sont égales aux coordonnées du point donné multipliées par le facteur
        /// donné.
        /// </summary>
        /// <param name="p"> Le point à multiplier </param>
        /// <param name="value"> Le facteur de multiplication </param>
        /// <returns> Le point correspondant au point multiplié par le facteur </returns>
        public static Point operator *(Point p, double value)
        {
            return new Point(p.X * value, p.Y * value);
        }

        /// <summary>
        /// Retourne un point dont les cordonnées sont égales aux coordonnées du point donné divisées par le facteur
        /// donné.
        /// </summary>
        /// <param name="p"> Le point à diviser </param>
        /// <param name="value"> Le facteur de division </param>
        /// <returns> Le point correspondant au point divisé par le facteur </returns>
        public static Point operator /(Point p, double value)
        {
            return new Point(p.X / value, p.Y / value);
        }

        /// <summary>
        /// Retourne un point dont les coodonnées sont l'opposé des coordonnées du point donné.
        /// </summary>
        /// <param name="p"> Le point dont l'opposé est requis </param>
        /// <returns> L'opposé du point donné </returns>
        public static Point operator -(Point p)
        {
            return new Point( - p.X, - p.Y);
        }

        /// <summary>
        /// Change les coordonnées du point en entiers puis les passe dans la structure Point de System.Drawing.
        /// Utilisé pour dessiner ave les fonctions de System.Drawing en utilisant cette classe.
        /// </summary>
        /// <param name="p"> Le point à convertir </param>
        /// <returns> La structure point </returns>
        public static implicit operator System.Drawing.Point(Point p)
        {
            return new System.Drawing.Point((int) p.X, (int) p.Y);
        }

        /// <summary>
        /// Retourne les coordonnées du point
        /// </summary>
        /// <returns> Les coordonnées du point </returns>
        public override string ToString()
        {
            return "(" + X + "," + Y + ")";
        }
    }
}