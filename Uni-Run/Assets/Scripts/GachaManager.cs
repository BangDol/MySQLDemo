using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;

public class GachaManager : MonoBehaviour
{
    // 변수 선언
    public static GachaManager instance;

    public GameObject selectUI;
    public GameObject gachaUI;

    public Image gunImage;
    public List<Sprite> rndGuns = new List<Sprite>();

    private string currentMemberID;

    // GachaManager 인스턴스
    public static GachaManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType(typeof(GachaManager)) as GachaManager;

                if (instance == null)
                    Debug.Log("no singleton obj");
            }
            return instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        selectUI = GameManager.Instance.selectUI;
}

    // Update is called once per frame
    void Update()
    {
        
    }

    // 버튼 클릭 시 호출
        // 유니티 외부에서 작성된 MySQL의 쿼리문
        // 확률 랜덤 뽑기 로직의 프로시져 호출
        // 테이블 내 정해진 확률(필드)로 무작위 이름을 가져옴
    public void GetRandomGuns()
    {
        string ipAddress = "localhost";
        string db_id = "root";
        string db_pw = "unitytest1234";
        string db_name = "gamedb";

        string strConn = string.Format("server={0};uid={1};pwd={2};database={3};charset=utf8 ;",
                                        ipAddress, db_id, db_pw, db_name);

        MySqlConnection conn = new MySqlConnection(strConn);
        conn.Open();

        MySqlCommand cmd = new MySqlCommand("call testPro();", conn);

        MySqlDataReader rdr = cmd.ExecuteReader();

        while (rdr.Read())
        {
            
            // 무작위로 가져온 캐릭터 이름이
            // rndGun 리스트 내 하나와 일치하면
            // 화면 중앙의 이미지 교체
            for (int i = 0; i < rndGuns.Count; i++)
            { 
                if(rndGuns[i].ToString() == rdr[0].ToString() + " (UnityEngine.Sprite)")
                {
                    gunImage.sprite = rndGuns[i];
                    gunImage.color = Color.white;

                    // 해당 이름을 다른 테이블에 저장 
                    AddGunToUser(rdr);
                }
            }
        }

        conn.Close();
    }

    // 무작위 뽑기 후 호출
    // 현재 로그인한 유저의 ID와 뽑은 캐릭터의 이름을 CharPerUserTBL에 추가
    public void AddGunToUser(MySqlDataReader gun)
    {
        currentMemberID = GameManager.Instance.getCurrentUser;

        string ipAddress = "localhost";
        string db_id = "root";
        string db_pw = "unitytest1234";
        string db_name = "gamedb";

        string strConn = string.Format("server={0};uid={1};pwd={2};database={3};charset=utf8 ;",
            ipAddress, db_id, db_pw, db_name);

        MySqlConnection conn = new MySqlConnection(strConn);
        conn.Open();

        Debug.Log(currentMemberID + "은(는) ");
        Debug.Log(gun[0].ToString() + "을(를) 뽑았다!");

        MySqlCommand command = new MySqlCommand("insert into charperusertbl values (null, @memberID, @gunName);", conn);
        
        command.Parameters.AddWithValue("@memberID", currentMemberID);
        command.Parameters.AddWithValue("@gunName", gun[0].ToString());

        MySqlDataReader rdr = command.ExecuteReader();
    }
}