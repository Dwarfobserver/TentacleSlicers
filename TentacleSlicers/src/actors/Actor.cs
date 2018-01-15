
using System;
using System.Drawing;
using TentacleSlicers.collisions;
using TentacleSlicers.graphics;
using TentacleSlicers.interfaces;
using TentacleSlicers.maps;
using Point = TentacleSlicers.general.Point;

namespace TentacleSlicers.actors
{
    /// <summary>
    /// Classe centrale du jeu, les acteurs sont les seules entités générées dans le monde crée. Ils sont déclinés en
    /// sous-classes pour représenter les missiles, les joueurs, les ennemis, le décor ou une simple animation.
    /// </summary>
    public class Actor : IDrawable, ITickable, IComparable<Actor>
    {
        private static long _nextId;
        public long Id { get; }
        private bool _dead;

        public Point SpeedVector;
        public Point Orientation;
        public SpriteHandler SpriteHandler;
        public int Speed;

        public Faction Faction { get; protected set; }
        public Point Position { get; private set; }
        public ActorCollision Collision { get; private set; }

        protected ResiduData ResiduData;

        /// <summary>
        /// Attribue un identifiant unique au nouvel acteur et le rajoute dans le monde avec la position indiquée.
        /// Par défaut, son orientation est tournée vers le bas, il ne génère pas de résidus à sa mort et il
        /// appartient à la faction neutre (il n'est donc ami ou ennemi avec personne).
        /// </summary>
        /// <param name="position"> La position de l'acteur dans le monde </param>
        public Actor(Point position)
        {
            Id = ++_nextId;
            Position = new Point(position);
            World.GetWorld().AddActor(this);
            SpeedVector = Point.Null;
            Orientation = new Point(0, 1);
            _dead = false;
            Faction = Faction.Neutral;
        }

        /// <summary>
        /// Affiche l'acteur si il possède un SpriteHandler à partir de sa position basse.
        /// </summary>
        /// <param name="shift"> La position relative pour afficher l'acteur </param>
        /// <param name="g"> L'objet permettant de dessiner </param>
        public virtual void Draw(Point shift, Graphics g)
        {
            SpriteHandler?.Draw(shift + Bottom(), g);
        }

        /// <summary>
        /// Actualise le SpriteHandler de l'acteur (si il en possède un).
        /// </summary>
        /// <param name="ms"> Le temps écoulé en millisecondes </param>
        public virtual void Tick(int ms)
        {
            SpriteHandler?.Tick(ms);
        }

        /// <summary>
        /// Déplace l'acteur dans le monde, actualise ses collisions (si il en a) et le retrie dans la
        /// liste d'acteurs du monde pour tenir compte de sa nouvelle ordonnée dans l'ordre d'affichage.
        /// </summary>
        /// <param name="shift"> Les coordonnées du point correspondent au déplacement </param>
        public void Move(Point shift)
        {
            Replaces(Position + shift);
            Collision?.UpdateAndCollide();
        }

        /// <summary>
        /// Place l'acteur à une nouvelle position sans actualiser ses collisions.
        /// </summary>
        /// <param name="position"> La nouvelle position de l'acteur </param>
        public void Replaces(Point position)
        {
            World.GetWorld().RemoveActor(this);
            Position = position;
            Collision?.Update();
            World.GetWorld().AddActor(this);
        }

        /// <summary>
        /// Retire l'acteur du monde, le marque comme mort et crée un résidu si le booléen ResiduAtDeath est vrai (et
        /// si l'acteur possède des animations).
        /// Si il possède une collision, celle-ci est retirée du gestionnaire de collisions.
        /// </summary>
        public void Kill()
        {
            World.GetWorld().RemoveActor(this);
            _dead = true;
            Collision?.Dispose();
            // Création du résidu
            if (ResiduData != null && ResiduData.IsValidData())
            {
                new Residu(Bottom(), ResiduData.Animation, ResiduData.SpeedVector);
            }
        }

        /// <summary>
        /// Ressussite l'acteur en lui restaurant sa collision, en lui jouant si possible une animation de type Birth
        /// (en donnant Stand comme type d'animation par défaut) et en le remettant dans le monde.
        /// </summary>
        public virtual void Revive()
        {
            _dead = false;
            if (Collision != null) Collision = new ActorCollision(Collision);

            var animation = SpriteHandler as AnimationHandler;
            if (animation != null)
            {
                animation.PlayAnimation(AnimationType.Birth, false);
                animation.AnimationType = AnimationType.Stand;
            }

            World.GetWorld().AddActor(this);
        }

