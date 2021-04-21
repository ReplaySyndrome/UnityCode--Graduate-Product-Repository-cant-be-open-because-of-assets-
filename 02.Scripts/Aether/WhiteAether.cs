using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class WhiteAether : Aether
{
    private int addAmount = 1;



    // Start is called before the first frame update
    void Start()
    {
        AetherStart();
    }

    // Update is called once per frame

    void Update()
    {
        MoveToCharacter();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            player.GetComponent<PlayerController>().AddWhiteAether(addAmount);
            player.GetComponent<PlayerController>().showCurrInterface();
            Destroy(gameObject);
        }
    }


}
