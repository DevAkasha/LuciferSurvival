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
        OnNight?.Invoke();
    }

    private void Update()
    {
        // ���϶� �ð��� �� �Ǹ� ������ ����
        if (CurrentState == TimeState.Night)
        {
            // �� - �ð� = 0�̵ɶ� ������ ��ȯ
            nightTimer -= Time.deltaTime;
            if (nightTimer <= 0)
                SetTimeState(TimeState.Day);
        }
    }

    // ��� ���� �׾����� - ���� �ɼ��ְ� - �� �����ϴ� ��ũ��Ʈ�ʿ� ��� ���� ������ ȣ��
    public void AllUnitDie()
    {
        if (CurrentState == TimeState.Day)
            SetTimeState(TimeState.Night);
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

    // �㿡 ���� �ڿ��Ĺ� �� �ٸ��͵��� ���������� ������
    public bool IsNight() => CurrentState == TimeState.Night;
}
