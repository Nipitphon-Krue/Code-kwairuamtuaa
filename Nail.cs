using UnityEngine;

public class Nail : MonoBehaviour
{
    public int damage = 1;

    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, 3f); // ลบกระสุนหลัง 3 วิ กันเกมหนัก
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Zombie"))
        {
            // ทำดาเมจ
            collision.GetComponent<Zombie>().TakeDamage(damage);
            Destroy(gameObject); // ทำลายกระสุน
        }
    }
}
