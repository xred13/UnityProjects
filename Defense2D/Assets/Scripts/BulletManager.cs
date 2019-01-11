using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    public float maximumBulletDistanceFromPlayer;
    public float bulletSpawnDistanceFromPlayer;
    public GameManager gameManager;
    public List<GameObject> bullets;


    // Update is called once per frame
    void Update()
    {
        if(bullets.Count > 0)
        {
            for(int i = 0; i < bullets.Count; i++)
            {
                Vector3 position = bullets[i].transform.position;
                if(Vector3.Distance(position, Vector3.zero) > maximumBulletDistanceFromPlayer || Vector3.Distance(position, Vector3.zero) <= gameManager.playerInnerCircleRadius)
                {
                    Destroy(bullets[i]);
                    bullets.RemoveAt(i);

                    if(bullets.Count == 0)
                    {
                        gameManager.gameState = GameManager.GameState.OnGoingAfterShooting;
                    }
                }
            }
        }
    }
}
