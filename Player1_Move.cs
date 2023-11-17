using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1_Move : MonoBehaviour
{
    Rigidbody2D rigid;
    CapsuleCollider2D capsuleCollider;
    Animator animator;
    SpriteRenderer spriteRenderer; //�÷��̾� ���� ��ȯ
    Player2_Move Player2;

    public GameManager gameManager;
    public float jumpPower;
    public float maxSpeed;
    public float maxPosition; //���ϵ����� �ִ� ��ġ
    public bool isclear;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }
    void Update()
    {
        //���� ���� �� �������� ����, Player 1�� wasd�� �̵�
        if (Input.GetKeyDown(KeyCode.W) && !animator.GetBool("P1_isJumping"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            animator.SetBool("P1_isJumping", true);
        }

        //���ϵ����� ����
        if (!animator.GetBool("P1_isJumping")) //���� ���� ��
        {
            if (maxPosition - transform.position.y > 10)
            {
                OnDamaged(rigid.transform.position);
                maxPosition = 0;
            }
        }
        else
        {
            if(rigid.velocity.y < 0 && maxPosition < transform.position.y){
                maxPosition = transform.position.y;
            }
        }

        //�̲����� ����
        if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A))
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.7f, rigid.velocity.y);

        //���� ��ȯ
        if (Input.GetKey(KeyCode.A))
        {
            spriteRenderer.flipX = true;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            spriteRenderer.flipX = false;
        }

        //�ȴ� �ִϸ��̼� �۵�
        if (Mathf.Abs(rigid.velocity.x) < 0.3)
        {
            animator.SetBool("P1_isWalking", false);
        }
        else
        {
            animator.SetBool("P1_isWalking", true);
        }
    }

    void FixedUpdate()
    {
        //�¿� �̵� ����, Player 1�� wasd�� �̵�
        if (Input.GetKey(KeyCode.A))
        {
            rigid.AddForce(Vector2.left, ForceMode2D.Impulse);

            if (rigid.velocity.x < maxSpeed * (1))
                SetMaxSpeed(KeyCode.A);
        }

        else if (Input.GetKey(KeyCode.D))
        {
            rigid.AddForce(Vector2.right, ForceMode2D.Impulse);

            if (rigid.velocity.x > maxSpeed)
                SetMaxSpeed(KeyCode.D);
        }

        //���� ���� Ȯ�ο� ����ĳ��Ʈ ����
        if (rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            if (rayHit.collider != null) //�÷����� ����ĳ��Ʈ�� ������ (���� �����ϸ�)
            {
                if (rayHit.distance < 0.5f)
                {
                    animator.SetBool("P1_isJumping", false);
                }
            }
        }

        //������ ������� ���� Ȯ�ο� ����ĳ��Ʈ ����
        if (isclear)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 0, 1));
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 0, LayerMask.GetMask("Finish"));
            if (rayHit.collider == null) //Finish ������ �����
            {
                isclear = false;
                Debug.Log("P1 ������ ��Ż");
            }
        }
    }

    void SetMaxSpeed(KeyCode key) //�̵����⿡ ���� �ִ�ӵ� ����
    {
        if (key == KeyCode.A)
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);
        else if (key == KeyCode.D)
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
            else {
                //������ �Ӹ��� ���� �� �ƴϸ� ������ ����
                OnDamaged(collision.transform.position);
            }

        }

        if (collision.gameObject.tag == "Obstacle" || collision.gameObject.tag == "P1_obstacle")
        {
            //��ֹ�, P1_��ֹ��� �±׵��� �� ������ ����
            OnDamaged(collision.transform.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "P1_Finish") //P1 Finish�� ����� �� 
        {
            Debug.Log("P1 ����");
            isclear = true;
        }
    }


    void OnDamaged(Vector2 targetPos)
    {
        //ü�� ����
        gameManager.P1_HealthDown();

        //�������� ���̾�� ����
        gameObject.layer = 12;

        //�������� ��ȯ�� ���� ����
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

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
        gameObject.layer = 6;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    void OnAttack(Transform enemy)
    {
        //EnemyAttack
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();

        //���� ��� �ݹ߷� �߻�
        rigid.AddForce(Vector2.up * 3, ForceMode2D.Impulse);

        //���� óġ ����
        gameManager.stagePoint += 100;
    }
    public void OnDie()
    {
        //Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        //flipY
        spriteRenderer.flipY = true;
        //Color Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.5f);
        //BoxCollider Enabled
        capsuleCollider.enabled = false;
    }

    public void VelocityZero()
    {
        rigid.velocity=Vector2.zero;
    }

}