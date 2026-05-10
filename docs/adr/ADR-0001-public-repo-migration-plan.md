---
adr: "0001"
title: "개인 레포 → 회사 org public repo 이관 플랜"
status: "executed"
date: "2026-05-07"
deciders:
  - user (사용자 확정)
  - design-advisor (설계)
  - documentation-advisor (문서화)
source_session: ".claude/work-session/20260507-101342/"
supersedes: null
superseded_by: null
execution_log:
  - session: "20260507-121142"
    date: "2026-05-07"
    scope: "라이선스 MIT 확정 / org = sundaytoz 확정 / LICENSE·AUTHORS·SECURITY.md 신규 작성 / README Credits·License 섹션 + 복사 제외 목록 확장 / TEMPLATE_DEV.md:9 org 치환"
    resolved_items: ["D4 (라이선스 TBD → MIT)", "TR1 (<org> 치환값 = sundaytoz)"]
    not_yet_executed: ["T10 (D+7 리디렉션·외부 참조 검증)"]
  - session: "20260507-124344"
    date: "2026-05-07"
    scope: "T4 git 이력 재작성 (filter-branch env-filter) + README 축소 (546→137줄) + docs/ 분기 (6개 신규 파일) + §2.4 system-reminder 규약"
    resolved_items: ["T4 (git 이력 재작성)", "D6 (README 축소)", "D9 (이력 재작성)"]
  - session: "20260507-152928"
    date: "2026-05-07"
    scope: "§4.3 집합 전수 체크 AC 패턴 + §8 planned/actual_workers 필드 + docs frontmatter description 보강"
    resolved_items: ["프로토콜 규약 강화 (§4.3, §8)"]
  - session: "20260507-161922"
    date: "2026-05-09"
    scope: "2차 민감정보 감사 + sundaytoz/agent-team-protocol push (T7 대체) + origin URL 전환 (T9) + backup 브랜치 삭제"
    resolved_items: ["T7 (mirror push to sundaytoz — transfer 불가 대체)", "T9 (origin remote URL 치환)", "2차 민감정보 게이트"]
---

# ADR-0001: 개인 레포 → 회사 org public repo 이관 플랜

> **카테고리 선택 근거**: 본 문서는 public 범위·배포 방식·이관 수단·라이선스 정책·거버넌스 파일 범위 등 10개 결정을 담은 되돌리기 어려운 아키텍처·운영 결정이다. 이관 절차·민감정보 게이트·URL 매트릭스는 모두 이 결정들의 Consequences(결과 경로)이므로 단일 ADR 로 편입한다.

---

## Context (배경)

`stzjungsoo/agent-team-protocol` 은 Claude Code 에이전트 팀 프로토콜 템플릿으로 개인 계정에서 운영되어 왔다.
회사 org public repo 로 이관하기로 결정함에 따라 다음 사항을 결정·설계해야 했다.

- 공개 범위 및 배포 방식 결정
- README 구조 개편 (533줄 → 150줄 축소 + docs/ 분기)
- 민감 정보 처리 기준 (개인 이메일·실명·내부 URL)
- 거버넌스 파일 신설 (SECURITY.md, AUTHORS, LICENSE TBD)
- git 이력 재작성 여부
- 이관 10단계 절차 및 Go/No-Go 게이트 정의

세션 전체 요구사항·조사는 `.claude/work-session/20260507-101342/requirements.md` 및 `research/findings.md` 참조.

---

## Decision (확정 결정 10건)

`report.md` Decisions 섹션에서 사용자가 확정한 결정을 단일 표로 정리한다.

