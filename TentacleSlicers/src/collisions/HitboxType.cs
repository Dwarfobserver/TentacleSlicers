namespace TentacleSlicers.collisions
{
    /// <summary>
    /// Classifie les hitbox des Actor et définit si elles se replacent par rapport à une autre hitbox.
    /// Immaterial ne se replace jamais et ne replace personne.
    /// Unmovable ne se replace jamais et force l'autre hitbox si elle n'est pas Immaterial ou Unmovable à se replacer.
    /// Light, Player, Medium et Heavy vont se replacer face à une hitbox plus lourde (ou de même nature si c'est elle
    /// qui a bougé, la priorité est supérieure pour la hitbox immobile) et vont forcer les hitbox plus légères à se
    /// replacer (en dehors de Immaterial).
    /// Player est donc situé, en terme de poids, entre les hitbox Light et les hitbox Medium.
    /// </summary>
    public enum HitboxType
    {
        Immaterial,
        Light,
        Player,
        Medium,
        Heavy,
        Unmovable
    }
}