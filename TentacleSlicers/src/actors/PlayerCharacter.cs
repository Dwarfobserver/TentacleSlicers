using System;
using TentacleSlicers.general;
using TentacleSlicers.maps;
using TentacleSlicers.spells;

namespace TentacleSlicers.actors
{
    /// <summary>
    /// Représente le personnage d'un joueur.
    /// Il possède des fonctions appelées par le PlayerController de ce personnage lors de la pression, l'appui ou le
    /// relâchement de touches du clavier.
    /// Il peut ramasser des powerups pour les assigner à une des deux touches de sort.
    /// Il dispose de statistiques sauvegardes et chargeables qui modifient notamment l'impact de ses sorts.
    /// </summary>
    public class PlayerCharacter : LivingActor
    {
        public static readonly string ImagePath = graphics.SpriteHandler.ImagePath + "heros/";

        private bool _keyDown;
        private bool _keyUp;
        private bool _keyLeft;
        private bool _keyRight;
        private int _axisX;
        private int _axisY;

        private int _nextCast;
        private Powerup _powerup;

        public PlayerStats Stats { get; }
        public PlayerStats SavableStats { get; private set; }

        /// <summary>
        /// Crée un personnage sans sorts associé à la faction Players.
        /// Si l'objet PlayerStats n'est pas donné, le personnage est crée avec le statistiques par défaut.
        /// </summary>
        /// <param name="position"> La position initiale du personnage </param>
        /// <param name="lifeMax"> La vie maximale du personnage </param>
        /// <param name="stats"> Les statistiques attribuées au personnage </param>
        protected PlayerCharacter(Point position, int lifeMax, PlayerStats stats = null) : base(position, lifeMax)
        {
            _keyDown = false;
            _keyUp = false;
            _keyLeft = false;
            _keyRight = false;
            _axisX = 0;
            _axisY = 0;
            _nextCast = -1;
            _powerup = null;
            Faction = Faction.Players;
            Stats = stats ?? new PlayerStats();
            SaveStats();
        }

        /// <summary>
        /// Enregistre les statistiques stockées lors d'une sauvegarde.
        /// </summary>
        public void SaveStats()
        {
            SavableStats = new PlayerStats(Stats);
        }

        /// <summary>
        /// Les statistiques du personnages influencent sur ses dégâts physiques.
        /// </summary>
        /// <returns> Le facteur de multiplication des dégâts </returns>
        public override double PhysicalDamagesRatio()
        {
            return Stats.PhysicalDamagesRatio();
        }

        /// <summary>
        /// Les statistiques du personnages influencent sur l'impact de ses sorts.
        /// </summary>
        /// <returns> Le facteur de puissance des sorts </returns>
        public override double SpellPowerRatio()
        {
            return Stats.SpellPowerRatio();
        }

        /// <summary>
        /// Les statistiques du personnages influencent sur le pourcentage de vie gagné par rapport aux dommages
        /// infligés.
        /// </summary>
        /// <returns> Le pourcentage de vie rendu au possesseur par rapport aux dégâts infligés </returns>
        public override double VampirismRatio()
        {
            return Stats.VampirismRatio();
        }

        /// <summary>
        /// Les statistiques du personnages lui permettent de réduire les dégâts subis.
        /// </summary>
        /// <param name="value"> Les dégâts entrants, avant leur réduction </param>
        public override void Damages(double value)
        {
            base.Damages(value * Stats.DamagesTakenRatio());
        }

        /// <summary>
        /// Lance un éventuel sort en attente, puis appelle la fontion Tick de la classe mère.
        /// Ensuite, le vecteur de vitesse est actualisé en fonction de paramètres actualisés lors de l'appui et le
        /// relâchement de touches du clavier.
        /// </summary>
        /// <param name="ms"> Le temps écoulé en millisecondes </param>
        public override void Tick(int ms)
        {
            if (_nextCast != -1 && !CastsLocked())
            {
                KeyPressed_Spell(_nextCast);
                _nextCast = -1;
            }

            base.Tick(ms);

            if (IsDead())
            {
                _axisX = 0;
                _axisY = 0;
                _nextCast = -1;
            }
            double axisSpeed = Speed;
            if (_axisX != 0 && _axisY != 0)
            {
                axisSpeed *= Math.Sqrt(2) / 2;
            }
            SpeedVector = new Point(axisSpeed * _axisX, axisSpeed * _axisY);
        }

        /// <summary>
        /// Indique si le joueur doit assigner une touche au sort qu'il a précédemment obtenu avec le powerup associé.
        /// </summary>
        /// <returns> Vrai si le joueur est en train d'associer une touche à son nouveau sort </returns>
        public bool IsChoosingSpell()
        {
            return _powerup != null;
        }

