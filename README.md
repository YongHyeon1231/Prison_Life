# Prison Life

## 👋 프로젝트 소개

- **게임명:** Prison Life 모작
- **개발 기간:** 2025.05.07 ~ 2026.05.11
- **게임 장르:** 모바일 캐주얼 / 하이퍼캐 캐주얼 / 아이들 시뮬레이션
- **프로젝트 브로셔:** [브로셔 보기](https://www.notion.so/Prison-Life-364b586bfd0f8005a268d25693a0a8a7?source=copy_link)
- **프로젝트 소개:** 플레이어가 광산에서 암석을 채굴하고 기계를 통해 삽으로 가공한 뒤, 게스트에게 보급하고 캠프를 성장시키는 Unity 기반 모바일 아이들 시뮬레이션입니다. 조이스틱 입력, NavMesh 기반 Worker AI, 애니메이션 Fill-bar 업그레이드 시스템, 컷씬 연출을 포함합니다.
- **프로젝트 목표:** 채굴 → 가공 → 보급 → 캠프 확장으로 이어지는 핵심 게임 루프를 구현하고, SOLID 원칙 기반 모듈화 아키텍처를 설계·적용하는 것을 목표로 합니다. 반복 로직은 추상 베이스 클래스와 제네릭 상호작용 시스템으로 통합했으며, 모든 씬 오브젝트 참조는 Manager 계층을 통해 관리합니다.

---

## 🛠️ 기술 스택

<b>Language</b><br>
<img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white" alt="C#"/>
<br>
<b>Engine</b><br>
<img src="https://img.shields.io/badge/Unity-000000?style=for-the-badge&logo=unity&logoColor=white" alt="Unity"/>
<br>
<b>AI / Navigation</b><br>
<img src="https://img.shields.io/badge/NavMesh-FF6C00?style=for-the-badge&logo=unity&logoColor=white" alt="NavMesh"/>
<br>
<b>Animation</b><br>
<img src="https://img.shields.io/badge/DOTween-E74C3C?style=for-the-badge&logo=unity&logoColor=white" alt="DOTween"/>
<br>
<b>VCS</b><br>
<img src="https://img.shields.io/badge/GitHub-181717?style=for-the-badge&logo=github&logoColor=white" alt="GitHub"/>

---

<details>
<summary>✅ 구현된 기능 전체 목록</summary>

### 플레이어
- 조이스틱 기반 이동 (45도 회전 보정), 지면 Y 고정
- 무기 레벨(1~3)에 따른 장비 교체, 채굴 구역 진입/이탈 시 자동 장착/해제
- 암석 채굴 — 무기 Collider 충돌 시 애니메이션 이벤트 없이 즉시 타격 판정
- 채굴 완료 시 Rock 아이템 스택 적재 (최대치 초과 시 UI Max 표시)
- 트레이(Tray) 보유 시 Serving 애니메이션 상태로 전환
- 플레이어 잠금(SetLocked) — 컷씬 재생 중 이동 차단
- ESC 키로 앱 종료 (.exe 빌드)

### 인벤토리 / 자원 관리
- InventoryManager — Rock · Spade · Star 수량 Dictionary 관리, 이벤트 발행
- ResourceManager — Rock · Spade 카운트 동기화 (기계 출력 연동)
- PlayerInventoryStack — 등 뒤에 아이템을 시각적으로 쌓는 애니메이션 스택
- TrayController — 트레이 위 아이템 적재/인출, MeshRenderer 자동 토글

### 채굴 시스템
- Rock — SetActive 기반 비파괴 채굴, TryHit(requiredHits) 다단 타격 지원
- RockPool — 씬 내 전체 Rock 자동 수집, 채굴 후 딜레이 리스폰
- WeaponInteraction — Rock 오브젝트에 부착, 무기 Collider 충돌 감지
- MachineStandInput — 플레이어가 스탠드 진입 시 등 뒤 스택 → PutDownTheRockPile 자동 이전

### 기계 시스템
- MachineController (추상) — Idle ↔ Running 상태 루프, 플레이어 존재 감지
- Spade_MachineController — Rock 1개 소비 → DOTween 소멸 → Spade 출력, 효과음 연동
- WorkGround — 플레이어 트리거 진입/이탈 시 채굴 구역 상태 관리

### Worker AI
- MiningWorkerController — NavMesh 패트롤(A↔B), Rock 감지 시 자동 채굴, MiningArea에서 줄 배정
- WorkerCounterController — Supply → InfoDesk → Monitor 순환, 트레이 수령/투하, 모니터 감시 후 재출발
- MiningArea — 패트롤 줄(PatrolLane) 배열 관리, Worker 소환 순서대로 줄 배정

### 게스트 시스템
- GuestManager — 주기적 스폰(NavMesh 위치 보정), 대기열 관리, 서빙, 별 보상(수량 × 7)
- GuestController — NavMesh 이동, 가중 랜덤 주문 수량(1~5개), 말풍선·삽·NoSpace 빌보드 토글
- GuestInteraction — 게스트 전용 트리거 감지 (GuestController 타입)

### 캠프 시스템
- CampController — 수용 슬롯 관리, 오버플로우(대기) 처리, 업그레이드(12→24) 후 슬롯 확장
- CampFloorZone — 캠프 바닥 트리거, 진입/이탈 시 CampController 알림
- 수용 초과 시 캠프 만석 컷씬 자동 재생 → 업그레이드 UI 등장

### 별(Star) 수집 시스템
- StarPile — AnimatedPile 기반, GuestManager 서빙 완료 시 별 적재
- GetStarInteraction — 플레이어가 별 더미에 접근하면 DOJump로 스택에 이동, 첫 수집 시 Mine Shop 오픈 컷씬 재생

### 업그레이드 UI (Fill-bar)
- UI_BaseConstructionArea (추상) — 플레이어 접촉 시 Star 소비 → 진행도 누적
- UI_FillProgressArea (추상) — fillImage 채우기 + Star 아이템 날아가는 시각 효과
- UI_ConstructionArea — 완료 시 플레이어 무기 레벨 +1, Fill 초기화(반복 사용)
- UI_PlayerLevelUpArea — 첫 완료: Mine Shop 애니메이션 + 무기 업그레이드 / 두 번째: 비활성화
- UI_CampCapacityUpgradeArea — 완료 시 캠프 수용 인원 업그레이드 + 컷씬
- UI_WorkerShopAreaBase (추상) — Worker 소환 공통 로직, 완료 후 딜레이 비활성화
- UI_WorkerPurchaseArea — Waypoints 기반 Mining Worker 다중 소환, 다음 상점 활성화
- UI_CounterWorkerShopArea — 단일 지점에 Counter Worker 소환

### 컷씬 시스템
- CutsceneController — 카메라 XZ 이동 + 복귀, 플레이어 잠금 연동
  - PlayMineShopOpen: 첫 별 수집 시 Mine Shop 방향 이동 + 오픈 애니메이션
  - PlayCampFull: 캠프 만석 시 캠프 방향 이동 + 업그레이드 UI 등장
  - PlayCampUpgrade: 업그레이드 완료 시 카메라 연출 + 오버플로우 게스트 재배치

### 튜토리얼 시스템
- TutorialGuide — 단계별 화살표 이동(LateUpdate 추적) + TutorialZone 활성/비활성
- TutorialZone — 플레이어 접촉 시 해당 단계 완료 트리거

### 상호작용 시스템
- GenericInteraction\<T\> (추상) — OnInteraction · OnEntered · OnExited 이벤트, 주기적 인터랙션 코루틴
- PlayerInteraction — 플레이어 전용 트리거
- GuestInteraction — 게스트 전용 트리거
- CounterInteraction — Counter Worker 전용 트리거

### UI / HUD
- UI_JoystickManager — 터치/마우스 감지, 유휴 타임아웃(10초) 후 JoystickIdle 복귀, 컷씬 중 차단
- UI_Joystick — 조이스틱 입력 처리, JoystickDir 벡터 GameManager에 전달
- UI_StarText — Star 수량 실시간 텍스트 갱신
- UI_OrderBubble — 게스트 주문 수량 말풍선
- CompassArrow — 카메라 방향 기준 나침반 표시
- JoystickBlocker — 특정 UI 활성 시 조이스틱 입력 차단
- UI_SoundToggle — 사운드 On/Off 토글
- UI_LinkButton — 외부 URL 연결 버튼

### 사운드 시스템
- SoundManager — 효과음 10종 (GetStar, GuestGetItem, ItemPutDown, Mine1, Mine2, OpenAD, PotDownRock, GetOnSpade, Purchase, MachineSound)

</details>

---

## 📌 주요 기능

- **플레이어** — 조이스틱 이동, 무기 레벨 채굴, 트레이 배달, 컷씬 중 잠금
- **채굴 시스템** — 다단 타격·리스폰 Rock, 스탠드 자동 이전, 기계 Rock→Spade 가공
- **Worker AI** — Mining Worker NavMesh 패트롤 채굴, Counter Worker Supply→InfoDesk 순환
- **게스트 시스템** — 자동 스폰·대기열·서빙, 별 보상, 캠프 배정
- **캠프 시스템** — 수용 슬롯 관리, 오버플로우, 업그레이드 확장
- **별 수집 & 업그레이드** — Star 소비 Fill-bar, 무기/캠프/Worker 업그레이드
- **컷씬** — 카메라 이동 연출, 플레이어 잠금, 단계별 이벤트 트리거
- **튜토리얼** — 화살표 추적 단계 안내, 구역 진입으로 단계 완료
- **UI / HUD** — 조이스틱, 별 카운터, 주문 말풍선, 나침반, 사운드 토글

---

## ⚙️ 아키텍처

```
[Define Layer]
  Define.cs — InventoryItemType / EState / EGuestState / ResourceType / WorkerType / SoundType
              Animator.StringToHash 상수 중앙 관리

[Manager Layer]
  GameManager (Singleton)
  ├── SoundManager       — 효과음 재생
  ├── InventoryManager   — Rock · Spade · Star 수량 관리
  ├── ResourceManager    — Rock · Spade 카운트 동기화
  └── InteractionManager — 씬 내 Interaction · Waypoint 참조 허브

[Gameplay Layer]
  BaseCharacterController (추상)
  ├── PlayerController   — 조이스틱 이동, 채굴, 트레이 배달
  ├── GuestController    — NavMesh 대기열 이동, 주문/서빙
  ├── MiningWorkerController  — NavMesh 패트롤, Rock 자동 채굴
  └── WorkerCounterController — Supply→InfoDesk→Monitor 순환 AI

  AnimatedStackBase (추상)
  ├── PlayerInventoryStack — 등 뒤 아이템 스택 (Rock / Star)
  └── TrayController       — 트레이 위 Spade 스택

  PileBase (추상)
  └── AnimatedPile (추상)
      ├── StarPile             — 별 보상 더미
      ├── SupplyStack          — Spade 공급 더미
      ├── PutDownTheRockPile   — Rock 투하 더미
      └── TrayToItemPlacePile  — InfoDesk 아이템 더미

  MachineController (추상)
  └── Spade_MachineController — Rock → Spade 가공 기계

[Interaction Layer]
  GenericInteraction<T> (추상)
  ├── PlayerInteraction   — 플레이어 전용 트리거
  ├── GuestInteraction    — 게스트 전용 트리거
  └── CounterInteraction  — Counter Worker 전용 트리거

[UI Layer]
  UI_BaseConstructionArea (추상)
  └── UI_FillProgressArea (추상)
      ├── UI_ConstructionArea
      ├── UI_PlayerLevelUpArea
      ├── UI_CampCapacityUpgradeArea
      └── UI_WorkerShopAreaBase (추상)
          ├── UI_WorkerPurchaseArea
          └── UI_CounterWorkerShopArea
```

---

## 📁 디렉토리 구조

<details>
<summary>📂 Assets/@Scripts</summary>

```
@Scripts/
├── Manager/
│   ├── GameManager.cs           — 싱글턴 진입점, Worker 프리팹 관리
│   ├── SoundManager.cs          — 효과음 10종 재생
│   ├── InteractionManager.cs    — 씬 내 Interaction · Waypoint 참조 허브
│   ├── InventoryManager.cs      — Rock · Spade · Star 수량 관리
│   └── ResourceManager.cs       — Rock · Spade 카운트 이벤트
│
├── Character/
│   ├── Base/
│   │   ├── BaseCharacterController.cs  — 이동속도 · 회전속도 · Animator 공통
│   │   └── AnimatedStackBase.cs        — DOTween 곡선 스택 베이스
│   ├── Player/
│   │   ├── PlayerController.cs         — 이동 · 채굴 · 상태머신 · 무기관리
│   │   ├── PlayerInventoryStack.cs     — 등 뒤 Rock · Star 스택
│   │   └── TrayController.cs           — 트레이 위 Spade 스택
│   ├── Guest/
│   │   ├── GuestController.cs          — NavMesh 대기열 이동, 주문/서빙
│   │   └── GuestManager.cs             — 스폰 · 대기열 · 서빙 · 별 보상
│   └── Worker/
│       ├── MiningWorkerController.cs   — NavMesh 패트롤, Rock 자동 채굴
│       ├── WorkerCounterController.cs  — Supply→InfoDesk→Monitor 순환 AI
│       └── MiningArea.cs               — 패트롤 줄 배정 관리
│
├── Mining/
│   ├── PileBase.cs              — 격자 쌓기 베이스
│   ├── AnimatedPile.cs          — DOJump 애니메이션 파일 베이스
│   ├── MachineController.cs     — Idle↔Running 상태 루프 추상 베이스
│   ├── Spade_MachineController.cs — Rock → Spade 가공 기계
│   ├── MachineStandInput.cs     — 플레이어 접근 시 Rock 자동 이전
│   ├── Rock.cs                  — 다단 타격, SetActive 비파괴, 리스폰
│   ├── RockPool.cs              — 전체 Rock 풀 관리, 딜레이 리스폰
│   ├── WeaponInteraction.cs     — 무기 Collider 충돌 채굴 감지
│   ├── MineShopController.cs    — Animation Event로 UI 활성화
│   ├── PutDownTheRockPile.cs    — Rock 투하 더미
│   └── TrayToItemPlacePile.cs   — InfoDesk 아이템 더미
│
├── Camp/
│   ├── CampController.cs        — 슬롯 관리, 오버플로우, 업그레이드
│   ├── CampFloorZone.cs         — 캠프 바닥 트리거
│   ├── Counter.cs               — InfoDesk 플레이어 인터랙션
│   ├── StarPile.cs              — 별 보상 더미
│   ├── SupplyStack.cs           — Spade 공급 더미
│   └── GetStarInteraction.cs    — 별 수집 + Mine Shop 오픈 컷씬
│
├── Interaction/
│   ├── GenericInteraction.cs    — 제네릭 트리거 베이스
│   ├── PlayerInteraction.cs     — 플레이어 전용
│   ├── GuestInteraction.cs      — 게스트 전용
│   └── CounterInteraction.cs    — Counter Worker 전용
│
├── UI/
│   ├── UI_BaseConstructionArea.cs   — Star 소비 Fill-bar 추상 베이스
│   ├── UI_FillProgressArea.cs       — fillImage + 날아가는 아이템 시각 효과
│   ├── UI_ConstructionArea.cs       — 무기 레벨 업그레이드 (반복)
│   ├── UI_PlayerLevelUpArea.cs      — 2단계 레벨업 + Mine Shop 연출
│   ├── UI_CampCapacityUpgradeArea.cs — 캠프 수용 업그레이드
│   ├── UI_WorkerShopAreaBase.cs     — Worker 소환 공통 추상 베이스
│   ├── UI_WorkerPurchaseArea.cs     — Mining Worker 다중 소환
│   ├── UI_CounterWorkerShopArea.cs  — Counter Worker 단일 소환
│   ├── UI_JoystickManager.cs        — 터치/마우스 감지, 유휴 타임아웃
│   ├── UI_Joystick.cs               — 조이스틱 입력 처리
│   ├── UI_OrderBubble.cs            — 게스트 주문 말풍선
│   ├── UI_StarText.cs               — 별 수량 텍스트
│   ├── UI_Handler.cs                — UI 패널 공통 핸들러
│   ├── UI_SoundToggle.cs            — 사운드 On/Off 토글
│   ├── UI_LinkButton.cs             — 외부 URL 버튼
│   ├── CompassArrow.cs              — 나침반 방향 표시
│   └── JoystickBlocker.cs           — 특정 UI 활성 시 조이스틱 차단
│
├── Camera/
│   └── CameraController.cs      — Lerp 추적, DOTween XZ 이동
│
├── Cutscene/
│   └── CutsceneController.cs    — Mine Shop · Camp Full · Camp Upgrade 컷씬
│
├── Tutorial/
│   ├── TutorialGuide.cs         — 단계별 화살표 추적 안내
│   └── TutorialZone.cs          — 진입 시 단계 완료 트리거
│
└── Utils/
    ├── Define.cs                — 전체 enum · Animator 해시 상수 중앙 관리
    ├── Singleton.cs             — 제네릭 싱글턴 베이스
    ├── Waypoints.cs             — 자식 Transform 경유지 관리
    ├── Billboard.cs             — 카메라 방향으로 항상 회전
    ├── ResourceVisualMover.cs   — 아이템 날아가기 DOTween 유틸
    ├── Utils.cs                 — GetOrAddComponent / FindChild 유틸
    └── AppQuit.cs               — ESC 키 앱 종료
```

</details>

---

## 🔗 핵심 게임 루프 흐름

<details>
<summary>채굴 → 가공 → 공급 흐름</summary>

```
[플레이어 채굴]
PlayerController.EnterWorkGround() → 무기 장착
WeaponInteraction.OnTriggerEnter() → Rock.TryHit()
  └─ [requiredHits 달성] → Rock.Mine() → SetActive(false)
                           RockPool.ReturnRock() → 딜레이 후 Respawn()
  └─ PlayerController.OnRockMined() → PlayerInventoryStack.AddItem()

[스탠드 투하]
MachineStandInput.OnTransferRock()
  → PlayerInventoryStack.TakeItem() → PutDownTheRockPile.ReceiveItem()
  → ResourceManager.SetRockCount(count)

[기계 가공]
Spade_MachineController.CoRunCycle()
  → PutDownTheRockPile.RemoveFromPile() (Rock 소멸 DOTween)
  → SupplyStack.AddItem() (Spade 출력)
  → ResourceManager.SetSpadeCount(count)
```

</details>

<details>
<summary>Counter Worker 순환 흐름</summary>

```
WorkerCounterController
  GoToSupply → SupplyStack 트리거 진입
    └─ HandleSupplyInteraction() → TrayController.ReceiveItem(Spade)
       [Tray 가득 차면] → GoToInfoDesk

  GoToInfoDesk → TrayToItemPlacePile 트리거 진입
    └─ HandleInfoDeskInteraction() → TrayController.TakeItem() → Pile에 투하
       [Tray 비면] → GoToMonitor

  WatchMonitor → Pile 수량 변화 감시
    └─ [idleThreshold 초과] → GoToSupply (순환 재시작)
```

</details>

<details>
<summary>게스트 서빙 → 캠프 배정 흐름</summary>

```
GuestManager.CoSpawnGuest()
  → NavMesh 위치 보정 → GuestController 스폰 → 대기열 뒤에 배치

GuestManager.CheckFrontArrival()
  → [맨 앞 도착 + 주문 없음] → GuestController.ShowOrder() (가중 랜덤 1~5)

GuestManager.TryServe()
  → [Pile 수량 ≥ 주문 수 + 캠프 여유] → CoServe()
     → DOJump 아이템 이동 → GuestController.OnServed()
     → StarPile.AddItem() × (count × 7)
     → [캠프 여유] → WalkToWaitPoint
     → [캠프 만석] → CampController.TryRouteToOverflow()

CampController.OnGuestEnteredFloor()
  → AssignSpot() → WalkToWaitPoint (슬롯 위치)
  → [GuestCount == maxCapacity] → CutsceneController.PlayCampFull()
       └─ 카메라 이동 → CampUpgradeArea 활성화 → 카메라 복귀
```

</details>

<details>
<summary>별 수집 → 업그레이드 흐름</summary>

```
GetStarInteraction.OnPickup()
  → StarPile.RemoveFromPile() → DOJump → PlayerInventoryStack.AddToStack()
  → [최초 수집] → CutsceneController.PlayMineShopOpen()

UI_FillProgressArea.OnPlayerTick()
  → InventoryManager.GetResourceCount(Star) > 0
  → PlayerController.TakeResourceItem(Star)
  → ResourceVisualMover.FlyToTarget() + fillImage.fillAmount 갱신
  → [progress >= 1] → OnComplete()
       ├── UI_ConstructionArea       : UpgradeWeaponLevel() + ResetFill()
       ├── UI_PlayerLevelUpArea      : MineShop 애니메이션 or 비활성화
       ├── UI_CampCapacityUpgradeArea: CampController.Upgrade() + 컷씬
       └── UI_WorkerShopAreaBase     : SpawnWorkers() + 딜레이 비활성화
```

</details>

---

## 👩‍💻 개발자

| 이름   | GitHub |
|--------|--------|
| 박용현 | [YongHyeon1231](https://github.com/YongHyeon1231/) |

---

## 🔧 아쉬운 점 및 개선 방향

<details>
<summary>1. 저장/불러오기 시스템 부재</summary>

**현재 문제**
게임을 종료하면 인벤토리 수량, 업그레이드 단계, 스폰된 Worker 수 등 모든 진행 상황이 초기화된다. 아이들 장르의 핵심인 "진행 축적"이 동작하지 않는다.

**개선 방안**
- JSON 직렬화 + `Application.persistentDataPath` 로 저장
- 저장 대상: 업그레이드 단계, Worker 수, 캠프 수용 인원, 튜토리얼 완료 여부
- `SaveManager` 단일 클래스에서 저장/불러오기 중앙 관리

</details>

<details>
<summary>2. Worker AI 상태 관리 비구조화</summary>

**현재 문제**
`WorkerCounterController`의 상태 전환이 `Update()` 내 `switch-case`로 처리된다. 상태가 추가될수록 분기가 중첩되어 유지보수성이 낮아진다.

**개선 방안**
- `IState` 인터페이스 + 상태별 클래스 분리로 FSM 구조화
- Unity Animator의 `StateMachineBehaviour`와 게임 로직 상태를 일치시켜 애니메이션 전환과 동기화

</details>

<details>
<summary>3. 오브젝트 풀링 미적용</summary>

**현재 문제**
게스트 스폰, 별 아이템, Spade 아이템 등이 `Instantiate`/`Destroy` 로 처리된다. 게스트와 아이템이 빈번하게 생성·소멸되면 GC 호출이 증가해 모바일에서 프레임 드롭이 발생할 수 있다.

**개선 방안**
- Unity `ObjectPool<T>` 또는 커스텀 풀 구현
- GuestController, 별 아이템, Spade 아이템을 우선 대상으로 풀링 적용

</details>

<details>
<summary>4. 캠프 업그레이드 수치 하드코딩</summary>

**현재 문제**
`CampController.Upgrade()` 내 업그레이드 티어 수치(`{ 12, 24 }`)가 코드에 직접 박혀 있다. 추가 티어 확장 시 코드를 직접 수정해야 한다.

**개선 방안**
- 업그레이드 티어를 `ScriptableObject`로 분리하거나 `SerializeField int[]`로 Inspector 노출
- `CampUpgradeTierEffect` 리스트와 티어 수치를 하나의 데이터 클래스로 통합

</details>

<details>
<summary>5. 게스트 서빙 중단 조건 단순화</summary>

**현재 문제**
`TryServe()`가 캠프 만석 + 오버플로우 만석을 동시에 확인하지만, 오버플로우가 없는 씬에서도 `IsOverflowFull`이 `true`를 반환하도록 설계되어 있어 조건이 직관적이지 않다.

**개선 방안**
- 캠프 수용 가능 여부를 `CampController.CanAcceptGuest()` 단일 메서드로 캡슐화
- 오버플로우 존재 여부에 따라 내부에서 분기 처리

</details>

<details>
<summary>6. 컷씬 중 일부 상호작용 트리거 미차단</summary>

**현재 문제**
컷씬 재생 중 플레이어 이동은 `SetLocked`로 차단되지만, 트리거 구역(Fill-bar, 별 수집)은 여전히 작동한다. 컷씬 도중 의도치 않은 업그레이드가 시작될 수 있다.

**개선 방안**
- `JoystickBlocker`처럼 컷씬 진행 중 `InteractionManager`를 통해 전체 PlayerInteraction을 일시 비활성화
- 또는 `UI_BaseConstructionArea`에 플레이어 잠금 여부 체크 추가

</details>

<details>
<summary>7. SOLID 원칙 일부 미준수</summary>

**현재 문제**
- **ISP 위반:** `InteractionManager`가 Player · Guest · Worker · Waypoint 등 모든 씬 참조를 하나의 클래스에 집중시켜, 필요 없는 참조까지 의존하게 된다.
- **DIP 위반:** 일부 컴포넌트가 `GameManager.Instance.Player`처럼 구체 클래스에 직접 접근한다.

**개선 방안**
- `InteractionManager`를 역할별로 분리하거나 인터페이스로 추상화
- 핵심 시스템에 `IDamageable`, `IInteractable` 등 인터페이스 정의 후 구체 타입 대신 인터페이스에 의존

</details>
