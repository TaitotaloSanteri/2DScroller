using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform player;

    [SerializeField]
    [Range(0.01f, 1f)]
    private float cameraFollowSpeed;
    private static float cameraY = -10000f;
    [SerializeField]
    private Vector2 offset;
    private Vector3 velocity = Vector3.zero;
    private Vector3 oldPosition;
    private float minX, maxX;

    private void Start()
    {
        float width = Camera.main.orthographicSize * Screen.width / Screen.height;
        minX = LayerManager.instance.worldStart.position.x + width;
        maxX = LayerManager.instance.worldEnd.position.x - width;
        transform.position = GetWantedPosition(false);
        cameraY = transform.position.y;
    }

    private Vector3 GetWantedPosition(bool followY = true)
    {
        float x = Mathf.Clamp(player.position.x + offset.x, minX, maxX);
        float y = followY ? player.position.y + offset.y : transform.position.y;
        if (!followY && cameraY != -10000f)
        {
            y = cameraY;
        }
        return new Vector3(x, y, -10f);
    }

    private void Update()
    {
       
        Vector3 wantedPosition = GetWantedPosition(false);

        LayerManager.instance.MoveLayers(transform.position - oldPosition);
        oldPosition = transform.position;

        transform.position = Vector3.SmoothDamp(transform.position,
                                                wantedPosition,
                                                ref velocity,
                                                cameraFollowSpeed);
    }

}
