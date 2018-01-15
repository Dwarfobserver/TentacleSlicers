using System;
using TentacleSlicers.general;

namespace TentacleSlicers.collisions
{
    /// <summary>
    /// Une forme dont la zone de collision est un segment, d'épaisseur nulle.
    /// </summary>
    public class Segment : Form
    {
        private readonly Point _target;

        /// <summary>
        /// Crée le segment d'après les coordonnées de sa cible. Son origine est considérée initialement comme étant
        /// le point (0, 0).
        /// </summary>
        /// <param name="x"> L'abscisse de la cible </param>
        /// <param name="y"> L'ordonnée de la cible </param>
        public Segment(double x, double y)
        {
            _target = new Point(x, y);
        }

        /// <summary>
        /// Crée le segment d'après sa cible. Son origine est considérée initialement comme étant le point (0, 0).
        /// </summary>
        /// <param name="cible"> La cible indiquée </param>
        public Segment(Point cible)
        {
            _target = new Point(cible);
        }

        /// <summary>
        /// Indique si le segment, avec pour origine p1, est en collision avec le rectangle donné de centre p2.
        /// </summary>
        /// <param name="hitbox"> Le rectangle donné </param>
        /// <param name="p1"> L'origine du segment </param>
        /// <param name="p2"> le centre du rectangle </param>
        /// <returns></returns>
        public override bool Collides(Rectangle hitbox, Point p1, Point p2)
        {
            var retour = true;
            /*
            * les points A B C D sont les sommets du rectangles,
            * A est en haut à gauche puis on continue dans le sens des aiguilles d'une montre
            */
            var pA = new Point(p2.X - p1.X - hitbox.Width / 2, p2.Y - p1.Y + hitbox.Height / 2);
            var pB = new Point(p2.X - p1.X + hitbox.Width / 2, p2.Y - p1.Y + hitbox.Height / 2);
            var pC = new Point(p2.X - p1.X + hitbox.Width / 2, p2.Y - p1.Y - hitbox.Height / 2);
            var pD = new Point(p2.X - p1.X - hitbox.Width / 2, p2.Y - p1.Y - hitbox.Height / 2);

            if ( pA.X > Math.Max(0, _target.X)         // la hitbox est à droite
                 || pC.X < Math.Min(0, _target.X)      // la hitbox est à gauche
                 || pA.Y < Math.Min(0, _target.Y)      // la hitbox est en dessous
                 || pC.Y > Math.Max(0, _target.Y))     // la hitbox est au dessus
            {
                retour = false;
            }

            // On calcule le coefficient directeur du segment
            double coeff = 1000;
            if (Math.Abs(_target.X) > 0) coeff = _target.Y / _target.X;

            // le segment est considéré comme verticale ou ne touche pas la hitbox
            if (Math.Abs(coeff) >= 1000 || !retour)  return retour;

            double xCollide, yCollide;
            Point pCollide;
            retour = false;

            // On test si le côté AB croise le segment
            xCollide = pA.Y / coeff;
            pCollide = new Point(xCollide, pA.Y);
            if (Have(pCollide) && xCollide >= pA.X && xCollide <= pB.X) retour = true;

            // On test si le côté BC croise le segment
            yCollide = pC.X * coeff;
            pCollide = new Point(pC.X, yCollide);
            if (Have(pCollide) && yCollide >= pC.Y && yCollide <= pB.Y) retour = true;

            // On test si le côté CD croise le segment
            xCollide = pC.Y / coeff;
            pCollide = new Point(xCollide, pC.Y);
            if (Have(pCollide) && xCollide >= pD.X && xCollide <= pC.X) retour = true;

            // On test si le côté DA croise le segment
            yCollide = pA.X * coeff;
            pCollide = new Point(pA.X, yCollide);
            if (Have(pCollide) && yCollide >= pD.Y && yCollide <= pA.Y) retour = true;

            return retour;
        }

        /// <summary>
        /// Retourne un rectangle incluant le segment :
        /// Il a comme coin les deux bouts du segments, et le milieu du segment comme position.
        /// </summary>
        /// <param name="position"> L'origine du segment </param>
        /// <returns> Le rectangle incluant le segment et son centre </returns>
        public override ShiftedRectangle IncludingRectangle(Point position)
        {
            var middle = position + _target / 2;
            return new ShiftedRectangle(middle, new Rectangle(Math.Abs(_target.X), Math.Abs(_target.Y)));
        }

