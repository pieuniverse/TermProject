using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public float targetOffset = 2.0f;
    public Transform targetTr;
    private Transform camTr;

    [Range(2.0f, 20.0f)]
    public float distance = 3.0f;

    [Range(0.0f, 10.0f)]
    public float height = 1.0f;

    public float damping = 0.1f;

    private Vector3 velocity = Vector3.zero;

    private bool isFirstPerson = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        camTr = GetComponent<Transform>();
    }

    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            isFirstPerson = !isFirstPerson;
        }

        Vector3 targetPosition;

        if (isFirstPerson)
        {
            targetPosition = targetTr.position + targetTr.up * 1.2f; // 캐릭터 키에 맞게 조정
            camTr.position = Vector3.Lerp(camTr.position, targetPosition, Time.deltaTime * 10f);
            camTr.rotation = Quaternion.Lerp(camTr.rotation, targetTr.rotation, Time.deltaTime * 10f);
        }
        else
        {
            Vector3 pos = targetTr.position 
                        + (-targetTr.forward * distance) 
                        + (Vector3.up * height);
                    camTr.position = Vector3.SmoothDamp(camTr.position, //        ġ
                                                        pos,            //   ǥ   ġ
                                                        ref velocity,   //       ӵ 
                                                        damping);
                    camTr.LookAt(targetTr.position + (targetTr.up * targetOffset));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
