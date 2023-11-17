using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2_Move : MonoBehaviour
{

    Rigidbody2D rigid;
    CapsuleCollider2D capsuleCollider;
    Animator animator;
    SpriteRenderer spriteRenderer; //�÷��̾� ���� ��ȯ

    public GameManager gameManager;
    public float jumpPower;
    public float maxSpeed;
    public float maxPosition; //���ϵ����� �ִ� ��ġ
    public bool isclear; //�������� �̵� ���� Ȯ��

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }
    void Update()
    {

        //���� ���� �� �������� ����, Player 2�� ����Ű�� �̵�
        if (Input.GetKeyDown(KeyCode.UpArrow) && !animator.GetBool("P2_isJumping"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            animator.SetBool("P2_isJumping", true);
        }

        //���ϵ����� ����
        if (!animator.GetBool("P2_isJumping")) //���� ���� ��
        {
            if (maxPosition - transform.position.y > 10)
            {
                OnDamaged(rigid.transform.position);
                maxPosition = 0;
            }
        }
        else
        {
            if (rigid.velocity.y < 0 && maxPosition < transform.position.y)
            {
                maxPosition = transform.position.y;
            }
        }

        //�̲����� ����
        if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.7f, rigid.velocity.y);

        //���� ��ȯ
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            spriteRenderer.flipX = true;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            spriteRenderer.flipX = false;
        }

        //�ȴ� �ִϸ��̼� �۵�
        if (Mathf.Abs(rigid.velocity.x) < 0.3)
        {
            animator.SetBool("P2_isWalking", false);
        }
        else
        {
            animator.SetBool("P2_isWalking", true);
        }
    }

    void FixedUpdate() 
    {
        //�¿� �̵� ����, Player 2�� ����Ű�� �̵�
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rigid.AddForce(Vector2.left, ForceMode2D.Impulse);

            if (rigid.velocity.x < maxSpeed * (1))
                SetMaxSpeed(KeyCode.LeftArrow);
        }

        else if (Input.GetKey(KeyCode.RightArrow))
        {
            rigid.AddForce(Vector2.right, ForceMode2D.Impulse);

            if (rigid.velocity.x > maxSpeed)
                SetMaxSpeed(KeyCode.RightArrow);
        }

        //���� ���� Ȯ�ο� ����ĳ��Ʈ ����
        if (rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayhit = Physics2D.Raycast(rigid.position, Vector3.down, 3, LayerMask.GetMask("Platform"));
            if (rayhit.collider != null)
            {
                if (rayhit.distance < 0.5f)
                    animator.SetBool("P2_isJumping", false);
            }
        }

        //������ ������� ���� Ȯ�ο� ����ĳ��Ʈ ����

        if (isclear)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 0, 1));
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Finish"));
            if (rayHit.collider == null) //Finish ������ �����
            {
                isclear = false;
                Debug.Log("P2 ������ ��Ż");
            }
        }


    }

    void SetMaxSpeed(KeyCode key) //�̵����⿡ ���� �ִ�ӵ� ����
    {
        if (key == KeyCode.LeftArrow)
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);
        else if (key == KeyCode.RightArrow)
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            //Attack
            if ((collision.transform.position.y < transform.position.y) && rigid.velocity.y < 0)
            {
                //Enemy Die
                OnAttack(collision.transform);
            }
            else
            {
                //������ �Ӹ��� ���� �� �ƴϸ� ������ ����
                OnDamaged(collision.transform.position);
            }
        }

        if (collision.gameObject.tag == "Obstacle" || collision.gameObject.tag == "P2_obstacle")
        {
            //��ֹ�, P1_��ֹ��� �±׵��� �� ������ ����
            OnDamaged(collision.transform.position);
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "P2_Finish") //P1 Finish�� ����� �� 
        {
            Debug.Log("P2 ����");
            isclear = true;
        }
    }

    void OnDamaged(Vector2 targetPos)
    {
        //ü�� ����
        gameManager.P2_HealthDown();

        //�������� ���̾�� ����
        gameObject.layer = 12;

        //�������� ��ȯ�� ���� ����
        spriteRenderer.color = new Color(1,1,0.7f,0.4f);

        //�ðܳ����� ���� ����
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1; 
        rigid.AddForce(new Vector2(dirc, 1)*7, ForceMode2D.Impulse);

        // �ǰ� �ִϸ��̼�
        animator.SetTrigger("DoDamaged");

        //�������´� 3�ʸ� ����
        Invoke("OffDamaged", 3);
    }

    void OffDamaged() //�������� ����
    {
        gameObject.layer = 7;
        spriteRenderer.color = new Color(1, 1, 0.7f, 1);
    }

    void OnAttack(Transform enemy)
    {
        {
            //EnemyAttack
            EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
            enemyMove.OnDamaged();

            //���� ��� �ݹ߷� �߻�
            rigid.AddForce(Vector2.up * 3, ForceMode2D.Impulse);

            //���� óġ ����
            gameManager.stagePoint += 100;
        }
    }
    public void OnDie()
    {
        //Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        //flipY
        spriteRenderer.flipY = true;
        //Color Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.5f);
        //capsuleCollider Enabled
        capsuleCollider.enabled = false;
    }
    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }
}