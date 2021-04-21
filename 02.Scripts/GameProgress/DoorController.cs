using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{

    public AnimationClip dooranimation;
    private Animation anim;
    private string dooranimName = "DoorAnimation";
    private bool open = false;
    private bool conti = true;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        if (open == true && conti == true)
        {
            anim.AddClip(dooranimation, dooranimName);
            anim.Play(dooranimName);
            conti = false;
        }
    }

    public void DoorOpen()
    {
        open = true;

    }

    
}
