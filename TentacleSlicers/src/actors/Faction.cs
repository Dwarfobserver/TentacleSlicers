namespace TentacleSlicers.actors
{
    /// <summary>
    /// Indique la faction d'un acteur.
    /// Un acteur neutre n'est allié ou ennemi avec personne, sinon deux acteurs sont amis si ils sont de la même
    /// faction, ou ennemis si leurs factions sont différentes.
    /// En règle générale, le décor est neutre, les joueurs appartiennent à la faction Players et les autres
    /// personnages appartiennent à la faction Cultists.
    /// </summary>
    public enum Faction
    {
        Neutral,
        Players,
        Cultists
    }
}