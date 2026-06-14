using System.Collections.Generic;
using UnityEngine;

public class ParticleInit : MonoBehaviour
{
    [SerializeField] private List<ParticleInfo> particleList;

    void Awake()
    {
        ParticleGenerator.Initialize(transform, particleList);
    }

    [System.Serializable]
    public class ParticleInfo
    {
        [Tooltip("識別キー（enumで指定）")]
        public ParticleKey key;

        [Tooltip("このキーで使うパーティクル群（ランダム/ラウンドロビンで出せます）")]
        public List<GameObject> prefabs = new List<GameObject>();
    }
}
