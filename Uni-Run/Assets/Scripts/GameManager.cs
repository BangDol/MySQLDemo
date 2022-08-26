using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool isGameover = false;

    // 로그인
    public bool isLoggIn = false;
    string memberID;
    public Text idText;
    public InputField passText;
    private static string selectedGun;

    // UI 참조
    public GameObject loginUI;
    public GameObject selectUI;
    private static GameObject selectUIInstance;

    // 뽑기
    public GameObject gachaUI;
    private bool isShowingGacha;

    // 회원가입
    public GameObject singupUI;
    public Text idTextForSignup;
    public Text emailTextForSignup;
    public InputField passTextForSignup;

    // 테이블에서 가져올 gunName을 저장할 리스트
    List<string> gunList = new List<string>();
    public List<GameObject> charSelect = new List<GameObject>();

    void Awake()
    {
        // 게임 매니저
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("씬에 두 개 이상의 게임 매니저가 존재합니다!");
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        LockAllCharacters();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static GameManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType(typeof(GameManager)) as GameManager;

                if (instance == null)
                    Debug.Log("no singleton obj");
            }
            return instance;
        }
    }

    // 기본값 : 캐릭터 모두 잠금처리
    void LockAllCharacters()
    {
        
        for (int i = 0; i < charSelect.Count; i++)
        {
            charSelect[i].SetActive(false);
        }
    }

    // 캐릭터 선택 창에서 선택한 캐릭터 정보 가져옴
    public string SelectedGun
    {
        get { return selectedGun; }
    }

    // 현재 로그인한 유저 ID 정보 가져오기
    public string getCurrentUser
    {
        get { return memberID; }
    }

    // 로그인 완료시 호출
    // 회원정보만 있는 memberTBL에 각 유저가 가진 캐릭터의 정보를 담는 CharPerUserTBL을 Inner Join하고
    // 유저가 보유한 캐릭터 이름을 선택
    public void SelectGuns()
    {
        // 뽑기창이 보여지고 있으면 보이지 않게 함
        if (isShowingGacha)
        {
            gachaUI.SetActive(false);
            isShowingGacha = false;
        }

        if (!isLoggIn)
            return;

        string id = idText.text;
        string pass = passText.text;
        string ipAddress = "localhost";
        string db_id = "root";
        string db_pw = "unitytest1234";
        string db_name = "gamedb";
        string strConn = string.Format("server={0};uid={1};pwd={2};database={3};charset=utf8 ;",
                                        ipAddress, db_id, db_pw, db_name);
        MySqlConnection conn = new MySqlConnection(strConn);
        conn.Open();

        
        MySqlCommand command = new MySqlCommand("select gunName from membertbl inner join charperusertbl on membertbl.memberid = charperusertbl.memberid where membertbl.memberID = @memberid;", conn);
        command.Parameters.AddWithValue("@memberid", id);
        MySqlDataReader rdr = command.ExecuteReader();

        // 캐릭터 이름 정보 가져와서 gunList에 추가
        while (rdr.Read())
        {
            for(int i = 0; i < rdr.FieldCount; i++)
            {
                gunList.Add(rdr[i].ToString());
            }
        }

        // gunList와 charSelect 탐색
        for(int j = 0; j < gunList.Count; j++)
        {
            for(int i = 0; i < charSelect.Count; i++)
            {
                // charSelect내 이름과 gunList가 일치하는 경우
                if (charSelect[i].name == gunList[j])
                {
                    // charSelect에 해당하는 게임 오브젝트 활성화
                    // 즉, 선택 화면에서 현재 로그인한 유저가 보유한 캐릭터에 한정하여 활성화
                    charSelect[i].SetActive(true);
                }
            }
        }
        
        conn.Close();
    }

    // 로그인
    public void Login()
    {
        string id = idText.text;
        string pass = passText.text;
        Debug.Log("pass" + pass);

        string ipAddress = "localhost";
        string db_id = "root";
        string db_pw = "unitytest1234";
        string db_name = "gamedb";

        string strConn = string.Format("server={0};uid={1};pwd={2};database={3};charset=utf8 ;",
            ipAddress, db_id, db_pw, db_name);

        MySqlConnection conn = new MySqlConnection(strConn);
        conn.Open();

        MySqlCommand command = new MySqlCommand("select * from membertbl where memberID=@memberID and memberName=@memberName;", conn);

        command.Parameters.AddWithValue("@memberID", id);
        command.Parameters.AddWithValue("@memberName", pass);

        MySqlDataReader rdr = command.ExecuteReader();
        string temp = string.Empty;

        // DataReader가 null인 경우
            // 로그인 실패
        if (rdr == null)
        {
            temp = "No return";
            Debug.Log("로그인이 실패 했습니다!");
        }
        else
        {
            int idCounter = 0;

            // 입력한 데이터가 memberTBL에 없는 경우
                // 로그인 실패
            while (rdr.Read())
            {
                idCounter++;
            }
            if(idCounter <= 0)
            {
                Debug.Log("로그인이 실패 했습니다!");
                return;
            }

            // 로그인됐다는 정보는 참값
            isLoggIn = true;
            // 로그인한 ID를 memberID 대입
            memberID = id;
            SelectGuns();
            // 로그인 화면 비활성화
            // 캐릭터 선택 창 활성화
            loginUI.SetActive(false);
            selectUI.SetActive(true);
        }
        conn.Close();
    }

    // 캐릭터 선택 시 게임 시작
    public void StartGun(string id)
    {
        // 인스펙터창에서 입력한 내용을 selectedGun에 대입
        // SpriteControl에서 이용
        selectedGun = id;
        Debug.Log(selectedGun);
        SceneManager.LoadScene("SampleScene");
    }        

    // 캐릭터 죽으면 다시 시작
    public void Restart()
    {
        SceneManager.LoadScene("SampleScene");
    }

    // 회원가입
    // 로그인 창의 SIGN UP 버튼 누르면 호출
    // 3개의 텍스트에 입력한 내용을 memberTBL에 추가
    public void SignUp()
    {
        loginUI.SetActive(false);
        singupUI.SetActive(true);

        string id = idTextForSignup.text;
        string email = emailTextForSignup.text;
        string pass = passTextForSignup.text;

        string ipAddress = "localhost";
        string db_id = "root";
        string db_pw = "unitytest1234";
        string db_name = "gamedb";

        string strConn = string.Format("server={0};uid={1};pwd={2};database={3};charset=utf8 ;",
            ipAddress, db_id, db_pw, db_name);

        MySqlConnection conn = new MySqlConnection(strConn);
        conn.Open();

        MySqlCommand command = new MySqlCommand("INSERT INTO memberTbl VALUES(@memberID, @memberName, @emailAddress);", conn);

        command.Parameters.AddWithValue("@memberID", id);
        command.Parameters.AddWithValue("@emailAddress", email);
        command.Parameters.AddWithValue("@memberName", pass);

        MySqlDataReader rdr = command.ExecuteReader();

        // 회원가입 끝나면 다시 로그인 창으로
        singupUI.SetActive(false);
        loginUI.SetActive(true);
    }
    
    // 뽑기 UI 보여주기
    public void ShowRndGunsUI()
    {
        gachaUI.SetActive(true);
        isShowingGacha = true;
    }
}