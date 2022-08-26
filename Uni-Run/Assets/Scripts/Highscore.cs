using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;


public class Highscore : MonoBehaviour
{
    public static Highscore instance;
    
    public GameObject scoreBoard;
    //private static GameObject scoreBoardInstance;
    private int finalScore = 0;
    public List<Text> nameTexts = new List<Text>();
    public List<Text> scoreTexts = new List<Text>();

    private string currentUserID;

    public static Highscore Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType(typeof(Highscore)) as Highscore;

                if (instance == null)
                    Debug.Log("no singleton obj");
            }
            return instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentUserID = GameManager.Instance.getCurrentUser;
        Debug.Log(currentUserID.ToString());

        scoreBoard.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 캐릭터 사망 시 호출
    public void ShowLeaderboard()
    {
        scoreBoard.SetActive(true);

        UpdateHighscoreTBL();
        UpdateLeaderboard();
    }

    // 캐릭터 사망 시 함께 호출
        // 현재 유저ID와 최종 스코어를 HighscoreTBL에 추가
    void UpdateHighscoreTBL()
    {
        finalScore = Score.score;

        string ipAddress = "localhost";
        string db_id = "root";
        string db_pw = "unitytest1234";
        string db_name = "gamedb";

        string strConn = string.Format("server={0};uid={1};pwd={2};database={3};charset=utf8 ;",
            ipAddress, db_id, db_pw, db_name);

        MySqlConnection conn = new MySqlConnection(strConn);
        conn.Open();
        MySqlCommand command = new MySqlCommand("insert into highscoretbl values (null, @memberID, @score);", conn);
        command.Parameters.AddWithValue("@memberID", currentUserID);
        command.Parameters.AddWithValue("@score", finalScore);
        MySqlDataReader rdr = command.ExecuteReader();
    }

    // 캐릭터 사망 시 함께 호출
        // 내림차순으로 정렬된 HighscoreTBL의 내용을 가져와서
        // 리더보드 UI에 적용
    void UpdateLeaderboard()
    {
        string ipAddress = "localhost";
        string db_id = "root";
        string db_pw = "unitytest1234";
        string db_name = "gamedb";

        string strConn = string.Format("server={0};uid={1};pwd={2};database={3};charset=utf8 ;",
            ipAddress, db_id, db_pw, db_name);

        MySqlConnection conn = new MySqlConnection(strConn);
        conn.Open();
        MySqlCommand command = new MySqlCommand("select * from highscoretbl order by score desc;", conn);
        MySqlDataReader rdr = command.ExecuteReader();

        int i = 0;
        while (rdr.Read())
        {
            //Debug.Log(rdr["memberID"].ToString());
            nameTexts[i].text = rdr["memberID"].ToString();
            scoreTexts[i].text = rdr["score"].ToString();
            i++;
        }
        conn.Close();
    }
}