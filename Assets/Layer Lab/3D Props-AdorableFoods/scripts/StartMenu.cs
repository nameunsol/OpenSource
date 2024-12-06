using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class StartMenu : MonoBehaviour
{
    public Button StartBtn; //�Ļ��ϱ� ��ư
    public Button TeuniBtn; //Ʈ�� Ű��� â �̵� ��ư
    //private string EatingScene = "EatingScene"; //�Ļ� ar ���
    public Slider HPbar; //Ʈ�� HP

    // Start is called before the first frame update
    void Start()
    {
        StartBtn.onClick.AddListener(() => ChangeScene("EatingScene")); // �Ļ�ar��
        TeuniBtn.onClick.AddListener(() => ChangeScene("GrowingScene")); //Ʈ��Ű����
        HPbar.onValueChanged.AddListener(OnHPValueChanged);
        //Ʈ�� HP ���� �� UI �ݿ�
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeScene(string SceneName)
    {
        if (!string.IsNullOrEmpty(SceneName))
        {
            SceneManager.LoadScene(SceneName);
        }
        else
        {
            Debug.LogError("Target scene name is not set!");
        }
    }

    void OnHPValueChanged(float value)
    {
        Debug.Log("ScrollBar Value: " + value);
    }
}
