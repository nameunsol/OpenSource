using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class FoodTeaching : MonoBehaviour
{
    public TeuniInven TeuniInven;

    public Button HomeBtn; //Ȩ ��ư
    public TextMeshProUGUI TeachingText;
    public Button NextBtn;
    public Button ExitBtn;

    public GameObject TeachingPrefab;
    private string[] TeachTextArray = { "�׽�Ʈ �ؽ�Ʈ", "��� ������ ������ ��� ������ ���� �� �ֽ��ϴ�",
        "����� ������ ������ ����� ������ ���� �� �ֽ��ϴ�", "������ ������ ������ ������ ������ ���� �� �ֽ��ϴ�" };
    private int TNum = 0;

    // Start is called before the first frame update
    void Start()
    {
        HomeBtn.onClick.AddListener(LoadingScene); //���� â���� ���ư�

        // Next ��ư �̺�Ʈ ����
        NextBtn.onClick.AddListener(OnNextButtonClick);
        // Exit ��ư �̺�Ʈ ����
        ExitBtn.onClick.AddListener(OnExitButtonClick);

        // �ʱ� ����
        TeachingPrefab.SetActive(true);
        TeachingText.text = TeachTextArray[TNum];
    }

    private void OnNextButtonClick()
    {
        TNum++;

        if (TNum >= TeachTextArray.Length) // �ؽ�Ʈ �迭�� ��� �����ָ� ������ ��Ȱ��ȭ
        {
            TeachingPrefab.SetActive(false);
            TNum = 0; // �ٽ� ������ ��츦 ����� �ʱ�ȭ
        }
        else
        {
            TeachingText.text = TeachTextArray[TNum];
        }
    }

    private void OnExitButtonClick()
    {
        TeachingPrefab.SetActive(false);
    }

    public void LoadingScene()
    {
        SceneManager.LoadScene("StartScene");
    }
}
