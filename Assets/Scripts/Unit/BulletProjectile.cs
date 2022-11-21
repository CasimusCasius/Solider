using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{

    [SerializeField] private float bulletSpeed = 200;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] Transform bulletHitVFXPrefab;
    private Vector3 targetPosition;
    public void Setup(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    private void Update()
    {
        Vector3 movDir = (targetPosition - transform.position).normalized;
        float distanceBeforeMoving = Vector3.Distance(targetPosition, transform.position);
        transform.position += movDir * bulletSpeed * Time.deltaTime;
        float distanceAfterMoving = Vector3.Distance(targetPosition, transform.position);

        if (distanceBeforeMoving < distanceAfterMoving ) // fast object
        {
            transform.position = targetPosition;

            trailRenderer.transform.parent = null;

            Destroy(gameObject);

            Instantiate(bulletHitVFXPrefab,targetPosition,Quaternion.identity);
        }
    }
}
