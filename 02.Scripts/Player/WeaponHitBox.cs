using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHitBox : MonoBehaviour
{
    GameObject player;

    HashSet<int> collidedObject = new HashSet<int>();

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Damageable>() != null)
        {
            
            if (!collidedObject.Contains(other.gameObject.GetInstanceID()))
            {
               
                if (gameObject.tag == "Monster")
                {
                    Debug.Log(other.gameObject.name);
                    collidedObject.Add(other.gameObject.GetInstanceID());
                    Debug.Log(other.gameObject.name + "이 충돌했습니다.");
                    PlayerController pc = player.GetComponent<PlayerController>();
                    SkillInformation skillinfo = pc.CurrSkillInformation;
                    if (skillinfo.mpConsumption < pc.CurrMP && skillinfo.comboGaugeConsumption < pc.CurrComboGauge)
                    {
                        pc.CurrMP = pc.CurrMP - skillinfo.mpConsumption;
                        pc.CurrComboGauge = pc.CurrComboGauge + skillinfo.comboGaugeConsumption + skillinfo.comboGaugeGain;
                    }
                    print("공격성공 " + pc.CurrMP + " " + pc.CurrComboGauge);

                    
                }

                if(other.gameObject.GetComponent<CyberSolider>() != null)
                {
                    other.gameObject.GetComponent<CyberSolider>().HitByPlayer();
                }



                
            }
            else
            {
                Debug.Log("이미 충돌한 객체입니다.");
            }
        }



    }

    private void OnEnable()
    {
        collidedObject.Clear();
        StartCoroutine("DisableThis");
    }

    private void OnDisable()
    {
    }

    

    IEnumerator DisableThis()
    {
        for(int i=0;i<3;++i)
        {
            yield return new WaitForEndOfFrame();
        }
        gameObject.SetActive(false);
    }

    
}
