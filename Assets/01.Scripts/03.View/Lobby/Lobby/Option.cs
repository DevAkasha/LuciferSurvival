using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Option : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;

    private void Start()
    {
        // 초기화: 현재 볼륨을 슬라이더에 반영
        bgmSlider.value = AudioManager.Instance.GetBGMVolume();

        // 슬라이더 값이 바뀔 때마다 AudioManager에 바로 전달
        bgmSlider.onValueChanged.AddListener(AudioManager.Instance.SetBGMVolume);
    }
}
