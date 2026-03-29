# Octo Games Test Task

This repository contains a Unity test project focused on practical engineering decisions in UI, runtime systems, and clean architecture.  
The implementation intentionally stays lightweight: the goal is to show clear reasoning, readable APIs, and production-oriented Unity code without over-engineering.

## Project Focus

The project currently demonstrates the following task areas:

1. `Save / Load Utility`
2. `Popup / UI System`
3. `Code Review / Refactoring`
4. `Gameplay / State Logic`

## Task 2: Save / Load Utility

### Goal

Build a simple and reusable save/load utility that:

- saves arbitrary data types
- loads them safely
- handles missing or corrupted files without crashing
- stays practical for real game data such as settings, progress, flags, or VN state

### Implemented Approach

I implemented a generic JSON-based save/load utility with an optional encryption layer.

The main design goals were:

- keep the API small and reusable
- avoid boilerplate for each saved data type
- support both plain-text and protected saves
- fail safely when data is missing or invalid

### Main Files

- [`JsonFileSaveLoadService.cs`](Assets/Scriprs/Service/SaveLoad/JsonFileSaveLoadService.cs)
- [`AesStringProtector.cs`](Assets/Scriprs/Service/SaveLoad/AesStringProtector.cs)
- [`SaveLoadTest.cs`](Assets/Scriprs/View/SaveLoadTest.cs)

### API

The save/load service exposes:

- `Save<T>(string key, T data)`
- `Load<T>(string key, T fallback = default)`
- `TryLoad<T>(string key, out T data)`
- `Exists(string key)`
- `Delete(string key)`

This makes the utility generic enough to save and load any serializable runtime data model without adding type-specific code.

### Why Newtonsoft.Json

I used `Newtonsoft.Json` instead of Unity `JsonUtility` because it is more flexible and reliable for production-style data.  
It handles more real-world serialization cases cleanly and gives better control when the saved data grows beyond very simple DTOs.

### Optional Encryption

Encryption is intentionally separated behind the `IStringProtector` interface.

That means the same save/load service can be used:

- without protection for quick local saves or debugging
- with protection for settings, progress, or other user-facing save files

The included implementation uses AES in:

- [`AesStringProtector.cs`](Assets/Scriprs/Service/SaveLoad/AesStringProtector.cs)

### Safety and Error Handling

The implementation is designed to fail safely:

- invalid or empty keys are rejected
- missing files do not crash the game
- corrupted content is caught in `TryLoad`
- `Load<T>` supports fallback values
- load failures are logged as warnings instead of breaking runtime flow

### Demo

The save/load implementation is demonstrated directly in the scene through:

- [`SaveLoadTest.cs`](Assets/Scriprs/View/SaveLoadTest.cs)

What to check during review:

1. Enter Play Mode
2. Watch the Unity Console
3. Confirm that:
   - plain JSON save/load works for `PlayerProgress`
   - encrypted save/load works for `GameSettings`
   - values are loaded back correctly and printed to the console

This scene-level demo was chosen intentionally to keep the implementation easy to verify without extra UI.


The code is organized under [`Assets/Scriprs`](Assets/Scriprs), with Zenject used for dependency injection and runtime composition.

## High-Level Architecture

The project uses a small service-oriented structure:

- Installers register runtime services in Zenject
- Services own orchestration and data access
- Factories create runtime objects when prefab-based construction is needed
- MonoBehaviours stay focused on view logic and scene interaction

Main installer entry points:

- [`BootstrapInstaller.cs`](Assets/Scriprs/Installers/BootstrapInstaller.cs)
- [`SceneInitializationInstaller.cs`](Assets/Scriprs/Installers/SceneInitializationInstaller.cs)
- [`UIInitializer.cs`](Assets/Scriprs/Installers/UIInitializer.cs)

## Task 3: Popup / UI System

### Goal

Design a simple popup/window system that supports:

- loading a popup
- setting title and body text
- showing 1 to 5 buttons
- assigning callbacks to buttons

### Implemented Approach

I implemented this as a lightweight window system built around:

- `WindowService` for open/close orchestration
- `WindowFactory` for prefab creation
- `StaticDataService` for resolving window prefabs by `WindowId`
- `BaseWindow` as the common runtime window contract
- `WindowWithParams` as a dynamic popup-style window with runtime content

### Main Files

