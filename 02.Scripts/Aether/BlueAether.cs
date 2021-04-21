using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BlueAether : Aether
{
    protected float recoverAmount = 100f;

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
        if (other.CompareTag("Player"))
        {
            player.GetComponent<PlayerController>().RecoverMp(recoverAmount);
            player.GetComponent<PlayerController>().showCurrInterface();
            Destroy(gameObject);
        }
    }
}
