using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Game/Player Data")]
public class TeuniInven : ScriptableObject
{
    public float MaxHp = 100;
    public float hp = 40;
    public int redCoin = 50;
    public int whiteCoin = 50;
    public int greenCoin = 50;
    public int yellowCoin = 50;

    public int redFood = 0;
    public int yellowFood = 0;
    public int greenFood = 0;
    public int whiteFood = 0;

    public delegate void OnHPChanged(int currentHP);
    public event OnHPChanged HPChanged;

    public float maxGauge = 100; // ������ �ִ밪
    public float whiteGauge = 0;
    public float redGauge = 0;
    public float yellowGauge = 0;
    public float greenGauge = 0;

    private DateTime lastUpdateTime;

    //Start �Լ�ó�� ���
    public void ResetData()
    {
        hp = 40;
        redCoin = 50;
        whiteCoin = 50;
        greenCoin = 50;
        yellowCoin = 50;
        
        redFood = 0;
        yellowFood = 0;
        greenFood = 0;
        whiteFood = 0;

        whiteGauge = 0;
        redGauge = 0;
        yellowGauge = 0;
        greenGauge = 0;

        lastUpdateTime = DateTime.Now;
        Debug.Log("hp reset: "+hp);
    }

    public void UpdateHP(int delta)
    {
        hp += delta;

        // HP�� ������ ����� �ʵ��� ����
        if (hp > MaxHp) hp = MaxHp;
        if (hp < 0) hp = 0;

        // HP ���� �̺�Ʈ Ʈ����
        HPChanged?.Invoke((int)hp);
        //�̰� ����� �۵��� �ȵǳ�?

    }

    // �ð��� ����ϸ� HP ����
    public void UpdateTimeAndHP()
    {
        DateTime currentTime = DateTime.Now;
        TimeSpan elapsed = currentTime - lastUpdateTime;

        int hoursPassed = (int)elapsed.TotalHours;

        if (hoursPassed > 0)
        {
            hp -= hoursPassed * 8; // 1�ð��� HP 8 ����
            if (hp < 0) hp = 0;

            lastUpdateTime = currentTime;
            HPChanged?.Invoke((int)hp);

            Debug.Log($"HP decreased by {hoursPassed * 8}. Current HP: {hp}");
        }
    }

    // ������ �Ծ��� �� HP�� ������ ������Ʈ
    public void EatFood(string color)
    {
        if (hp <= 0)
        {
            Debug.Log("Cannot eat food. HP is zero.");
            return;
        }

        switch (color)
        {
            case "Red":
                if (redFood > 0 && redGauge < maxGauge)
                {
                    redFood--;
                    redGauge += 20;
                    UpdateHP(10);
                }
                break;

            case "White":
                if (whiteFood > 0 && whiteGauge < maxGauge)
                {
                    whiteFood--;
                    whiteGauge += 20;
                    UpdateHP(10);
                }
                break;

            case "Yellow":
                if (yellowFood > 0 && yellowGauge < maxGauge)
                {
                    yellowFood--;
                    yellowGauge += 20;
                    UpdateHP(10);
                }
                break;

            case "Green":
                if (greenFood > 0 && greenGauge < maxGauge)
                {
                    greenFood--;
                    greenGauge += 20;
                    UpdateHP(10);
                }
                break;

            default:
                Debug.Log("Invalid food color.");
                break;
        }

        NormalizeGauges();
        Debug.Log($"Food eaten. Gauges - Red: {redGauge}, White: {whiteGauge}, Yellow: {yellowGauge}, Green: {greenGauge}");
    }


    private void NormalizeGauges()
    {
        // �� �������� maxGauge�� �ʰ����� �ʵ��� ����
        whiteGauge = Mathf.Clamp(whiteGauge, 0, maxGauge);
        redGauge = Mathf.Clamp(redGauge, 0, maxGauge);
        yellowGauge = Mathf.Clamp(yellowGauge, 0, maxGauge);
        greenGauge = Mathf.Clamp(greenGauge, 0, maxGauge);
    }

    public void DecreaseGauges()
    {
        // �Ϸ簡 ������ �������� 50%�� ����
        whiteGauge *= 0.5f;
        redGauge *= 0.5f;
        yellowGauge *= 0.5f;
        greenGauge *= 0.5f;

        Debug.Log("Gauges decreased by 50%.");
    }

    //���� �������� ���� �ǹ�...
    public float CalculateOverallGauge()
    {
        // ���� �������� ��հ����� ���
        return (whiteGauge + redGauge + yellowGauge + greenGauge) / 4;
    }
}
