# Claude Code Configuration Export

Generated: 2026-09-15
Purpose: portable migration record for moving to a new account and machine.
Source machine: Windows 10 Pro 10.0.19045, user `Admin`, config root `C:\Users\Admin\.claude` (`CLAUDE_CONFIG_DIR` was **not** set, so this is the default location).

**Secrets policy applied:** `.credentials.json` was never opened. `.claude.json` was read and reviewed line-by-line, but contains no raw API keys/tokens (Claude Code stores auth separately in `.credentials.json`); its OAuth account block is profile metadata (email, org, plan), not a secret, and is included below only because it's needed to know which account to sign back into. Nothing below is a usable credential.

---

## Section A — Environment Inventory

| Item | Value |
|---|---|
| Claude Code version (last recorded) | 2.1.235 |
| OS | Windows 10 Pro 10.0.19045 |
| Shell | Git Bash (POSIX sh) primary; PowerShell 5.1 also available |
| Node.js | v24.19.0 |
| Git | 2.53.0.windows.1 |
| Config directory | `C:\Users\Admin\.claude` (default; `CLAUDE_CONFIG_DIR` unset) |
| Account file | `C:\Users\Admin\.claude.json` (~50 KB) |
| Credentials file | `C:\Users\Admin\.claude\.credentials.json` — **exists, never read** |
| Account email | jcabaldan@netsolar.com.ph |
| Account display name | Joshua |
| Organization | NetSolar (`organizationType: claude_team`, seat tier `team_standard`) |
| Account created | 2026-08-11 |
| Claude Code first used | 2026-08-11T03:46:20Z |

### `~/.claude` top-level contents (what actually exists)

```
.claude/
├── .credentials.json        (exists, 501 bytes — untouched)
├── .last-cleanup            (timestamp only)
├── backups/                 (5 rolling backups of .claude.json)
├── cache/
├── file-history/            (undo/redo scratch — 3 entries, ephemeral)
├── history.jsonl            (CLI input history — small, 339 bytes)
├── ide/                     (empty — no IDE integration cached)
├── plugins/
│   ├── known_marketplaces.json
│   └── marketplaces/claude-plugins-official/
├── policy-limits.json       (org-pushed restrictions)
├── projects/
│   └── E--Users-Documents-Unity-Projects-Game-Language-feature-menu/
│       ├── memory/           ← auto-memory (see Section B)
│       └── 18 × <uuid>.jsonl ← session transcripts (see Section F)
├── remote-settings.json     ({} — empty)
├── session-env/             (per-session env snapshots, ephemeral)
├── sessions/                (active session registry cache, ephemeral)
├── settings.json            (see Section D)
├── shell-snapshots/         (ephemeral)
└── skills/
    └── unity-mcp-skill/     (see Section C — sync-managed, not hand-authored)
```

**Not found anywhere (global or project):** `CLAUDE.md`, `CLAUDE.local.md`, `rules/*.md`, `agents/*.md`, `commands/*.md`, `output-styles/*.md`, `workflows/*.js`, `~/.claude/plans`, any `settings.local.json`, any project-level `.claude/` directory, any `.mcp.json`.

### Registered projects (from `.claude.json` → `projects`)

Four keys exist, covering two real project roots (Windows path-casing/slash variants create duplicate entries — worth cleaning up on the new machine):

1. `C:/Users/Admin` — the home directory itself got trust-registered once (probably an accidental invocation there). `mcpServers: {}`. Not meaningful to migrate.
2. `E:/Users/Documents/Unity Projects/Game_Language-feature-menu` — **the real project**, trust accepted, `mcpServers` has `UnityMCP` (see Section E).
3. `E:\Users\Documents\Unity Projects\Game_Language-feature-menu` (backslash variant) — trust accepted, no `mcpServers` key recorded.
4. `e:/Users/Documents/Unity Projects/Game_Language-feature-menu` (lowercase drive letter variant) — trust **not** accepted, empty `mcpServers`.

Only variant #2 carries the working MCP registration. On the new machine, only one canonical path will exist — no need to recreate the duplicates.

### Git repository

- Remote: `https://github.com/cabaldanjehoshaphat/Lakbay-Salita.git`
- Git user.name: `cabaldanJehoshaphat`
- Git user.email: `cabaldanjehoshaphat@gmail.com`
- Local path was tracked in `.claude.json` under `githubRepoPaths["cabaldanjehoshaphat/game_language"]`.

### Plugins / marketplaces

