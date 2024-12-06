using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooImage : MonoBehaviour
{
    public GameObject[] imageObjects; 
    private float timeToActivate = 60f; 
    private float[] timers; 
    private bool[] isActive; 

    void Start()
    {
        // �̹��� ��Ȱ��ȭ �� Ÿ�̸� �ʱ�ȭ
        timers = new float[imageObjects.Length];
        isActive = new bool[imageObjects.Length];

        for (int i = 0; i < imageObjects.Length; i++)
        {
            imageObjects[i].SetActive(false);
            timers[i] = 0f;
            isActive[i] = false;
        }
    }

    void Update()
    {
        for (int i = 0; i < imageObjects.Length; i++)
        {
            // ��Ȱ��ȭ ���¿����� Ÿ�̸� ����
            if (!isActive[i]) 
            {
                timers[i] += Time.deltaTime; 

                // Ÿ�̸Ӱ� 3���� ������ �̹��� Ȱ��ȭ
                if (timers[i] >= timeToActivate)
                {
                    ActivateImage(i);
                }
            }
        }
    }

    // Ư�� �ε����� �̹����� Ȱ��ȭ 
    public void ActivateImage(int index)
    {
        if (index >= 0 && index < imageObjects.Length)
        {
            imageObjects[index].SetActive(true);
            isActive[index] = true;
            timers[index] = 0f; // Ÿ�̸� �ʱ�ȭ
        }
    }

    // Ư�� �ε����� �̹����� ��Ȱ��ȭ 
    public void DeactivateImage(int index)
    {
        if (index >= 0 && index < imageObjects.Length)
        {
            imageObjects[index].SetActive(false);
            isActive[index] = false; 
            timers[index] = 0f; 
        }
    }

    
    public void ActivateAllImages()
    {
        for (int i = 0; i < imageObjects.Length; i++)
        {
            ActivateImage(i);
        }
    }

    
    public void DeactivateAllImages()
    {
        for (int i = 0; i < imageObjects.Length; i++)
        {
            DeactivateImage(i);
        }
    }
}
