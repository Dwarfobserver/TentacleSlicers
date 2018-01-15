
using System.Collections.Generic;
using TentacleSlicers.AI;
using TentacleSlicers.collisions;
using TentacleSlicers.general;
using TentacleSlicers.graphics;
using TentacleSlicers.maps;
using TentacleSlicers.spells;

namespace TentacleSlicers.actors
{
    /// <summary>
    /// Spécifie la classe LivingActor pour créer un personnage contrôlé par le jeu, à priori hostile au joueur (par
    /// défaut dans la Faction opposée Cultists).
    /// Le mob peut recevoir une série de points qui vont déterminer son chemin, et une cible qu'il va tenter
    /// d'attaquer si il peut lancer des sorts et que ceux-ci ont une portée suffisante.
    /// Sa vie et ses dégâts sont augmentés au fil des niveaux.
    /// </summary>
    public class Mob : LivingActor
    {
        public static readonly string ImagePath = SpriteHandler.ImagePath + "mobs/";

        protected AiBehavior Behavior;
        protected double LootRate;
        protected int ScoreValue;

        private readonly Queue<Point> _targets;

        /// <summary>
        /// Crée un mob avec la vie maximale et initiale indiquée, modifiée par le niveau courant.
        /// Par défaut, un comportement générique (AiBehavior) lui est assigné, qui va déterminer ses destinations et
        /// ses cibles, si elles sont en ligne de vue directe.
        /// </summary>
        /// <param name="position"> La position initiale du mob </param>
        /// <param name="lifeMax"> La vie maximale du mob </param>
        /// <param name="showingLife"> Indique si la vie du mob est temporairement affichée au dessus de lui </param>
        public Mob(Point position, int lifeMax, bool showingLife) : base(position,
            (int) (lifeMax * World.GetWorld().LifeRatio()), showingLife)
        {
            _targets = new Queue<Point>();
            Faction = Faction.Cultists;
            Behavior = new AiBehavior(this);
            ScoreValue = 1;
        }

        /// <summary>
        /// Crée un mob avec toutes les données fournies par la classe MobData.
        /// Un comportement générique est assigné au mob pour lui déterminer ses destinations et ses cibles, si elles
        /// sont en ligne de vue directe.
        /// L'ordre des sorts fournis correspond à leur niveau de priorité, le premier étant le plus prioritaire.
        /// Utilise également le taux de loot indiqué par le paramètre data.
        /// </summary>
        /// <param name="position"> La position initiale du mob </param>
        /// <param name="data"> Les différents paramètres du mob </param>
        public Mob(Point position, MobData data) :
            base(position, (int) (data.Life * World.GetWorld().LifeRatio()), true)
        {
            _targets = new Queue<Point>();
            Faction = Faction.Cultists;
            Behavior = new AiBehavior(this);

            Speed = data.Speed;
            SpriteHandler = new AnimationHandler(this, data.Animations);
            SetCollision(new ActorCollision(this, new Rectangle(data.Size, data.Size), HitboxType.Medium));
            for (var i = 0; i < data.Spells.Length; ++i)
            {
                CreateSpell(i, data.Spells[i]);
            }
            LootRate = data.LootRate;
            ScoreValue = data.ScoreValue;
        }

        /// <summary>
        /// Si le mob meurt après être blessé, une chance correspondant au pourcentage de son taux de loot permet de
        /// faire apparaître un nouveau powerup augmentant les caractéristiques d'un joueur.
        /// De plus, il augmente le score de la partie.
        /// </summary>
        /// <param name="value"> Les dégâts reçus </param>
        public override void Damages(double value)
        {
            base.Damages(value);
            if (!IsDead()) return;

            var lootChance = World.Random.Next(100);
            if (lootChance >= (int) (LootRate * 100)) return;
            var loots = customs.Powerups.PassivePowerups();
            var powerup = loots[World.Random.Next(loots.Count)];
            Powerup.Create(Position, powerup);

            World.GetWorld().AddScore(ScoreValue);
        }