- [`IWindowService.cs`](Assets/Scriprs/Service/Windows/IWindowService.cs)
- [`WindowService.cs`](Assets/Scriprs/Service/Windows/WindowService.cs)
- [`IWindowFactory.cs`](Assets/Scriprs/Service/Windows/IWindowFactory.cs)
- [`WindowFactory.cs`](Assets/Scriprs/Service/Windows/WindowFactory.cs)
- [`BaseWindow.cs`](Assets/Scriprs/Service/Windows/BaseWindow.cs)
- [`WindowConfig.cs`](Assets/Scriprs/Service/Windows/Configs/WindowConfig.cs)
- [`WindowsConfig.cs`](Assets/Scriprs/Service/Windows/Configs/WindowsConfig.cs)
- [`StaticDataService.cs`](Assets/Scriprs/Service/StaticData/StaticDataService.cs)
- [`WindowWithParams.cs`](Assets/Scriprs/View/WindowWithParams.cs)
- [`WindowOpener.cs`](Assets/Scriprs/View/WindowOpener.cs)

### How It Works

- `UIInitializer` provides the `UIRoot` where runtime windows are spawned
- `WindowOpener` triggers window opening from scene buttons
- `WindowService` asks `WindowFactory` to instantiate a window
- `WindowFactory` resolves the prefab from static config and injects dependencies via Zenject
- `WindowWithParams` reads payload data and creates a runtime button list

### Popup Data Model

The popup-style window uses:

- `WindowWithParamsPayload` for title, body, and buttons
- `CustomButton` for button label + callback

That allows opening a parameterized popup in this style:

```csharp
_windowService.Open(
    WindowId.GameActionsWindow,
    new WindowWithParamsPayload(
        "Settings",
        "Set game params",
        new[]
        {
            new CustomButton("Close", () => _windowService.Close(WindowId.GameActionsWindow)),
            new CustomButton("Log", () => Debug.Log("Log"))
        }));
```

### Why This Design

This approach keeps the popup logic practical:

- the service owns lifecycle
- the factory owns instantiation
- the window view owns rendering and button wiring

It is simple enough for a test task, but still demonstrates clean separation of concerns and runtime flexibility.

## Task 4: Code Review / Refactoring

### Goal

Review the original UI update code, identify correctness and performance issues, and refactor it into a safer and more maintainable implementation.

### Main File

- [`CharactersView.cs`](Assets/Scriprs/CharactersView.cs)

### Problems in the Original Version

The original implementation had several issues:

- incorrect API assumptions such as expecting `GetComponents<Character>()` to behave like a single component
- invalid collection usage such as using `Length` with `List<T>` instead of `Count`
- incorrect average calculation
- repeated `GetComponent<Text>()` calls inside the update flow
- excessive logging and unnecessary UI refreshes
- `FixedUpdate()` used for UI logic even though this is physics-timed, not UI-timed

### What Was Changed

The refactored version in [`CharactersView.cs`](Assets/Scriprs/CharactersView.cs) now:

- caches the `Text` reference in `Awake()`
- uses `Update()` instead of `FixedUpdate()`
- refreshes on a configurable interval through `_updateInterval`
- uses `Time.unscaledDeltaTime` so UI still updates correctly during pause or time-scale changes
- iterates over `List<Character>` directly with `Count`
- calculates the average correctly as `totalValue / activeCount`
- guards against division by zero
- skips UI assignment if the displayed string has not changed

### Why These Changes Were Made

The main goals were correctness, readability, and runtime efficiency.

From a correctness perspective, the refactor fixes API misuse, invalid collection handling, incorrect average logic, and null-safety issues.  
From a performance perspective, it removes repeated component lookups, avoids refreshing the UI every frame, and reduces unnecessary allocations and UI rebuilds.

### Structural Improvement

The original version mixed several responsibilities together:

- gathering data
- calculating values
- formatting text
- updating UI

The new version separates this more clearly by moving the aggregation and rendering logic into `RefreshView()`.  
It is still intentionally lightweight, but much easier to maintain and extend.

### Production Note

In a larger production project, I would likely push this one step further into an event-driven approach:

- update the UI only when character data changes
- maintain aggregates incrementally instead of recalculating them each refresh

That would remove polling entirely and scale better for larger datasets.

### Demo

To review this task:

