using System;
using System.Collections;
using System.Collections.Generic;

public interface ITrackableRxModel
{
    void RegisterRx(RxBase rx); // Rx 필드를 모델에 등록
}

public interface IModifiableTarget
{
    IEnumerable<IModifiable> GetModifiables(); // 수정 가능한 필드 목록 반환
}

public interface IRxReadable<T>
{
    T Value { get; }
    void AddListener(Action<T> listener); // 값 변경을 구독할 수 있음
    void RemoveListener(Action<T> listener); // 구독 해제
}
public interface IRxField
{
    string FieldName { get; }
}

public interface IRxField<T> : IRxField, IRxReadable<T> { }

public interface IConditionCheckable
{
    bool Satisfies(Func<object, bool> predicate);
}


