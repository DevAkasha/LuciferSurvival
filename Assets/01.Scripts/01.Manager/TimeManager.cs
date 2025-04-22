using System;
using UnityEngine;

public class TimeManager : Singleton<TimeManager>
{
    protected override bool IsPersistent => false;

    public enum TimeState { Day, Night } // �ð�����
    public TimeState CurrentState { get; set; } = TimeState.Night; // �ʱⰪ�� ��

    public float nightDuration = 30f; // ���� �ð�
    public float nightTimer; // ���� �ð��� �帣�� ���� Ÿ�̸�

    // �ٸ������� ������ ������ �˰����� �뵵�� �׼�
    public event Action OnDay;
    public event Action OnNight;

    private void Start()
    {
        nightTimer = nightDuration; // ������ �����ϹǷ� Ÿ�̸ӵ� �ʱ�ȭ
        NightSet();
    }

    private void Update()
    {
        // ���϶� �ð��� �� �Ǹ� ������ ����
        if (CurrentState == TimeState.Night)
        {
            nightTimer -= Time.deltaTime; // �� �ð��� �帧
            if (nightTimer <= 0) DaySet(); // ���� ������ ������ ��ȯ
        }
    }

    private void NightSet()
    {
        SetTimeState(TimeState.Night);
        // ���϶� �߰��Ǵ� ���ǵ� (����, Ȱ��ȭ ������Ʈ ��)
    }

    private void DaySet()
    {
        SetTimeState(TimeState.Day);
        // ���϶� �߰��Ǵ� ���ǵ� (����, Ȱ��ȭ ������Ʈ ��)
    }

    // �� - �� ��ȯ
    private void SetTimeState(TimeState state)
    {
        if (CurrentState == state) return;

        CurrentState = state;

        if (state == TimeState.Night)
        {
            nightTimer = nightDuration;
            OnNight?.Invoke();
        }
        else
        {
            OnDay?.Invoke();
        }
    }

    // ��� ���� �׾����� - ���� �ɼ��ְ� - �� �����ϴ� ��ũ��Ʈ�ʿ� ��� ���� ������ ȣ��
    public void AllUnitDie()
    {
        if (CurrentState == TimeState.Day)
            SetTimeState(TimeState.Night);
        DaySet();
    }

    // �㿡 ���� �ڿ��Ĺ� �� �ٸ��͵��� ���������� ������
    public bool IsNight() => CurrentState == TimeState.Night;
    public float NightTimeLeft => nightTimer; // �� �ð� ��ȯ
}
