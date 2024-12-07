using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class StartMenu : MonoBehaviour
{
    public TeuniInven TeuniInven;

    public Button StartBtn; //�Ļ��ϱ� ��ư
    public Button TeuniBtn; //Ʈ�� Ű��� â �̵� ��ư
    public Slider HPbar; //Ʈ�� HP

    // Start is called before the first frame update
    void Start()
    {
        StartBtn.onClick.AddListener(() => ChangeScene("EatingScene")); // �Ļ�ar��
        TeuniBtn.onClick.AddListener(() => ChangeScene("GrowingScene")); //Ʈ��Ű����
        HPbar.onValueChanged.AddListener(OnHPValueChanged);
        //Ʈ�� HP ���� �� UI �ݿ�

        // �����̴� �ʱ�ȭ
        //HPbar.maxValue = TeuniInven.MaxHp; // �ִ밪 ����
        TeuniInven.ResetData();
        HPbar.value = TeuniInven.hp / TeuniInven.MaxHp;       // ���簪 ����
        Debug.Log(TeuniInven.hp);
        Debug.Log(TeuniInven.MaxHp);
        Debug.Log(HPbar.value);

        // HP ���� �� UI �ڵ� ������Ʈ
        TeuniInven.HPChanged += UpdateHPBar;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateHPBar(int currentHP)
    {
        HPbar.value = currentHP; // HP ���� �����̴��� �ݿ�
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

    private void OnEnable()
    {

        if (TeuniInven != null)
        {
            // HP ���� �̺�Ʈ ����
            TeuniInven.HPChanged += UpdateSlider;
        }

    }

    private void UpdateSlider(int currentHP)
    {
        if (HPbar != null)
        {
            HPbar.value = currentHP / 100f;
        }
    }
}
