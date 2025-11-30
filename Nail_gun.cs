using Unity.VisualScripting;
using UnityEngine;

public class Nail_gun : MonoBehaviour
{
    public int ammo = 3;
    public float bulletSpeed = 10f;
    Player player;

    // ปืนจะถูกทำลายเมื่อกระสุนหมดเท่านั้น
    public void CheckAmmo()
    {
        if (ammo <= 0)
        {
            Debug.Log("Gun destroyed");
            player = GetComponent<Player>();
            player.nailGunCount = 0; // รีเซ็ตจำนวนปืนตะปูใน Player
        }
    }
}
