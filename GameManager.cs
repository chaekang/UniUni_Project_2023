using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int P1_HP;
    public int P2_HP;
    public Player1_Move Player1;
    public Player2_Move Player2;
    public GameObject[] Stages;

    //UI ���� ���� ����
    public Image[] UIP1_HP;
    public Image[] UIP2_HP;
    public TextMeshProUGUI UIStage;
    public GameObject UIRestartBtn;

    private void Update()
    {
        if (Player1.isclear && Player2.isclear)
        {
            Player1.isclear = false;
            Player2.isclear = false;
            NextStage();
        }
    }

    public void NextStage()
    {
        //���� �������� �̵�
        if(stageIndex < Stages.Length-1)
        {
            PlayerReposition();
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);

            UIStage.text = "STAGE " + (stageIndex+1);
        }
        else //���� Ŭ����
        {
            //�ð� ����
            Time.timeScale = 0;
            //��� ȭ�� UI
            Debug.Log("���� Ŭ����!");
            //����� ��ư UI
            TextMeshProUGUI btnText = UIRestartBtn.GetComponentInChildren<TextMeshProUGUI>();
            btnText.text = "Clear!";
            UIRestartBtn.SetActive(true);
        }


        //�������� ������ ��ü ���� ������ �߰� �� �ʱ�ȭ
        totalPoint += stagePoint;
        stagePoint = 0;
    }

    public void P1_HealthDown() //�÷��̾� 1 ü�� ��ġ �� UI ����, ������ dead �ִϸ��̼ǰ� restart ��ư ȣ��
    {
        if (P1_HP > 1)
        {
            P1_HP--;
            UIP1_HP[P1_HP].color = new Color(1,1,1,0.2f);
        }
        else
        {
            UIP1_HP[0].color = new Color(1, 1, 1, 0.2f);
            Dead();
        }
    }

    public void P2_HealthDown() //�÷��̾� 2 ü�� ��ġ �� UI ����, ������ dead �ִϸ��̼ǰ� restart ��ư ȣ��
    {
        if (P2_HP > 1)
        {
            P2_HP--;
            UIP2_HP[P2_HP].color = new Color(1, 1, 0.7f, 0.2f);
        }
        else
        {
            UIP2_HP[0].color = new Color(1, 1, 0.7f, 0.2f);
            Dead();
        }
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void Dead()
    {
        Player1.OnDie();
        Player2.OnDie();
        UIRestartBtn.SetActive(true);
    }

    void PlayerReposition()
    {
        Player1.transform.position = new Vector3(0,0,-1);
        Player1.VelocityZero();
        Player2.transform.position = new Vector3(0, 0, -1);
        Player2.VelocityZero();
    }
}
