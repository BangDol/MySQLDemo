using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;

public class SpriteControl : MonoBehaviour
{
    // 스프라이트 리스트
    public List<Sprite> gunSprite;

    // Start is called before the first frame update
    void Start()
    {        
        // 선택한 캐릭터가 스프라이트 리스트 내 하나와 일치하면 해당 스프라이트로 변경
        for (int i = 0; i < gunSprite.Count; i++)
        {
            if (GameManager.Instance.SelectedGun + " (UnityEngine.Sprite)" == gunSprite[i].ToString())
            {
                GetComponent<SpriteRenderer>().sprite = gunSprite[i];
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

 
}