- Marketplace `claude-plugins-official` (GitHub `anthropics/claude-plugins-official`), auto-installed (`officialMarketplaceAutoInstalled: true`) — no manual action needed, Claude Code reinstalls this automatically on first run.
- `pluginUsage` recorded two **host-provided inline** skill packs actually used: `anthropic-skills` and `netgroup-analyst-toolkit` (both suffixed `@inline` — these ship with the Claude Code environment itself, not something the user installed; nothing to migrate).
- `skillUsage`: the `pdf` skill was used once.

---

## Section B — Consolidated Memory and Rules (verbatim)

### Global memory (`~/.claude/CLAUDE.md`)
**Not found.** No global memory file exists.

### Project memory (`CLAUDE.md` / `CLAUDE.local.md` in project root)
**Not found.** Neither file exists in `E:\Users\Documents\Unity Projects\Game_Language-feature-menu`.

### Rules (`rules/*.md`)
**Not found**, globally or per-project.

### Auto memory
Location: `~/.claude/projects/E--Users-Documents-Unity-Projects-Game-Language-feature-menu/memory/`
(`autoMemoryDirectory` was not overridden in settings, so this is the default path for this project.)

**`MEMORY.md`** (index file, verbatim):
```markdown
- [C# script comments](feedback_csharp_comments.md) — always add a summary comment of purpose/usage to every C# script uploaded or generated
```

**`feedback_csharp_comments.md`** (verbatim):
```markdown
---
name: feedback-csharp-comments
description: "User wants a summary/comment block on every C# script (uploaded or newly generated) explaining what it is and what it's used for"
metadata: 
  node_type: memory
  type: feedback
  originSessionId: 45e1435c-265d-4327-a6bf-a9e1cd4644bd
  modified: 2026-08-19T06:11:38.504Z
---

Every C# script — whether the user uploads an existing one or I generate a new one — should get a summary comment describing what the script is and what it's used for.

**Why:** Explicit standing request from the user (2026-08-19) for this Unity project. This overrides the general default of "no comments unless non-obvious" specifically for C# scripts in this project.

**How to apply:**
- When generating a new C# script (via `create_script`, `manage_script`, or Write/Edit on a `.cs` file), add a brief header comment (e.g. an XML `/// <summary>` or a top-of-file block) stating the script's purpose and role (what it's attached to / what system it belongs to).
- When the user uploads/shares an existing C# script that lacks such a summary, add one describing what it does, rather than leaving it uncommented.
- Keep the summary concise — what the class/script is for and how it's used — not a line-by-line narration of the code.
```

This is the **only** memory/rule content that exists anywhere in this setup. Everything else in this export (Sections C–E) is tooling/settings, not memory.

---

## Section C — Skills, Agents, Commands, Output Styles, Workflows (full contents)

### Agents (`agents/*.md`)
**None found**, globally or per-project.

### Slash commands (`commands/*.md`)
**None found**, globally or per-project.

### Output styles (`output-styles/*.md`)
**None found**, globally or per-project.

### Workflows (`workflows/*.js`)
**None found**, globally or per-project.

### Skills (`skills/*/SKILL.md`)

Only one skill exists, at `~/.claude/skills/unity-mcp-skill/`. It carries a marker file `.unity-mcp-skill-sync` containing the string `managed-by-unity-mcp-skill-sync` — **this skill is auto-generated/synced by the "MCP for Unity" server itself whenever Claude Code connects to it, not hand-authored.** It also has two large bundled reference docs (`references/tools-reference.md`, 60 KB, and `references/workflows.md`, 74 KB) which are likewise sync-managed. Given that provenance, this export includes the top-level `SKILL.md` verbatim for completeness, but does **not** duplicate the 134 KB of reference docs — they will regenerate automatically the first time the new machine connects to a "MCP for Unity" bridge (see Section G, rebuild step 9). If you want them anyway, they're plain files at the path above and can be copied directly.

**`SKILL.md`** (verbatim, frontmatter name is `unity-mcp-orchestrator`; directory name is `unity-mcp-skill`):

~~~markdown
---
name: unity-mcp-orchestrator
description: Orchestrate Unity Editor via MCP (Model Context Protocol) tools and resources. Use when working with Unity projects through MCP for Unity - creating/modifying GameObjects, editing scripts, managing scenes, running tests, or any Unity Editor automation. Provides best practices, tool schemas, and workflow patterns for effective Unity-MCP integration.
---

# Unity-MCP Operator Guide

This skill helps you effectively use the Unity Editor with MCP tools and resources.

## Template Notice

Examples in `references/workflows.md` and `references/tools-reference.md` are reusable templates. They may be inaccurate across Unity versions, package setups (UGUI/TMP/Input System), and project-specific conventions. Please check console, compilation errors, or use screenshot after implementation.

Before applying a template:
- Validate targets/components first via resources and `find_gameobjects`.
- Treat names, enum values, and property payloads as placeholders to adapt.

## Quick Start: Resource-First Workflow

**Always read relevant resources before using tools.** This prevents errors and provides the necessary context.

```
1. Check editor state     → mcpforunity://editor/state
2. Understand the scene   → mcpforunity://scene/gameobject-api
3. Find what you need     → find_gameobjects or resources
4. Take action            → tools (manage_gameobject, create_script, script_apply_edits, apply_text_edits, validate_script, delete_script, get_sha, etc.)
5. Verify results         → read_console, manage_camera(action="screenshot"), resources
```

## Critical Best Practices

### 1. After Writing/Editing Scripts: Wait for Compilation and Check Console

```python
# After create_script or script_apply_edits:
# Both tools already trigger AssetDatabase.ImportAsset + RequestScriptCompilation automatically.
# No need to call refresh_unity — just wait for compilation to finish, then check console.

