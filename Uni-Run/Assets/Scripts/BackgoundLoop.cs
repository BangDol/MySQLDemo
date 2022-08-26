using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgoundLoop : MonoBehaviour
{
    private float width; // 배경의 가로 길이
    

    void Awake()
    {
        // 박스콜라이다 컴포넌트의 사이즈 필드의 x 값을 가로 길이로 사용
        BoxCollider2D backgroundCollider = GetComponent<BoxCollider2D>();
        width = backgroundCollider.size.x;
    }


    // Update is called once per frame
    void Update()
    {
        if(transform.position.x <= -width)
        {
            Reposition();
        }
    }

    void Reposition()
    {
        Vector2 offset = new Vector2(width * 2f, 0);
        transform.position = (Vector2)transform.position + offset;
    }
}
