using System.Collections.Generic;
using UnityEngine;

public enum ParticleKey
{
    MouseClick,
    DeleteBall,
    Combo,
    ItemSpawn,
}

public static class ParticleGenerator
{
    // キーごとに複数Prefabを持てる
    private static readonly Dictionary<ParticleKey, List<GameObject>> prefabMap = new();

    // ラウンドロビン用インデックス（均等に使いたい時に使用）
    private static readonly Dictionary<ParticleKey, int> nextIndex = new();

    private static Transform instanceParent;
    private static bool initialized;

    public static void Initialize(Transform parent, List<ParticleInit.ParticleInfo> list)
    {
        if (initialized) return;

        instanceParent = parent;
        prefabMap.Clear();
        nextIndex.Clear();

        if (list != null)
        {
            foreach (var info in list)
            {
                if (info == null) continue;

                if (!prefabMap.TryGetValue(info.key, out var bucket))
                {
                    bucket = new List<GameObject>();
                    prefabMap.Add(info.key, bucket);
                    nextIndex[info.key] = 0;
                }

                if (info.prefabs != null)
                {
                    foreach (var prefab in info.prefabs)
                    {
                        if (prefab == null) continue;
                        bucket.Add(prefab);
                    }
                }
            }
        }

        // 空バケット警告
        foreach (var kv in prefabMap)
        {
            if (kv.Value.Count == 0)
                Debug.LogWarning($"[ParticleManager] Key '{kv.Key}' に有効なPrefabがありません。");
        }

        initialized = true;
    }

    /// <summary>
    /// 単発再生（登録群からランダム or ラウンドロビン選択）
    /// </summary>
    public static void Play(
        ParticleKey key,
        Vector3 position,
        Quaternion rotation,
        bool randomPick = true)
    {
        InternalPlay(key, position, rotation, 1, 0f, randomPick);
    }

    /// <summary>
    /// 複数同時スポーン（count個）／半径radius内にランダム散布
    /// </summary>
    public static void Play(
        ParticleKey key,
        Vector3 position,
        Quaternion rotation,
        int count,
        float scatterRadius = 0f,
        bool randomPick = true)
    {
        InternalPlay(key, position, rotation, Mathf.Max(1, count), Mathf.Max(0f, scatterRadius), randomPick);
    }

    /// <summary>
    /// 2D糖衣
    /// </summary>
    public static void Play(
        ParticleKey key,
        Vector2 position,
        int count = 1,
        float scatterRadius = 0f,
        bool randomPick = true,
        float z = 0f)
        => Play(key, new Vector3(position.x, position.y, z), Quaternion.identity, count, scatterRadius, randomPick);

    /// <summary>
    /// 登録されている全バリエーションを一斉に出す
    /// </summary>
    public static void PlayAllVariants(
        ParticleKey key,
        Vector3 position,
        Quaternion rotation,
        float scatterRadius = 0f)
    {
        if (!CheckReady(key, out var list)) return;

        foreach (var prefab in list)
        {
            var pos = Scatter(position, scatterRadius);
            SpawnAndAutoDestroy(prefab, pos, rotation);
        }
    }

    // 内部実装
    private static void InternalPlay(
        ParticleKey key,
        Vector3 position,
        Quaternion rotation,
        int count,
        float scatterRadius,
        bool randomPick)
    {
        if (!CheckReady(key, out var list)) return;

        for (int i = 0; i < count; i++)
        {
            var prefab = Pick(list, key, randomPick);
            var pos = Scatter(position, scatterRadius);
            SpawnAndAutoDestroy(prefab, pos, rotation);
        }
    }

    private static bool CheckReady(ParticleKey key, out List<GameObject> list)
    {
        list = null;

        if (!initialized)
        {
            Debug.LogWarning("[ParticleManager] Initializeが未実行です");
            return false;
        }
        if (!prefabMap.TryGetValue(key, out list) || list == null || list.Count == 0)
        {
            Debug.LogWarning($"[ParticleManager] 未登録のキー、またはPrefabが空です: {key}");
            return false;
        }
        return true;
    }

    private static GameObject Pick(List<GameObject> list, ParticleKey key, bool randomPick)
    {
        if (list.Count == 1) return list[0];

        if (randomPick)
        {
            int idx = Random.Range(0, list.Count);
            return list[idx];
        }
        else
        {
            int idx = nextIndex[key];
            var prefab = list[idx];
            nextIndex[key] = (idx + 1) % list.Count; // ラウンドロビン
            return prefab;
        }
    }

    private static Vector3 Scatter(Vector3 center, float radius)
    {
        if (radius <= 0f) return center;

        // 2D/俯瞰前提でXZ平面に散布（2DならX,Yにしたければここを変更）
        var offset = Random.insideUnitCircle * radius;
        return new Vector3(center.x + offset.x, center.y, center.z + offset.y);
    }

    private static void SpawnAndAutoDestroy(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        var go = Object.Instantiate(prefab, pos, rot, instanceParent);

        // 子や複数PSを想定して、配下のParticleSystemを一括で扱う
        var systems = go.GetComponentsInChildren<ParticleSystem>(true);
        float maxLife = 0.5f;

        if (systems != null && systems.Length > 0)
        {
            foreach (var ps in systems)
            {
                ps.Play();
                maxLife = Mathf.Max(maxLife, GetApproxLifetime(ps));
            }
        }
        else
        {
            // 念のため保険
            maxLife = 5f;
            Debug.LogWarning("[ParticleManager] ParticleSystemが見つかりませんでした");
        }

        Object.Destroy(go, maxLife);
    }

    private static float GetApproxLifetime(ParticleSystem ps)
    {
        var main = ps.main;
        if (main.loop) return 3f; // ループは適宜短めに切る（必要ならStop管理へ）

        float life = main.duration;

        switch (main.startLifetime.mode)
        {
            case ParticleSystemCurveMode.TwoConstants:
                life += main.startLifetime.constantMax;
                break;
            case ParticleSystemCurveMode.TwoCurves:
                // 簡略：最大値としてconstantを流用（厳密に取りたければcurve評価）
                life += Mathf.Max(main.startLifetime.constant, 0f);
                break;
            default:
                life += main.startLifetime.constant;
                break;
        }
        return Mathf.Max(0.1f, life);
    }
}
