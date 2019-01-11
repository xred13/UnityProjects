using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public LayerMask circleLayerMask;
    public float speed;
    public int damage;

    Rigidbody2D rb;

    public BulletManager bulletManager;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void InitializeBullet(Vector3 savedMousePos)
    {
        SetDirection(savedMousePos);
        SetSpeed(savedMousePos);
        SetPosition(savedMousePos);
    }

    public void SetDirection(Vector3 savedMousePos)
    {
        float degrees = Vector3.SignedAngle(new Vector3(1,0,0), savedMousePos, Vector3.forward);
        transform.Rotate(new Vector3(0,0,1),degrees);
    }

    public void SetSpeed(Vector3 savedMousePos)
    {
        Vector3 rbVelocity = savedMousePos.normalized * speed;
        rb.velocity = rbVelocity;
    }

    public void SetPosition(Vector3 savedMousePos)
    {
        Vector3 direction = savedMousePos.normalized;
        transform.position = Vector3.zero + direction * bulletManager.bulletSpawnDistanceFromPlayer;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Circle"))
        {
            collision.gameObject.GetComponent<Circle>().TakeDamage(damage);
        }
    }

}