1. Open the scene with the UI text bound to [`CharactersView.cs`](Assets/Scriprs/CharactersView.cs)
2. Enter Play Mode
3. Observe that the UI updates on a timed interval rather than every physics tick
4. Verify that the displayed count and average are correct
5. Inspect the script to confirm the refactor decisions listed above

## Task 5: Gameplay / State Logic

### Goal

Design a system that:

- tracks gameplay entities
- returns only active entities
- handles disabled and removed entities cleanly
- stays readable and safe for production use

### Implemented Approach

I used a small event-driven registration model:

- gameplay objects implement `IGameplayEntity`
- they register through `TrackedGameplayEntity`
- `GameplayEntityTracker` stores all known entities
- callers ask the tracker for active entities only

This avoids scene-wide searches and keeps runtime logic explicit.

### Main Files

- [`IGameplayEntity.cs`](Assets/Scriprs/Service/Gameplay/IGameplayEntity.cs)
- [`IGameplayEntityTracker.cs`](Assets/Scriprs/Service/Gameplay/IGameplayEntityTracker.cs)
- [`GameplayEntityTracker.cs`](Assets/Scriprs/Service/Gameplay/GameplayEntityTracker.cs)
- [`TrackedGameplayEntity.cs`](Assets/Scriprs/Service/Gameplay/TrackedGameplayEntity.cs)
- [`GameplayEntitiesDebugView.cs`](Assets/Scriprs/View/GameplayEntitiesDebugView.cs)
- [`DemoEnemy.cs`](Assets/Scriprs/View/DemoEnemy.cs)

### How It Works

- Any gameplay entity can inherit from `TrackedGameplayEntity`
- On enable, it registers itself in `GameplayEntityTracker`
- On destroy, it unregisters itself
- The tracker keeps a list of all tracked entities
- `GetActiveEntities(List<IGameplayEntity> results)` fills a caller-provided buffer with active ones only

The base implementation uses `isActiveAndEnabled`, but entities can override `IsActive` to add custom gameplay rules such as:

- dead enemies
- completed interactables
- actors temporarily unavailable for interaction

### Why This Design

This solution was chosen because it balances simplicity and production-safety:

- no `FindObjectsOfType` in gameplay flow
- no per-frame scanning in the tracker
- no per-call GC allocations from returning fresh collections
- explicit ownership of registration and cleanup

It is small, testable, and easy to extend later if the project grows.

## Demo Guide

### Popup / Window Demo

1. Open the scene with the UI buttons wired to [`WindowOpener.cs`](Assets/Scriprs/View/WindowOpener.cs)
2. Press the simple open button to show the standard window
3. Press the custom open button to show the parameterized popup window
4. Verify that:
   - title and body are filled from payload
   - dynamic buttons are created
   - callbacks execute correctly
   - the close action destroys the correct window

### Gameplay Entity Tracker Demo

1. Place several tracked objects in the scene using [`DemoEnemy.cs`](Assets/Scriprs/View/DemoEnemy.cs)
2. Add [`GameplayEntitiesDebugView.cs`](Assets/Scriprs/View/GameplayEntitiesDebugView.cs) to a UI text object
3. Enter play mode and observe the current tracked and active counts
4. Use the demo buttons on `DemoEnemy`:
   - deactivate one entity by setting `IsDead = true`
   - destroy one entity with `Destroy(gameObject)`
5. Verify that:
   - inactive entities are excluded from the active result
   - destroyed entities are removed from tracking
   - counts update without scene scanning

## Performance Notes

The implementation tries to stay conscious of common Unity runtime costs:

- no repeated `FindObjectsOfType`
- no `GetComponent` inside hot gameplay loops
- caller-provided lists for active entity queries to reduce allocations
- limited UI refresh frequency in debug view
- string updates are skipped if rendered text has not changed

## Known Limitations

- `WindowService.Close(WindowId)` closes by type, so it is best suited for one open instance per window type
- the popup window flow is intentionally simple and does not include modal stacking or transition animation
- the gameplay tracker currently exposes a generic entity list, not type-filtered queries

These are reasonable tradeoffs for the scope of a test task, and the code is structured so those features could be added later without rewriting the foundations.

## Summary

This project demonstrates a pragmatic Unity architecture centered on:

- clear APIs
- low-complexity runtime systems
- prefab-driven UI
- explicit gameplay entity tracking
- Zenject-based dependency wiring

The goal was not to build a full framework, but to show sound engineering judgment, production awareness, and code that a reviewer can read quickly without getting lost.
