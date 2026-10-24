using UnityEngine;

/// <summary>
/// Tipos de carril. Para añadir Agua o Vía de tren, añade el valor aquí
/// y un nuevo case en Lane.Init().
/// </summary>
public enum LaneType
{
    Grass,
    Road
}

/// <summary>
/// Datos de una variante de carril (carretera lenta, carretera rápida, césped con árboles...).
/// Crear desde: Assets > Create > Lanes > Lane Config.
/// Las variantes se crean cambiando valores, no creando clases nuevas.
/// </summary>
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

    [Header("Tráfico (carretera)")]
    public GameObject[] vehiclePrefabs;
    public Vector2 speedRange = new Vector2(4f, 7f);
    [Tooltip("Segundos entre vehículos consecutivos (mín, máx)")]
    public Vector2 spawnIntervalRange = new Vector2(1.5f, 3f);
    [Tooltip("Separación mínima entre vehículos en unidades de mundo. Súbela si tus vehículos son largos")]
    [Min(0f)] public float minGapUnits = 4f;
    [Tooltip("Carriles que ocupa un vehículo de esta variante")]
    [Min(1)] public int laneSpan = 1;

    /// <summary>laneSpan seguro (nunca menor que 1).</summary>
    public int Span => Mathf.Max(1, laneSpan);

    /// <summary>
    /// Racha mínima real: al menos minConsecutive y siempre un múltiplo de laneSpan,
    /// para que los grupos de carriles queden completos.
    /// </summary>
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