# 1. Poll editor state until compilation completes
# Read mcpforunity://editor/state → wait until is_compiling == false

# 2. Check for compilation errors
read_console(types=["error"], count=10, include_stacktrace=True)
```

**Why:** Unity must compile scripts before they're usable. `create_script` and `script_apply_edits` already trigger import and compilation automatically — calling `refresh_unity` afterward is redundant.

### 2. Use `batch_execute` for Multiple Operations

```python
# 10-100x faster than sequential calls
batch_execute(
    commands=[
        {"tool": "manage_gameobject", "params": {"action": "create", "name": "Cube1", "primitive_type": "Cube"}},
        {"tool": "manage_gameobject", "params": {"action": "create", "name": "Cube2", "primitive_type": "Cube"}},
        {"tool": "manage_gameobject", "params": {"action": "create", "name": "Cube3", "primitive_type": "Cube"}}
    ],
    parallel=True  # Hint only: Unity may still execute sequentially
)
```

**Max 25 commands per batch by default (configurable in Unity MCP Tools window, max 100).** Use `fail_fast=True` for dependent operations.

**Tip:** Also use `batch_execute` for discovery — batch multiple `find_gameobjects` calls instead of calling them one at a time:
```python
batch_execute(commands=[
    {"tool": "find_gameobjects", "params": {"search_term": "Camera", "search_method": "by_component"}},
    {"tool": "find_gameobjects", "params": {"search_term": "Player", "search_method": "by_tag"}},
    {"tool": "find_gameobjects", "params": {"search_term": "GameManager", "search_method": "by_name"}}
])
```

### 3. Use Screenshots to Verify Visual Results

```python
# Basic screenshot (saves to Assets/, returns file path only)
manage_camera(action="screenshot")

# Inline screenshot (returns base64 PNG directly to the AI)
manage_camera(action="screenshot", include_image=True)

# Use a specific camera and cap resolution for smaller payloads
manage_camera(action="screenshot", camera="MainCamera", include_image=True, max_resolution=512)

# Batch surround: captures front/back/left/right/top/bird_eye around the scene
manage_camera(action="screenshot", batch="surround", max_resolution=256)

# Batch surround centered on a specific object
manage_camera(action="screenshot", batch="surround", view_target="Player", max_resolution=256)

# Positioned screenshot: place a temp camera and capture in one call
manage_camera(action="screenshot", view_target="Player", view_position=[0, 10, -10], max_resolution=512)

# Scene View screenshot: capture what the developer sees in the editor
manage_camera(action="screenshot", capture_source="scene_view", include_image=True)

# Scene View framed on a specific object
manage_camera(action="screenshot", capture_source="scene_view", view_target="Canvas", include_image=True)
```

**Best practices for AI scene understanding:**
- Use `include_image=True` when you need to *see* the scene, not just save a file.
- Use `batch="surround"` for a comprehensive overview (6 angles, one command).
- Use `view_target`/`view_position` to capture from a specific viewpoint without needing a scene camera.
- Use `capture_source="scene_view"` to see the editor viewport (gizmos, wireframes, grid).
- Keep `max_resolution` at 256–512 to balance quality vs. token cost.

```python
# Agentic camera loop: point, shoot, analyze
manage_gameobject(action="look_at", target="MainCamera", look_at_target="Player")
manage_camera(action="screenshot", camera="MainCamera", include_image=True, max_resolution=512)
# → Analyze image, decide next action

# Multi-view screenshot (6-angle contact sheet)
manage_camera(action="screenshot_multiview", max_resolution=480)

