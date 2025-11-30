using UnityEngine;
using System.Collections.Generic;

public class RD_Item : MonoBehaviour
{
    [Header("ไอเท็มที่ต้องเกิดอย่างน้อย 1 ชิ้น")]
    public List<GameObject> itemPrefabs;

    [Header("จำนวนไอเท็มสุ่มเพิ่มเติม (นอกเหนือจากแบบบังคับ)")]
    public int extraRandomItems = 3;

    [Header("พื้นที่ที่อนุญาตให้สุ่มไอเท็มได้ (ต้องมี Collider2D)")]
    public List<Transform> spawnZones;

    [Header("Layer ที่ไม่ให้ไอเท็มเกิดทับ")]
    public LayerMask blockLayer;

    [Header("ตำแหน่งผู้เล่น")]
    public Transform player;

    [Header("ห้ามเกิดใกล้ผู้เล่นเกินระยะนี้")]
    public float minDistanceFromPlayer = 10f;

    // ใช้กันไม่ให้ตำแหน่งซ้ำ
    private HashSet<Vector2> usedPositions = new HashSet<Vector2>();

    // กันไม่ให้ตำแหน่งอยู่ใกล้วัตถุมากเกินไป
    private float overlapRadius = 5f;


    private void Start()
    {
        SpawnAllItems();
    }

    // ===============================
    // 1) Spawn ไอเท็มทุกชนิดอย่างละ 1 ชิ้น
    // ===============================
    void SpawnAllItems()
    {
        // Spawn ไอเท็มบังคับ
        foreach (var item in itemPrefabs)
        {
            Vector2 pos = FindValidSpawnPosition();
            if (pos != Vector2.negativeInfinity)
            {
                Instantiate(item, pos, Quaternion.identity);
                usedPositions.Add(pos);
            }
        }

        // Spawn ไอเท็มเพิ่มเติมแบบสุ่ม
        for (int i = 0; i < extraRandomItems; i++)
        {
            GameObject randomItem = itemPrefabs[Random.Range(0, itemPrefabs.Count)];

            Vector2 pos = FindValidSpawnPosition();
            if (pos != Vector2.negativeInfinity)
            {
                Instantiate(randomItem, pos, Quaternion.identity);
                usedPositions.Add(pos);
            }
        }
    }


    // ===============================
    // 2) หา Position ที่ถูกต้อง
    // ===============================
    Vector2 FindValidSpawnPosition()
    {
        int safety = 0;

        while (safety < 300)
        {
            safety++;

            // เลือกโซนสุ่ม
            Transform zone = spawnZones[Random.Range(0, spawnZones.Count)];

            // สุ่มตำแหน่งในโซนนั้น
            Vector2 pos = GetRandomPointInsideZone(zone);

            // ❌ ห้ามใกล้ผู้เล่น
            if (Vector2.Distance(pos, player.position) < minDistanceFromPlayer)
                continue;

            // ❌ ห้ามใกล้ Item อื่น ๆ ที่เกิดไปแล้ว
            if (usedPositions.Count > 0)
            {
                // ตรวจสอบระยะห่างกับตำแหน่งที่ใช้ไปแล้ว
                bool tooClose = false;
                foreach (var usedPos in usedPositions)
                {
                    if (Vector2.Distance(pos, usedPos) < overlapRadius * 2)
                    {
                        tooClose = true;
                        break;
                    }
                }
                // ถ้าใกล้เกินไป ให้ข้ามตำแหน่งนี้
                if (tooClose)
                    continue;
            }

            // ❌ ห้ามตำแหน่งซ้ำ
            if (usedPositions.Contains(pos))
                continue;

            // ❌ ห้ามทับ Prop (Layer 1 หรือ Tag "Prop")
            if (IsOverlapWithProp(pos))
                continue;

            // ❌ ห้ามทับสิ่งอื่น ๆ เช่นกำแพง
            if (Physics2D.OverlapCircle(pos, overlapRadius, blockLayer))
                continue;

            // ⭐ ต้องเกิดบน Spawn_Zones เท่านั้น
            if (!IsOnFloor(pos))
                continue;

            // ✔ ผ่านทุกเงื่อนไข → ใช้ตำแหน่งนี้ได้เลย
            return pos;
        }

        Debug.LogWarning("Spawn FAILED: No valid spawn zones position found.");
        return Vector2.negativeInfinity;
    }


    // ===============================
    // 3) ตรวจว่าเกิดบน Spawn_Zones หรือไม่
    // ===============================
    bool IsOnFloor(Vector2 pos)
    {
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);

        if (hit.collider == null)
            return false;

        return hit.collider.CompareTag("Spawn_Zones");
    }


    // ===============================
    // 4) ห้ามทับ Prop
    // ===============================
    bool IsOverlapWithProp(Vector2 pos)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, overlapRadius);

        foreach (var h in hits)
        {
            if (h.gameObject.layer == 1) return true;        // Layer 1 = Prop
            if (h.CompareTag("Prop")) return true;           // Tag = Prop
        }

        return false;
    }


    // ===============================
    // 5) สุ่มตำแหน่งภายใน Collider2D
    // ===============================
    Vector2 GetRandomPointInsideZone(Transform zone)
    {
        Collider2D col = zone.GetComponent<Collider2D>();

        if (col == null)
        {
            Debug.LogWarning("Zone ไม่มี Collider2D: " + zone.name);
            return zone.position;
        }

        Bounds b = col.bounds;

        // พยายามสุ่มให้เจอพื้นที่จริง
        for (int i = 0; i < 30; i++)
        {
            float x = Random.Range(b.min.x, b.max.x);
            float y = Random.Range(b.min.y, b.max.y);

            Vector2 pos = new Vector2(x, y);

            if (col.OverlapPoint(pos))
                return pos;
        }

        return col.bounds.center;
    }
}