| ID | 결정 내용 | 확정 시각 | 영향 요구사항 |
|---|---|---|---|
| **D1** | public 범위 = **회사 org public repo** (org-branded, 인터넷 공개) | 2026-05-07 10:25 | FR-001/002/003/007/008/010, NFR-S-001/002 |
| **D2** | 배포 방식 = **cp-R 전용**. GitHub Template 기능 미도입 | 2026-05-07 10:28 | FR-003/004, NFR-C-001 |
| **D3** | 이관 트리거 = **GitHub repo transfer** (유력, 전환 시점 재확정 허용) | 2026-05-07 10:30 | FR-005/006/009, NFR-M-001 |
| **D4** | 라이선스 = **미정 (TBD)**. README·SECURITY.md 에 `License: TBD` 명시 `[→ MIT 확정 2026-05-07 12:11, session 20260507-121142]` | 2026-05-07 10:30 | FR-007/010, NFR-A-002 |
| **D5** | 원작자 표기 = **README 크레딧 + AUTHORS 파일 + (라이선스 확정 후) LICENSE copyright** 3단 병행 | 2026-05-07 10:32 | FR-009 |
| **D6** | README 개편 = **1페이지 축소 (~150줄) + docs/ 로 §6~§15 분기**. GitHub Pages/Wiki 미도입 | 2026-05-07 10:32 | FR-001/002, NFR-U-001/002 |
| **D7** | 민감 정보 기준 = **원작자 표기 맥락에 한해 실명/handle 허용**. 사내 프로젝트명·내부 URL 제거 | 2026-05-07 10:32 | FR-008 |
| **D8** | 거버넌스 파일 = **SECURITY.md 만 신규 작성**. CONTRIBUTING/COC 는 라이선스 확정 후 재검토 | 2026-05-07 10:38 | FR-010/011, NFR-S-002 |
| **D9** | **git 이력 재작성** = 이관 **직전** 권고. `git filter-repo --mailmap` 사용. 파괴적 게이트 명시 | 2026-05-07 10:55 | FR-008, NFR-S-001 |
| **D10** | **AUTHORS = cp-R 복사 제외**. SECURITY.md 와 동일 정책. 이식자는 독자 작성 | 2026-05-07 10:55 | NFR-C-002 |

### 결정 간 상호의존

- **D4 → D5, D8**: 라이선스 미정이므로 LICENSE copyright·CONTRIBUTING 은 후행.
- **D2 → D10, D8**: cp-R 전용 배포이므로 SECURITY.md / AUTHORS / LICENSE 는 모두 복사 제외 목록 편입.
- **D9 → D3**: 이력 재작성은 GitHub transfer **이전**에만 실행 가능 (transfer 후 force-push 불가).
- **D6 → D2**: README §4(설치) 는 반드시 README 에 잔존 (cp-R 전용 배포의 5분 진입 정보).

---

## Consequences (결과 및 실행 경로)

### 1. cp-R 복사 제외 목록 (최종)

| # | 파일 | 상태 | 제외 이유 |
|---|---|---|---|
| 1 | `README.md` | 기존 제외 | 프로젝트 고유 README |
| 2 | `TEMPLATE_DEV.md` | 기존 제외 | 템플릿 백로그·이력 |
| 3 | `.claude/work-session/` | 기존 제외 | 세션 임시 산출물 |
| 4 | `SECURITY.md` | **신규 제외** | 이식자가 자체 보안 채널 직접 작성해야 함 (D8) |
| 5 | `AUTHORS` | **신규 제외** | 템플릿 원작자 표기 전파 방지 (D10) |
| 6 | `LICENSE` | **신규 제외** `[→ MIT 로 2026-05-07 생성, cp-R 제외 적용]` | 라이선스 자동 적용 방지 (D4) |

**권위 위치**: README §3(설치). TEMPLATE_DEV.md 의 표는 요약 미러 (갱신 시 README 먼저, TEMPLATE_DEV 뒤따름).

### 2. README 구조 개편 설계 (D6)

신규 README 목차 (목표 ≤150줄):

| # | 섹션 | 예상 줄 수 |
|---|---|---|
| 0 | 헤더 (제목 + 1문장 요약 + 배지 예약) | 6 |
| 1 | 왜 이 템플릿인가 (문제·해법 표) | 18 |
| 2 | 개념 — 3-tier 다이어그램 | 14 |
| 3 | 설치 (cp-R 5분) | 32 |
| 4 | 프로젝트 적응 (3군데) | 18 |
| 5 | 더 읽기 (docs/ 링크 테이블) | 16 |
| 6 | 크레딧 (원작자) | 10 |
| 7 | 보안 | 4 |
| 8 | 라이선스 (`License: TBD`) | 4 |
| 9 | 변경/백로그/기여 | 4 |
|   | **합계** | **~126** (허용 상한 160) |

현행 README §6~§15 는 docs/ 로 분기:

