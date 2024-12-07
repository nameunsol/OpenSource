using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaceObjectOnPlane : MonoBehaviour
{
    private static PlaceObjectOnPlane instance;
    public static PlaceObjectOnPlane Instance
    {  
        get { return instance; } 
    }

    public GameObject happyPrefab; 
    public GameObject sadPrefab;   
    public GameObject neutralPrefab;

    private GameObject spawnedObject;
    private Animator spawnedAnimator;
    private bool isObjectPlaced = false;
    private Vector3 teuniScale = new Vector3(1f, 1f, 1f);
    private bool isMax = true;

    [HideInInspector]
    public bool isPoo = true;
    [HideInInspector]
    public bool isEvolution = true;

    private ARRaycastManager raycastManager; // AR Raycast Manager
    private List<ARRaycastHit> hits = new List<ARRaycastHit>(); // Raycast ��� ����� ����Ʈ

    //public TeuniInven TeuniInven;
    private float timer = 0f;
    private bool timerActive = false; 

    void Awake()
    {
        if(null == instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        // ARRaycastManager �ʱ�ȭ
        raycastManager = FindObjectOfType<ARRaycastManager>();

        if (raycastManager == null)
        {
            Debug.LogError("ARRaycastManager not found in the scene!");
        }
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                HandleTouch(touch);
            }
        }

        if (TeuniManager.Instance.Hp == 40 || TeuniManager.Instance.Hp == 50 || TeuniManager.Instance.Hp == 100)
        {
            UpdateObjectBasedOnHP();
        }

        if (isObjectPlaced && timerActive)
        {
            timer += Time.deltaTime;

            // Ÿ�̸Ӱ� 1���� ������ neutralPrefab�� ���� ��ġ�� ��ġ
            if (timer >= 60f)
            {
                isPoo = true;
                PlaceNeutralPrefab();
            }
        }
    }

    private void HandleTouch(Touch touch)
    {
        // ������Ʈ�� �̹� ��ġ�Ǿ��ٸ� �� �̻� ��ġ���� ����
        if (isObjectPlaced)
        {
            Debug.Log("Object already placed. No further changes allowed.");
            return;
        }

        // ��ġ ��ġ���� Raycast ����
        if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose; // Raycast�� ù ��° Plane�� ��ġ

            if (spawnedObject == null)
            {
                GameObject prefabToSpawn = GetPrefabBasedOnHP();
                SetPrefabScale();
                // ������Ʈ ���� �� ��ġ
                spawnedObject = Instantiate(prefabToSpawn, hitPose.position, hitPose.rotation);
                spawnedObject.transform.localScale = teuniScale;
                spawnedAnimator = spawnedObject.GetComponent<Animator>();

                // y������ 180�� �߰� ȸ��
                spawnedObject.transform.rotation = hitPose.rotation * Quaternion.Euler(0, 180, 0);

                isObjectPlaced = true; // ��ġ ����
                timerActive = true; 
                Debug.Log("AR Object instantiated at: " + hitPose.position);
            }
        }
        else
        {
            Debug.Log("No plane detected at touch position.");
        }
    }

    private GameObject GetPrefabBasedOnHP()
    {
        // HP ���¿� ���� ������ ����
        return TeuniManager.Instance.Hp < 50 ? sadPrefab : happyPrefab;
    }

    private void SetPrefabScale()
    {
        if (TeuniManager.Instance.Hp < 100)
        {
            isMax = true;
            return;
        }

        if (TeuniManager.Instance.Hp >= 100 && isMax)
        {
            teuniScale *= 2f;
            isMax = false;
        }
    }

    private void PlaceNeutralPrefab()
    {
        if (spawnedObject != null)
        {
            Destroy(spawnedObject); 
        }
        SetPrefabScale();
        spawnedObject = Instantiate(neutralPrefab, spawnedObject.transform.position, spawnedObject.transform.rotation);
        spawnedObject.transform.localScale = teuniScale;
        spawnedAnimator = spawnedObject.GetComponent<Animator>();
        Debug.Log("Neutral prefab placed at: " + spawnedObject.transform.position);
        timer = 0f; 
        timerActive = false; 
    }

    // HP ���� �޼��� (�ܺο��� ȣ�� ����)
    public void ChangeHP(float amount)
    {
        TeuniManager.Instance.Hp += amount;
        Debug.Log("HP changed to: " + TeuniManager.Instance.Hp);
        UpdateObjectBasedOnHP(); // HP ���� �� ������Ʈ ������Ʈ
    }

    // HP ���¿� ���� ������Ʈ�� ������Ʈ�ϴ� �޼���
    public void UpdateObjectBasedOnHP()
    {
        if (isObjectPlaced && spawnedObject != null && !isPoo)
        {
            GameObject prefabToSpawn = GetPrefabBasedOnHP();
            SetPrefabScale();
            // ���� ��ġ�� ������Ʈ�� ���ο� ������Ʈ�� �ٸ��� ��ü
            if (spawnedObject.name != prefabToSpawn.name + "(Clone)") 
            {
                Destroy(spawnedObject);
                spawnedObject = Instantiate(prefabToSpawn, spawnedObject.transform.position, spawnedObject.transform.rotation);
                spawnedObject.transform.localScale = teuniScale;
                spawnedAnimator = spawnedObject.GetComponent<Animator>();
                Debug.Log("AR Object updated to: " + prefabToSpawn.name);
                timerActive = true;
            }

            if(TeuniManager.Instance.Hp == 100)
            {
                spawnedObject.transform.localScale = teuniScale;
            }
        }
    }

    public void UpdateObjectForCleaning()
    {
        if (isObjectPlaced && spawnedObject != null)
        {
            GameObject prefabToSpawn = GetPrefabBasedOnHP();

            // neutralPrefab ���¶�� ������Ʈ�� ������ ��ü
            if (spawnedObject.name == neutralPrefab.name + "(Clone)") 
            {
                Destroy(spawnedObject);
                SetPrefabScale();
                spawnedObject = Instantiate(prefabToSpawn, spawnedObject.transform.position, spawnedObject.transform.rotation);
                spawnedObject.transform.localScale = teuniScale;
                spawnedAnimator = spawnedObject.GetComponent<Animator>();
                Debug.Log("AR Object updated to: " + prefabToSpawn.name);

                timerActive = true; 
                isPoo = false;
            }

            ActivateBubblesByName();
        }
    }

    public void RemoveSpawnedObject()
    {
        if (spawnedObject != null)
        {
            Destroy(spawnedObject);
            spawnedObject = null;
            isObjectPlaced = false; // ���� �ʱ�ȭ
            timerActive = false; 
        }
        else
        {
            Debug.Log("No object to remove.");
        }
    }

    private void ActivateBubbles()
    {
        // Bubble �±׸� ���� ��� ������Ʈ Ȱ��ȭ
        GameObject[] bubbles = GameObject.FindGameObjectsWithTag("Bubble");
        foreach (GameObject bubble in bubbles)
        {
            bubble.SetActive(true);
            Debug.Log("Activated bubble: " + bubble.name); // ����� �α� �߰�
        }
    }

    private void ActivateBubblesByName()
    {
        // ��� ��Ȱ��ȭ�� Bubble ������Ʈ�� ã��
        GameObject[] bubbles = GameObject.FindObjectsOfType<GameObject>(true);

        foreach (GameObject bubble in bubbles)
        {
            if (bubble.name.StartsWith("Bubble") && !bubble.activeInHierarchy)
            {
                bubble.SetActive(true); // ��Ȱ��ȭ�� ù ��° ���� Ȱ��ȭ
                Debug.Log("Activated bubble: " + bubble.name);
                return; // �� ���� Ȱ��ȭ �� ����
            }
        }

        Debug.LogWarning("No inactive bubbles found to activate.");
    }

    public void PlayAnimation(string triggerName)
    {
        if (spawnedAnimator != null)
        {
            spawnedAnimator.ResetTrigger("Eat");  // ���� Ʈ���� �ʱ�ȭ
            spawnedAnimator.ResetTrigger("Jump"); // ���� Ʈ���� �ʱ�ȭ
            spawnedAnimator.SetTrigger(triggerName); // ���ο� Ʈ���� ����
        }

        if (spawnedAnimator != null)
        {
            spawnedAnimator.SetTrigger(triggerName);
        }
    }
}
