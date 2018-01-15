namespace TentacleSlicers.graphics
{
    /// <summary>
    /// Les différentes sortes d'animations :
    /// Stand est l'animation par défaut (en première postion),
    /// Birth est l'animation lancée (si existante) lors de la création,
    /// Run est l'animation des controlledActor si leur vecteur de vitesse n'est pas nul,
    /// Attack est l'animation lancée par les sorts crées via AttackSpellData (au corps-à-corps),
    /// Cast est l'animation lancée par les sorts crées via MissileSpellData (lancement d'un projectile),
    /// Explodes est l'animation lancée lors de la collision d'un projectile crée via MissileData,
    /// Death est l'animation généralement lancée lors de la mort d'un acteur via son ResiduData.
    /// </summary>
    public enum AnimationType
    {
        Stand,
        Birth,
        Run,
        Attack,
        Cast,
        Explodes,
        Death
    }
}