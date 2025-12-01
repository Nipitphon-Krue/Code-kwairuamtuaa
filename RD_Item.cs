using UnityEngine;
using System.Collections.Generic;

public class RD_Item : MonoBehaviour
{
    [Header("ไอเท็มที่สามารถสุ่มได้ในแต่ละโซน")]
    public List<GameObject> itemPrefabs;

    [Header("จำนวนไอเท็มสุ่มเพิ่มเติมหลังจากแต่ละโซนได้ 1 ชิ้นแล้ว")]
    public int extraRandomItems = 0;

    [Header("พื้นที่ Spawn Zone (ต้องมี Collider2D)")]
    public List<Transform> spawnZones;

    [Header("Layer ที่ไม่ให้เกิดทับ")]
    public LayerMask blockLayer;

    [Header("ตำแหน่งผู้เล่น")]
    public Transform player;

    [Header("ห้ามเกิดใกล้ผู้เล่นเกินระยะนี้")]
    public float minDistanceFromPlayer = 0f;

    private HashSet<Vector2> usedPositions = new HashSet<Vector2>();

    private float overlapRadius = 0.5f;


    private void Start()
    {
        SpawnOneItemPerZone();
        SpawnExtraItems();
    }

    // =======================================================
    // 1) ให้ทุก Zone มี Item อย่างน้อย 1 ชิ้น
    // =======================================================
    void SpawnOneItemPerZone()
    {
        if (itemPrefabs.Count != spawnZones.Count)
        {
            Debug.LogError("จำนวน Item ในลิสต์ ต้องเท่ากับจำนวน Spawn Zone ถ้าต้องการให้แต่ละ Zone มี Item แบบบังคับ!");
            return;
        }

        for (int i = 0; i < spawnZones.Count; i++)
        {
            Transform zone = spawnZones[i];
            GameObject mustSpawnItem = itemPrefabs[i];   // ใช้ตามลำดับ ไม่สุ่ม!

            Vector2 pos = FindValidSpawnPositionInZone(zone);

            if (pos != Vector2.negativeInfinity)
            {
                Instantiate(mustSpawnItem, pos, Quaternion.identity);
                usedPositions.Add(pos);
            }
            else
            {
                Debug.LogWarning($"Zone {zone.name} ไม่มีตำแหน่งที่เหมาะสม!");
            }
        }
    }

    // =======================================================
    // 2) สุ่มไอเท็มเพิ่มเติม (ไม่บังคับทุกโซน)
    // =======================================================
    void SpawnExtraItems()
    {
        for (int i = 0; i < extraRandomItems; i++)
        {
            Transform zone = spawnZones[Random.Range(0, spawnZones.Count)];
            Vector2 pos = FindValidSpawnPositionInZone(zone);

            if (pos != Vector2.negativeInfinity)
            {
                GameObject item = itemPrefabs[Random.Range(0, itemPrefabs.Count)];
                Instantiate(item, pos, Quaternion.identity);
                usedPositions.Add(pos);
            }
        }
    }

    // =======================================================
    // 3) หา Position ที่ถูกต้องภายใน Zone นั้นๆ
    // =======================================================
    Vector2 FindValidSpawnPositionInZone(Transform zone)
    {
        int safety = 0;

        while (safety < 200)
        {
            safety++;

            Vector2 pos = GetRandomPointInsideZone(zone);

            // ห้ามใกล้ผู้เล่น
            if (Vector2.Distance(pos, player.position) < minDistanceFromPlayer)
                continue;

            // ห้ามตำแหน่งซ้ำ
            if (usedPositions.Contains(pos))
                continue;

            // ห้ามทับ Prop (Layer 1 หรือ Tag "Prop")
            if (IsOverlapWithProp(pos))
                continue;

            // ห้ามทับ blockLayer เช่น ผนัง
            if (Physics2D.OverlapCircle(pos, overlapRadius, blockLayer))
                continue;

            // ต้องเป็นพื้น Floor
            if (!IsOnFloor(pos))
                continue;

            return pos;
        }

        return Vector2.negativeInfinity;
    }

    // =======================================================
    // ตรวจ Floor
    // =======================================================
    bool IsOnFloor(Vector2 pos)
    {
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
        return hit.collider != null && hit.collider.CompareTag("Spawn_Zones");
    }

    // =======================================================
    // ตรวจทับ Prop
    // =======================================================
    bool IsOverlapWithProp(Vector2 pos)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, overlapRadius);

        foreach (var h in hits)
        {
            if (h.gameObject.layer == 1) return true;
            if (h.CompareTag("Prop")) return true;
        }

        return false;
    }

    // =======================================================
    // สุ่มตำแหน่งภายใน Zone
    // =======================================================
    Vector2 GetRandomPointInsideZone(Transform zone)
    {
        Collider2D col = zone.GetComponent<Collider2D>();

        if (col == null)
        {
            Debug.LogWarning("Spawn Zone ไม่มี Collider2D: " + zone.name);
            return zone.position;
        }

        Bounds b = col.bounds;

        for (int i = 0; i < 30; i++)
        {
            float x = Random.Range(b.min.x, b.max.x);
            float y = Random.Range(b.min.y, b.max.y);

            Vector2 pos = new Vector2(x, y);

            if (col.OverlapPoint(pos))
                return pos;
        }

        return b.center;
    }
}
