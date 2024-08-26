using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
    private LineRenderer lr;
    private Vector3 grapplepoint;
    private SpringJoint joint;
    public LayerMask grappleable;
    public Transform gunTip, camera, player;
    public float maxDistance;


    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            startgrapple();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            stopgrapple();
        }
    }

    void LateUpdate()
    {
        drawrope();
    }

    void startgrapple()
    {
        RaycastHit hit;
        if(Physics.Raycast(camera.position, camera.forward, out hit, maxDistance, grappleable))
        {
            grapplepoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplepoint;

            float distance = Vector3.Distance(player.position, grapplepoint);

            joint.maxDistance = distance * 0.8f;
            joint.minDistance = distance * 0.25f;

            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

        }

    }

    void drawrope()
    {
        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(0, grapplepoint);
    }

    void stopgrapple()
    {

    }

}
