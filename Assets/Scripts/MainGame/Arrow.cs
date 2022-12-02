using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KWY;

public class Arrow : MonoBehaviour
{
    public Vector3 targetPosition;
    //[SerializeField] Vector3 targetPosAdj;
    internal bool ArrowFlies = false;
    private float velocity;

    private void Start()
    {
        ArrowFlies = true;
        velocity = 2f;
    }

    // Update is called once per frame
    void Update()
    {
        if (ArrowFlies)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, velocity * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                ArrowFlies = false;
                DestroyMe();
            }
        }
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }
}
