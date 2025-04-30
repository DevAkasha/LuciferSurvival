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

