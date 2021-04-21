using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboSystem : MonoBehaviour
{
    enum ComboRank { D = 0, C, B, A, S, SS, SSS };
    string[] rankArray;
    private float currRank = 0;


    public int enoughAnimNum = 5;
    public float increaseRankAmount = 0.25f;
    public float decreaseStartTime = 5f;
    public float decreaseAmount = 0.1f;
    public float decreasePeriod = 0.5f;


    private PlayerController player;
    private List<string> animList;

    private void Awake()
    {
        animList = new List<string>();
        rankArray = new string[(int)ComboRank.SSS + 2] {null, "D", "C", "B", "A", "S", "SS", "SSS" };
    }

    
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<PlayerController>();
        StartCoroutine("decreaseRank");
    }


    void FixedUpdate()
    {
        
    }

    void Attack(string animname)
    {
        int existindex = animList.FindIndex(x => x == animname); // 반드시 람다를 써주어야 하는 것 같습니다.
        if (existindex == -1 || existindex > enoughAnimNum) // 기존에 취했던 동일한 행동이 없다. 
        {
            currRank += increaseRankAmount;
        }

        animList.Insert(0, animname);
        player.LastAttackTime = Time.time;
        ClampComboRank();
    }

    IEnumerator decreaseRank()
    {
        while(true)
        {
           
            if (Time.time - player.LastAttackTime > decreaseStartTime)
            {
                currRank -= decreaseAmount;                
            }
            ClampComboRank();
            if (currRank > 0f)
            {
                Debug.Log(rankArray[(int)currRank] + " " + currRank.ToString() + "  " + (currRank - (int)currRank).ToString());
            }
            yield return new WaitForSeconds(decreasePeriod);
        }
    }

    void ClampComboRank()
    {
        currRank = Mathf.Clamp(currRank, 0f, (float)ComboRank.SSS + 1);
    }
}