| 현행 섹션 | 도착 경로 | 신규/기존 |
|---|---|---|
| §6 `/task` 호출 흐름 | `docs/development/agent-team-protocol.md` | 기존 흡수 |
| §7 에이전트 카탈로그 | `docs/development/agent-catalog.md` | **신규** |
| §8~§14 검증·스키마·graphify·회고 등 | `docs/development/agent-team-protocol.md` | 기존 흡수 |
| §15 FAQ | `docs/usage/faq.md` | **신규** |
| §3 구성 파일 맵 (tree) | `docs/architecture/file-map.md` | **신규** |

신규 docs/ 파일 3건(agent-catalog.md, faq.md, file-map.md) 의 본문 작성은 이관 집행 세션의 범위다.
신규 카테고리 `docs/usage/` 도입 필요 (docs/index.md + document-category-classification.md 갱신 포함).

### 3. 민감 정보 게이트 (이관 전 필수)

이관 집행 전 S1~S12 **전 항목 PASS** 필요. Go/No-Go 조건 G5.

| 항목 | 점검 명령 | PASS 기준 |
|---|---|---|
| **S1** | `grep -rn "stzjungsoo" . --include='*.md' --exclude-dir=.git --exclude-dir=.claude/work-session` | 출력 0줄 |
| **S2** | `grep -rn "이정수" . --include='*.md' --exclude-dir=.git --exclude-dir=.claude/work-session` | 출력 0줄 |
| **S3** | `grep -rn "stz888" . --exclude-dir=.git --exclude-dir=.claude/work-session` | 출력 0줄 |
| **S4** | `grep -rn "wemadeplay\.com" . --exclude-dir=.git --exclude-dir=.claude/work-session` | 출력 0줄 (git author 는 S8) |
| **S5** | `grep -rn "/Users/wemadeplay" . --exclude-dir=.git --exclude-dir=.claude/work-session` | 출력 0줄 |
| **S6** | `grep -rnE "internal\.\|인트라넷\|사내\|정수\.local" . --exclude-dir=.git ...` | 출력 0줄 또는 수동 판정 |
| **S7** | `git ls-files .claude/work-session/` | 출력 0줄 |
| **S8** | `git log --all --format='%ae\|%an' \| sort -u` | `stz888.local`, `wemadeplay.com`, `이정수` 미포함 (D9 재작성 완료 후) |
| **S9** | `grep -n "stzjungsoo/agent-team-protocol" TEMPLATE_DEV.md` | 출력 0줄 |
| **S10** | `grep -rn "stzjungsoo/" . --include='*.md' --exclude-dir=.git ...` | 출력 0줄 |
| **S11** | `grep -n "work-session" .gitignore` | 1줄 이상 |
| **S12** | `git config --local --get user.email` (옵션) | 미설정 또는 org 정책 이메일 |

1건이라도 FAIL 시 **전 항목 재검사** (부분 재검사 금지).
S8 은 D9(이력 재작성) 선행 없이는 FAIL 예상.

### 4. git 이력 재작성 (D9) — 파괴적 조작 게이트

- 도구: `git filter-repo --mailmap .mailmap` (기본). BFG 대체, 수동 rebase 비상용.
- 치환 매핑:

| 원본 | 치환 후 |
|---|---|
| `wemadeplay <wemadeplay@stz888.local>` | `wemadeplay <wemadeplay@users.noreply.github.com>` |
| `이정수 <jungsoo.lee@wemadeplay.com>` | `<org>-maintainer <noreply@<org>.example>` |
| Co-Authored-By Claude | 유지 |

- 실행 사전조건 P1~P7 전원 충족 필수 (백업 브랜치 생성, dry-run 검증 포함).
- force-push 는 **개인 계정 remote 에만** 적용. org transfer 이후 불가.
- 실패 시 `git reset --hard backup/pre-history-rewrite` 로 복구.

### 5. 이관 10단계 D-day 체크리스트 (T1~T10)

전체 절차 상세(담당자·사전조건·완료판정·롤백 가능여부)는 세션 원본 §7 을 참조.

