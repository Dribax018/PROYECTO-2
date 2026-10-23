using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Genera el mundo carril a carril alrededor del jugador.
/// - Mantiene una ventana de carriles: rowsAhead por delante y rowsBehind por detrás.
/// - Usa un índice de progreso (fila máxima alcanzada) en vez de depender solo de coordenadas.
/// - Elige el tipo de carril con aleatoriedad controlada (pesos, fila mínima, máximo de seguidos).
/// - Escala la dificultad con el progreso, con límites mínimos y máximos.
///
/// Supone que el jugador avanza hacia +Z, que cada fila mide tileSize y que empieza en z = 0.
/// Colócalo en un GameObject vacío en (0,0,0).
/// </summary>
public class LaneGenerator : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform player;

    [Header("Cuadrícula")]
    [SerializeField] private float tileSize = 1f;
    [Tooltip("Columnas a cada lado de la central. 6 = 13 columnas en total")]
    [SerializeField] private int columnsEachSide = 6;

    [Header("Ventana de carriles")]
    [SerializeField] private int rowsAhead = 12;
    [SerializeField] private int rowsBehind = 4;

    [Header("Tipos de carril")]
    [Tooltip("Césped sin obstáculos: se usa al inicio y como respaldo")]
    [SerializeField] private LaneConfig safeConfig;
    [SerializeField] private LaneConfig[] laneConfigs;
    [Tooltip("Filas iniciales que siempre son seguras")]
    [SerializeField] private int safeStartRows = 3;

    [Header("Dificultad")]
    [Tooltip("Filas necesarias para llegar a la dificultad máxima")]
    [SerializeField] private int rowsToMaxDifficulty = 100;
    [Tooltip("Multiplicador máximo de velocidad (límite superior)")]
    [SerializeField] private float maxSpeedMultiplier = 2f;
    [Tooltip("Multiplicador mínimo del intervalo entre vehículos (límite inferior)")]
    [SerializeField] private float minIntervalMultiplier = 0.6f;

    /// <summary>Fila máxima alcanzada. Útil para puntuación o dificultad.</summary>
    public int Progress { get; private set; }
    public event Action<int> OnProgressChanged;

    private readonly Queue<Lane> lanes = new Queue<Lane>();
    private readonly List<LaneConfig> candidates = new List<LaneConfig>();
    private int nextRow;
    private int safeColumn;
    private LaneType? lastType;
    private int sameTypeCount;
    private LaneConfig lastConfig;
    private int sameConfigCount;

    private void Start()
    {
        if (player == null || safeConfig == null || laneConfigs == null || laneConfigs.Length == 0)
        {
            Debug.LogError("LaneGenerator: falta asignar player, safeConfig o laneConfigs.");
            enabled = false;
            return;
        }

        for (int row = -rowsBehind; row <= rowsAhead; row++)
        {
            SpawnLane(row);
        }
        nextRow = rowsAhead + 1;
    }

    private void Update()
    {
        // Índice de progreso: solo sube, nunca baja aunque el jugador retroceda
        int playerRow = Mathf.RoundToInt(player.position.z / tileSize);
        if (playerRow > Progress)
        {
            Progress = playerRow;
            OnProgressChanged?.Invoke(Progress);
        }
        while (nextRow <= Progress + rowsAhead)
        {
            SpawnLane(nextRow);
            nextRow++;
        }
        // Retirar los muy alejados por detrás (se deja un margen)
        while (lanes.Count > 0 && lanes.Peek().Row < Progress - rowsBehind)
        {
            lanes.Dequeue().Retire();
        }
    }

    private void SpawnLane(int row)
    {
        LaneConfig config = PickConfig(row);
        float difficulty = Mathf.Clamp01(Mathf.Max(row, 0) / (float)rowsToMaxDifficulty);
        float speedMultiplier = Mathf.Lerp(1f, maxSpeedMultiplier, difficulty);
        float intervalMultiplier = Mathf.Lerp(1f, minIntervalMultiplier, difficulty);
        int nextSafeColumn = Mathf.Clamp(safeColumn + UnityEngine.Random.Range(-1, 2),
                                         -columnsEachSide, columnsEachSide);
        Vector3 position = new Vector3(0f, 0f, row * tileSize);
        Lane lane = Instantiate(config.lanePrefab, position, Quaternion.identity, transform);
        lane.Init(config, row, tileSize, columnsEachSide,
                  speedMultiplier, intervalMultiplier, safeColumn, nextSafeColumn);

        safeColumn = nextSafeColumn;
        lanes.Enqueue(lane);
    }
    private LaneConfig PickConfig(int row)
    {
        if (row < safeStartRows) return Register(safeConfig);
        if (lastConfig != null && sameConfigCount < lastConfig.minConsecutive)
        {
            return Register(lastConfig);
        }

        candidates.Clear();
        float totalWeight = 0f;

        foreach (LaneConfig config in laneConfigs)
        {
            if (config == null || row < config.minRow) continue;
            if (config.type == lastType &&
                config.maxConsecutive - sameTypeCount < config.minConsecutive) continue;

            candidates.Add(config);
            totalWeight += config.weight;
        }

        if (candidates.Count == 0) return Register(safeConfig);

        float roll = UnityEngine.Random.value * totalWeight;
        foreach (LaneConfig config in candidates)
        {
            roll -= config.weight;
            if (roll <= 0f) return Register(config);
        }
        return Register(candidates[candidates.Count - 1]);
    }
    private LaneConfig Register(LaneConfig config)
    {
        if (config == lastConfig)
        {
            sameConfigCount++;
        }
        else
        {
            lastConfig = config;
            sameConfigCount = 1;
        }

        if (config.type == lastType)
        {
            sameTypeCount++;
        }
        else
        {
            lastType = config.type;
            sameTypeCount = 1;
        }
        return config;
    }
}