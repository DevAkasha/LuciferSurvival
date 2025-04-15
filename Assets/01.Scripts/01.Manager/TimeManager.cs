using System;
using UnityEngine;

public class TimeManager : Singleton<TimeManager>
{
    protected override bool IsPersistent => false;

    public enum TimeState { Day, Night } // 시간상태
    public TimeState CurrentState { get; set; } = TimeState.Night; // 초기값은 밤

    public float nightDuration = 30f; // 밤의 시간
    public float nightTimer; // 밤의 시간을 흐르게 위한 타이머

    // 다른곳에서 낮인지 밤인지 알게해줄 용도의 액션
    public event Action OnDay;
    public event Action OnNight;

    private void Start()
    {
        nightTimer = nightDuration; // 밤으로 시작하므로 타이머도 초기화
        OnNight?.Invoke();
    }

    private void Update()
    {
        // 밤일때 시간이 다 되면 낮으로 변경
        if (CurrentState == TimeState.Night)
        {
            // 밤 - 시간 = 0이될때 낮으로 전환
            nightTimer -= Time.deltaTime;
            if (nightTimer <= 0)
                SetTimeState(TimeState.Day);
        }
    }

    // 모든 적이 죽었을때 - 밤이 될수있게 - 적 관리하는 스크립트쪽에 모든 적이 없을때 호출
    public void AllUnitDie()
    {
        if (CurrentState == TimeState.Day)
            SetTimeState(TimeState.Night);
    }

    // 낮 - 밤 전환
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

    // 밤에 보통 자원파밍 및 다른것들이 가능해지기 때문에
    public bool IsNight() => CurrentState == TimeState.Night;
}