# Scene View for editor-level inspection (shows gizmos, debug overlays, etc.)
manage_camera(action="screenshot", capture_source="scene_view", view_target="Player", include_image=True)
```

### 4. Check Console After Major Changes

```python
read_console(
    action="get",
    types=["error", "warning"],  # Focus on problems
    count=10,
    format="detailed"
)
```

### 5. Always Check `editor_state` Before Complex Operations

```python
# Read mcpforunity://editor/state to check:
# - is_compiling: Wait if true
# - is_domain_reload_pending: Wait if true  
# - ready_for_tools: Only proceed if true
# - blocking_reasons: Why tools might fail
```

## Parameter Type Conventions

These are common patterns, not strict guarantees. `manage_components.set_property` payload shapes can vary by component/property; if a template fails, inspect the component resource payload and adjust.

### Vectors (position, rotation, scale, color)
```python
# Both forms accepted:
position=[1.0, 2.0, 3.0]        # List
position="[1.0, 2.0, 3.0]"     # JSON string
```

### Booleans
```python
# Both forms accepted:
include_inactive=True           # Boolean
include_inactive="true"         # String
```

### Colors
```python
# Auto-detected format:
color=[255, 0, 0, 255]         # 0-255 range
color=[1.0, 0.0, 0.0, 1.0]    # 0.0-1.0 normalized (auto-converted)
```

### Paths
```python
# Assets-relative (default):
path="Assets/Scripts/MyScript.cs"

