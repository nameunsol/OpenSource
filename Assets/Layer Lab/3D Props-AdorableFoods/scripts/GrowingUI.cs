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
using EasyUI.Popup;
using Unity.VisualScripting;

public class GrowingUI : MonoBehaviour
{
    public List<GameObject> uiElements; // ��� UI�� ������ ����Ʈ
                                        // 0. shop
                                        // 1. growing_Bath
                                        // 2. Eat
                                        // 3. Refri

    //������ �׽�Ʈ������ 50 ����. ���߿� ȹ�濡 ���� �����ǰ� ��

    // Shop Btn
    public UnityEngine.UI.Button redButton;
    public UnityEngine.UI.Button yellowButton;
    public UnityEngine.UI.Button greenButton;
    public UnityEngine.UI.Button whiteButton;

    // �ؽ�Ʈ �迭
    public TextMeshProUGUI[] coinTexts;  // ���� ���� �ؽ�Ʈ
    public TextMeshProUGUI[] foodTexts;  // ���� ���� �ؽ�Ʈ

    //public TeuniInven TeuniInven;

    public UnityEngine.UI.Slider TeuniHPSlider;

    // Refri_food ��ư �迭
    public UnityEngine.UI.Button[] refriFoodButtons;

    // Feed ��ư (�̹����� ������ ��ư)
    public UnityEngine.UI.Button feedButton;

    // Feed ��ư�� Image ������Ʈ
    private UnityEngine.UI.Image feedButtonImage;

    // �̹��� �迭 (Refri_food ��ư�� ����� �̹�����)
    public Sprite[] foodImages;

    private Vector3 originalPosition; // ��ư�� ���� ��ġ
    private Vector3 offset; // Ŭ���� ��ġ�κ����� ������

    //3d ���� ��ȣ�ۿ�
    // AR ���� ����
    public GameObject[] arObjectPrefabs; // �� ���Ŀ� ���� AR ������ �迭
    private GameObject spawnedObject; // ������ AR ������Ʈ
    private ARRaycastManager raycastManager;
    private GameObject selectedPrefab; // ���� ���õ� ������ AR ������

    //���� ����
    private string[] TutorialText = { "�̰������� Ʈ�ϸ� ���� �� �־��.", "Ʈ�ϰ� ���������� �̰����� Ʈ�ϸ� �İܺ���."};
    private string[] popupText = {"������� Ʈ�Ͽ��� �� ������ �����ϼ���", "���� �����մϴ�!", "������ �����մϴ�!" };

    //public string FoodColor;
    public Text DebugText;

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

        TeuniHPSlider.value = (float)TeuniManager.Instance.Hp / (float)TeuniManager.Instance.MaxHp;
        Debug.Log(TeuniManager.Instance.Hp);
        Debug.Log(TeuniHPSlider.value);

