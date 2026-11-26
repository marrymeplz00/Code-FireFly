using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAttackEffect : MonoBehaviour
{
    private Vector3 targetPosition;
    private float speed;

    public float destroyDistance = 0.1f;
    public float lifeTime = 3f;

    public void SetTarget(Vector3 target, float moveSpeed)
    {
        targetPosition = target;
        speed = moveSpeed;

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) <= destroyDistance)
        {
            Destroy(gameObject);
        }
    }
}
