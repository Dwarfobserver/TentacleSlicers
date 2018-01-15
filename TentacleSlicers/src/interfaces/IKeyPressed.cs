using System.Windows.Forms;

namespace TentacleSlicers.interfaces
{
    /// <summary>
    /// Les classes qui implémentent cette interface reçoivent des évènements de touches pressées qu'elles doivent
    /// traiter. Si aucune action n'est déclenchée, la fonction KeyPressed doit renvoyer vrai pour indiquer qu'il y a
    /// encore peut-être une action à déclencher avec cet évènement.
    /// </summary>
    public interface IKeyPressed
    {
        bool KeyPressed(KeyPressEventArgs e);
    }
}