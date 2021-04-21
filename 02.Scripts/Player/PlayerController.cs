using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public struct SkillInformation
{
    public readonly string skillName;
    public readonly int skillNameHash;
    public readonly float attackPercentage;
    public float mpConsumption;
    public int comboGaugeConsumption;
    public int comboGaugeGain;
    public bool isAble;
    public GameObject hitBox;

    public SkillInformation(string skillname, int skillnamehash, float attackpercentage, float mpconsumption, int combogaugeconsumption, 
        int combogaugegain, bool isable, GameObject hitbox)
    {
        skillName = skillname;
        skillNameHash = skillnamehash;
        attackPercentage = attackpercentage;
        mpConsumption = mpconsumption;
        comboGaugeConsumption = combogaugeconsumption;
        comboGaugeGain = combogaugegain;
        isAble = isable;
        hitBox = hitbox;
    }

    public string SkillName
    {
        get
        {
            return skillName;
        }
    }

   

    public void activeHitbox()
    {
        hitBox.SetActive(true);
    }


}


[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    //script
    private static CharacterController characterController;
    private static Animator animator;
    private CommandSystem commandSystem;

    //animator state info
    private AnimatorStateInfo currStateInfo;
    private AnimatorStateInfo lastStateInfo;
    private AnimatorStateInfo nextStateInfo;


    //almost character center
    [Header("Almost Character Center")]
    public Transform characterCenter;

    public GameObject greatSword;
    public GameObject rapier;
    public GameObject greatSwordNormalSizeHitbox;
    public GameObject greatSwordBigSizeHitbox;
    public GameObject rapierHitbox;





    //character variation
    private float speed = 5f;
    private float gravity = 9.8f;
    private float jumpspeed = 10f;
    private float rotSpeed = 5f;
    private Vector3 dir;
    private bool canAirCombo = true;
    private float attackDamage = 100;

    private float currHp = 100f;
    private float maxHp = 1000f;
    private float currMp = 100f;
    private float maxMp = 1000f;
    private float currComboGauge = 0;
    private float maxComboGauge = 1000f;

    private List<SkillInformation> skillInfoList;
    private SkillInformation currStateSkillInfo;

    
    //property
    public float LastAttackTime { get; set; } = 0f;
    public SkillInformation CurrSkillInformation
    {
        get
        {
            return currStateSkillInfo;
        }
    }
    public float CurrHP
    {
        get
        {
            return currHp;
        }
        set
        {
            currHp = value;
        }
    }
    public float CurrMP
    {
        get
        {
            return currMp;
        }
        set
        {
            currMp = value;
        }
    }

    public float CurrComboGauge
    {
        get
        {
            return currComboGauge;
        }
        set
        {
            currComboGauge = value;
        }
    }



    //item information 나중에 고쳐야 한다
    private int whiteAetherAmount = 0;
    private int maxWhiteAetherAmount = 10000000;


    //mouse button info
    private const int mouseLeftButtonCode = 0;
    private const int mouseMiddleButtonCode = 1;
    private const int mouseRightButtonCode = 2;

    //animator parameter Name
    private string equipRapier = "EquipRapier";
    private string equipGreatSword = "EquipGreatSword";
    private string nextCombo1 = "NextCombo1";
    private string nextCombo2 = "NextCombo2";
    private string idleWalk = "IdleWalk";
    
    //Rapier Parameter
    private string rapierNormalAttack = "RapierNormalAttack";
    private string rapierNormalAttack1 = "RapierNormalAttack1";
    private string rapierNormalAttack2 = "RapierNormalAttack2";
    private string rapierNormalAttack3 = "RapierNormalAttack3";
    private string rapierNormalAttack4 = "RapierNormalAttack4";
    private string rapierRush = "RapierRush";
    private string elementalWave = "ElementalWave";
    private string elementalDance = "ElementalDance";
    private string elementalDance1 = "ElementalDance1";
    private string elementalDance2 = "ElementalDance2";
    private string elementalDance3 = "ElementalDance3";
    private string rapidSting = "RapidSting";
    private string rapidSting1 = "RapidSting1";
    private string elementalTanz = "ElementalTanz";

    //GreatSword Parameter
    private string greatSwordNormalAttackA = "GreatSwordNormalAttackA";
    private string greatSwordNormalAttackA1 = "GreatSwordNormalAttackA1";
    private string greatSwordNormalAttackA2 = "GreatSwordNormalAttackA2";
    private string greatSwordNormalAttackB = "GreatSwordNormalAttackB";
    private string greatSwordNormalAttackB1 = "GreatSwordNormalAttackB1";
    private string greatSwordNormalAttackB2 = "GreatSwordNormalAttackB2";
    private string greatSwordNormalAttackB3 = "GreatSwordNormalAttackB3";
    private string greatSwordRush = "GreatSwordRush";
    private string risingCut = "RisingCut";
    private string elementalSlash = "ElementalSlash";
    private string elementalSlash1 = "ElementalSlash1";
    private string elementalSlash2 = "ElementalSlash2";
    private string elementalSlash3 = "ElementalSlash3";
    private string elementalRush = "ElementalRush";
    private string elementalRush1 = "ElementalRush1";
    private string elementalRush2 = "ElementalRush2";
    private string elementalRush3 = "ElementalRush3";
    private string swordOfElemental = "SwordOfElemental";







    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        commandSystem = GetComponent<CommandSystem>();
    }

    // Start is called before the first frame update
    void Start()
    {
        animator.SetTrigger(equipRapier);
        SkillInfoAdd();
        greatSword.SetActive(false);
        rapier.SetActive(true);

    }


    // Update is called once per frame
    void Update()
    {
        GetCurAnimatorStateCache();
        CheckKeyboardInput();
        CheckMouseInput();
        PlayerMove();
        SetAnimatorParameter();
        OrganizeAnimatorCache();

        
    }

    void PlayerRotate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        transform.Rotate(Vector3.up * rotSpeed * mouseX);
    }

    void GetCurAnimatorStateCache()
    {
        currStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if(lastStateInfo.shortNameHash != currStateInfo.shortNameHash)
        {
            ResetNextComboTrigger();
        }

        if(currStateInfo.IsTag("IdleWalk"))
        {
            animator.applyRootMotion = false;
        }
    }

    void CheckKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && currStateInfo.IsTag(idleWalk))
        {
            animator.SetTrigger(equipGreatSword);
            greatSword.SetActive(true);
            rapier.SetActive(false);
        }

        else if(Input.GetKeyDown(KeyCode.Alpha2) && currStateInfo.IsTag(idleWalk))
        {
            animator.SetTrigger(equipRapier);
            greatSword.SetActive(false);
            rapier.SetActive(true);
        }
    }

    void CheckMouseInput()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        //키가 여러가지가 있죠.
        //여기서는 왼쪽키만 사용했지만 마우스 중간버튼, 오른쪽버튼도 하려면 배열로 처리하면 될 것 같습니다.
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (currStateInfo.IsTag("IdleWalk")) //must fix
                {
                    commandSystem.FindCommand(currStateInfo, mouseLeftButtonCode);
                    
                }

                if (currStateInfo.IsTag("InCombo"))
                {
                    animator.SetTrigger(nextCombo1);
                }

            }
            if (Input.GetMouseButtonDown(1))
            {
                
                if (currStateInfo.IsTag("IdleWalk")) //must fix
                {

                    commandSystem.FindCommand(currStateInfo, mouseMiddleButtonCode);
                }

                if (currStateInfo.IsTag("InCombo"))
                {
                    animator.SetTrigger(nextCombo2);
                }
            }
            if (Input.GetMouseButtonDown(2))
            {
                
                if (currStateInfo.IsTag("IdleWalk")) //must fix
                {

                    commandSystem.FindCommand(currStateInfo, mouseRightButtonCode);
                }
            }
            if (currStateInfo.IsName("Jump") || currStateInfo.IsName("Fall"))
            {
                if (canAirCombo)
                {
                    canAirCombo = false;
                    animator.SetTrigger("AirCombo1");
                }
            }  
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void PlayerMove()
    {
        if (characterController.isGrounded && currStateInfo.IsTag(idleWalk))
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            dir = new Vector3(x, 0, z);

            dir = transform.TransformDirection(dir);
            PlayerRotate();
            dir = dir * speed;
            canAirCombo = true;
            if (Input.GetButton("Jump"))
            {
                dir.y = jumpspeed;
                animator.SetTrigger("Jump");
            }

            ResetNextComboTrigger();
        }

        if (characterController.isGrounded && !currStateInfo.IsTag(idleWalk))
        {
            canAirCombo = true;
            dir = new Vector3(0, 0, 0);
        }

        if (currStateInfo.IsTag("InAir"))
        {
            dir = new Vector3(0, 0, 0);
        }

        dir.y -= gravity * Time.deltaTime;
        characterController.Move(dir * Time.deltaTime);
    }

    void SetAnimatorParameter()
    {
        animator.SetFloat("MoveX", Input.GetAxis("Horizontal"));
        animator.SetFloat("MoveZ", Input.GetAxis("Vertical"));
        animator.SetFloat("SpeedY", dir.y);
        animator.SetBool("IsGround", characterController.isGrounded);
    }

    void OrganizeAnimatorCache()
    {
        lastStateInfo = currStateInfo;
    }

    public void AddWhiteAether(int n)
    {
        whiteAetherAmount += n;
        whiteAetherAmount = Mathf.Clamp(whiteAetherAmount,0,maxWhiteAetherAmount); 
    }

    public void RecoverHp(float hp)
    {
        currHp += hp;
        currHp = Mathf.Clamp(currHp, 0, maxHp);
    }

    public void RecoverMp(float mp)
    {
        currMp += mp;
        currMp = Mathf.Clamp(currMp, 0, maxMp);
    }

    public void RecoverComboGauge(float comboGauge)
    {
        currComboGauge += comboGauge;
        currComboGauge = Mathf.Clamp(currComboGauge, 0, maxComboGauge);
    }
    
    public void showCurrInterface()
    {
        Debug.Log("현재 체력:" + currHp + " 현재 마나:" + currMp + " 현재 콤보게이지:" + currComboGauge + " 현재 화이트보유량:" + whiteAetherAmount);
    }

    void SkillInfoAdd()
    {
        skillInfoList = new List<SkillInformation>();


        //GreatSword SkillInformantion real mannae
        skillInfoList.Add(new SkillInformation(greatSwordNormalAttackA,Animator.StringToHash(greatSwordNormalAttackA), 1f, 0f, 0, 5, true,greatSwordNormalSizeHitbox));
        skillInfoList.Add(new SkillInformation(greatSwordNormalAttackA1, Animator.StringToHash(greatSwordNormalAttackA1), 1f, 0f, 0, 5, true, greatSwordNormalSizeHitbox));
        skillInfoList.Add(new SkillInformation(greatSwordNormalAttackA2, Animator.StringToHash(greatSwordNormalAttackA2), 1f, 0f, 0, 5, true, greatSwordNormalSizeHitbox));
        skillInfoList.Add(new SkillInformation(greatSwordNormalAttackB, Animator.StringToHash(greatSwordNormalAttackB), 1.5f, 0, 0, 5, true, greatSwordNormalSizeHitbox));
        skillInfoList.Add(new SkillInformation(greatSwordNormalAttackB1, Animator.StringToHash(greatSwordNormalAttackB1), 1.5f, 0, 0, 5, true, greatSwordNormalSizeHitbox));
        skillInfoList.Add(new SkillInformation(greatSwordNormalAttackB2, Animator.StringToHash(greatSwordNormalAttackB2), 1.5f, 0, 0, 5, true, greatSwordNormalSizeHitbox));
        skillInfoList.Add(new SkillInformation(greatSwordNormalAttackB3, Animator.StringToHash(greatSwordNormalAttackB3), 1.5f, 0, 0, 5, true, greatSwordNormalSizeHitbox));
        skillInfoList.Add(new SkillInformation(greatSwordRush, Animator.StringToHash(greatSwordRush), 2f, 0, 0, 15, true, greatSwordNormalSizeHitbox));
        skillInfoList.Add(new SkillInformation(risingCut, Animator.StringToHash(risingCut), 1f, 0, 0, 15, true, greatSwordNormalSizeHitbox));
        skillInfoList.Add(new SkillInformation(elementalSlash, Animator.StringToHash(elementalSlash), 1.5f, 0, 10, 0, true, greatSwordNormalSizeHitbox));
        skillInfoList.Add(new SkillInformation(elementalSlash1, Animator.StringToHash(elementalSlash1), 1.5f, 0, 10, 0, true, greatSwordNormalSizeHitbox));
        skillInfoList.Add(new SkillInformation(elementalSlash2, Animator.StringToHash(elementalSlash2), 1.5f, 0, 10, 0, true, greatSwordNormalSizeHitbox));
        skillInfoList.Add(new SkillInformation(elementalSlash3, Animator.StringToHash(elementalSlash3), 1.5f, 0, 10, 0, true, greatSwordNormalSizeHitbox));
        skillInfoList.Add(new SkillInformation(elementalRush, Animator.StringToHash(elementalRush), 1.5f, 0, 10, 0, true, greatSwordNormalSizeHitbox));
        skillInfoList.Add(new SkillInformation(elementalRush1, Animator.StringToHash(elementalRush1), 1.5f, 0, 10, 0, true, greatSwordNormalSizeHitbox));
        skillInfoList.Add(new SkillInformation(elementalRush2, Animator.StringToHash(elementalRush2), 1.5f, 0, 10, 0, true, greatSwordNormalSizeHitbox));
        skillInfoList.Add(new SkillInformation(elementalRush3, Animator.StringToHash(elementalRush3), 1.5f, 0, 10, 0, true, greatSwordNormalSizeHitbox));
        skillInfoList.Add(new SkillInformation(swordOfElemental, Animator.StringToHash(swordOfElemental), 5f, 0, 0, 100, true, greatSwordNormalSizeHitbox));



        //rapier skillInformation igodo real mannae
        skillInfoList.Add(new SkillInformation(rapierNormalAttack, Animator.StringToHash(rapierNormalAttack), 5f, 0, 0, 100, true, rapierHitbox));
        skillInfoList.Add(new SkillInformation(rapierNormalAttack1, Animator.StringToHash(rapierNormalAttack1), 5f, 0, 0, 100, true, rapierHitbox));
        skillInfoList.Add(new SkillInformation(rapierNormalAttack2, Animator.StringToHash(rapierNormalAttack2), 5f, 0, 0, 100, true, rapierHitbox));
        skillInfoList.Add(new SkillInformation(rapierNormalAttack3, Animator.StringToHash(rapierNormalAttack3), 5f, 0, 0, 100, true, rapierHitbox));
        skillInfoList.Add(new SkillInformation(rapierNormalAttack4, Animator.StringToHash(rapierNormalAttack4), 5f, 0, 0, 100, true, rapierHitbox));
        skillInfoList.Add(new SkillInformation(rapierRush, Animator.StringToHash(rapierRush), 5f, 0, 0, 100, true, rapierHitbox));
        skillInfoList.Add(new SkillInformation(elementalWave, Animator.StringToHash(elementalWave), 5f, 0, 0, 100, true, rapierHitbox));
        skillInfoList.Add(new SkillInformation(elementalDance, Animator.StringToHash(elementalDance), 5f, 0, 0, 100, true, rapierHitbox));
        skillInfoList.Add(new SkillInformation(elementalDance1, Animator.StringToHash(elementalDance1), 5f, 0, 0, 100, true, rapierHitbox));
        skillInfoList.Add(new SkillInformation(elementalDance2, Animator.StringToHash(elementalDance2), 5f, 0, 0, 100, true, rapierHitbox));
        skillInfoList.Add(new SkillInformation(elementalDance3, Animator.StringToHash(elementalDance3), 5f, 0, 0, 100, true, rapierHitbox));
        skillInfoList.Add(new SkillInformation(rapidSting, Animator.StringToHash(rapidSting), 5f, 0, 0, 100, true, rapierHitbox));
        skillInfoList.Add(new SkillInformation(rapidSting1, Animator.StringToHash(rapidSting1), 5f, 0, 0, 100, true, rapierHitbox));






    }

    void ResetNextComboTrigger()
    {
        animator.ResetTrigger(nextCombo1);
        animator.ResetTrigger(nextCombo2);
        
    }

    void playerAttack()
    {

        currStateSkillInfo = skillInfoList.Find(e => (currStateInfo.IsName(e.SkillName)));
        print(currStateSkillInfo.SkillName);
        if(currStateSkillInfo.hitBox == null)
        {
            print("널너런러널널널너ㅓㄹㄴ러너러너러너러너러");
            return;
        }
        currStateSkillInfo.activeHitbox();
    }
}


