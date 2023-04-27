namespace DefaultNamespace.HealthSystem.Damageable
{
    public interface IDamageable
    {
        void TakeDamage(int _damage, Target.Team _team);
        void Heal(int _healAmount);
    }
}