# README

## 목차

---

- 프로젝트 소개
- 게임 소개
- 게임의 흐름
- 사용된 기술 스택
- 클라이언트 구조 워크 프레임
- 게임 플레이 방식

## 프로젝트 소개

---

![LUCERTitle](https://github.com/user-attachments/assets/d9d85a73-007b-4fae-afa3-831c83b34793)



- 프로젝트 이름 : LUCIFER SURVIVAL
- 프로젝트 기간 : 2025.04.04 ~ 2025.06.02 (8주)
- 개발 엔진 : Unity 2022.3.17f1
- 언어 : C#
- 장르 : #로그라이크 #디펜스 #뱀서라이크 #덱빌딩 #랜덤
- 컨셉 : #3D #캐주얼한 그래픽 #서브컬쳐 #오컬트
- 플랫폼 : 크로스플렛폼 (PC)
- 멤버 : 류영재, 허민영, 박성주, 임지환, 류은지
- 시연영상 링크 : https://youtu.be/bNhzaPrOtWU

| 류영재 | 허민영 | 박성주 | 임지환 | 류은지 |
| --- | --- | --- | --- | --- |
| 기획 | 리드개발 | 개발 | 개발 | 개발 |
| [블로그](https://9reend1tto.tistory.com/) | [블로그](https://devakasha.tistory.com/) | [블로그](https://blog.naver.com/miroon2_) | [블로그](https://g-hwan.tistory.com/) | [블로그](https://seseeeu.tistory.com/) |
| [깃](https://github.com/greenmetamong/) | [깃](https://github.com/DevAkasha) | [깃](https://github.com/ProgramCnt) | [깃](https://github.com/YataGarasu8) | [깃](https://github.com/EE-uE) |
- Git

https://github.com/DevAkasha/LuciferSurvival

## 게임 소개

---


![image](https://github.com/user-attachments/assets/ea165074-5138-4efb-be22-0fbffd1455f9)



### 한명의 타락천사, 천상의 군단에 맞서다.


🌗 **밤에는 파밍을, 낮에는 전투를**

밤이되면 루시퍼는 건물에 다가가 영혼석을 흡수할 수 있고, 낮이되면 천사군단이 몰려오면서 전투가 진행됩니다.

🎵 **로그라이크**

당신의 선택에 따라 전투가 달라집니다. 매 플레이마다 당신만의 조합을 만들어 보세요.

🕹 **뱀서류**

낮이 될 때마다 웨이브 방식으로 천사 군단이 몰려옵니다. 회피 스킬을 사용하면서 전투에서 살아남으십시오.

🔥 **덱 강화**

유닛을 업그레이드 하여 더욱 강력한 전투력을 갖춰 보십시오, 자신이 만든 타락천사와 악마들이 당신을 보호할 것 입니다.

## 게임의 흐름

---

![image](https://github.com/user-attachments/assets/9c9ab6fb-fe89-4c0e-938e-e5ad5efba0a0)


## 사용된 기술 스택

---

- 프로그래밍 언어 : C#
- 개발환경
    - 게임엔진 : Unity 2022.3.17f1 (Universal 3D)
    - 확장패키지 : UniTask, NevMash, InputSystem(new),
    - IDE : VisualStudio 2022
    - 운영체제 : Window
    - 버전관리 : Git, Github
    - 데이터관리 :  구글시트(자체프레임워크)

## 클라이언트 구조 워크 프레임

---

- 워크 프레임

![image](https://github.com/user-attachments/assets/b4522249-f3cb-48a7-afa1-d4d407b717e9)


- 클래스 구조

![image](https://github.com/user-attachments/assets/97e3a118-9a9c-40b0-ad8b-d89ed91fe5b0)


## 게임 플레이 방식

---

- 조작법

| 조작법 | 이동 | 스킬 | 상호작용 | 시야 |
| --- | --- | --- | --- | --- |
| 키보드 | W A S D (상하좌우) | SPACE, H, J, K | E | 마우스 휠 |
| 버튼 | 조이스틱 | 우측 하단 버튼 | 우측 하단 버튼 |  |

- 로비

![image](https://github.com/user-attachments/assets/f6f6fe32-90a8-4934-bd99-8456587fc692)


- 게임시작
    - 스테이지 선택
    
    게임시작 전 스테이지를 선택 가능합니다.
    
    ![image](https://github.com/user-attachments/assets/8911a144-9ebc-417c-9d0d-f9dff288e088)

    
- 도감창
    
    유닛에 대해 볼수 있습니다.
    
- 설정
    
    게임 설정(배경음, 효과음, 투명도 등) 조절이 가능합니다.
    
- 게임 종료

- 튜토리얼 진행

게임 최초 진행 시 게임 조작에 대해 간단하게 안내해주는 튜토리얼이 실행됩니다

![image](https://github.com/user-attachments/assets/e06bcefd-fd12-4fea-a697-3bfa98931d34)


- 게임 씬

![image](https://github.com/user-attachments/assets/9a0b565a-db1d-4d5d-98b6-4a0c57f5f1a1)


밤동안은 파밍할 수 있으며 시간이 지나면 낮으로 전환됩니다.

파밍이나 유닛 배치가 끝났다면 밤 스킵 버튼을 통해 스킵이 가능합니다.
