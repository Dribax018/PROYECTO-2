using UnityEngine;
public enum LaneType
{
    Grass,
    Road,
    Gravel,
    Water,
    Shooter
}
[CreateAssetMenu(menuName = "Lanes/Lane Config", fileName = "NewLaneConfig")]
public class LaneConfig : ScriptableObject
{
    [Header("General")]
    public LaneType type;
    [Tooltip("Prefab del carril: suelo + script Lane + puntos de aparición")]
    public Lane lanePrefab;

    [Header("Reglas de aparición")]
    [Min(0f)] public float weight = 1f;
    [Tooltip("Fila a partir de la cual puede aparecer este carril")]
    [Min(0)] public int minRow = 0;
    [Tooltip("Mínimo de carriles seguidos de esta variante: cuando sale, se repite hasta llegar a este número")]
    [Min(1)] public int minConsecutive = 1;
    [Tooltip("Máximo de carriles seguidos de este tipo")]
    [Min(1)] public int maxConsecutive = 3;

    [Header("Obstáculos (césped)")]
    public GameObject[] obstaclePrefabs;
    [Min(0)] public int minObstacles = 0;
    [Min(0)] public int maxObstacles = 4;

    [Header("Tráfico (carretera) / Proyectiles (Shooter)")]
    [Tooltip("Vehículos en carretera. En carriles Shooter, aquí van los proyectiles (con LaneVehicle)")]
    public GameObject[] vehiclePrefabs;
    [Tooltip("Velocidad. En Shooter es la velocidad del proyectil")]
    public Vector2 speedRange = new Vector2(4f, 7f);
    [Tooltip("Segundos entre vehículos/disparos consecutivos (mín, máx)")]
    public Vector2 spawnIntervalRange = new Vector2(1.5f, 3f);
    [Tooltip("Separación mínima entre vehículos en unidades de mundo. Súbela si tus vehículos son largos")]
    [Min(0f)] public float minGapUnits = 4f;
    [Tooltip("Carriles que ocupa un vehículo de esta variante")]
    [Min(1)] public int laneSpan = 1;

    [Header("Troncos (agua)")]
    public GameObject[] logPrefabs;
    public Vector2 logSpeedRange = new Vector2(2f, 4f);
    [Tooltip("Segundos entre troncos consecutivos")]
    public Vector2 logSpawnIntervalRange = new Vector2(2f, 3f);
    [Tooltip("Separación mínima entre troncos")]
    [Min(0f)] public float logMinGapUnits = 5f;

    [Header("Disparadores (Shooter)")]
    [Tooltip("Enemigo que se coloca en el lado desde el que dispara")]
    public GameObject enemyPrefab;
    [Tooltip("Altura a la que sale el proyectil. Debe quedar por encima del collider agachado")]
    public float projectileHeight = 0.8f;

    public int Span => Mathf.Max(1, laneSpan);
    public int EffectiveMinConsecutive
    {
        get
        {
            int min = Mathf.Max(minConsecutive, Span);
            return (min + Span - 1) / Span * Span;
        }
    }

    private void OnValidate()
    {
        if (maxConsecutive < EffectiveMinConsecutive) maxConsecutive = EffectiveMinConsecutive;
    }
}