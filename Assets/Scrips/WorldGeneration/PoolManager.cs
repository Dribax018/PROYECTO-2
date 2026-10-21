using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// Gestor de Object Pooling. Mantiene un ObjectPool por cada prefab.
/// Colócalo en un GameObject vacío en (0,0,0) y escala 1.
/// </summary>
public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    [SerializeField] private int defaultCapacity = 10;
    [SerializeField] private int maxSize = 50;

    private readonly Dictionary<GameObject, ObjectPool<GameObject>> pools =
        new Dictionary<GameObject, ObjectPool<GameObject>>();
    private readonly Dictionary<GameObject, GameObject> prefabOf =
        new Dictionary<GameObject, GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>Obtiene una instancia activa del prefab, ya colocada.</summary>
    public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!pools.TryGetValue(prefab, out ObjectPool<GameObject> pool))
        {
            pool = CreatePool(prefab);
            pools.Add(prefab, pool);
        }

        GameObject instance = pool.Get();
        instance.transform.SetPositionAndRotation(position, rotation);
        return instance;
    }

    /// <summary>Devuelve una instancia a su pool (se desactiva en OnRelease).</summary>
    public void Release(GameObject instance)
    {
        if (prefabOf.TryGetValue(instance, out GameObject prefab))
        {
            pools[prefab].Release(instance);
        }
        else
        {
            Destroy(instance);
        }
    }

    private ObjectPool<GameObject> CreatePool(GameObject prefab)
    {
        return new ObjectPool<GameObject>(
            createFunc: () =>
            {
                GameObject go = Instantiate(prefab, transform);
                prefabOf[go] = prefab;
                return go;
            },
            actionOnGet: go => go.SetActive(true),
            actionOnRelease: go => go.SetActive(false),
            actionOnDestroy: go =>
            {
                prefabOf.Remove(go);
                Destroy(go);
            },
            collectionCheck: true,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize);
    }
}