        /// <summary>
        /// Retourne le premier point de collision entre le segment et le rectangle donné, en partant de l'origine du
        /// segment.
        /// </summary>
        /// <param name="hitbox"> Le rectangle donné </param>
        /// <param name="p1"> L'origine du segment </param>
        /// <param name="p2">  </param>
        /// <returns></returns>
        public Point Impact(Rectangle hitbox, Point p1, Point p2)
        {

            /*
            * les points A B C D sont les sommets du rectangles,
            * A est en haut à gauche puis on continue dans le sens des aiguilles d'une montre
            */
            var pA = new Point(p2.X - p1.X - hitbox.Width / 2, p2.Y - p1.Y + hitbox.Height / 2);
            var pB = new Point(p2.X - p1.X + hitbox.Width / 2, p2.Y - p1.Y + hitbox.Height / 2);
            var pC = new Point(p2.X - p1.X + hitbox.Width / 2, p2.Y - p1.Y - hitbox.Height / 2);
            var pD = new Point(p2.X - p1.X - hitbox.Width / 2, p2.Y - p1.Y - hitbox.Height / 2);

            // On calcule le coefficient directeur du segment
            double coeff = 1000;
            if (Math.Abs(_target.X) > 0) coeff = _target.Y / _target.X;

            // le segment est considéré comme vertical
            if (Math.Abs(coeff) >= 1000)
            {
                return Math.Min(Math.Abs(pA.Y), Math.Abs(pC.Y)) == Math.Abs(pC.Y) ?
                    new Point(p1.X, p1.Y + pC.Y) : new Point(p1.X, p1.Y + pA.Y);
            }


            var impact = _target;
            double xCollide, yCollide;
            Point pCollide;

            // On test si le côté AB croise le segment
            xCollide = pA.Y / coeff;
            pCollide = new Point(xCollide, pA.Y);
            if (Have(pCollide) && xCollide >= pA.X && xCollide <= pB.X &&
                (Segment)pCollide<(Segment)impact) impact = pCollide + p1;

            // On test si le côté BC croise le segment
            yCollide = pC.X * coeff;
            pCollide = new Point(pC.X, yCollide);
            if (Have(pCollide) && yCollide >= pC.Y && yCollide <= pB.Y &&
                (Segment)pCollide<(Segment)impact) impact = pCollide + p1;

            // On test si le côté CD croise le segment
            xCollide = pC.Y / coeff;
            pCollide = new Point(xCollide, pC.Y);
            if (Have(pCollide) && xCollide >= pD.X && xCollide <= pC.X &&
                (Segment)pCollide<(Segment)impact) impact = pCollide + p1;

            // On test si le côté CD croise le segment
            yCollide = pA.X * coeff;
            pCollide = new Point(pA.X, yCollide);
            if (Have(pCollide) && yCollide >= pD.Y && yCollide <= pA.Y &&
                (Segment)pCollide<(Segment)impact) impact = pCollide + p1;

            return impact;
        }

        /// <summary>
        /// Permet de savoir si le point p appartient au segment, sachant que p appartient forcément à la droite
        /// décrite par le segment.
        /// </summary>
        /// <param name="p"> Le point donné </param>
        /// <returns> Vrai si le point donné appartient au segment </returns>
        private bool Have(Point p)
        {
            return _target.X >= 0 && p.X <= _target.X && p.X >= 0
                   || _target.X < 0 && p.X >= _target.X && p.X <= 0;
        }

        /// <summary>
        /// Alternative au constructeur du segment en indiquant sa cible.
        /// </summary>
        /// <param name="p"> La cible du segment </param>
        /// <returns> Le segment allant de (0, 0) à la cible donnée </returns>
        public static explicit operator Segment(Point p)
        {
            return new Segment(p);
        }


        public static bool operator <(Segment p1, Segment p2)
        {
            return Point.Null.Length(p1._target) < Point.Null.Length(p2._target);
        }

        public static bool operator >(Segment p1, Segment p2)
        {
            return Point.Null.Length(p1._target) > Point.Null.Length(p2._target);
        }

    }
}