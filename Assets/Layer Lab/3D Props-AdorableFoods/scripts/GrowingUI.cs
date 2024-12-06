using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class GrowingUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public List<GameObject> uiElements; // 모든 UI를 관리할 리스트
                                        // 0. shop
                                        // 1. growing_Bath
                                        // 2. Eat
                                        // 3. Refri

    //코인은 테스트용으로 50 설정. 나중에 획득에 따라 수정되게 함

    // Shop Btn
    public Button redButton;
    public Button yellowButton;
    public Button greenButton;
    public Button whiteButton;

    // 텍스트 배열
    public TextMeshProUGUI[] coinTexts;  // 코인 관련 텍스트
    public TextMeshProUGUI[] foodTexts;  // 음식 관련 텍스트

    private int redCoin = 50;
    private int whiteCoin = 50;
    private int greenCoin = 50;
    private int yellowCoin = 50;

    private int redFood = 0;
    private int yellowFood = 0;
    private int greenFood = 0;
    private int whiteFood = 0;

    public Slider TeuniHP;

    // Refri_food 버튼 배열
    public Button[] refriFoodButtons;

    // Feed 버튼 (이미지를 변경할 버튼)
    public Button feedButton;

    // Feed 버튼의 Image 컴포넌트
    private Image feedButtonImage;

    // 이미지 배열 (Refri_food 버튼에 연결된 이미지들) -> 3D로 변경
    public Sprite[] foodImages;

    private Vector3 originalPosition; // 버튼의 원래 위치
    private Vector3 offset; // 클릭한 위치로부터의 오프셋

    //3d 음식 상호작용
    // AR 관련 변수
    public GameObject[] arObjectPrefabs; // 각 음식에 대한 AR 프리팹 배열
    private GameObject spawnedObject; // 생성된 AR 오브젝트
    private ARRaycastManager raycastManager;
    private GameObject selectedPrefab; // 현재 선택된 음식의 AR 프리팹

    // Start is called before the first frame update
    void Start()
    {
        // 모든 UI를 초기 상태에서 비활성화
        foreach (var ui in uiElements)
        {
            ui.SetActive(false);
        }

        // 키우기 UI 활성화 (첫 화면)
        if (uiElements.Count > 0)
        {
            uiElements[2].SetActive(true); // Growing_Eat을 기본 활성화
        }

        // 버튼에 클릭 리스너 추가
        redButton.onClick.AddListener(() => ShopFunction("Red"));
        yellowButton.onClick.AddListener(() => ShopFunction("Yellow"));
        greenButton.onClick.AddListener(() => ShopFunction("Green"));
        whiteButton.onClick.AddListener(() => ShopFunction("White"));

        UpdateUI();

        // Feed 버튼 초기화
        feedButtonImage = feedButton.GetComponent<Image>();
        feedButton.onClick.AddListener(InstantiateSelectedObject);

        // 냉장고 버튼 리스너 추가
        for (int i = 0; i < refriFoodButtons.Length; i++)
        {
            int index = i;
            refriFoodButtons[i].onClick.AddListener(() => SelectFood(index));
        }

        // Raycast Manager 초기화
        raycastManager = FindObjectOfType<ARRaycastManager>();

        // 원래 위치 저장
        originalPosition = feedButton.transform.position;
    }

    void Update()
    {
        // 터치로 AR 오브젝트 이동
        if (Input.touchCount > 0 && spawnedObject != null)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                //MoveObject(touch);
            }
        }
    }

    // 음식 선택 시 Feed 버튼 이미지 및 AR 프리팹 설정
    private void SelectFood(int index)
    {
        if (index >= 0 && index < foodImages.Length && index < arObjectPrefabs.Length)
        {
            feedButtonImage.sprite = foodImages[index];
            selectedPrefab = arObjectPrefabs[index];
        }
        else
        {
            Debug.LogError("Invalid food selection!");
        }
    }

    // Feed 버튼 클릭 시 3D 오브젝트 생성
    private void InstantiateSelectedObject()
    {
        if (selectedPrefab != null)
        {
            Vector3 spawnPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.5f;
            spawnedObject = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogError("No prefab selected!");
        }
    }


    void MoveObject(Touch touch)
    {
        Vector2 touchPosition = touch.position;
        List<ARRaycastHit> hits = new List<ARRaycastHit>();

        if (raycastManager.Raycast(touchPosition, hits, TrackableType.Planes))
        {
            Pose hitPose = hits[0].pose;

            // 생성된 오브젝트를 새로운 위치로 이동
            spawnedObject.transform.position = hitPose.position;
        }
    }

    // 터치 끝나면 고정
    public void OnPointerUp(PointerEventData eventData)
    {
        if (spawnedObject != null)
        {
            Debug.Log("Object placed in AR space.");
        }
    }

    // 버튼을 드래그할 때 실행되는 함수
    public void OnPointerDown(PointerEventData eventData)
    {
        // Pointer Down 이벤트 처리 로직
        Debug.Log("Pointer Down!");
    }

    // 버튼을 떼었을 때 실행되는 함수
    public void OnDrag(PointerEventData eventData)
    {
        // 특정 버튼인지 확인
        if (CompareTag("DraggableButton")) // 예시로 태그를 비교
        {
            // 버튼이 원래 위치로 돌아가도록 설정
            transform.position = originalPosition;
        }
    }

    public void ChangeScene(string SceneName) 
    //인스펙터에서 버튼 할당 X 버튼 onClick에서 함수 할당. 여러개의 HomeBtn에서 쓰기 위함
    {
        SceneName = "SampleScene";

        if (!string.IsNullOrEmpty(SceneName))
        {
            SceneManager.LoadScene(SceneName);
        }
        else
        {
            Debug.LogError("Target scene name is not set!");
        }
    }

    public void ChangeUI(GameObject targetUI)
    {
        // 모든 UI를 비활성화
        foreach (var ui in uiElements)
        {
            ui.SetActive(false);
        }

        // 선택된 UI만 활성화
        if (targetUI != null)
        {
            targetUI.SetActive(true);
        }
        else
        {
            Debug.LogError("Target UI is null!");
        }
    }

    //각 버튼의 btn onClick에 함수 할당해둠

    public void ShopFunction(string itemType)
    {
        switch (itemType)
        {
            case "Red":
                if (redCoin >= 5)
                {
                    redCoin -= 5;
                    redFood += 1;
                }
                else
                {
                    Debug.Log("Not enough Red Coins!");
                }
                break;

            case "Yellow":
                if (yellowCoin >= 5)
                {
                    yellowCoin -= 5;
                    yellowFood += 1;
                }
                else
                {
                    Debug.Log("Not enough Yellow Coins!");
                }
                break;

            case "Green":
                if (greenCoin >= 5)
                {
                    greenCoin -= 5;
                    greenFood += 1;
                }
                else
                {
                    Debug.Log("Not enough Green Coins!");
                }
                break;

            case "White":
                if (whiteCoin >= 5)
                {
                    whiteCoin -= 5;
                    whiteFood += 1;
                }
                else
                {
                    Debug.Log("Not enough White Coins!");
                }
                break;

            default:
                Debug.LogError("Invalid item type!");
                break;
        }

        UpdateUI();
    }

    // UI 텍스트 업데이트
    private void UpdateUI()
    {
        // 코인과 음식의 상태 업데이트
        coinTexts[0].text = $"{redCoin}";
        coinTexts[1].text = $"{whiteCoin}";
        coinTexts[2].text = $"{greenCoin}";
        coinTexts[3].text = $"{yellowCoin}";

        foodTexts[0].text = $"{redFood}";
        foodTexts[1].text = $"{whiteFood}";
        foodTexts[2].text = $"{greenFood}";
        foodTexts[3].text = $"{yellowFood}";
    }

    private void ChangeFeedImage(int index)
    {
        // 유효한 인덱스일 때만 이미지 변경
        if (index >= 0 && index < foodImages.Length)
        {
            feedButtonImage.sprite = foodImages[index];
        }
        else
        {
            Debug.LogError("Invalid index for food image!");
        }
    }

}
