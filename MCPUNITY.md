# Unity MCP (MCP for Unity) 가이드

**저장소**: https://github.com/CoplayDev/unity-mcp.git?path=/MCPForUnity

Unity MCP는 AI 어시스턴트(Claude, Cursor, VS Code 등)가 Unity Editor와 직접 상호작용할 수 있는 **MCP(Model Context Protocol)** 브리지입니다.

---

## 설치 방법

### 1. Unity 패키지 설치

Unity 에디터에서:
```
Window > Package Manager > + > Add package from git URL...
```

**안정 버전**:
```
https://github.com/CoplayDev/unity-mcp.git?path=/MCPForUnity
```

**최신 베타 버전**:
```
https://github.com/CoplayDev/unity-mcp.git?path=/MCPForUnity#beta
```

**다른 설치 방법**:
- **Unity Asset Store**: "MCP for Unity" 검색 후 설치
- **OpenUPM**: `openupm add com.coplaydev.unity-mcp`

---

## Editor 윈도우 가이드

### 윈도우 열기
- Unity 메뉴: `Window > MCP for Unity`
- 키보드 단축키: `Cmd+Shift+M` (macOS) / `Ctrl+Shift+M` (Windows, Linux)

윈도우는 4개 섹션으로 구성됩니다:
1. **Server Status** - 서버 설치 상태 및 설정
2. **Unity Bridge** - Unity 브리지 프로세스 제어
3. **MCP Client Configuration** - MCP 클라이언트별 설정
4. **Script Validation** - 스크립트 유효성 검사 레벨

### 1. Server Status 섹션

| 항목 | 설명 |
|------|------|
| **Status** | Installed / Installed (Embedded) / Not Installed |
| **Mode** | Auto 또는 Standard |
| **Ports** | Unity (가변), MCP 6500 |
| **Auto-Setup** | MCP 클라이언트 자동 설정, 브리지 연결 확인 |
| **Rebuild MCP Server** | Python 기반 MCP 서버 재빌드 |
| **Select server folder…** | 로컬 `Server` 폴더 선택 (개발용) |
| **Verify again** | 서버 존재 재확인 |

#### HTTP Server Command foldout
- `uvx` 실행 명령 표시
- 복사 버튼 및 "Start Local HTTP Server" 액션
- 다른 위치에서 명령을 실행하거나 재사용 가능

### 2. Unity Bridge 섹션

| 상태 | 설명 |
|------|------|
| **Running** 🟢 | 브리지 활성화, MCP 클라이언트와 통신 가능 |
| **Stopped** 🔴 | 브리지 중지됨 |

- **Start/Stop Bridge**: 브리지 프로세스 토글
- Auto-Setup 후 Auto 모드에서 자동 시작될 수 있음

### 3. MCP Client Configuration 섹션

**Select Client** 드롭다운에서 클라이언트 선택 후 각각의 설정 사용:

#### Cursor / VS Code / Windsurf

| 액션 | 설명 |
|------|------|
| **Auto Configure** | `uvx`로 서버 실행 설정 자동写入 |
| **Manual Setup** | 복사/붙여넣기용 JSON 스니펫 표시 |
| **Choose UV Install Location** | uv/uvx가 PATH에 없을 때 실행 파일 선택 |

**표시되는 상태**: "Configured", "uv Not Found" 등

#### Claude Code

| 액션 | 설명 |
|------|------|
| **Register with Claude Code** | Claude Code에 MCP 등록 |
| **Unregister MCP for Unity** | 등록 해제 |
| **Choose Claude Install Location** | CLI를 찾을 수 없을 때 경로 선택 |

### 4. Script Validation 섹션

| 레벨 | 설명 |
|------|------|
| **Basic** | 구문 검사만 수행 |
| **Standard** | 구문 + Unity 관행 검사 |
| **Comprehensive** | 전체 검사 + 시맨틱 분석 |
| **Strict** | 완전한 시맨틱 검증 (Roslyn 필요) |

**Strict 모드 활성화 방법**:
1. NuGetForUnity 설치
2. `Window > NuGet Package Manager` → `Microsoft.CodeAnalysis` v5.0 설치
3. `SQLitePCLRaw.core`와 `SQLitePCLRaw.bundle_e_sqlite3` v3.0.2 설치
4. `Player Settings > Scripting Define Symbols`에 `USE_ROSLYN` 추가
5. Unity 재시작

