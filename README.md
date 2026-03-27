# TurretRunner

**TurretRunner** is a high-performance hyper-casual mobile shooter prototype. Developed with a focus on clean architectural patterns, efficient resource management, and high-fidelity "juice" (gameplay feedback), this project demonstrates a production-ready approach to mobile game development.

> **Development Time:** ~14 working hours.

---

## Gameplay Features

* **Swipe-to-Aim Combat**: Responsive turret mechanics with precision Hitscan logic.
* **Procedural Road System**: Infinite segment cycling with randomized environment props to ensure varied gameplay.
* **Dynamic Level Progress**: Real-time progress tracking via UI slider and a cinematic finish sequence.
* **Polished UI/UX**: Custom per-character vertex animations using TextMeshPro `TextInfo` for "Win/Loss" states.
* **Destruction Visuals**: Car damage visualized through Material Property Blocks (MPB) for optimized performance and particle-based environmental effects.

---

## Technical Implementation

* **Dependency Injection (VContainer)**: Utilized for decoupled, maintainable, and testable code.
* **Advanced Object Pooling**: Comprehensive pooling for projectiles, enemies, VFX, and damage popups to eliminate runtime Garbage Collection.
* **Master Config (ScriptableObjects)**: Centralized game balance (speed, damage, crit rates) for rapid designer iteration without code changes.
* **DOTween Pro**: Orchestrating all UI transitions, camera impulses, and smooth object rotations.
* **Mobile-Optimized URP**: Lightweight Post-Processing stack featuring Bloom, Color Grading, and Vignette.
* **Finite State Machine (FSM)**: Robust management of game flows: Menu -> ReadyToPlay -> Gameplay -> Won/Lost .

---

## Architecture & SoC

The project strictly follows the **Separation of Concerns (SoC)** principle:
* **Core**: State management, interfaces, and shared data structures.
* **Infrastructure**: Dependency registration (LifetimeScope) and hardware input providers.
* **Gameplay**: Independent systems for vehicle movement, health logic, and visual representation .

---

## Getting Started

1.  **Clone the Repo**: `git clone https://github.com/your-username/TurretRunner.git`
2.  **Unity Version**: Recommended 2022.3 LTS or newer.
3.  **Dependencies**: Project requires **VContainer**, **DOTween**, and **TextMeshPro**.
4.  **Configuration**: 
    * Assign the `GameSettings` ScriptableObject to the `GameLifetimeScope` on the scene.
    * Open `Gameplay.unity` scene and press **Play**.
