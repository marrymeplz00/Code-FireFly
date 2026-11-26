using UnityEngine;

[ExecuteAlways]
public class FogMemoryWorld : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Renderer fogRenderer;

    [Header("World Bounds")]
    public Vector2 mapMin = new Vector2(-50f, -50f);
    public Vector2 mapMax = new Vector2(50f, 50f);

    [Header("Vision")]
    public float visionRadius = 1.5f;

    [Header("Vision Softness")]
    [Range(0.1f, 5f)]
    public float visionSoftness = 0.7f;

    [Header("Memory")]
    public int resolution = 1024;

    private Texture2D memoryTexture;
    private MaterialPropertyBlock propertyBlock;
    private Vector3 lastPlayerPosition;
    private int radiusX;
    private int radiusY;

    void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();
    }

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("FogMemoryWorld: ยังไม่ได้ใส่ Player");
            return;
        }

        if (fogRenderer == null)
        {
            Debug.LogError("FogMemoryWorld: ยังไม่ได้ใส่ Fog Renderer");
            return;
        }

        // ใน Play Mode เท่านั้น
        if (Application.isPlaying)
        {
            CreateMemoryTexture();
            CalculateMemoryRadius();
            lastPlayerPosition = player.position;
            RevealArea(player.position);
            UpdateShader();
        }
        else
        {
            // Scene Mode
            UpdateShader();
        }
    }

    void Update()
    {
        if (player == null || fogRenderer == null)
            return;

        // Scene Mode
        if (!Application.isPlaying)
        {
            UpdateShader();
            return;
        }
    }

    void LateUpdate()
    {
        if (player == null || fogRenderer == null)
            return;

        // Scene Mode
        if (!Application.isPlaying)
        {
            UpdateShader();
            return;
        }

        // Play Mode
        UpdateShader();

        Vector3 currentPosition = player.position;
        float distance = Vector3.Distance(currentPosition, lastPlayerPosition);

        if (distance < 0.01f)
            return;

        RevealBetweenPoints(lastPlayerPosition, currentPosition);
        lastPlayerPosition = currentPosition;
    }

    void OnValidate()
    {
        // อัปเดตตำแหน่งใน Scene Mode
        if (!Application.isPlaying)
        {
            if (player == null || fogRenderer == null)
                return;

            if (propertyBlock == null)
                propertyBlock = new MaterialPropertyBlock();

            UpdateShader();
        }
    }

    void CreateMemoryTexture()
    {
        memoryTexture = new Texture2D(resolution, resolution, TextureFormat.R8, false);
        memoryTexture.filterMode = FilterMode.Bilinear;
        memoryTexture.wrapMode = TextureWrapMode.Clamp;

        Color32[] pixels = new Color32[resolution * resolution];

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = new Color32(0, 0, 0, 255);
        }

        memoryTexture.SetPixels32(pixels);
        memoryTexture.Apply();

        CalculateMemoryRadius();
        UpdateShader();
    }

    void CalculateMemoryRadius()
    {
        float mapWidth = mapMax.x - mapMin.x;
        float mapHeight = mapMax.y - mapMin.y;

        radiusX = Mathf.CeilToInt(visionRadius / mapWidth * resolution);
        radiusY = Mathf.CeilToInt(visionRadius / mapHeight * resolution);

        radiusX = Mathf.Max(1, radiusX);
        radiusY = Mathf.Max(1, radiusY);
    }

    void UpdateShader()
    {
        if (fogRenderer == null || player == null)
            return;

        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();

        fogRenderer.GetPropertyBlock(propertyBlock);

        // Player Position
        propertyBlock.SetVector("_PlayerPosition", new Vector4(player.position.x, player.position.y, 0f, 0f));

        // Vision
        propertyBlock.SetFloat("_VisionRadius", visionRadius);
        propertyBlock.SetFloat("_Softness", visionSoftness);

        // Memory
        if (memoryTexture != null)
            propertyBlock.SetTexture("_MemoryTex", memoryTexture);

        // Map Bounds
        propertyBlock.SetVector("_MapMin", new Vector4(mapMin.x, mapMin.y, 0f, 0f));
        propertyBlock.SetVector("_MapMax", new Vector4(mapMax.x, mapMax.y, 0f, 0f));

        fogRenderer.SetPropertyBlock(propertyBlock);
    }

    void RevealBetweenPoints(Vector3 from, Vector3 to)
    {
        float distance = Vector3.Distance(from, to);

        int steps = Mathf.Max(1, Mathf.CeilToInt(distance / (visionRadius * 0.35f)));

        for (int i = 0; i <= steps; i++)
        {
            float t = i / (float)steps;
            Vector3 position = Vector3.Lerp(from, to, t);
            RevealArea(position);
        }

        memoryTexture.Apply();
    }

    void RevealArea(Vector3 worldPosition)
    {
        if (memoryTexture == null)
            return;

        float normalizedX = Mathf.InverseLerp(mapMin.x, mapMax.x, worldPosition.x);
        float normalizedY = Mathf.InverseLerp(mapMin.y, mapMax.y, worldPosition.y);

        int centerX = Mathf.RoundToInt(normalizedX * (resolution - 1));
        int centerY = Mathf.RoundToInt(normalizedY * (resolution - 1));

        int minX = Mathf.Max(0, centerX - radiusX);
        int maxX = Mathf.Min(resolution - 1, centerX + radiusX);
        int minY = Mathf.Max(0, centerY - radiusY);
        int maxY = Mathf.Min(resolution - 1, centerY + radiusY);

        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                float dx = (x - centerX) / (float)radiusX;
                float dy = (y - centerY) / (float)radiusY;

                if (dx * dx + dy * dy > 1f)
                    continue;

                memoryTexture.SetPixel(x, y, Color.white);
            }
        }
    }

    public Texture2D GetMemoryTexture()
    {
        return memoryTexture;
    }
}