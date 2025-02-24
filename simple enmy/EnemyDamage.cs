using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public int damage = 1;
    public float knockbackPower = 10f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage, knockbackDirection);
        }
    }
}
