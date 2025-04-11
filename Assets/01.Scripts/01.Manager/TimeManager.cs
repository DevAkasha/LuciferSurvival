using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TimeManager : Singleton<TimeManager>
{
    protected override bool IsPersistent => false;

    public enum TimeState { Day, Night } // 시간상태
    public TimeState CurrentState { get; set; } = TimeState.Night; // 초기값은 밤

    [SerializeField] public float nightDuration = 30f;
    public float nightTimer;

    // 다른곳에서 낮인지 밤인지 알게해줄 용도
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
            nightTimer -= Time.deltaTime;
            if (nightTimer <= 0)
                SetTimeState(TimeState.Day);
        }
    }

    // 모든 적이 죽었을때 - 이건 추후 확인 필요, 밤이 될수있게
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
