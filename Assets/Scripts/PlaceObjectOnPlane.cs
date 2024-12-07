using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaceObjectOnPlane : MonoBehaviour
{
    public GameObject happyPrefab; 
    public GameObject sadPrefab;   
    public GameObject neutralPrefab; 
    private GameObject spawnedObject;
    private Animator spawnedAnimator;
    private bool isObjectPlaced = false; 

    private ARRaycastManager raycastManager; // AR Raycast Manager
    private List<ARRaycastHit> hits = new List<ARRaycastHit>(); // Raycast ��� ����� ����Ʈ

    private float hp = 100f; 
    //public TeuniInven TeuniInven;
    private float timer = 0f;
    private bool timerActive = false; 

    void Awake()
    {
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

        if (isObjectPlaced && timerActive)
        {
            timer += Time.deltaTime;

            // Ÿ�̸Ӱ� 1���� ������ neutralPrefab�� ���� ��ġ�� ��ġ
            if (timer >= 60f)
            {
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

                // ������Ʈ ���� �� ��ġ
                spawnedObject = Instantiate(prefabToSpawn, hitPose.position, hitPose.rotation);
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
        return hp < 50 ? sadPrefab : happyPrefab;
    }

    private void PlaceNeutralPrefab()
    {
        if (spawnedObject != null)
        {
            Destroy(spawnedObject); 
        }
        spawnedObject = Instantiate(neutralPrefab, spawnedObject.transform.position, spawnedObject.transform.rotation);
        spawnedAnimator = spawnedObject.GetComponent<Animator>();
        Debug.Log("Neutral prefab placed at: " + spawnedObject.transform.position);
        timer = 0f; 
        timerActive = false; 
    }

    // HP ���� �޼��� (�ܺο��� ȣ�� ����)
    public void ChangeHP(float amount)
    {
        hp += amount;
        Debug.Log("HP changed to: " + hp);
        UpdateObjectBasedOnHP(); // HP ���� �� ������Ʈ ������Ʈ
    }

    // HP ���¿� ���� ������Ʈ�� ������Ʈ�ϴ� �޼���
    public void UpdateObjectBasedOnHP()
    {
        if (isObjectPlaced && spawnedObject != null)
        {
            GameObject prefabToSpawn = GetPrefabBasedOnHP();
            // ���� ��ġ�� ������Ʈ�� ���ο� ������Ʈ�� �ٸ��� ��ü
            if (spawnedObject.name != prefabToSpawn.name + "(Clone)") 
            {
                Destroy(spawnedObject); 
                spawnedObject = Instantiate(prefabToSpawn, spawnedObject.transform.position, spawnedObject.transform.rotation);
                spawnedAnimator = spawnedObject.GetComponent<Animator>();
                Debug.Log("AR Object updated to: " + prefabToSpawn.name);
                timerActive = true; 
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
                spawnedObject = Instantiate(prefabToSpawn, spawnedObject.transform.position, spawnedObject.transform.rotation);
                spawnedAnimator = spawnedObject.GetComponent<Animator>();
                Debug.Log("AR Object updated to: " + prefabToSpawn.name);

                timerActive = true; 
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
