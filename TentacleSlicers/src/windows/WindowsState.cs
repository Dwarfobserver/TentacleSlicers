using System.Drawing;
using System.Windows.Forms;
using TentacleSlicers.hud;
using TentacleSlicers.interfaces;
using Point = TentacleSlicers.general.Point;

namespace TentacleSlicers.windows
{
    /// <summary>
    /// Gère l'application en recevant de la fenêtre les fonctions d'actualisation des données, (Tick) de leur
    /// affichage (Draw) et les évènements provenant du clavier (KeyDown, keyPressed et KeyUp).
    /// </summary>
    public abstract class WindowsState : IController, IDrawable, ITickable
    {
        public static readonly string ImagePath = HudComponent.ImagePath + "windows/";

        protected MainForm Form { get; }

        /// <summary>
        /// Crée l'état de la fenêtre en enregistrant la fenêtre associée.
        /// </summary>
        /// <param name="form"> La fenêtre de l'état </param>
        protected WindowsState(MainForm form)
        {
            Form = form;
        }

        /// <summary>
        /// Par défaut, ne déclenche aucune action lors de l'appui d'une touche.
        /// </summary>
        /// <param name="e"> L'objet détaillant l'évènement </param>
        /// <returns> Vrai </returns>
        public virtual bool KeyDown(KeyEventArgs e)
        {
            return true;
        }

        /// <summary>
        /// Par défaut, ne déclenche aucune action lors de lu relâchement d'une touche.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public virtual bool KeyUp(KeyEventArgs e)
        {
            return true;
        }

        /// <summary>
        /// Par défaut, n'actualise pas de données lorsque le temps s'écoule.
        /// </summary>
        /// <param name="ms"> Le temps écoulé en millisecondes </param>
        public virtual void Tick(int ms) {}

        public abstract bool KeyPressed(KeyPressEventArgs e);
        public abstract void Draw(Point shift, Graphics g);
    }
}