### 디버그 모드
- 윈도우 헤더의 **"Show Debug Logs"** 체크박스 활성화
- 콘솔에 상세 로그 출력 (문제 진단 시 유용)

---

## MCP Tools 명령어 정리

### 🎮 에디터 제어

| Tool | 설명 |
|------|------|
| `manage_editor` | Unity 에디터 상태 제어 (Play/Pause/Stop, 툴 설정, Tag/Layer 관리) |
| `execute_menu_item` | Unity 메뉴 항목 실행 |
| `refresh_unity` | Asset Database 새로고침, 스크립트 컴파일 요청 |

### 🎯 GameObject 관리

| Tool | 설명 |
|------|------|
| `manage_gameobject` | GameObject CRUD (생성, 수정, 삭제, 복제, 이동) |
| `find_gameobjects` | 이름/태그/레이어/컴포넌트/경로로 GameObject 검색 |
| `manage_components` | 컴포넌트 추가/제거/속성 설정 |

### 📦 에셋 관리

| Tool | 설명 |
|------|------|
| `manage_asset` | 에셋 CRUD (임포트, 생성, 수정, 삭제, 검색, 폴더 생성) |

### 🎨 재질 & 텍스처

| Tool | 설명 |
|------|------|
| `manage_material` | 재질 생성, 속성/색상/셰이더 설정, 렌더러에 할당 |
| `manage_texture` | 텍스처 생성 (단색, 패턴, 그라데이션, 노이즈) |

### 🎬 VFX & 파티클

| Tool | 설명 |
|------|------|
| `manage_vfx` | ParticleSystem, VisualEffect, LineRenderer, TrailRenderer 관리 |

### 🗺️ 씬 관리

| Tool | 설명 |
|------|------|
| `manage_scene` | 씬 생성/로드/저장, 계층 구조 조회, 빌드 설정, 스크린샷 |

### 📁 프리팹 관리

| Tool | 설명 |
|------|------|
| `manage_prefabs` | 프리팹 생성, 계층 구조 조회, 헤드리스 편집 |

### 📜 스크립트 관리

| Tool | 설명 |
|------|------|
| `create_script` | C# 스크립트 생성 |
| `delete_script` | C# 스크립트 삭제 |
| `apply_text_edits` | 텍스트 범위 기반 스크립트 편집 |
| `script_apply_edits` | 구조화된 C# 편집 (메서드/클래스 단위) |
| `validate_script` | C# 스크립트 유효성 검사 |
| `get_sha` | 스크립트 SHA256 해시 조회 |
| `find_in_file` | 파일 내 정규식 검색 |

### 🔧 Shader & ScriptableObject

| Tool | 설명 |
|------|------|
| `manage_shader` | 셰이더 스크립트 CRUD |
| `manage_scriptable_object` | ScriptableObject 에셋 생성/수정 |

### 🧪 테스트

| Tool | 설명 |
|------|------|
| `run_tests` | Unity 테스트 비동기 실행 (EditMode/PlayMode) |
| `get_test_job` | 테스트 작업 상태 조회 |

### 📝 콘솔 & 유틸리티

| Tool | 설명 |
|------|------|
| `read_console` | Unity 에디터 콘솔 메시지 조회/클리어 |
| `batch_execute` | 다중 MCP 명령어 배치 실행 (10-100x 더 빠름) |
| `execute_custom_tool` | 프로젝트 스코프 커스텀 툴 실행 |

---

## MCP Resources

읽기 전용 데이터 조회용 리소스:

| Resource | URI | 설명 |
|----------|-----|------|
| **에디터 상태** | `mcpforunity://editor/state` | 에디터 준비 상태, 조언, stale 정보 |
| **에디터 선택** | `mcpforunity://editor/selection` | 현재 선택된 오브젝트 정보 |
| **프로젝트 정보** | `mcpforunity://project/info` | 프로젝트 경로, Unity 버전, 플랫폼 |
| **프로젝트 태그** | `mcpforunity://project/tags` | 정의된 모든 태그 |
| **프로젝트 레이어** | `mcpforunity://project/layers` | 정의된 모든 레이어 (0-31) |
| **Unity 인스턴스** | `mcpforunity://instances` | 실행 중인 Unity 에디터 인스턴스 목록 |
| **메뉴 항목** | `mcpforunity://menu-items` | 모든 Unity 메뉴 항목 |
| **커스텀 툴** | `mcpforunity://custom-tools` | 프로젝트의 커스텀 툴 목록 |
| **테스트** | `mcpforunity://tests` | 모든 테스트 목록 |
| **GameObject API** | `mcpforunity://scene/gameobject-api` | GameObject 리소스 문서 |
| **Prefab API** | `mcpforunity://prefab-api` | Prefab 리소스 문서 |

