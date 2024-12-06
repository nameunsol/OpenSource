using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class FoodTeaching : MonoBehaviour
{
    public Button HomeBtn; //홈 버튼
    public TextMeshProUGUI TeachingText;
    //public GameObject TeachingPrefab;

    // Start is called before the first frame update
    void Start()
    {
        HomeBtn.onClick.AddListener(LoadingScene); //시작 창으로 돌아감
        //TeachingPrefab.active = true;
        //필요할 때만 학습 창 나오게 할 것임
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
