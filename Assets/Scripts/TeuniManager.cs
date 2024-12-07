using System;
using UnityEngine;
using System.Collections;

public class TeuniManager : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    private static TeuniManager _instance;

    public static TeuniManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("TeuniManager");
                _instance = obj.AddComponent<TeuniManager>();
                DontDestroyOnLoad(obj); // �� ��ȯ �ÿ��� ����
            }
            return _instance;
        }
    }

    // ���� ������
    public float MaxHp = 100;
    public float Hp { get; set; } = 40;


    public int RedCoin { get; set; } = 15;
    public int WhiteCoin { get; set; } = 15;
    public int GreenCoin { get; set; } = 15;
    public int YellowCoin { get; set; } = 15;

    public int RedFood { get; set; } = 0;
    public int YellowFood { get; set; } = 0;
    public int GreenFood { get; set; } = 0;
    public int WhiteFood { get; set; } = 0;

    public float MaxGauge = 100;
    public float WhiteGauge { get; private set; } = 0;
    public float RedGauge { get; private set; } = 0;
    public float YellowGauge { get; private set; } = 0;
    public float GreenGauge { get; private set; } = 0;

    private DateTime _lastUpdateTime;

    // �̺�Ʈ (HP ����)
    public event Action<int> HPChanged;

    public string FoodColor = "";
    public static bool StartSceneTutorial { get; set; } = false;
    public static bool EatingSceneTutorial { get; set; } = false;
    public static bool TeuniSceneTutorial { get; set; } = false;
    
    private void Awake()
    {
        // �̱��� �ߺ� ����
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        StartCoroutine(ReduceStatsOverTime());
    }

    private IEnumerator ReduceStatsOverTime()
    {
        while (true) // ���� �ݺ�
        {
            yield return new WaitForSeconds(60);

            // HP ����
            UpdateHP(-10);

            // ������ ����
            WhiteGauge = Mathf.Max(WhiteGauge - 10, 0);
            RedGauge = Mathf.Max(RedGauge - 10, 0);
            YellowGauge = Mathf.Max(YellowGauge - 10, 0);
            GreenGauge = Mathf.Max(GreenGauge - 10, 0);

            // ������ ����
            NormalizeGauges();

            Debug.Log($"HP: {Hp}, WhiteGauge: {WhiteGauge}, RedGauge: {RedGauge}, YellowGauge: {YellowGauge}, GreenGauge: {GreenGauge}");
        }
    }

    // HP ���� �޼���
    public void UpdateHP(int delta)
    {
        Hp += delta;

        // HP ����
        Hp = Mathf.Clamp(Hp, 0, MaxHp);

        // �̺�Ʈ ȣ��
        HPChanged?.Invoke((int)Hp);
    }

    // �ð� ����� ���� HP ���� -> ���� ���� HP �������� �ʿ�
    public void UpdateTimeAndHP()
    {
        DateTime currentTime = DateTime.Now;
        TimeSpan elapsed = currentTime - _lastUpdateTime;

        int hoursPassed = (int)elapsed.TotalHours;

        if (hoursPassed > 0)
        {
            UpdateHP(-hoursPassed * 8); // 1�ð��� 8 ����
            _lastUpdateTime = currentTime;
        }
    }

    // ���� �Ա�
    public void EatFood(string color)
    {
        if (Hp <= 0)
        {
            Debug.Log("Cannot eat food. HP is zero.");
            return;
        }

        switch (color)
        {
            case "Red":
                if (RedFood > 0 && RedGauge < MaxGauge)
                {
                    RedFood--;
                    RedGauge += 20;
                    UpdateHP(10);
                }
                break;

            case "White":
                if (WhiteFood > 0 && WhiteGauge < MaxGauge)
                {
                    WhiteFood--;
                    WhiteGauge += 20;
                    UpdateHP(10);
                }
                break;

            case "Yellow":
                if (YellowFood > 0 && YellowGauge < MaxGauge)
                {
                    YellowFood--;
                    YellowGauge += 20;
                    UpdateHP(10);
                }
                break;

            case "Green":
                if (GreenFood > 0 && GreenGauge < MaxGauge)
                {
                    GreenFood--;
                    GreenGauge += 20;
                    UpdateHP(10);
                }
                break;

            default:
                Debug.Log("Invalid food color.");
                break;
        }

        NormalizeGauges();
    }

    // ������ ����ȭ
    private void NormalizeGauges()
    {
        WhiteGauge = Mathf.Clamp(WhiteGauge, 0, MaxGauge);
        RedGauge = Mathf.Clamp(RedGauge, 0, MaxGauge);
        YellowGauge = Mathf.Clamp(YellowGauge, 0, MaxGauge);
        GreenGauge = Mathf.Clamp(GreenGauge, 0, MaxGauge);
    }

    // ������ ����
    public void DecreaseGauges()
    {
        WhiteGauge *= 0.5f;
        RedGauge *= 0.5f;
        YellowGauge *= 0.5f;
        GreenGauge *= 0.5f;
    }

    // ���� ������ ���
    public float CalculateOverallGauge()
    {
        return (WhiteGauge + RedGauge + YellowGauge + GreenGauge) / 4;
    }

    // ������ �ʱ�ȭ
    public void ResetData()
    {
        Hp = 40;
        RedCoin = 50;
        WhiteCoin = 50;
        GreenCoin = 50;
        YellowCoin = 50;

        RedFood = 0;
        YellowFood = 0;
        GreenFood = 0;
        WhiteFood = 0;

        WhiteGauge = 0;
        RedGauge = 0;
        YellowGauge = 0;
        GreenGauge = 0;

        _lastUpdateTime = DateTime.Now;
    }
}
