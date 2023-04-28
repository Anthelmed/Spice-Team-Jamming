namespace DefaultNamespace.HealthSystem.Damageable
{
    public interface IDamageable
    {
        void TakeDamage(int _damage, Targetable.Team _team);
        void Heal(int _healAmount);
    }
}