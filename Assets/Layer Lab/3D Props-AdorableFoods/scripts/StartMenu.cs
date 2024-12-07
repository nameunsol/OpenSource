using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using EasyUI.Popup;
using EasyUI.Helpers;

public class StartMenu : MonoBehaviour
{
    public TeuniInven TeuniInven;

    public Button StartBtn; //�Ļ��ϱ� ��ư
    public Button TeuniBtn; //Ʈ�� Ű��� â �̵� ��ư
    public Slider HPbar; //Ʈ�� HP Slider

    private string[] StartSceneTutorialText = { "�ʷϻ� �ٴ� Ʈ���� HP�Դϴ�. ���� ���·� ���� ���� Ʈ�ϸ� ���� ���� ������!", "Ʈ�ϸ� ���� �ϸ� Ʈ�ϸ� ���� �� �ֽ��ϴ�!", "Start ��ư�� ������ �Ļ縦 �����ؿ�." };
    // Start is called before the first frame update
    void Start()
    {
        StartBtn.onClick.AddListener(() => ChangeScene("EatingScene")); // �Ļ�ar��
        TeuniBtn.onClick.AddListener(() => ChangeScene("GrowingScene")); //Ʈ��Ű����
        HPbar.onValueChanged.AddListener(OnHPValueChanged);
        //Ʈ�� HP ���� �� UI �ݿ�

        // �����̴� �ʱ�ȭ
        //HPbar.maxValue = TeuniInven.MaxHp; // �ִ밪 ����
        //TeuniInven.ResetData();

        //HPbar.value = TeuniInven.hp / TeuniInven.MaxHp;       // ���簪 ����
        HPbar.value = TeuniManager.Instance.Hp / TeuniManager.Instance.MaxHp;
        Debug.Log(TeuniManager.Instance.Hp);
        Debug.Log(TeuniManager.Instance.MaxHp);
        Debug.Log(HPbar.value);


        // HP ���� �� UI �ڵ� ������Ʈ
        //TeuniInven.HPChanged += UpdateHPBar;
        TeuniManager.Instance.HPChanged += UpdateSlider;

        if (!TeuniManager.StartSceneTutorial)
        {
            Popup.Show("���� ���� ȭ��", "[ Ʈ�� ��ư ]\nƮ�ϸ� ���� �� �־��\r\n\n[ Start ��ư ]\n�Ļ縦 �����ؿ�");
            TeuniManager.StartSceneTutorial = true;
        }
    }

    void StartTutorial2()
    {
        Popup.Show("���� ���� ȭ��", "Start ��ư�� ������ �Ļ縦 �����ؿ�!");
    }

    void TurnTrue()
    {
        TeuniManager.StartSceneTutorial = true;
        //��¥ ����ȿ�����ε� �� ���� �ǰ��� ���ؼ�
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
        /*        if (TeuniInven != null)
                {
                    // HP ���� �̺�Ʈ ����
                    TeuniInven.HPChanged += UpdateSlider;
                }*/

        UpdateSlider((int)TeuniManager.Instance.Hp);

    }

    private void UpdateSlider(int currentHP)
    {
        if (HPbar != null)
        {
            HPbar.value = currentHP / 100f;
        }
    }

    private void OnDestroy()
    {
        TeuniManager.Instance.HPChanged -= UpdateSlider;
    }
}
