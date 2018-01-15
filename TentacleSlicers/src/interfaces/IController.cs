using System.Windows.Forms;

namespace TentacleSlicers.interfaces
{
    /// <summary>
    /// Les classes qui implémentent la classe IController doivent recevoir tous les évènements du clavier, déléguées
    /// depuis la fenêtre de l'application (si il n'ont pas été traités avant). De la même manière qu'avec KeyPressed,
    /// les fonctions KeyDown et KeyUp doivent renvoyer vrai si aucune action n'a été déclenchée par l'évènement.
    /// </summary>
    public interface IController : IKeyPressed
    {
        bool KeyDown(KeyEventArgs e);
        bool KeyUp(KeyEventArgs e);
    }
}