---

## 성능 팁

### `batch_execute` 사용
다중 작업은 `batch_execute`로 한 번에 실행하세요. **10-100배 더 빠릅니다.**

```python
# 나쁜 예: 개별 호출
create_cube()
create_sphere()
create_cylinder()

# 좋은 예: 배치 실행
batch_execute(commands=[
    {"tool": "manage_gameobject", "params": {...}},
    {"tool": "manage_gameobject", "params": {...}},
    {"tool": "manage_gameobject", "params": {...}}
])
```

---

## 멀티 Unity 인스턴스

여러 Unity 에디터를 실행 중인 경우:

1. `unity_instances` 리소스로 인스턴스 목록 확인
2. `set_active_instance`로 타겟 지정 (`Name@hash` 형식, 예: `MyProject@abc123`)
3. 이후 모든 툴은 해당 인스턴스로 라우팅

---

## 클라이언트 수동 설정

Auto-Setup이 작동하지 않는 경우:

### HTTP (Claude Desktop, Cursor, Windsurf)

```json
{
  "mcpServers": {
    "unityMCP": {
      "url": "http://localhost:8080/mcp"
    }
  }
}
```

### VS Code

```json
{
  "servers": {
    "unityMCP": {
      "type": "http",
      "url": "http://localhost:8080/mcp"
    }
  }
}
```

### Stdio (uvx)

**macOS/Linux**:
```json
{
  "mcpServers": {
    "unityMCP": {
      "command": "uvx",
      "args": ["--from", "mcpforunityserver", "mcp-for-unity", "--transport", "stdio"]
    }
  }
}
```

**Windows**:
```json
{
  "mcpServers": {
    "unityMCP": {
      "command": "C:/Users/YOUR_USERNAME/AppData/Local/Microsoft/WinGet/Links/uvx.exe",
      "args": ["--from", "mcpforunityserver", "mcp-for-unity", "--transport", "stdio"]
    }
  }
}
```

---

## 트러블슈팅

| 문제 | 해결 방법 |
|------|----------|
| **Unity Bridge 연결 안됨** | `Window > MCP for Unity` 상태 확인, Unity 재시작 |
| **Server 시작 안됨** | `uv --version` 확인, 터미널 에러 확인 |
| **Client 연결 안됨** | HTTP 서버 실행 중인지 확인, URL 일치 확인 |
| **Python 또는 uv 없음** | [Fix Unity MCP with Cursor, VSCode & Windsurf](https://github.com/CoplayDev/unity-mcp/wiki/1.-Fix-Unity-MCP-and-Cursor,-VSCode-&-Windsurf) |
| **Claude CLI 없음** | [Fix Unity MCP with Claude Code](https://github.com/CoplayDev/unity-mcp/wiki/2.-Fix-Unity-MCP-and-Claude-Code) |

---

## 예시 프롬프트

- "빨간색, 파란색, 노란색 큐브 만들어줘"
- "간단한 플레이어 컨트롤러 빌드해줘"
- "현재 씬의 모든 Point Light를 찾아서 강도를 2로 설정해줘"
- "새로운 Material을 만들어서 선택된 큐브에 적용해줘"

---

## Telemetry & 개인정보

- 익명의 개인정보 보호 텔레메트리 (코드, 프로젝트명, 개인 데이터 미수집)
- 옵트아웃: `DISABLE_TELEMETRY=true` 환경변수 설정

---

## 라이선스

**MIT License** - 무료 및 오픈소스

---

## 관련 링크

- **GitHub**: https://github.com/CoplayDev/unity-mcp
- **Discord**: [Coplay Discord](https://discord.gg/coplay)
- **Unity Asset Store**: MCP for Unity
- **MCP Registry**: [MCP Enabled](https://mcpx.dev)
