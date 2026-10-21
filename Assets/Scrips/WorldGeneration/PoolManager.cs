using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    [SerializeField] private int defaultCapacity = 10;
    [SerializeField] private int maxSize = 50;

    private readonly Dictionary<GameObject, ObjectPool<GameObject>> pools =
        new Dictionary<GameObject, ObjectPool<GameObject>>();

    // Para saber a qué pool pertenece cada instancia cuando se devuelve
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
    public void Release(GameObject instance)
    {
        if (prefabOf.TryGetValue(instance, out GameObject prefab))
        {
            pools[prefab].Release(instance);
        }
        else
        {
            Destroy(instance); // No venía del pool
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