# URI forms:
uri="mcpforunity://path/Assets/Scripts/MyScript.cs"
uri="file:///full/path/to/file.cs"
```

## Core Tool Categories

| Category | Key Tools | Use For |
|----------|-----------|---------|
| **Scene** | `manage_scene`, `find_gameobjects` | Scene operations, finding objects. Multi-scene editing (additive load, close, set active, move GO between scenes), scene templates (`3d_basic`, `2d_basic`, `empty`, `default`), scene validation with `auto_repair`. For build settings, use `manage_build(action="scenes")`. |
| **Objects** | `manage_gameobject`, `manage_components` | Creating/modifying GameObjects |
| **Scripts** | `create_script`, `script_apply_edits`, `validate_script` | C# code management (auto-refreshes on create/edit) |
| **Assets** | `manage_asset`, `manage_prefabs` | Asset operations. **Prefab instantiation** is done via `manage_gameobject(action="create", prefab_path="...")`, not `manage_prefabs`. |
| **Editor** | `manage_editor`, `execute_menu_item`, `read_console` | Editor control, package deployment (`deploy_package`/`restore_package`), undo/redo (`undo`/`redo` actions) |
| **Testing** | `run_tests`, `get_test_job` | Unity Test Framework |
| **Batch** | `batch_execute` | Parallel/bulk operations |
| **Camera** | `manage_camera` | Camera management (Unity Camera + Cinemachine). **Tier 1** (always available): create, target, lens, priority, list, screenshot. **Tier 2** (requires `com.unity.cinemachine`): brain, body/aim/noise pipeline, extensions, blending, force/release. 7 presets: follow, third_person, freelook, dolly, static, top_down, side_scroller. Resource: `mcpforunity://scene/cameras`. Use `ping` to check Cinemachine availability. See [tools-reference.md](references/tools-reference.md#camera-tools). |
| **Graphics** | `manage_graphics` | Rendering and post-processing management. 33 actions across 5 groups: **Volume** (create/configure volumes and effects, URP/HDRP), **Bake** (lightmaps, light probes, reflection probes, Edit mode only), **Stats** (draw calls, batches, memory), **Pipeline** (quality levels, pipeline settings), **Features** (URP renderer features: add, remove, toggle, reorder). Resources: `mcpforunity://scene/volumes`, `mcpforunity://rendering/stats`, `mcpforunity://pipeline/renderer-features`. Use `ping` to check pipeline status. See [tools-reference.md](references/tools-reference.md#graphics-tools). |
| **Packages** | `manage_packages` | Install, remove, search, and manage Unity packages and scoped registries. Query actions: list installed, search registry, get info, ping, poll status. Mutating actions: add/remove packages, embed for editing, add/remove scoped registries, force resolve. Validates identifiers, warns on git URLs, checks dependents before removal (`force=true` to override). See [tools-reference.md](references/tools-reference.md#package-tools). |
| **ProBuilder** | `manage_probuilder` | 3D modeling, mesh editing, complex geometry. **When `com.unity.probuilder` is installed, prefer ProBuilder shapes over primitive GameObjects** for editable geometry, multi-material faces, or complex shapes. Supports 12 shape types, face/edge/vertex editing, smoothing, and per-face materials. See [ProBuilder Guide](references/probuilder-guide.md). |
| **UI** | `manage_ui`, `batch_execute` with `manage_gameobject` + `manage_components` | **UI Toolkit**: Use `manage_ui` to create UXML/USS files, attach UIDocument, inspect visual trees. **uGUI (Canvas)**: Use `batch_execute` for Canvas, Panel, Button, Text, Slider, Toggle, Input Field. **Read `mcpforunity://project/info` first** to detect uGUI/TMP/Input System/UI Toolkit availability. (see [UI workflows](references/workflows.md#ui-creation-workflows)) |
| **Docs** | `unity_reflect`, `unity_docs` | API verification and documentation lookup. **`unity_reflect`** inspects live C# APIs via reflection (requires Unity connection): `search` types across assemblies, `get_type` for member summary, `get_member` for full signatures. **`unity_docs`** fetches official docs from docs.unity3d.com (no Unity connection needed): `get_doc` (ScriptReference), `get_manual` (Manual pages), `get_package_doc` (package docs), `lookup` (parallel search all sources + project assets). **Trust hierarchy: reflection > project assets > docs.** Workflow: `unity_reflect` search -> get_type -> get_member -> `unity_docs` lookup. See [tools-reference.md](references/tools-reference.md#docs-tools). |

## Common Workflows

### Creating a New Script and Using It

```python
# 1. Create the script (automatically triggers import + compilation)
create_script(
    path="Assets/Scripts/PlayerController.cs",
    contents="using UnityEngine;\n\npublic class PlayerController : MonoBehaviour\n{\n    void Update() { }\n}"
)

# 2. Wait for compilation to finish
# Read mcpforunity://editor/state → wait until is_compiling == false

# 3. Check for compilation errors
read_console(types=["error"], count=10)

# 4. Only then attach to GameObject
manage_gameobject(action="modify", target="Player", components_to_add=["PlayerController"])
```

### Finding and Modifying GameObjects

```python
# 1. Find by name/tag/component (returns IDs only)
result = find_gameobjects(search_term="Enemy", search_method="by_tag", page_size=50)

# 2. Get full data via resource
# mcpforunity://scene/gameobject/{instance_id}

# 3. Modify using the ID
manage_gameobject(action="modify", target=instance_id, position=[10, 0, 0])
```

### Running and Monitoring Tests

```python
# 1. Start test run (async)
result = run_tests(mode="EditMode", test_names=["MyTests.TestSomething"])
job_id = result["job_id"]

# 2. Poll for completion
result = get_test_job(job_id=job_id, wait_timeout=60, include_failed_tests=True)
```

## Pagination Pattern

Large queries return paginated results. Always follow `next_cursor`:

```python
cursor = 0
all_items = []
while True:
    result = manage_scene(action="get_hierarchy", page_size=50, cursor=cursor)
    all_items.extend(result["data"]["items"])
    if not result["data"].get("next_cursor"):
        break
    cursor = result["data"]["next_cursor"]
```

## Multi-Instance Workflow

When multiple Unity Editors are running:

```python
# 1. List instances via resource: mcpforunity://instances
# 2. Set active instance
set_active_instance(instance="MyProject@abc123")
# 3. All subsequent calls route to that instance
```

## Error Recovery

| Symptom | Cause | Solution |
|---------|-------|----------|
| Tools return "busy" | Compilation in progress | Wait, check `editor_state` |
| "stale_file" error | File changed since SHA | Re-fetch SHA with `get_sha`, retry |
| Connection lost | Domain reload | Wait ~5s, reconnect |
| Commands fail silently | Wrong instance | Check `set_active_instance` |

## Reference Files

For detailed schemas and examples:

- **[tools-reference.md](references/tools-reference.md)**: Complete tool documentation with all parameters
- **[resources-reference.md](references/resources-reference.md)**: All available resources and their data
- **[workflows.md](references/workflows.md)**: Extended workflow examples and patterns
~~~

---

## Section D — Settings (secrets redacted)

### Global `~/.claude/settings.json`
```json
{
  "syntaxHighlightingDisabled": false,
  "theme": "auto"
}
```
Purely cosmetic — safe to recreate verbatim or skip (these are also the CLI defaults).

### Global `~/.claude/settings.local.json`
**Does not exist.**

### Project `.claude/settings.json` / `.claude/settings.local.json`
**Does not exist** — this project has no project-scoped `.claude/` directory at all.

### `~/.claude/policy-limits.json` (organization-pushed restrictions — read-only, pushed by NetSolar's Claude Team admin, not user-editable)
```json
{
  "restrictions": {
    "allow_remote_control": { "allowed": false },
    "allow_quick_web_setup": { "allowed": false },
    "enforce_web_search_mcp_isolation": { "allowed": false }
  },
  "compliance_taints": [],
  "monitoring_notice": null,
  "defaults": { "remote_control_at_startup": false }
}
```
This will be **re-pushed automatically** by the NetSolar organization the moment the new machine signs in with the same account — nothing to migrate manually, and it can't be overridden locally anyway.

### `~/.claude/remote-settings.json`
```json
{}
```
Empty — nothing to migrate.

### `.claude.json` per-project settings for the real project (secrets redacted — there were none to redact; only identifiers are noted)
```json
{
  "allowedTools": [],
  "mcpServers": {
    "UnityMCP": { "type": "http", "url": "http://127.0.0.1:8080/mcp" }
  },
  "hasTrustDialogAccepted": true,
  "enabledMcpjsonServers": [],
  "disabledMcpjsonServers": []
}
```
(Trimmed to the fields that matter for migration; the full entry also has per-session usage counters like `lastCost`, `lastDuration`, etc. — those are historical telemetry, not configuration, and are omitted here as noise.)

### Redacted / withheld entirely
| What | Where | Why |
|---|---|---|
| OAuth session token | `~/.claude/.credentials.json` | Never opened. `<REDACTED — re-authenticate on new machine>`. Sign in again with jcabaldan@netsolar.com.ph. |
| `machineID` | `.claude.json` top level | Device fingerprint hash — regenerates automatically per-machine, not portable, not a secret but not useful to copy either. |
| `userID` | `.claude.json` top level | Account-derived hash — regenerates on re-auth, don't copy. |
| Cached GrowthBook feature flags (~700 `tengu_*` keys) | `.claude.json` → `cachedGrowthBookFeatures` / `cachedExperimentFeatures` / `cachedExperimentData` | Server-side experiment/feature-gate cache, refetched fresh on every install. Not configuration — omitted entirely as noise, not a secret. |
| `clientDataCacheSlots` (~10 entries) | `.claude.json` | Per-session billing/entitlement cache snapshots. Ephemeral, regenerates. |

No API keys, bearer tokens, passwords, or connection strings were found in any readable file.

---

## Section E — MCP Servers

| Server | Scope | Type | Endpoint | Purpose |
|---|---|---|---|---|
| `UnityMCP` | Project (`E:/Users/Documents/Unity Projects/Game_Language-feature-menu` only — registered in `.claude.json`, **not** a `.mcp.json` file, since none exists in the repo) | `http` | `http://127.0.0.1:8080/mcp` | Bridge to the **MCP for Unity** Unity Editor package/plugin installed inside this Unity project. Gives Claude Code ~48 tools to drive the Unity Editor directly: scene CRUD, GameObject/component manipulation, C# script creation/editing with auto-compile, prefab management, screenshots, Play-mode control, console reading, and more. This was the primary tool surface for the entire Wordle-game Unity project documented in Section F. |

**Re-auth / re-setup steps for the new machine:**
1. This is a **local HTTP server**, not a hosted/OAuth MCP connector — there's no cloud credential to re-authenticate. It only requires the Unity Editor (with the "MCP for Unity" package installed in this specific project) to be running locally with its bridge listening on port 8080 (or whatever port its own settings specify).
2. Clone/open the `Lakbay-Salita` Unity project (see Section A git remote) in Unity Editor on the new machine.
3. Confirm the MCP for Unity package is present in the project's `Packages/manifest.json` (or wherever it's installed — this project used it heavily; if the package itself isn't in source control, reinstall it via the Unity Package Manager using whatever git URL / Asset Store entry was originally used).
4. Start the Unity Editor; the MCP for Unity bridge typically auto-starts and listens on `127.0.0.1:8080`.
5. Because the registration lives in `.claude.json` keyed by the **exact** project path, and the new machine will almost certainly have a different absolute path (different drive letter, username, or folder layout), **the `UnityMCP` entry will not carry over automatically** — Claude Code will need to re-detect/re-register it the first time you open this project on the new machine (it typically prompts to add a detected local MCP server, or you can add it manually: server name `UnityMCP`, type `http`, url `http://127.0.0.1:8080/mcp`).
6. Once connected, the `unity-mcp-skill` (Section C) will re-sync itself automatically — no manual step needed there.

No other MCP servers were found registered anywhere (no `.mcp.json` in the project, no other entries under any of the four project keys in `.claude.json`).

---

## Section F — Session Index

All 18 transcripts live under one project folder: `~/.claude/projects/E--Users-Documents-Unity-Projects-Game-Language-feature-menu/`. **Important characteristic of this history:** these are not 18 independent conversations — Claude Code's `--resume`/`--continue` and auto-compaction create a **new transcript file that re-embeds the full prior conversation** each time a session is resumed. So the files form two "lineages," each one a chain of ever-larger forks of the same continuous project thread; the *last* file in each lineage is the fullest record of that lineage. Summaries below were derived by sampling the first few real user messages in each file (not by reading full transcripts) plus direct knowledge from the still-open session that produced this export.

**Lineage 1 — initial buildout** (2026-08-19 → 2026-09-03, opens with *"Hey! Claude AI, I currently have an open session with my Unity, can you tell what Scene name I'm using?"*):

| Session (uuid) | Size | Date range (UTC) | Summary |
|---|---|---|---|
| `45e1435c-265d-4327-a6bf-a9e1cd4644bd` | 0.2 MB | 08-19 04:56 → 08-20 04:11 | First-ever session. Established the "always add a C# summary comment" standing preference (now in auto-memory). Built the "Claude AI Folder" test scene: Canvas, camera, black background, a generic rows/columns grid generator script. |
| `1301b42b-6073-44ca-a63f-74ac5b5ac48e` | 1.15 MB | 08-20 04:59 → 08-25 07:57 | Continuation: grid generator refactor (GridSettings ScriptableObject, OnValidate live-regen), added prefab-image cell generation. |
| `49fc9c6b-d2ca-4b6e-a6b9-ce8e98aeeed4` | 1.17 MB | 08-25 07:57 → 08:39 | Short continuation same day. |
| `6c3928d5-8231-4e74-a4d9-5cf4ee99759e` | 3.46 MB | 08-25 08:39 → 10:58 | Continuation — likely ImageContainer (char→sprite lookup ScriptableObject + custom Editor drawer) work. |
| `edb7eff6-b400-4340-98d4-2fa683973b04` | 3.68 MB | 08-25 11:30 → 12:26 | Continuation. |
| `15229105-7233-4c52-89ac-e50e6866bd4c` | 4.05 MB | 08-25 12:26 → 14:17 | Continuation. |
| `abfdb2af-da2e-4395-bec9-afb076b86722` | 4.17 MB | 08-25 14:18 → 08-26 03:34 | Continuation — Main Menu / project structure review era. |
| `e14d1790-fd73-4159-b2ef-ee6088cc2b1f` | 4.89 MB | 08-26 03:35 → 09-01 06:50 | Longest quiet stretch (idle days between bursts) — Profile Icon scene, PlayerData/PlayerDatabase, edit/delete profile buttons, resolution/Canvas Scaler standardization to 1920×1080. |
| `cab2d171-b9f3-43a1-89ed-ab0c09983072` | 4.99 MB | 09-01 06:54 → 16:28 | Early Wordle system scaffolding begins (`WordleRowsColumnGenerator`, `WordleCell` prefab). |
| `df25c274-4f1c-4622-89a9-7dae853c75e9` | 13.89 MB | 09-01 16:28 → 09-03 05:07 | Major Wordle buildout: duplicated base scene into 30 puzzle scenes (Cebuano/Ilonggo/Tagalog), multiple grid-size resizing passes, keyboard typing input. |
| `043cbc7d-728e-4c2c-b279-f21afc899e03` | 13.58 MB | 09-03 05:12 → 06:10 | Continuation — word verification wiring across scenes. |
| `04d8ffc4-8046-4c22-966a-3e6d308d48fa` | 13.91 MB | 09-03 06:13 → 06:40 | Continuation. |
| `70199212-6549-433c-9287-835f7586e1ff` | 21.79 MB | 09-03 06:41 → 18:18 | **Largest in this lineage** — Wordle keyboard typer/verifier finalized (two-pass duplicate-safe scoring, green/yellow/red), removed ImageContainer dependency, Roboto font family imported and tested (9 weights). |

**Lineage 2 — post-compaction continuation** (2026-09-03 18:19 → 2026-09-15 14:46, opens with a compacted summary of Lineage 1 followed by *"test every scene of ... Assets/Main/Scenes/2 Wordle/..."*):

| Session (uuid) | Size | Date range (UTC) | Summary |
|---|---|---|---|
| `167c5637-7297-464c-b477-15f2cfb38fb7` | 16.4 MB | 09-03 18:19 → 09-04 07:21 | Tested all 30 Wordle scenes (row-by-row verification), generated a PDF test report; found + fixed a TMP font-asset material/atlas corruption bug across all 9 Roboto weights; applied ExtraBold font project-wide; built the `SceneNavigator`/`LanguageSelector` menu-navigation system and rolled the back-to-main-menu button out to all 30 puzzle scenes. |
| `dc1ca161-cec6-4d66-8864-4c092f9f8bae` | 15.62 MB | 09-04 07:22 → 18:43 | Built the Dialog Panel word-definition display (`WordDefinitionDisplay`), the shared `WordleTextStyle` font/size ScriptableObject system, and `WordleDialogPanelLayout` (percentage-based left/right padding) — rolled out across all 30 scenes. |
| `ec05b7b6-9116-44c5-9157-6009b0e327e4` | 15.74 MB | 09-04 18:44 → 18:57 | Short continuation. |
| `1c7a86c2-907a-49e7-978d-b2cd15fd908b` | 32.0 MB | 09-04 18:57 → 09-15 04:16 | Fixed Dialog Panel text overflow (auto-growing height via VerticalLayoutGroup/ContentSizeFitter); built the countdown `Timer` widget, `WordleTimerSettings` (font/delay/position) and `WordleSceneTimerDurations` (per-scene duration lookup table) ScriptableObjects; rolled both out across all 30 scenes. |
| `4780f59e-34bb-4385-9487-da8ffde6d917` | 36.5 MB | 09-15 04:17 → 14:46 | Made the Timer stop counting down once the puzzle is solved correctly (`WordleKeyboardTyper.Solved` read by `WordleTimer`); consolidated Dialog Panel font size/style/color onto `WordleDialogPanelLayout`; this export was produced in the session that continues from here. |

**Total transcript volume:** ~280 MB across 18 files, spanning 2026-08-19 to 2026-09-15 (this export).

---

## Section G — Rebuild Checklist (ordered)

1. **Install Claude Code** on the new machine (any current version; this account last ran 2.1.235).
2. **Sign in** with `jcabaldan@netsolar.com.ph` (NetSolar organization, `team_standard` seat). This alone restores the OAuth account, org membership, plan/entitlements, and org-pushed `policy-limits.json` — do not attempt to copy `.credentials.json` or `.claude.json`'s cached account block.
3. **Clone the project repo**:
   ```bash
   git clone https://github.com/cabaldanjehoshaphat/Lakbay-Salita.git
   cd Lakbay-Salita
   git config user.name "cabaldanJehoshaphat"
   git config user.email "cabaldanjehoshaphat@gmail.com"
   ```
4. **Open the project folder in Claude Code** and accept the trust-project prompt when it appears (there is no `.claude/` folder or `CLAUDE.md` in the repo to carry over — this project ran entirely on default settings + auto-memory).
5. **Restore auto-memory** so the C#-comment preference survives: create the file
   `~/.claude/projects/<new-encoded-project-path>/memory/MEMORY.md` and `feedback_csharp_comments.md`
   with the exact verbatim content from Section B above (or script it from `claude-code-export.json`, key `memory.autoMemory`). The encoded folder name Claude Code will use is derived from the new project's absolute path the same way `E--Users-Documents-Unity-Projects-Game-Language-feature-menu` was derived here (slashes/colons → `-`); let Claude Code create the folder once by opening the project there first, then drop the two files in.
6. *(Optional, cosmetic)* Recreate `~/.claude/settings.json`:
   ```json
   { "syntaxHighlightingDisabled": false, "theme": "auto" }
   ```
   These are also the defaults, so this step can be skipped.
7. **Open the Unity project** (this repo) in Unity Editor, with the **MCP for Unity** package installed in it (check `Packages/manifest.json`; if it's not tracked in source control, reinstall it via the Unity Package Manager from whatever source was originally used).
8. **Start the Unity Editor** so the MCP for Unity bridge begins listening (default `http://127.0.0.1:8080/mcp`).
9. **Let Claude Code detect/register `UnityMCP`** the first time you work in this project on the new machine (it will not carry over automatically, since the old registration was keyed to the old absolute path) — accept the detected local MCP server, or add it manually as an `http` server pointing at `http://127.0.0.1:8080/mcp`.
10. **No action needed** for the `unity-mcp-skill` — it will regenerate itself automatically once step 9 is connected (confirmed sync-managed via its own marker file).
11. **No action needed** for plugins — `claude-plugins-official` auto-installs on first run.
12. **Verify the bridge works**: ask Claude Code "what Unity scene am I using?" (the exact first question ever asked in this project) and confirm it can see the Editor.
13. *(Optional)* **Carry over session history** for browsing/resuming: copy `~/.claude/projects/E--Users-Documents-Unity-Projects-Game-Language-feature-menu/*.jsonl` (and the `memory/` folder, already covered in step 5) to the new machine's equivalent encoded project folder. This only enables `claude --resume` to list/continue old sessions if the new project's absolute path encodes to the *same* folder name — otherwise keep the files purely as a historical reference (e.g. attach `claude-code-export.md`/`.json` instead).
14. Nothing else to migrate — there is no global `CLAUDE.md`, no rules, no custom agents, no slash commands, no output styles, and no workflows in this setup.

---

*Companion machine-readable file: `claude-code-export.json` in the same directory.*
