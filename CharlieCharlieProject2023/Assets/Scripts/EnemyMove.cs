// EnemyMove.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public int nextMove;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    PolygonCollider2D polygonCollider;
    Animator animator;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        polygonCollider = GetComponent<PolygonCollider2D>();
        animator = GetComponent<Animator>();
        Invoke("Think", 3);
    }

    void FixedUpdate()
    {
        //이동
        rigid.velocity = new Vector2(nextMove * 2, rigid.velocity.y);

        //낭떠러지 감지
        Vector2 front = new Vector2(rigid.position.x + nextMove * 0.5f, rigid.position.y - 0.5f);
        Debug.DrawRay(front, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(front, Vector3.down, 0.2f, LayerMask.GetMask("Platform"));

        // 전방 장애물 유무 확인
        /*
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.2f, rigid.position.y);
        Debug.DrawRay(frontVec, new Vector2(nextMove, 0), new Color(1, 0, 0)); // 전방을 빨간색으로 그림
        RaycastHit2D frontRayHit = Physics2D.Raycast(frontVec, new Vector2(nextMove, 0), 0.2f, LayerMask.GetMask("Platform"));

        if(frontRayHit.collider != null)
        {

            Debug.Log("hit");
        }*/

        if (rayHit.collider == null)
        {
            if (spriteRenderer.flipY != true) //몬스터가 죽고난 다음에는 실행되지 않기 위한 코드
            {
                Turn();
            }
        }
    }


    public void Think()
    {
        nextMove = Random.Range(-1, 2); //이동방향 설정, 2는 포함되지 않음

        animator.SetInteger("WalkSpeed", nextMove); //걷는속도에 따라 걷기 애니메이션 전환 

        if (nextMove != 0) //바라보는 방향 전환
        {
            spriteRenderer.flipX = nextMove == 1;
        }

        Invoke("Think", 3);
    }


    void Turn()
    {
        nextMove *= -1;
        spriteRenderer.flipX = nextMove == 1;
        CancelInvoke();
        Invoke("Think", 2);
    }

    public void OnDamaged()
    {
        //Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        //flipY
        spriteRenderer.flipY = true;
        //Color Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.5f);
        //polygonCollider Enabled
        polygonCollider.enabled = false;
        //DeActive Delay
        Invoke("DeActive", 3);
    }

    void DeActive()
    {
        gameObject.SetActive(false);
    }

}