        /// <summary>
        /// Verrouille la fonction CreateSpell, car elle n'est pas destinée à être réécrite.
        /// </summary>
        /// <param name="numSpell"> Le numéro associé au sort </param>
        /// <param name="data"> Les informations pour créer le sort </param>
        public sealed override void CreateSpell(int numSpell, SpellData data)
        {
            base.CreateSpell(numSpell, data);
        }

        /// <summary>
        /// Les dégâts physiques infligés par le mob sont augmentés suivant le niveau courant.
        /// </summary>
        /// <returns> Le facteur de multiplication des dégâts </returns>
        public override double PhysicalDamagesRatio()
        {
            return World.GetWorld().DamagesRatio();
        }

        /// <summary>
        /// Les effets magiques provenant du mob sont augmentés suivant le niveau courant.
        /// </summary>
        /// <returns> Le facteur de puissance des sorts </returns>
        public override double SpellPowerRatio()
        {
            return World.GetWorld().DamagesRatio();
        }

        /// <summary>
        /// Ecrase les destinations actuelles du mob pour lui donner une nouvelle suite de points à parcourir.
        /// </summary>
        /// <param name="targets"> La liste des points, à parcourir dans l'ordre </param>
        public void SetTargets(List<Point> targets)
        {
            _targets.Clear();
            foreach (var p in targets)
            {
                _targets.Enqueue(p);
            }
        }

        /// <summary>
        /// Ecrase les destinations actuelles du mob pour lui en donner une nouvelle.
        /// </summary>
        /// <param name="target"> La nouvelle destination </param>
        public void SetTarget(Point target)
        {
            SetTargets(new List<Point> {target});
        }

        /// <summary>
        /// Actualise le mob avec la fonction Tick de la classe mère, puis modifie sa direction selon les destinations
        /// fournies.
        /// Ensuite, il tente d'attaquer la cible de son AiBehavior si elle existe en essayant ses sorts par ordre de
        /// priorité.
        /// Ces actions sont déclenchées par les fonctions ActualiseMoves puis TryToCast.
        /// L'actualisation est ralentie par le facteur de vitesse courant.
        /// </summary>
        /// <param name="ms"> Le temps écoulé en millisecondes </param>
        public override void Tick(int ms)
        {
            ms = (int) (ms * World.GetWorld().SpeedRatio());
            base.Tick(ms);
            Behavior.Tick(ms);
            ActualiseMoves(ms);
            TryToCast();
        }

        /// <summary>
        /// Modifie le vecteur de vitesse du mob pour qu'il s'approche de sa cible actuelle, directement ou via un
        /// chemin donné par son Behavior via l'algorithme A Star.
        /// </summary>
        /// <param name="ms"> Le temps écoulé en millisecondes </param>
        private void ActualiseMoves(int ms)
        {
            // Rend le mob immobile si il n'a pas de desinations
            if (_targets.Count == 0)
            {
                SpeedVector = Point.Null;
                Orientation = new Point(0, 1);
                return;
            }
            // Actualise les déplacements selon les cibles actuelles
            var p = _targets.Peek();
            if (Speed * ((double)ms / 1000) > Position.Length(p))
            {
                _targets.Dequeue();
                if (_targets.Count == 0)
                {
                    SpeedVector = Point.Null;
                    return;
                }
                p = _targets.Peek();
            }
            SpeedVector = Position.GetOrientation(p) * Speed;
        }

        /// <summary>
        /// Tente le lancer des sorts sur la cible indiquée par le Behavior du mob.
        /// Les sorts sont testés par ordre de priorité, et ils doivent donc être crée avec des numéros consécutifs.
        /// </summary>
        private void TryToCast()
        {
            // Regarde si le mob a une cible et si il peut lancer des sorts
            var target = Behavior.Target;
            if (target == null || CastsLocked()) return;
            // Tente d'attaquer selon la portée des sorts disponibles, par ordre de priorité des sorts
            var attacking = false;
            var indice = 0;
            var distance = Position.Length(target.Position);
            while (!attacking && indice < GetNbSpells())
            {
                var spell = GetSpell(indice);
                if (spell.Range > distance && spell.Castable(target.Position))
                {
                    attacking = true;
                    spell.Cast(target.Position);
                }
                indice++;
            }
        }
    }
}