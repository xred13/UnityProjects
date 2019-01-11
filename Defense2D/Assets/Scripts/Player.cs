using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public LayerMask circleLayerMask;

    public GameManager gameManager;
    public GameObject bullet;
    public GameObject bulletManager;

    Vector3 savedMousePos = Vector3.zero;
    Vector3 mousePos = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos = new Vector3(mousePos.x, mousePos.y, 0);
        if(savedMousePos != mousePos)
        {
            float degrees = Vector3.SignedAngle(savedMousePos, mousePos, new Vector3(0,0,1));
            transform.RotateAround(Vector3.zero, new Vector3(0, 0, 1), degrees);

            savedMousePos = mousePos;
        }


        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

        Ray2D a = new Ray2D(Vector3.zero, savedMousePos);
        Ray2D b;
        RaycastHit2D hit;

        if (Deflect(a, out b, out hit))
        {
            Debug.DrawLine(a.origin, hit.point);
            Debug.DrawLine(b.origin, b.origin + 3 * b.direction);
        }
    }

    void Shoot()
    {
        if (gameManager.gameState.Equals(GameManager.GameState.OnGoingWaitShooting))
        {

            GameObject spawnedBullet = Instantiate(bullet);
            spawnedBullet.GetComponent<Bullet>().bulletManager = bulletManager.GetComponent<BulletManager>();
            spawnedBullet.transform.SetParent(bulletManager.transform);
            bulletManager.GetComponent<BulletManager>().bullets.Add(spawnedBullet);

            spawnedBullet.GetComponent<Bullet>().InitializeBullet(savedMousePos);

            gameManager.gameState = GameManager.GameState.OnGoingDuringShooting;
        }
    }

    bool Deflect(Ray2D ray, out Ray2D deflected, out RaycastHit2D hit)
    {

        if (hit = Physics2D.Raycast(Vector3.zero,savedMousePos,20,circleLayerMask,0,0))
        {
            Vector3 normal = hit.normal;
            Vector3 deflect = Vector3.Reflect(ray.direction, normal);

            deflected = new Ray2D(hit.point, deflect);
            return true;
        }

        deflected = new Ray2D(Vector3.zero, Vector3.zero);
        return false;
    }
}
