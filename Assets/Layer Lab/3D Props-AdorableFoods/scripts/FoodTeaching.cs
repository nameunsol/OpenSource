using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class FoodTeaching : MonoBehaviour
{
    public Button HomeBtn; //Ȩ ��ư
    public TextMeshProUGUI TeachingText;
    //public GameObject TeachingPrefab;

    // Start is called before the first frame update
    void Start()
    {
        HomeBtn.onClick.AddListener(LoadingScene); //���� â���� ���ư�
        //TeachingPrefab.active = true;
        //�ʿ��� ���� �н� â ������ �� ����
    }

    public void LoadingScene()
    {
        SceneManager.LoadScene("SampleScene");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
