using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "TurretRunner/GameSettings")]
    public class GameSettings : ScriptableObject
    {
        [Header("Car Movement")] 
        public float CarForwardSpeed = 10;
        public float CarTiltSpeed = 5f;
        public int CarMaxHealth = 200;
        public float DriftSmoothTime = 0.8f;
        public float MaxTiltAngle = 12f;
        public float MinDriftInterval = 3f;
        public float MaxDriftInterval = 5f;

        [Header("Turret")] 
        public int WeaponDamage = 30;
        public float FireRate = 0.2f;
        public float WeaponRange = 30f;
        [Range(0f, 1f)] public float CritChance = 0.2f;
        public float CritMultiplier = 2.0f;
        public float RotationSpeed = 15f;
        public float MaxAngle = 70f;
        
        [Header("Enemies")]
        public int EnemyMaxHealth = 100;
        public float EnemySpeed = 4f;
        public int EnemyDamageToCar = 10;
        public float DetectionRadius = 15f;
        public float AttackRadius = 3.5f;
        public float StunDuration = 0.4f;

        [Header("Level")]
        public float LevelDistance = 200f;
        public int RoadSegmentsInReserve = 5;
        public int EnemiesPerChunk = 15;
        public int MaxActiveEnemies = 30;
        public float RoadWidth = 3.5f;
    }
}