using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public Transform target;    // ตัวละครที่จะให้กล้องติดตาม
    public float smoothSpeed = 0.125f;
    public Vector3 offset;      // ระยะห่างของกล้องจากผู้เล่น


    // Update is called once per frame
    void LateUpdate()
    {
        if (target == null) return; // ถ้ายังไม่มีตัวละครให้กล้องติดตาม

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z);

    }
}
