namespace TentacleSlicers.interfaces
{
    /// <summary>
    /// A chaque itération de la boucle principale, les classes qui implémentent cette interface doivent être
    /// actualisées en appelant leur fonction Tick. et en indiquant le temps écoulé en millisecondes.
    /// </summary>
    public interface ITickable
    {
        void Tick(int ms);
    }
}