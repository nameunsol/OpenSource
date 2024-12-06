using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodController : MonoBehaviour
{
    private Camera mainCamera; // ���� ī�޶� ����
    private Vector3 dragOffset; // �巡�� ���� �� ������
    private bool isDragging = false; // �巡�� ������ ����
    private bool isPlaced = false; // Ʈ���� �ݶ��̴� �ȿ� ��ġ�ߴ��� ����

    public ParticleSystem Eatting;
    public AudioSource EatSound;
    public PlaceObjectOnPlane placeObjectOnPlane;

    void Start()
    {
        mainCamera = Camera.main;
        Eatting.Stop();

        // ������Ʈ�� ������ �� 7�� �� �Ҹ�
        StartCoroutine(AutoDestroyAfterTime(10f));
    }

    void Update()
    {
        if (isDragging && !isPlaced) // ��ġ���� ���� ������Ʈ�� �巡�� ����
        {
            // ������� �巡�� ���⿡ ���� ������Ʈ �̵�
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 0.5f; // ������Ʈ�� ī�޶� �տ� ������ �Ÿ��� ����
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition + new Vector3(0, 0, 0.5f));
            transform.position = worldPosition + dragOffset;
        }
    }

    void OnMouseDown()
    {
        // ��ġ�� ������Ʈ�� �巡�� �������� ����
        if (!isPlaced)
        {
            isDragging = true;

            // �巡�� ���� �� ������ ���
            dragOffset = transform.position - mainCamera.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, 0.5f));
        }
    }

    void OnMouseUp()
    {
        // ���콺�� ���� �巡�� ����
        isDragging = false;
    }

    void OnTriggerEnter(Collider other)
    {
        // Ʈ���� �ݶ��̴��� ������ �浹���� �� ����
        if (other.CompareTag("Teuni") && CompareTag("Food"))
        {
            isPlaced = true;

            // Ʈ���� ��ġ�� ���� ���� ��ġ ���� (y�� 0.3 �ø��� ������ �̵�)
            Vector3 adjustedPosition = other.transform.position;
            adjustedPosition.y += 0.23f; 
            adjustedPosition += other.transform.forward * 0.35f; 
            transform.position = adjustedPosition;
            if (placeObjectOnPlane != null)
            {
                placeObjectOnPlane.PlayAnimation("Eat");
            }
            Eatting.transform.position = transform.position;
            EatSound.Play();
            Eatting.Play();
            StartCoroutine(DestroyAfterDelay(3f));
        }
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        // ������ �ð� �� ������Ʈ ����
        yield return new WaitForSeconds(delay);
        EatSound.Stop();
        Destroy(gameObject);
        if (placeObjectOnPlane != null)
        {
            placeObjectOnPlane.PlayAnimation("Jump");
        }
    }

    private IEnumerator AutoDestroyAfterTime(float time)
    {
        // ���浹 ���¿��� ���� �ð��� ������ ����
        yield return new WaitForSeconds(time);

        if (!isPlaced) // Ʈ���� �ݶ��̴��� ���� ���� ��쿡�� ����
        {
            // ���⼭ ���� �� �ٽ� ����
            Destroy(gameObject);
        }
    }
}
