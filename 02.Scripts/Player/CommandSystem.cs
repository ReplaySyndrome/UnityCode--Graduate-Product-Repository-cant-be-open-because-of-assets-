using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Command
{
    public string command;
    public string parameterName;
}
[System.Serializable]
public struct CommandArray
{
    public List<Command> commands;
}


public class CommandSystem : MonoBehaviour
{
    private List<KeyValuePair<float, char>> keyboardAxisList = new List<KeyValuePair<float, char>>();
    [SerializeField]
    private float listKeepTime = 1.0f;
    [SerializeField]
    private uint listMaxSize = 10;
    private Dictionary<string, int> commandDict;
    private char[] axisArray;

    private string[] currStateName = { "GreatSwordIdle", "RapierIdle", "BrushIdle" };


    private Animator playerAnimator;
    public List<CommandArray> commandArray = new List<CommandArray>();



    private const int mouseLeftButtonCode = 0;
    private const int mouseMiddleButtonCode = 1;
    private const int mouseRightButtonCode = 2;

    public List<KeyValuePair<float, char>> GetInputAxisList
    {
        get
        {
            return keyboardAxisList;
        }
    }


    void Awake()
    {
        commandDict = new Dictionary<string, int>();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerAnimator = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
        
        axisArray = new char[4] { 'w', 'a', 's', 'd' };
    }

    // Update is called once per frame
    void Update()
    {
        AxisInputCheck();
    }

    void FixedUpdate()
    {
        ListCheck();
    }


    void AxisInputCheck() // 입력받는 코드 주제에 코드가 너무 길어요. 나중에 수정해야겠습니다.
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (keyboardAxisList.Count >= listMaxSize)
            {
                keyboardAxisList.RemoveAt(0);
            }
            keyboardAxisList.Add(new KeyValuePair<float, char>(Time.time, axisArray[0]));
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (keyboardAxisList.Count >= listMaxSize)
            {
                keyboardAxisList.RemoveAt(0);
            }
            keyboardAxisList.Add(new KeyValuePair<float, char>(Time.time, axisArray[1]));
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (keyboardAxisList.Count >= listMaxSize)
            {
                keyboardAxisList.RemoveAt(0);
            }
            keyboardAxisList.Add(new KeyValuePair<float, char>(Time.time, axisArray[2]));
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (keyboardAxisList.Count >= listMaxSize)
            {
                keyboardAxisList.RemoveAt(0);
            }
            keyboardAxisList.Add(new KeyValuePair<float, char>(Time.time, axisArray[3]));
        }
    }

    void ListCheck() //시간이 지나거나 리스트가 꽉차면 앞에서부터 제거합니다.
    {
        for (int i = 0; i < keyboardAxisList.Count; ++i) //foreach로 해도 되긴 하지만 유니티 콘솔창에서 오류를 출력하네요.
        {
            if (Time.time - keyboardAxisList[i].Key > listKeepTime)
            {
                keyboardAxisList.Remove(keyboardAxisList[i]);
            }
        }
    }

    public void PrintDict()
    {
        foreach (var i in commandDict)
        {
            Debug.Log(i.Key + " " + i.Value.ToString());
        }
    }

    public void SetCommand(ref string[] skillNameArray, ref string[] commandArray)
    // 반드시고쳐야한다 inspector에 반드시 pair로 노출시킨다.
    {
        //Debug.Log(skillNameArray[0] + "," + commandArray[0]);
        for (int i = 0; i < skillNameArray.Length; ++i) // 둘이 길이가 같은지 계산하고 넘어오니깐 상관없다
        {
            commandDict.Add(commandArray[i], Animator.StringToHash(skillNameArray[i]));
        }
    }

    public void FindCommand(AnimatorStateInfo currstate, int mouseButton)
    {
        
        string keylist = "";
        foreach (var c in keyboardAxisList)
        {
            keylist += c.Value;
        }

        string tofindcommandstring = "";
        string invokeParameter = "";

        tofindcommandstring += mouseButton.ToString();

        for (int i = keylist.Length; i >= 0; --i) // 누른 키에 값을 하나하나 찾는다.
        {
            if (i != keylist.Length) //아무것도 누르지 않은 커맨드를 찾기 위해 조건을 걸어준다.
            {
                tofindcommandstring = keylist[i] + tofindcommandstring;
            }            
            for (int j = 0; j < currStateName.Length; ++j) // 현재 상태를 찾는다.
            {               
                if (currstate.IsName(currStateName[j]))
                {
                    
                    for (int k = 0; k < commandArray[j].commands.Count; ++k) // 현재 상태에 맞는 가장 적합한 커맨드를 찾는다.
                    {
                        
                        if (commandArray[j].commands[k].command == tofindcommandstring)
                        {
                            //Debug.Log(tofindcommandstring);
                            invokeParameter = commandArray[j].commands[k].parameterName;
                        }
                    }
                }
            }
        }

        if (invokeParameter != "")
        {
            playerAnimator.SetTrigger(invokeParameter);
        }        
        keyboardAxisList.Clear();
    }

    public bool ReSetCommand(string[] skillNamearr, ref UnityEngine.UI.InputField[] inputFields)
    // 첫번째 변수는 프로퍼티로 가져오는데 ref 한정자로 가져올수가 없네요 어쩔 수 없이 복사복을 생성합니다. 
    // 이제생각해보니 복사본이 더 안전한 것 같기도하네요.
    {
        HashSet<string> chechSet = new HashSet<string>();


        foreach (var i in inputFields)
        {
            bool isoverlap = !chechSet.Add(i.text);
            if (isoverlap)
            {
                Debug.Log("커맨드 설정 실패!\n동일한 커맨드가 존재합니다.");
                return false;
            }
        }

        commandDict.Clear();
        for (int i = 0; i < skillNamearr.Length; ++i) // 둘이 길이가 같은지 계산하고 넘어오니깐 상관없다
        {
            commandDict.Add(inputFields[i].text, Animator.StringToHash(skillNamearr[i]));
        }


        return true;
    }




}
