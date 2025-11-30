using UnityEngine;

public class Water : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // ถ้า Player เดินเข้าไปในน้ำ
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            player.speed *= 0.5f;       // ลดความเร็ว 50%
        }

        if (other.CompareTag("Zombie"))
        {
            Zombie zombie = other.GetComponent<Zombie>();
            zombie.speed *= 0.5f;
            zombie.chaseSpeed *= 0.5f;
            
        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // ออกจากน้ำ → กลับสู่ความเร็วปกติ
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            player.speed *= 2f;         // คืนความเร็ว
        }

        if (other.CompareTag("Zombie"))
        {
            Zombie zombie = other.GetComponent<Zombie>();
            zombie.speed *= 2f;
            zombie.chaseSpeed *= 2f;
        }
    }
}
