using System.Collections.Generic;
using UnityEngine;

public class CamObstacleHide : MonoBehaviour
{
    public Transform player;                    // 플레이어 위치
    public LayerMask obstacleMask;              // 가릴 수 있는 오브젝트 레이어
    public Material transparentMaterial;        // 투명 머티리얼
    public float checkRadius = 0.3f;            // SphereCast 반경

    private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();
    private List<Renderer> currentObstacles = new List<Renderer>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ShowObstacles(); // 먼저 이전 프레임의 투명화 해제
        HideObstacles(); // 현재 가리는 오브젝트 감지 후 투명화
    }

    void HideObstacles()
    {
        Vector3 direction = player.position - transform.position;
        float distance = direction.magnitude;

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, checkRadius, direction.normalized, distance, obstacleMask);

        foreach (RaycastHit hit in hits)
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (rend != null && !currentObstacles.Contains(rend))
            {
                originalMaterials[rend] = rend.materials;
                MakeTransparent(rend);
                currentObstacles.Add(rend);
            }
        }
    }

    void ShowObstacles()
    {
        foreach (Renderer rend in currentObstacles)
        {
            if (rend != null && originalMaterials.ContainsKey(rend))
            {
                rend.materials = originalMaterials[rend];
            }
        }

        currentObstacles.Clear();
        originalMaterials.Clear();
    }

    void MakeTransparent(Renderer rend)
    {
        Material[] transparentMats = new Material[rend.materials.Length];
        for (int i = 0; i < transparentMats.Length; i++)
        {
            transparentMats[i] = transparentMaterial;
        }
        rend.materials = transparentMats;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