        /// <summary>
        /// Retire l'acteur du monde pour supprimer son ancienne collision (si il en avait une) du gestionnaire de
        /// collision et lui attribue sa nouvelle collision, puis remet l'acteur dans le monde.
        /// </summary>
        /// <param name="actorCollision"> La nouvelle collision de l'acteur </param>
        public virtual void SetCollision(ActorCollision actorCollision)
        {
            World.GetWorld().RemoveActor(this);
            Collision?.Dispose();
            Collision = actorCollision;
            World.GetWorld().AddActor(this);
        }

        /// <summary>
        /// Indique si l'acteur a déjà été tué, c'est-à-dire retiré du monde.
        /// </summary>
        /// <returns> L'état de l'acteur </returns>
        public bool IsDead()
        {
            return _dead;
        }

        /// <summary>
        /// Indique si l'actor donné est ami avec l'acteur courant.
        /// Un acteur neutre n'est ami avec personne.
        /// </summary>
        /// <param name="actor"> L'acteur avec lequel on vérifie l'amitié </param>
        /// <returns> Vrai si les deux acteurs sont amis </returns>
        public bool FriendWith(Actor actor)
        {
            if (Faction == Faction.Neutral || actor.Faction == Faction.Neutral)
            {
                return false;
            }
            return Faction == actor.Faction;
        }

        /// <summary>
        /// Indique si l'actor donné est ennemi avec l'acteur courant.
        /// Un acteur neutre n'est ennemi avec personne.
        /// </summary>
        /// <param name="actor"> L'acteur avec lequel on vérifie l'ennemi </param>
        /// <returns> Vrai si les deux acteurs sont ennnemis </returns>
        public bool EnnemyWith(Actor actor)
        {
            if (Faction == Faction.Neutral || actor.Faction == Faction.Neutral)
            {
                return false;
            }
            return Faction != actor.Faction;
        }

        /// <summary>
        /// Les attaques au corps-à-corps utilisent ce ratio pour augmenter leurs dégâts.
        /// Initialement, les acteurs possèdent un ratio neutre.
        /// </summary>
        /// <returns> Le facteur de multiplication des dégâts </returns>
        public virtual double PhysicalDamagesRatio()
        {
            return 1;
        }

        /// <summary>
        /// Les sorts utilisent ce ratio pour augmenter l'impact de leurs effets.
        /// Initialement, les acteurs possèdent un ratio neutre.
        /// </summary>
        /// <returns> Le facteur de puissance des sorts </returns>
        public virtual double SpellPowerRatio()
        {
            return 1;
        }

        /// <summary>
        /// Retourne l'extrémité de l'acteur dans la direction de son orientation actuelle.
        /// </summary>
        /// <returns> L'extrémité de l'acteur </returns>
        public Point Muzzle(double gap = 0)
        {
            return Position + Orientation * (gap + (Collision.Hitbox.Width + Collision.Hitbox.Height) / 4);
        }

        /// <summary>
        /// Retourne le bas de l'acteur, à l'extrémité de sa zone de collision.
        /// </summary>
        /// <returns> Le bas de l'acteur </returns>
        public Point Bottom()
        {
            if (Collision == null) return Position;
            return Position + new Point(0, Collision.Hitbox.Height / 2);
        }

        /// <summary>
        /// Trie les acteurs en fonction de leur ordonnée pour les afficher dans le bon ordre.
        /// En cas d'indécision, l'identifiant des acteurs les départagent.
        /// </summary>
        /// <param name="other"> L'acteur à comparer </param>
        /// <returns> 0 si il s'agit du même acteur, 1 si l'acteur courant est plus bas, -1 sinon </returns>
        public int CompareTo(Actor other)
        {
            if (this == other) return 0;
            if (Bottom().Y > other.Bottom().Y)
            {
                return 1;
            }
            if (Bottom().Y < other.Bottom().Y)
            {
                return -1;
            }
            // Lorsque l'odronnée est la même
            if (Id > other.Id)
            {
                return 1;
            }
            return -1;
        }
    }
}