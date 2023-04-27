namespace DefaultNamespace.HealthSystem.Damageable
{
    public interface IDamageable
    {
        void TakeDamage(int _damage);
        void Heal(int _healAmount);
    }
}