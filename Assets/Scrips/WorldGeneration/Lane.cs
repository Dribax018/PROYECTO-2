using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Un carril (fila) del mundo. Va en el prefab del carril, junto al suelo y a los dos
/// puntos de aparición. LaneGenerator lo configura con Init() y él se encarga de
/// sus obstáculos (césped) o de su tráfico (carretera).
/// <summary>
public class Lane : MonoBehaviour
{
    [Header("Puntos de aparición (en los extremos, fuera de la vista y a la altura del suelo)")]
    [Tooltip("Rotación Y = 90: los vehículos que salen de aquí viajan hacia la derecha (+X)")]
    [SerializeField] private Transform leftPoint;
    [Tooltip("Rotación Y = -90: los vehículos que salen de aquí viajan hacia la izquierda (-X)")]
    [SerializeField] private Transform rightPoint;

    public int Row { get; private set; }

    private LaneConfig config;
    private float tileSize;
    private int columnsEachSide;
    private Transform spawnPoint;
    private Transform exitPoint;
    private float speed;
    private float intervalMultiplier = 1f;
    private float spawnTimer;
    private readonly List<LaneVehicle> activeVehicles = new List<LaneVehicle>();

    /// <param name="safeColumn">Columna que debe quedar libre en esta fila.</param>
    /// <param name="nextSafeColumn">Columna libre de la fila siguiente (a ±1 de safeColumn).</param>
    public void Init(LaneConfig config, int row, float tileSize, int columnsEachSide,
                     float speedMultiplier, float intervalMultiplier,
                     int safeColumn, int nextSafeColumn)
    {
        this.config = config;
        this.Row = row;
        this.tileSize = tileSize;
        this.columnsEachSide = columnsEachSide;
        this.intervalMultiplier = intervalMultiplier;

        switch (config.type)
        {
            case LaneType.Grass:
                SpawnObstacles(safeColumn, nextSafeColumn);
                break;
            case LaneType.Road:
                SetupTraffic(speedMultiplier);
                break;
        }
    }

    private void Update()
    {
        if (spawnPoint == null) return;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnVehicleAt(spawnPoint.position);
            spawnTimer = NextInterval();
        }
    }

    private void SpawnObstacles(int safeColumn, int nextSafeColumn)
    {
        if (config.obstaclePrefabs == null || config.obstaclePrefabs.Length == 0) return;
        List<int> columns = new List<int>();
        for (int c = -columnsEachSide; c <= columnsEachSide; c++)
        {
            if (c != safeColumn && c != nextSafeColumn) columns.Add(c);
        }

        int count = Mathf.Min(Random.Range(config.minObstacles, config.maxObstacles + 1), columns.Count);
        for (int i = columns.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (columns[i], columns[j]) = (columns[j], columns[i]);
        }

        for (int i = 0; i < count; i++)
        {
            GameObject prefab = config.obstaclePrefabs[Random.Range(0, config.obstaclePrefabs.Length)];
            GameObject obstacle = Instantiate(prefab, transform);
            Vector3 local = obstacle.transform.localPosition;
            obstacle.transform.localPosition = new Vector3(columns[i] * tileSize, local.y, 0f);
        }
    }
    private void SetupTraffic(float speedMultiplier)
    {
        if (config.vehiclePrefabs == null || config.vehiclePrefabs.Length == 0)
        {
            Debug.LogWarning($"LaneConfig '{config.name}' no tiene vehiclePrefabs.");
            return;
        }

        bool leftToRight = Random.value < 0.5f;
        spawnPoint = leftToRight ? leftPoint : rightPoint;
        exitPoint = leftToRight ? rightPoint : leftPoint;

        speed = Random.Range(config.speedRange.x, config.speedRange.y) * speedMultiplier;

        PrewarmVehicles();
        spawnTimer = NextInterval();
    }

    private float NextInterval()
    {
        float interval = Random.Range(config.spawnIntervalRange.x, config.spawnIntervalRange.y) * intervalMultiplier;
        float minInterval = config.minGapUnits / Mathf.Max(speed, 0.01f);
        return Mathf.Max(interval, minInterval);
    }

    private void PrewarmVehicles()
    {
        Vector3 start = spawnPoint.position;
        Vector3 direction = (exitPoint.position - start).normalized;
        float laneLength = Vector3.Distance(start, exitPoint.position);

        float distance = Random.Range(0f, speed * NextInterval());
        while (distance < laneLength)
        {
            SpawnVehicleAt(start + direction * distance);
            distance += speed * NextInterval();
        }
    }

    private void SpawnVehicleAt(Vector3 position)
    {
        GameObject prefab = config.vehiclePrefabs[Random.Range(0, config.vehiclePrefabs.Length)];
        GameObject instance = PoolManager.Instance.Get(prefab, position, spawnPoint.rotation);

        LaneVehicle vehicle = instance.GetComponent<LaneVehicle>();
        if (vehicle == null)
        {
            Debug.LogError($"El prefab '{prefab.name}' necesita el componente LaneVehicle.");
            PoolManager.Instance.Release(instance);
            return;
        }

        vehicle.Init(speed, exitPoint.position, OnVehicleFinished);
        activeVehicles.Add(vehicle);
    }

    private void OnVehicleFinished(LaneVehicle vehicle)
    {
        activeVehicles.Remove(vehicle);
        PoolManager.Instance.Release(vehicle.gameObject);
    }

    /// Retira el carril: devuelve sus vehículos al pool (no son hijos suyos) y lo destruye.
    public void Retire()
    {
        foreach (LaneVehicle vehicle in activeVehicles)
        {
            PoolManager.Instance.Release(vehicle.gameObject);
        }
        activeVehicles.Clear();
        Destroy(gameObject);
    }
}