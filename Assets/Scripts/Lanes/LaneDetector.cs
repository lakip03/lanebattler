using UnityEngine;

public class LaneDetector
{
    private readonly Camera camera;
    private readonly LayerMask laneLayer;
    private readonly float heightOffset;

    public LaneDetector(Camera camera, LayerMask laneLayer, float heightOffset = 0f)
    {
        this.camera = camera;
        this.laneLayer = laneLayer;
        this.heightOffset = heightOffset;
    }

    public Lane DetectLane(Vector2 screenPosition)
    {
        Ray ray = camera.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, laneLayer))
        {
            Lane lane = hit.collider.GetComponent<Lane>();
            
            if (lane == null)
            {
                lane = hit.collider.GetComponentInParent<Lane>();
            }

            return lane;
        }

        return null;
    }
    
}