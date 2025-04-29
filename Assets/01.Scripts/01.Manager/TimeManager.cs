using System;
using UnityEngine;

public class TimeManager : Singleton<TimeManager>
{
    // 시간 상태를 나타내는 열거형
    public enum TimeState
    {
        Day,
        Night
    }

    // 현재 시간 상태 (초기값: 밤) : 이부분은 다른곳에서 초기값 세팅이 있을 경우 제외하기
    [SerializeField] private TimeState currentTimeState = TimeState.Night;

    // 시간 상태 변화 이벤트 (다른 시스템에서 구독할 수 있음)
    public event Action<TimeState> OnTimeState;

    void Start()
    {
        // 초기 시간 상태 이벤트 발생
        NotifyChangeTime();
    }

    // 낮으로 전환하는 메서드
    public void SetDay()
    {
        if (currentTimeState != TimeState.Day)
        {
            currentTimeState = TimeState.Day;
            NotifyChangeTime();
        }
    }

    // 밤으로 전환하는 메서드
    public void SetNight()
    {
        if (currentTimeState != TimeState.Night)
        {
            currentTimeState = TimeState.Night;
            NotifyChangeTime();
        }
    }

    // 시간 상태 전환 메서드 (낮->밤, 밤->낮)
    public void ToggleTimeState()
    {
        // 상태 전환
        currentTimeState = (currentTimeState == TimeState.Day) ? TimeState.Night : TimeState.Day;

        // 변경된 상태 알림
        NotifyChangeTime();
    }

    // 시간 상태 변경 알림 메서드
    private void NotifyChangeTime()
    {
        // 이벤트가 있는 경우에만 호출
        OnTimeState?.Invoke(currentTimeState);
    }

    // 현재 시간 상태 확인 메서드
    public TimeState GetCurrentTimeState()
    {
        return currentTimeState;
    }

    // 현재 상태가 낮인지 확인하는 메서드
    public bool IsDay()
    {
        return currentTimeState == TimeState.Day;
    }

    // 현재 상태가 밤인지 확인하는 메서드
    public bool IsNight()
    {
        return currentTimeState == TimeState.Night;
    }

    // 시간 상태를 직접 설정하는 메서드
    public void SetTimeState(TimeState newState)
    {
        if (currentTimeState != newState)
        {
            currentTimeState = newState;
            NotifyChangeTime();
        }
    }
}
