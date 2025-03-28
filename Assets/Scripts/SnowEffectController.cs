using UnityEngine;

public class SnowEffectController : MonoBehaviour
{
    public ParticleSystem snowParticle; // ใส่ Particle System ที่สร้างไว้
    public Rigidbody affectedObject;    // วัตถุที่ได้รับผลจากหิมะ
    public float snowDrag = 0.5f;       // ค่าแรงต้านจากหิมะ

    void Start()
    {
        if (snowParticle != null)
        {
            snowParticle.Play();
        }
    }

    void FixedUpdate()
    {
        if (affectedObject != null)
        {
            // เพิ่มแรงต้าน (Drag) ให้วัตถุเคลื่อนที่ช้าลงเหมือนอยู่ท่ามกลางหิมะ
            affectedObject.linearDamping = snowDrag;
        }
    }
}
