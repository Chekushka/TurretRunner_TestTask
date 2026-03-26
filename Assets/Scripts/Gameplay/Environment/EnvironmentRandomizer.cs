using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Environment
{
    public class EnvironmentRandomizer : MonoBehaviour
    {
        [SerializeField] private GameObject[] _propsPrefabs;
        [SerializeField] private Transform[] _spawnPoints;
        [Range(0, 1)] [SerializeField] private float _spawnChance = 0.5f;

        private readonly List<GameObject> _activeProps = new();

        public void Randomize()
        {
            ClearProps();

            foreach (var point in _spawnPoints)
            {
                if (Random.value > _spawnChance) continue;

                var prefab = _propsPrefabs[Random.Range(0, _propsPrefabs.Length)];
                var prop = Instantiate(prefab, point.position, Quaternion.Euler(0, Random.Range(0, 360), 0), transform);
            
                prop.transform.localScale *= Random.Range(0.8f, 1.2f);
                _activeProps.Add(prop);
            }
        }

        private void ClearProps()
        {
            foreach (var prop in _activeProps)
            {
                if (prop != null) Destroy(prop);
            }
            _activeProps.Clear();
        }
    }
}