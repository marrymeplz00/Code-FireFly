using UnityEngine;

public class FogWorldController : MonoBehaviour
{
    public Transform player;

    public float visionRadius = 5f;

    public float softness = 1.5f;

    private Material fogMaterial;

    void Start()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();

        // กำหนดลำดับการ Render ให้ Fog อยู่ด้านบน
        renderer.sortingOrder = 100;

        fogMaterial = new Material(renderer.material);

        renderer.material = fogMaterial;
    }

    void Update()
    {
        if (player == null)
            return;

        fogMaterial.SetVector(
            "_PlayerPosition", player.position
        );

        fogMaterial.SetFloat(
            "_VisionRadius", visionRadius
        );

        fogMaterial.SetFloat(
            "_Softness", softness
        );
    }
}