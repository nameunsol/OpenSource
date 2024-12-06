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
    public List<GameObject> uiElements; // ��� UI�� ������ ����Ʈ
                                        // 0. shop
                                        // 1. growing_Bath
                                        // 2. Eat
                                        // 3. Refri

    //������ �׽�Ʈ������ 50 ����. ���߿� ȹ�濡 ���� �����ǰ� ��

    // Shop Btn
    public Button redButton;
    public Button yellowButton;
    public Button greenButton;
    public Button whiteButton;

    // �ؽ�Ʈ �迭
    public TextMeshProUGUI[] coinTexts;  // ���� ���� �ؽ�Ʈ
    public TextMeshProUGUI[] foodTexts;  // ���� ���� �ؽ�Ʈ

    private int redCoin = 50;
    private int whiteCoin = 50;
    private int greenCoin = 50;
    private int yellowCoin = 50;

    private int redFood = 0;
    private int yellowFood = 0;
    private int greenFood = 0;
    private int whiteFood = 0;

    public Slider TeuniHP;

    // Refri_food ��ư �迭
    public Button[] refriFoodButtons;

    // Feed ��ư (�̹����� ������ ��ư)
    public Button feedButton;

    // Feed ��ư�� Image ������Ʈ
    private Image feedButtonImage;

    // �̹��� �迭 (Refri_food ��ư�� ����� �̹�����) -> 3D�� ����
    public Sprite[] foodImages;

    private Vector3 originalPosition; // ��ư�� ���� ��ġ
    private Vector3 offset; // Ŭ���� ��ġ�κ����� ������

    //3d ���� ��ȣ�ۿ�
    // AR ���� ����
    public GameObject[] arObjectPrefabs; // �� ���Ŀ� ���� AR ������ �迭
    private GameObject spawnedObject; // ������ AR ������Ʈ
    private ARRaycastManager raycastManager;
    private GameObject selectedPrefab; // ���� ���õ� ������ AR ������

    // Start is called before the first frame update
    void Start()
    {
        // ��� UI�� �ʱ� ���¿��� ��Ȱ��ȭ
        foreach (var ui in uiElements)
        {
            ui.SetActive(false);
        }

        // Ű��� UI Ȱ��ȭ (ù ȭ��)
        if (uiElements.Count > 0)
        {
            uiElements[2].SetActive(true); // Growing_Eat�� �⺻ Ȱ��ȭ
        }

        // ��ư�� Ŭ�� ������ �߰�
        redButton.onClick.AddListener(() => ShopFunction("Red"));
        yellowButton.onClick.AddListener(() => ShopFunction("Yellow"));
        greenButton.onClick.AddListener(() => ShopFunction("Green"));
        whiteButton.onClick.AddListener(() => ShopFunction("White"));

        UpdateUI();

        // Feed ��ư �ʱ�ȭ
        feedButtonImage = feedButton.GetComponent<Image>();
        feedButton.onClick.AddListener(InstantiateSelectedObject);

        // ����� ��ư ������ �߰�
        for (int i = 0; i < refriFoodButtons.Length; i++)
        {
            int index = i;
            refriFoodButtons[i].onClick.AddListener(() => SelectFood(index));
        }

        // Raycast Manager �ʱ�ȭ
        raycastManager = FindObjectOfType<ARRaycastManager>();

        // ���� ��ġ ����
        originalPosition = feedButton.transform.position;
    }

    void Update()
    {
        // ��ġ�� AR ������Ʈ �̵�
        if (Input.touchCount > 0 && spawnedObject != null)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                //MoveObject(touch);
            }
        }
    }

    // ���� ���� �� Feed ��ư �̹��� �� AR ������ ����
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

    // Feed ��ư Ŭ�� �� 3D ������Ʈ ����
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

            // ������ ������Ʈ�� ���ο� ��ġ�� �̵�
            spawnedObject.transform.position = hitPose.position;
        }
    }

    // ��ġ ������ ����
    public void OnPointerUp(PointerEventData eventData)
    {
        if (spawnedObject != null)
        {
            Debug.Log("Object placed in AR space.");
        }
    }

    // ��ư�� �巡���� �� ����Ǵ� �Լ�
    public void OnPointerDown(PointerEventData eventData)
    {
        // Pointer Down �̺�Ʈ ó�� ����
        Debug.Log("Pointer Down!");
    }

    // ��ư�� ������ �� ����Ǵ� �Լ�
    public void OnDrag(PointerEventData eventData)
    {
        // Ư�� ��ư���� Ȯ��
        if (CompareTag("DraggableButton")) // ���÷� �±׸� ��
        {
            // ��ư�� ���� ��ġ�� ���ư����� ����
            transform.position = originalPosition;
        }
    }

    public void ChangeScene(string SceneName) 
    //�ν����Ϳ��� ��ư �Ҵ� X ��ư onClick���� �Լ� �Ҵ�. �������� HomeBtn���� ���� ����
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
        // ��� UI�� ��Ȱ��ȭ
        foreach (var ui in uiElements)
        {
            ui.SetActive(false);
        }

        // ���õ� UI�� Ȱ��ȭ
        if (targetUI != null)
        {
            targetUI.SetActive(true);
        }
        else
        {
            Debug.LogError("Target UI is null!");
        }
    }

    //�� ��ư�� btn onClick�� �Լ� �Ҵ��ص�

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

    // UI �ؽ�Ʈ ������Ʈ
    private void UpdateUI()
    {
        // ���ΰ� ������ ���� ������Ʈ
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
        // ��ȿ�� �ε����� ���� �̹��� ����
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
