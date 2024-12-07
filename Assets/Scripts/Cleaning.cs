using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cleaning : MonoBehaviour
{
    public GameObject soapButton; 
    public GameObject showerButton; 
    public GameObject soapDragObj; 
    public GameObject showerDragObj; 
    public GameObject bubblePrefab; 
    public AudioSource BubbleSound;
    public AudioSource ShowerSound;

    private Camera mainCamera;
    private GameObject activeClone; // �巡�� ���� ������Ʈ
    private bool isDragging = false;
    private float dragDuration = 0f; 

    private HashSet<Vector3> bubblePositions = new HashSet<Vector3>(); // �̹� ��ġ�� ���� ��ġ ����
    private PlaceObjectOnPlane placeObjectOnPlane;

    void Start()
    {
        mainCamera = Camera.main;
        placeObjectOnPlane = PlaceObjectOnPlane.Instance;
    }

    void Update()
    {
        if (isDragging && activeClone != null)
        {
            // �巡�� ���� ������Ʈ�� ���콺 ��ġ�� �̵�
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 10f; // ī�޶���� ���� �Ÿ�
            activeClone.transform.position = mainCamera.ScreenToWorldPoint(mousePosition);

            // �巡�� �� ������Ʈ�� ī�޶� ���ϵ��� ȸ��
            activeClone.transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);

            dragDuration += Time.deltaTime;

            if (activeClone.CompareTag("Soap") && dragDuration >= 1.5f)
            {
                placeObjectOnPlane.UpdateObjectForCleaning();
                dragDuration = 0f;
            }

            if (activeClone.CompareTag("Shower") && dragDuration >= 3f)
            {
                DeactivateAllBubbles();
                dragDuration = 0f;
            }
        }

        HandleDraggingInput();
    }

    private void HandleDraggingInput()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            if (IsPointerOverUIObject(out GameObject clickedButton))
            {
                if (clickedButton == soapButton || clickedButton == showerButton)
                {
                    StartDragging(clickedButton);
                }
            }
        }

        if (Input.GetMouseButtonUp(0)) 
        {
            StopDragging();
        }
    }

    private bool IsPointerOverUIObject(out GameObject clickedButton)
    {
        clickedButton = null;

        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            if (result.gameObject == soapButton || result.gameObject == showerButton)
            {
                clickedButton = result.gameObject;
                return true;
            }
        }

        return false;
    }

    public void StartDragging(GameObject button)
    {
        if (isDragging) return;

        if (button == soapButton)
        {
            BubbleSound.Play();
            activeClone = Instantiate(soapDragObj, button.transform.position, Quaternion.Euler(0, 90, 0));
            activeClone.tag = "Soap";
        }
        else if (button == showerButton)
        {
            ShowerSound.Play();
            activeClone = Instantiate(showerDragObj, button.transform.position, Quaternion.Euler(0, 90, 0));
            activeClone.tag = "Shower";
        }

        activeClone.SetActive(true);
        isDragging = true;
        dragDuration = 0f; 
    }

    public void StopDragging()
    {
        if (!isDragging) return;

        if (activeClone != null)
        {
            Destroy(activeClone);
        }
        isDragging = false;
        dragDuration = 0f; 
        BubbleSound.Stop();
        ShowerSound.Stop();
    }

    private void DeactivateAllBubbles()
    {
        GameObject[] bubbles = GameObject.FindGameObjectsWithTag("Bubble");
        foreach (GameObject bubble in bubbles)
        {
            bubble.SetActive(false);
        }
    }
}