        /// <summary>
        /// Essaye de ramasser le powerup indiqué et indique si la tentative a réussie.
        /// </summary>
        /// <param name="powerup"> Le powerup </param>
        /// <returns> Vrai si le powerup a été ramassé </returns>
        public bool LootPowerup(Powerup powerup)
        {
            if (CastsLocked()) return false;
            if (powerup.IsAutomaticallyConsumed())
            {
                powerup.Apply(this, 0);
                SpellHandler.ActualiseSpells(Stats);
                return true;
            }
            if (_powerup != null) return false;
            _powerup = powerup;
            return true;
        }

        /// <summary>
        /// Ajoute un sort à l'acteur et l'associe à un indice positif.
        /// Le sort est modifié par les statistiques du personnage.
        /// </summary>
        /// <param name="numSpell"> Le numéro associé au sort </param>
        /// <param name="data"> Les informations pour créer le sort </param>
        public override void CreateSpell(int numSpell, SpellData data)
        {
            SpellHandler.CreateSpell(numSpell, data, Stats);
        }

        /// <summary>
        /// Déclenché lors de l'activation d'une touche de sort, avec le numéro associé.
        /// Regarde si le joueur attribue un powerup aux sorts différents du 0 (qui ne peut être modifié), et applique
        /// le powerup ramassé au numéro si c'est le cas.
        /// Sinon, si le sort associé au numéro existe, il est enregistré pour être lancé lors de l'actualisation du
        /// personnage, avec comme cible l'extrémité du personnage suivant son orientation.
        /// </summary>
        /// <param name="num"> Le numéro du sort </param>
        public void KeyPressed_Spell(int num)
        {
            // Création du nouveau sort (l'abondonne si l'ataque de corsps-à-corps est utilisée)
            if (_powerup != null)
            {
                if (num == 0) // Fix rapide pour empêcher la suppression d'un sort si le joueur n'a pas deux sorts
                {
                    if (GetSpell(1) == null || GetSpell(2) == null) return;
                }
                else
                {
                    _powerup.Apply(this, num);
                }
                World.GetWorld().RemoveSpellPowerup();
                _powerup = null;
                return;
            }
            var spell = GetSpell(num);
            if (spell == null) return;

            // Lance le sort ou l'enregistre pour la fin du sort en cours
            var target = Muzzle();
            if (spell.Castable(target))
            {
                spell.Cast(target);
            }
            else
            {
                _nextCast = num;
            }
        }

        /// <summary>
        /// Déclenché lors de l'appui de la touche correspondant au mouvement vers le bas du personnage.
        /// </summary>
        public void KeyDown_Down()
        {
            _keyDown = true;
            if (!_keyUp) _axisY = 1;
        }

        /// <summary>
        /// Déclenché lors du relâchement de la touche correspondant au mouvement vers le bas du personnage.
        /// </summary>
        public void KeyUp_Down()
        {
            _keyDown = false;
            _axisY = !_keyUp ? 0 : -1;
        }

        /// <summary>
        /// Déclenché lors de l'appui de la touche correspondant au mouvement vers le haut du personnage.
        /// </summary>
        public void KeyDown_Up()
        {
            _keyUp = true;
            if (!_keyDown) _axisY = -1;
        }

        /// <summary>
        /// Déclenché lors du relâchement de la touche correspondant au mouvement vers le haut du personnage.
        /// </summary>
        public void KeyUp_Up()
        {
            _keyUp = false;
            _axisY = !_keyDown ? 0 : 1;
        }

        /// <summary>
        /// Déclenché lors de l'appui de la touche correspondant au mouvement vers la gauche du personnage.
        /// </summary>
        public void KeyDown_Left()
        {
            _keyLeft = true;
            if (!_keyRight) _axisX = -1;
        }

        /// <summary>
        /// Déclenché lors du relâchement de la touche correspondant au mouvement vers la gauche du personnage.
        /// </summary>
        public void KeyUp_Left()
        {
            _keyLeft = false;
            _axisX = !_keyRight ? 0 : 1;
        }

        /// <summary>
        /// Déclenché lors de l'appui de la touche correspondant au mouvement vers la droite du personnage.
        /// </summary>
        public void KeyDown_Right()
        {
            _keyRight = true;
            if (!_keyLeft) _axisX = 1;
        }

        /// <summary>
        /// Déclenché lors du relâchement de la touche correspondant au mouvement vers la droite du personnage.
        /// </summary>
        public void KeyUp_Right()
        {
            _keyRight = false;
            _axisX = !_keyLeft ? 0 : -1;
        }
    }
}