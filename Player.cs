using JetBrains.Annotations;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Player : MonoBehaviour
{
    #region variable

    public float playerHP = 2f;
    // Movement
    public float speed = 5f;
    float moveX;
    float moveY;
    Vector2 moveInput;
    // Animation Clips
    public AnimationClip idleAnimation;
    public AnimationClip moveUpAnimation;
    public AnimationClip moveDownAnimation;
    public AnimationClip moveLeftAnimation;
    public AnimationClip moveRightAnimation;

    // Item collected count
    public int oldKeyCount = 0;
    public int newKeyCount = 0;
    public int hammerCount = 0;
    public int ragCount = 0;
    public int ropeCount = 0;
    public int nailGunCount = 0;

    public GameObject bulletPrefab;
    public GameObject gunPrefab;

    Nail_gun gun;
    private Rigidbody2D rb;
    #endregion

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // อ่านการกดปุ่มจากผู้เล่น
        moveX = Input.GetAxis("Horizontal"); // A, D หรือ ลูกศรซ้ายขวา
        moveY = Input.GetAxis("Vertical");   // W, S หรือ ลูกศรขึ้นลง
        #region move animation
        // กำหนดแอนิเมชันตามทิศทางการเคลื่อนที่
        Animator animator = GetComponent<Animator>();
        if (moveX > 0)
        {
            animator.Play(moveRightAnimation.name);
        }
        else if (moveX < 0)
        {
            animator.Play(moveLeftAnimation.name);
        }
        else if (moveY > 0)
        {
            animator.Play(moveUpAnimation.name);
        }
        else if (moveY < 0)
        {
            animator.Play(moveDownAnimation.name);
        }
        else
        {
            animator.Play(idleAnimation.name);
        }
        #endregion

        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Crafting Item...");
            Craft_Item();
        }
        moveInput = moveInput.normalized;

        if (Input.GetMouseButtonDown(0) && nailGunCount >= 1)
        {
            Shoot();
        }
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(moveX * speed, moveY * speed);
    }

    public void TakeDamage(float dmg)
    {
        playerHP -= dmg;
        Debug.Log("Player HP: " + playerHP);

        if (playerHP <= 0)
        {
            Debug.Log("Player Dead!");
            Destroy(gameObject);
        }
    }

    // Pick up System 
    public void OnTriggerEnter2D(Collider2D target)
    {
        if (target.gameObject.CompareTag("Old Key"))
        {
            Destroy(target.gameObject);
            oldKeyCount++;
            Debug.Log("Old Keys: " + oldKeyCount);
        }
        if (target.gameObject.CompareTag("Hammer"))
        {
            Destroy(target.gameObject);
            hammerCount++;
            Debug.Log("Hammers: " + hammerCount);
        }
        if (target.gameObject.CompareTag("Rag"))
        {
            Destroy(target.gameObject);
            ragCount++;
            Debug.Log("Rags: " + ragCount);
        }
        if (target.gameObject.CompareTag("Nail_Gun"))
        {
            Destroy(target.gameObject);
            nailGunCount++;
            Debug.Log("Nail Guns: " + nailGunCount);

            if (GetComponent<Nail_gun>() == null)
            {
                gameObject.AddComponent<Nail_gun>();
            }
        }
    }

    // ปุ่มคราฟไอเท็ม
    public void Craft_Item()
    {
        if (oldKeyCount >= 1 && hammerCount >= 1)
        {
            newKeyCount += 1;
            oldKeyCount -= 1;
            hammerCount -= 1;
            Debug.Log("Craft New Key");
        }

        if (ragCount >= 3)
        {
            ropeCount += 1;
            ragCount -= 3;
            Debug.Log("Craft Rope");
        }
    }

    // ยิงปืนตะปู
    public void Shoot()
    {
        // ดึง Component ปืนจาก Player
        Nail_gun gun = GetComponent<Nail_gun>();

        // ถ้า Player ไม่มีปืน (ไม่มี Component Nail_gun)
        if (gun == null)
        {
            Debug.Log("You don't have a nail gun equipped.");
            return;
        }

        // เช็กว่ามีกระสุนไหม
        if (gun.ammo <= 0)
        {
            Debug.Log("Out of ammo!");
            gun.CheckAmmo();  // ถ้ากระสุนหมด → ลบปืน
            return;
        }

        // ใช้กระสุน 1 นัด
        gun.ammo--;

        Debug.Log("Fired! Remaining ammo: " + gun.ammo);

        // หาทิศทางเมาส์ในโลกจริงของเกม
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mousePos - transform.position);

        // สร้างกระสุน
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        // ใส่ความเร็วให้กระสุน
        bullet.GetComponent<Rigidbody2D>().velocity = dir * gun.bulletSpeed;

        // ถ้ากระสุนหมด → ลบปืน
        gun.CheckAmmo();
    }

}