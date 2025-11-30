using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    public string nextLevel;
    private Player player;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D target)
    {
        if (!target.CompareTag("Player")) return;

        if (CompareTag("Exit_Door"))
        {
            if (player.newKeyCount >= 1)
            {
                Debug.Log("You used a key to exit");
                player.newKeyCount -= 1; // ใช้กุญแจ 1 ดอก
                SceneManager.LoadScene(nextLevel);
            }
            else
            {
                Debug.Log("You need a Key to open this door!");
            }
        }

        else if (CompareTag("Exit_Window"))
        {
            if (player.ropeCount >= 1)
            {
                Debug.Log("You used a rope to exit!");
                player.ropeCount -= 1; // ใช้เชือก 1 เส้น
                SceneManager.LoadScene(nextLevel);
            }
            else
            {
                Debug.Log("You need a Rope to go through the window!");
            }
        }
        else
        {
            Debug.Log("You need a New key or Rope to exit");
        }
    }
}
