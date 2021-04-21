using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Aether : MonoBehaviour
{
    protected GameObject player;
    protected Rigidbody rb;
    protected float cortime = 0.1f;
    protected float speed = 3.0f;
    protected float moveToDistance = 10.0f;
    protected bool moveToCharcater = false;
    protected Collider collider;
    protected Vector3 characterCenter;

    protected void AetherStart()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }
    



    protected void MoveToCharacter()
    {
        if (!moveToCharcater)
        {
            if (Vector3.Distance(player.transform.position, transform.position) < moveToDistance)
            {
                moveToCharcater = true;
                rb.isKinematic = true;
                rb.useGravity = false;
                collider.isTrigger = true;
            }
        }
        else
        {
            Vector3 direction = player.GetComponent<PlayerController>().characterCenter.position - transform.position;
            transform.Translate(direction * speed * Time.deltaTime ); // 델타타임을 곱하는게 맞나? 1초에 60번 언저리로 호출될텐데
        }
    }

}
