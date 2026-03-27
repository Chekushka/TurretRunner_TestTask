TurretRunner
TurretRunner is a dynamic hyper-casual mobile shooter prototype focused on clean architecture, high mobile performance, and "juicy" gameplay feedback.

Development Time: ~14 working hours.

Gameplay Features
Dynamic Combat System: Swipe-controlled turret with precision Hitscan logic.

Procedural Generation: Infinite road segment cycling with randomized environment props.

Level Progression: Real-time progress tracking and a cinematic finish sequence with side-view car rotation.

Advanced UI: Per-character vertex animations (TextMeshPro TextInfo) for immersive Win/Loss screens.

Visual Damage System: Car destruction visualized via Material Property Blocks (MPB) for performance-friendly color shifts and particle-based fires.

Technical Highlights
Dependency Injection (VContainer): Decoupled architecture using VContainer for clean dependency management and testability.

Efficient Object Pooling: Customized pooling system for projectiles, enemies, VFX, and damage popups to minimize Garbage Collection.

Data-Driven Balance (ScriptableObjects): All game parameters (speed, damage, crit chance, spawn rates) are centralized in a single configuration file for rapid iteration [cite: 2026-03-27].

DOTween Integration: Powering UI interactions, camera movements, and game feedback loops.

Mobile-Optimized URP: Utilizing a lightweight Post-Processing stack (Bloom, Color Grading, Vignette) tailored for mobile GPU performance.

Robust State Machine: Centralized management of game flow: Menu, ReadyToPlay, Gameplay, Won, and Lost.

Getting Started
Clone the Repository: git clone https://github.com/your-username/TurretRunner.git.

Unity Version: Developed using Unity 2022.3 LTS (or newer).

Dependencies: Ensure VContainer, DOTween, and TextMeshPro are imported.

Initial Setup:

Open the Gameplay scene.

Assign the GameSettings asset to the GameSettings field in the GameLifetimeScope component.

Press Play.

Architecture Overview
The project follows the Separation of Concerns (SoC) principle:

Core: Global states, interfaces, and game settings.

Infrastructure: Dependency registration, input handling, and object resolvers.

Gameplay: Isolated logic for movement, combat, and visual representation [cite: 2026-03-27].
