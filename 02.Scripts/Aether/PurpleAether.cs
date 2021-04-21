using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PurpleAether : Aether
{
    private float recoverAmount = 100f;

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
            player.GetComponent<PlayerController>().RecoverComboGauge(recoverAmount);
            player.GetComponent<PlayerController>().showCurrInterface();
            Destroy(gameObject);
        }
    }
    
}
