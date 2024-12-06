using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class StartMenu : MonoBehaviour
{
    public Button StartBtn; //식사하기 버튼
    public Button TeuniBtn; //트니 키우기 창 이동 버튼
    //private string EatingScene = "EatingScene"; //식사 ar 장면
    public Slider HPbar; //트니 HP

    // Start is called before the first frame update
    void Start()
    {
        StartBtn.onClick.AddListener(() => ChangeScene("EatingScene")); // 식사ar씬
        TeuniBtn.onClick.AddListener(() => ChangeScene("GrowingScene")); //트니키우기씬
        HPbar.onValueChanged.AddListener(OnHPValueChanged);
        //트니 HP 변경 시 UI 반영
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
