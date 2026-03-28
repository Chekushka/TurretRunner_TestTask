using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "TurretRunner/GameSettings")]
    public class GameSettings : ScriptableObject
    {
        [Header("🚗 Car Movement")] 
        [Tooltip("Constant forward speed of the vehicle")]
        [Min(0)] public float CarForwardSpeed = 10;
        
        [Tooltip("The machine's roll rate during turns")]
        [Min(0)] public float CarTiltSpeed = 5f;
        
        [Tooltip("Maximum body tilt angle")]
        public float MaxTiltAngle = 12f;
        
        [Space(10)]
        [Tooltip("Player's maximum health")]
        [Min(1)] public int CarMaxHealth = 200;
        
        [Header("🔄 Drift & Physics")]
        [Tooltip("How smoothly the car returns to the center (the lower the value, the sharper the response)")]
        public float DriftSmoothTime = 0.8f;
        
        [Tooltip("Minimum interval between random drifts (sec)")]
        public float MinDriftInterval = 3f;
        
        [Tooltip("Maximum interval between random drifts (sec)")]
        public float MaxDriftInterval = 5f;

        [Header("🔫 Turret & Combat")] 
        [Tooltip("Base damage per bullet")]
        [Min(1)] public int WeaponDamage = 30;
        
        [Tooltip("Delay between shots (sec). Lower = faster")]
        [Min(0.05f)] public float FireRate = 0.2f;
        
        [Tooltip("Maximum firing range")]
        [Min(1)] public float WeaponRange = 30f;
        
        [Space(5)]
        [Tooltip("Critical hit chance (0 = 0%, 1 = 100%)")]
        [Range(0f, 1f)] public float CritChance = 0.2f;
        
        [Tooltip("Critical hit damage multiplier")]
        [Min(1)] public float CritMultiplier = 2.0f;
        
        [Space(5)]
        [Tooltip("Turret rotation speed controlled by a finger")]
        [Min(0)] public float RotationSpeed = 15f;
        
        [Tooltip("Maximum turret rotation angle in each direction")]
        [Range(0, 180)] public float MaxAngle = 70f;
        
        [Header("🧟 Enemy AI")]
        [Tooltip("The health of one enemy")]
        [Min(1)] public int EnemyMaxHealth = 100;
        
        [Tooltip("The enemy's running speed toward the vehicle")]
        [Min(0)] public float EnemySpeed = 4f;
        
        [Tooltip("Damage to the vehicle upon collision with the enemy")]
        [Min(0)] public int EnemyDamageToCar = 10;
        
        [Space(5)]
        [Tooltip("The radius within which an enemy spots the vehicle and starts running")]
        [Min(1)] public float DetectionRadius = 15f;
        
        [Tooltip("The distance at which the enemy launches an attack")]
        [Min(0.5f)] public float AttackRadius = 3.5f;
        
        [Tooltip("Stun duration when the enemy takes damage (sec)")]
        public float StunDuration = 0.4f;

        [Header("🛣️ Level Design & Spawning")]
        [Tooltip("Total length of the course to the finish lines (in segments)")]
        [Min(100)] public float LevelDistance = 5f;
        
        [Tooltip("Road width (left/right turn restrictions)")]
        public float RoadWidth = 3.5f;
        
        [Space(10)]
        [Tooltip("The number of road segments stored in memory ahead")]
        [Range(2, 10)] public int RoadSegmentsInReserve = 5;
        
        [Tooltip("The number of enemies that spawn per section of road")]
        [Min(0)] public int EnemiesPerChunk = 15;
        
        [Tooltip("Limit on the number of active enemies on screen at the same time")]
        [Min(1)] public int MaxActiveEnemies = 30;
    }
}