        TeuniManager.Instance.HPChanged += UpdateSlider;

        
        if (!TeuniManager.TeuniSceneTutorial)
        {
            Popup.Show("Ʈ�� ������", "[ ���� ]\nƮ�Ͽ��� ���ִ� ������ �Կ������?\r\n\n[ ���� ]\nƮ�ϰ� ���������� �ȵſ�!\n\n[ ����� ]\nƮ�Ͽ��� �� ������ ����");
            TeuniManager.TeuniSceneTutorial = true;
        }
    }

    public void TestHpUp()
    {
        //TeuniInven.hp += 10;
        //TeuniHPSlider.value = (float) TeuniInven.hp / 100f;
        //UpdateSlider((int)TeuniInven.hp);

        //TeuniInven.UpdateHP(10);  //�Ǵµ�???
        //Debug.Log(TeuniInven.hp);
        TeuniManager.Instance.UpdateHP(10);
        Debug.Log(TeuniManager.Instance.Hp);
    }

    // ���� ���� �� Feed ��ư �̹��� �� AR ������ ����
    private void SelectFood(int index)
    {
        if (index >= 0 && index < foodImages.Length && index < arObjectPrefabs.Length)
        {
            switch (index)
            {
                case 0:
                    if(TeuniManager.Instance.RedFood <= 0)
                    {
                        Debug.Log("Not Enought Food!");
                        Popup.Show("������ �����մϴ�!");
                        return;
                    }
                    break;
                case 1:
                    if (TeuniManager.Instance.GreenFood <= 0)
                    {
                        Debug.Log("������ �����մϴ�!");
                        Popup.Show("������ �����մϴ�!");
                        return;
                    }
                    break;
                case 2:
                    if (TeuniManager.Instance.WhiteFood <= 0)
                    {
                        Debug.Log("Not Enought Food!");
                        Popup.Show("������ �����մϴ�!");
                        return;
                    }
                    break;
                case 3:
                    if (TeuniManager.Instance.YellowFood <= 0)
                    {
                        Debug.Log("Not Enought Food!");
                        Popup.Show("������ �����մϴ�!");
                        return;
                    }
                    break;
            }
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
            // ���� ���� Ȯ��
            if (feedButtonImage.sprite == null)
            {
                Debug.LogError("No food selected for feeding!");
                return;
            }

            // feedButtonImage.sprite�� foodImages �迭�� ��Ī Ȯ��
            for (int i = 0; i < foodImages.Length; i++)
            {
                if (feedButtonImage.sprite == foodImages[i])
                {
                    // ���� ���� üũ
                    switch (i)
                    {
                        case 0: // Red Food
                            if (TeuniManager.Instance.RedFood <= 0)
                            {
                                Debug.LogWarning("Not enough Red Food!");
                                Popup.Show("������ �����մϴ�!");
                                return;
                            }
                            break;
                        case 1: // Green Food
                            if (TeuniManager.Instance.GreenFood <= 0)
                            {
                                Debug.LogWarning("Not enough Green Food!");
                                Popup.Show("������ �����մϴ�!");
                                return;
                            }
                            break;
                        case 2: // White Food
                            if (TeuniManager.Instance.WhiteFood <= 0)
                            {
                                Debug.LogWarning("Not enough White Food!");
                                Popup.Show("������ �����մϴ�!");
                                return;
                            }
                            break;
                        case 3: // Yellow Food
                            if (TeuniManager.Instance.YellowFood <= 0)
                            {
                                Debug.LogWarning("Not enough Yellow Food!");
                                Popup.Show("������ �����մϴ�!");
                                return;
                            }
                            break;
                        default:
                            Debug.LogError("Invalid food index!");
                            return;
                    }
                    break;
                }
            }

            // 3D ������Ʈ ����
            Vector3 spawnPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.5f;
            spawnedObject = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);

            // ���� ���� �� UI ������Ʈ
            UpdateFoodCount();
            UpdateUI();
        }
        else
        {
            Debug.LogError("No prefab selected!");
        }
    }

    // ������ ������ ī��Ʈ�� ���ҽ�Ű�� �Լ�
    private void UpdateFoodCount()
    {
        if (feedButtonImage.sprite == null)
        {
            Debug.LogError("No food selected for feeding!");
            return;
        }

        // feedButtonImage.sprite�� foodImages �迭�� ��Ī Ȯ��
        for (int i = 0; i < foodImages.Length; i++)
        {
            if (feedButtonImage.sprite == foodImages[i])
            {
                // �ش� ���� ����
                switch (i)
                {
                    case 0: // Red Food
                        if (TeuniManager.Instance.RedFood > 0)
                        {
                            TeuniManager.Instance.RedFood--;
                            TeuniManager.Instance.FoodColor = "Red";
                        }
                        else
                            Debug.LogWarning("Not enough Red Food!");
                        break;
                    case 1: // green Food
                        if (TeuniManager.Instance.GreenFood > 0)
                        {
                            TeuniManager.Instance.GreenFood--;
                            TeuniManager.Instance.FoodColor = "Green";
                        }
                        else
                            Debug.LogWarning("Not enough Green Food!");
                        break;
                    case 2: // White Food
                        if (TeuniManager.Instance.WhiteFood > 0)
                        {
                            TeuniManager.Instance.WhiteFood--;
                            TeuniManager.Instance.FoodColor = "White";
                        }
                        else
                            Debug.LogWarning("Not enough White Food!");
                        break;
                    case 3: // Yellow Food
                        if (TeuniManager.Instance.YellowFood > 0)
                        {
                            TeuniManager.Instance.YellowFood--;
                            TeuniManager.Instance.FoodColor = "Yellow";
                        }
                        else
                            Debug.LogWarning("Not enough Yellow Food!");
                        break;
                    default:
                        Debug.LogError("Invalid food index!");
                        break;
                }

                return; // ������ ���ҵǾ����Ƿ� �Լ� ����
            }
        }

        Debug.LogError("Food sprite does not match any known food type!");
    }

    /*
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
    */

    public void ChangeScene(string SceneName) 
    //�ν����Ϳ��� ��ư �Ҵ� X ��ư onClick���� �Լ� �Ҵ�. �������� HomeBtn���� ���� ����
    {
        SceneName = "StartScene";

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
                if (TeuniManager.Instance.RedCoin >= 5)
                {
                    TeuniManager.Instance.RedCoin -= 5;
                    TeuniManager.Instance.RedFood += 1;
                }
                else
                {
                    Debug.Log("Not enough Red Coins!");
                    Popup.Show("���� �����մϴ�!");
                }
                break;

            case "Yellow":
                if (TeuniManager.Instance.YellowCoin >= 5)
                {
                    TeuniManager.Instance.YellowCoin -= 5;
                    TeuniManager.Instance.YellowFood += 1;
                }
                else
                {
                    Debug.Log("Not enough Yellow Coins!");
                    Popup.Show("���� �����մϴ�!");
                }
                break;

            case "Green":
                if (TeuniManager.Instance.GreenCoin >= 5)
                {
                    TeuniManager.Instance.GreenCoin -= 5;
                    TeuniManager.Instance.GreenFood += 1;
                }
                else
                {
                    Debug.Log("Not enough Green Coins!");
                    Popup.Show("���� �����մϴ�!");
                }
                break;

            case "White":
                if (TeuniManager.Instance.WhiteCoin >= 5)
                {
                    TeuniManager.Instance.WhiteCoin -= 5;
                    TeuniManager.Instance.WhiteFood += 1;
                }
                else
                {
                    Debug.Log("Not enough White Coins!");
                    Popup.Show("���� �����մϴ�!");
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
        coinTexts[0].text = $"{TeuniManager.Instance.RedCoin}";
        coinTexts[1].text = $"{TeuniManager.Instance.WhiteCoin}";
        coinTexts[2].text = $"{TeuniManager.Instance.GreenCoin}";
        coinTexts[3].text = $"{TeuniManager.Instance.YellowCoin}";

        foodTexts[0].text = $"{TeuniManager.Instance.RedFood}";
        foodTexts[1].text = $"{TeuniManager.Instance.WhiteFood}";
        foodTexts[2].text = $"{TeuniManager.Instance.GreenFood}";
        foodTexts[3].text = $"{TeuniManager.Instance.YellowFood}";
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
        if (TeuniHPSlider != null)
        {
            TeuniHPSlider.value = currentHP / 100f;
        }
    }

    private void OnDestroy()
    {
        TeuniManager.Instance.HPChanged -= UpdateSlider;
    }
}
