using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public GameObject[] monsters;

    public GameObject door;
    public Camera maincamera;
    public Transform showCameraPos;
    private bool showDoorOpen = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool allMonsterDie = true;
        for(int i=0;i<monsters.Length;++i)
        {
            bool isDead = monsters[i].GetComponent<CyberSolider>().state == MonsterState.State.Die;
            print(isDead);
            allMonsterDie = allMonsterDie & isDead;
        }

        if(allMonsterDie && !showDoorOpen )
        {
            Debug.Log("다죽었다");
            door.GetComponent<DoorController>().DoorOpen();
            maincamera.GetComponent<SmoothFollowCam>().enabled = false;
            maincamera.transform.position = showCameraPos.position;
            maincamera.transform.LookAt(door.transform);
            showDoorOpen = true;
            StartCoroutine("ReturnCamera");
        }

        
    }

    IEnumerator ReturnCamera()
    {
        yield return new WaitForSeconds(3f);
        maincamera.GetComponent<SmoothFollowCam>().enabled = true;
    }
}