```
T1 (D-7)  : 조직 합의 확인, org 경로 확정
T2 (D-3)  : 민감 정보 게이트 1차 실행 (S1~S12)
T3 (D-2)  : 문자열 치환 (TEMPLATE_DEV.md:9 외)
T4 (D-1)  : git 이력 재작성 [파괴적 게이트]
T5 (D-1 야간) : 백업 재확인 + 2차 민감 정보 게이트
T6 (D-day 새벽) : 세션 잔존 확인 + 크레딧 초안 확정
T7 (D-day) : GitHub Settings → Transfer ownership [비가역]
T8 (D-day 직후) : clone 테스트 및 가시성 확인
T9 (D+1)  : README/<org> placeholder 치환 + 링크 검증
T10 (D+7) : 리디렉션 상태 확인 + 외부 참조 링크 검증
```

선후관계:
```
T1 → T2 → T3 → T4 → T5 → T6 → T7(비가역) → T8 → T9 → T10
```

파괴적 게이트 2건 (T4, T7) 은 `docs/development/agent-team-protocol.md` §10 의 승인 흐름을 따른다.

### 6. 거버넌스 파일 설계 요약 (D4/D5/D8)

**SECURITY.md** (신규, cp-R 제외):
- 섹션: Scope / Reporting / Response SLA (72h / 14d / 90d) / Disclosure Policy / License Note
- 주 채널: GitHub Security Advisory (GHSA). 보조: `security@<org>.example`
- Scope 는 "실행 코드 없는 문서·템플릿 레포" 특성 반영

**AUTHORS** (신규, cp-R 제외):
- 포맷: `- Jungsoo Lee (@stzjungsoo)` (handle 기반, 이메일 비공개)
- 파일 상단 HTML 주석: 복사 제외 의도 명시

**LICENSE** (미생성) `[→ 2026-05-07 MIT 로 생성, cp-R 제외 적용, session 20260507-121142]`:
- 라이선스 미결정 상태에서는 파일 생성하지 않음
- README·SECURITY.md 에 `License: TBD` 표기만

### 7. URL / 문자열 치환 매트릭스 요약 (전수 18항목)

| 시점 분류 | 항목 수 | 핵심 항목 |
|---|---|---|
| 이관 전 (D-2~D-1) | 2건 | U1 (TEMPLATE_DEV.md:9), U14 (git author) |
| 이관 직후 (D-day~D+1) | 5건 | U2~U5, U11 (README·SECURITY·Credits의 `<org>` 치환) |
| 라이선스 확정 후 | 2건 | U9 (README License 섹션), U10 (LICENSE 파일) |
| 담당자 지정 후 | 2건 | U6 (보안 이메일), U13 (Maintainer handle) |
| README 축소 세션 | 3건 | U15 (앵커 링크), U16 (docs/index.md), U17 (분류 기준) |
| 이관 후 D+7 | 1건 | U18 (외부 노트·블로그) |

전수 상세는 세션 원본 §8.1 참조.

### 8. 이식자 실수 카탈로그 (M1~M8)

이식자가 흔히 마주치는 실수 8건. 본문은 이관 집행 세션에서 `docs/usage/troubleshooting.md` 에 게시.

| # | 실수 유형 | 핵심 대응 |
|---|---|---|
| M1 | CLAUDE.md 플레이스홀더 방치 | 6종 grep 으로 확인 후 치환 |
| M2 | docs 카테고리 오분류 | document-category-classification.md 재확인 |
| M3 | 에이전트 tools 필드 누락 | frontmatter `tools:` 추가 |
| M4 | 세션 디렉토리 경로 불일치 | `.claude/work-session/<YYYYMMDD-HHMMSS>/` 고정 |
| M5 | cp-R 복사 제외 위반 | §4.2.2 안전망 검증 명령 실행 |
| M6 | 세션 ID 충돌 오해 | 프로젝트 루트 기준 상대경로이므로 실제 충돌 없음 |
| M7 | `License: TBD` 를 자유 사용으로 오해 | README License 섹션 주의 문구 + FAQ 항목 |
| M8 | `docs/feedback/inbox/` 경로 부재 | `mkdir -p docs/feedback/inbox` 실행 |

### 9. 이관 후 정기 점검 루틴 (NFR-M-002)

- **주기**: 분기 1회 (A.민감정보 재검사 / B.링크 깨짐 / C.maintainer 최신화 / D.백로그 처리율)
- **이관 후 180일**: 1회 회고 세션 (`docs/development/agent-team-protocol.md` §12 경로 사용)
- **결과 기록**: GitHub Discussion (category: `Maintenance / Quarterly Review`)

