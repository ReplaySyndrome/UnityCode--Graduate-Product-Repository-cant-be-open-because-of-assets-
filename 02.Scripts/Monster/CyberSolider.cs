using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CyberSolider : MonoBehaviour
{

    private Animator animator;
    private GameObject player;
    private Rigidbody rb;
    private LineRenderer lineRenderer;
    public MonsterState.State state = MonsterState.State.Idle;
    private Collider[] colliders;

    public Transform SpinePos;
    [Range(0,360)]
    public float viewAngle = 60f;

    private bool findPlayer = false;

    private string findPlayerParameter = "FindPlayer";

    private void Awake()
    {
        animator = GetComponent<Animator>();
        colliders = GetComponents<Collider>();
    }


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (state != MonsterState.State.Die)
        {
            Vector3 dirToPlayer = player.GetComponent<Transform>().position - transform.position;
            Debug.DrawRay(SpinePos.position, player.GetComponent<Transform>().position - transform.position, Color.cyan);
            Debug.DrawRay(transform.position, dirToPlayer, Color.red);



            RaycastHit hit;
            if (Physics.Raycast(SpinePos.position, dirToPlayer, out hit, 10))
            {
                if (hit.collider.gameObject.tag == "Player")
                {
                    Vector3 look = Vector3.Slerp(transform.forward, dirToPlayer.normalized, Time.deltaTime);



                    if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2)
                    {
                        transform.rotation = Quaternion.LookRotation(look, Vector3.up);
                        findPlayer = true;
                        animator.SetBool(findPlayerParameter, findPlayer);
                    }



                }

            }
            else
            {
                findPlayer = false;
                animator.SetBool(findPlayerParameter, findPlayer);
                //Vector3 returnRot = Vector3.Slerp(transform.forward, originalRot.normalized, Time.deltaTime);
                //transform.rotation = Quaternion.LookRotation(returnRot, Vector3.up);
            }
        }


    }

    public void HitByPlayer()
    {
        state = MonsterState.State.Die;
        animator.SetTrigger("Die");
        for(int i=0;i<colliders.Length;++i)
        {
            colliders[i].enabled = false;
        }
    }
}
