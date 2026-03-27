namespace Common
{
    public interface IDamageable
    {
        void TakeDamage(int amount, bool isCritical = false);
    }
}