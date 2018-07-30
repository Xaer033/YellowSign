public struct AttackResult
{
    public readonly IAttackTarget target;
    public readonly int totalDamageDelt;
    public readonly int targetHealthRemaining;
    public readonly bool hasKilledTarget;

    public AttackResult(IAttackTarget attackTarget, int damage, int healthRemaining, bool wasKilled)
    {
        target = attackTarget;
        totalDamageDelt = damage;
        targetHealthRemaining = healthRemaining;
        hasKilledTarget = wasKilled;
    }
}