---

## Alternatives Considered (검토한 대안)

| 결정 | 검토한 대안 | 미채택 이유 |
|---|---|---|
| D2: cp-R 전용 | GitHub Template 기능 도입 | 현행 README §4 절차 유지 선택. Template 버튼 추가 UI 비용 대비 이점 미미 |
| D3: GitHub transfer | 신규 org repo 생성 후 코드만 복사 | 이슈·PR·이력 손실. transfer 가 비용 낮음 |
| D8: SECURITY.md 만 | CONTRIBUTING.md, COC 동시 작성 | 라이선스 미정 상태에서 "기여=라이선스 동의" 조항 공백. 초기 관리 부담 최소화 |
| D9: filter-repo | 수동 rebase, BFG | filter-repo 가 재현 가능성 최고. BFG 는 지원 종료 흐름 |
| D9 포맷: handle 기반 | Unix 표준 1줄 (이메일 포함) | 이메일 공개 최소화 (NFR-S-001) |

---

## Go / No-Go 게이트 (이관 집행 세션 진입 조건)

| 조건 | 판정 방법 |
|---|---|
| **G1** 본 플랜(ADR-0001) 검토 완료 | documentation-advisor 확인 |
| **G2** 사용자 최종 승인 | report.md Decisions 에 "플랜 승인" 기록 |
| **G3** org 이름 확정 (`<org>` 치환값 결정) | 사용자 입력 수신 |
| **G4** 이력 재작성 최종 결정 + 사전 검증 완료 | 로컬 dry-run 로그 |
| **G5** 민감 정보 게이트(S1~S12) 전 항목 PASS | verification-advisor 체크리스트 |

**이관 집행 세션 진입 트리거 3건** (추가 미확정 항목):

| # | 트리거 |
|---|---|
| TR1 | `<org>` placeholder 치환값 확정 — **해소됨**: `sundaytoz` (2026-05-07, session 20260507-121142) |
| TR2 | 이력 재작성 최종 YES |
| TR3 | 이관 담당자 지정 (maintainer handle 또는 team) |

---

## 이관 집행 세션 안내

본 ADR 은 **계획 문서**다. 실제 이관 집행은 별도 `/task` 세션에서 수행한다.

집행 세션 진입 명령:
```
/task "ADR-0001 §7 의 T1~T10 이관 절차 집행"
```

집행 세션의 orchestrator 는 아래 순서로 문서를 읽는다:
1. `CLAUDE.md` → `docs/index.md` → `docs/development/agent-team-protocol.md`
2. 본 ADR (`docs/adr/ADR-0001-public-repo-migration-plan.md`)
3. `.claude/work-session/20260507-101342/design.md` (세부 절차 §7/§9/§5/§8)
4. `.claude/work-session/20260507-101342/requirements.md` (참조)

---

## 세션 원본 참조

본 ADR 은 세션 산출물을 요약한 문서다. 세부 내용은 아래 원본 파일에 있다.

| 항목 | 원본 경로 |
|---|---|
| 설계 전체 (Part 1~3, 1296줄) | `.claude/work-session/20260507-101342/design.md` |
| 요구사항 (FR/NFR, Success Criteria) | `.claude/work-session/20260507-101342/requirements.md` |
| 조사 결과 (RP-1~RP-8) | `.claude/work-session/20260507-101342/research/findings.md` |
| 확정 결정 10건 원문 | `.claude/work-session/20260507-101342/report.md` (Decisions 섹션) |

> 세션 디렉토리는 `.gitignore` 대상이므로 영구 링크가 아니다. 로컬 환경에서만 접근 가능.

---

## 관련 문서

- [`TEMPLATE_DEV.md`](../../TEMPLATE_DEV.md) — 본 ADR 의 G-P0-1~G-P0-4 백로그와 연계 (§8.3 참조)
- [`docs/development/agent-team-protocol.md`](../development/agent-team-protocol.md) — §10 파괴적 조작 게이트 (T4, T7 이중 게이트)
- [`docs/development/document-category-classification.md`](../development/document-category-classification.md) — `usage/` 카테고리 추가 대상 (U16, U17)
- `README.md` — 이 플랜이 실행되면 §6~§15 가 docs/ 로 이전되고 ~150줄로 축소됨 (이관 집행 세션에서 수행)
