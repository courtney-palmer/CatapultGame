using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDragging : MonoBehaviour {
    public float maxStretch = 3.0f;
    public LineRenderer leftArm;
    public LineRenderer rightArm;
    private SpringJoint2D spring;
    private Transform catapult;
    private Ray rayToMouse;
    private Ray leftCatapultToProjectile;
    private float maxStretchSqr;
    private float circleRadius;
    private bool clickedOn;
    private Vector2 prevVelocity;
    private new Rigidbody2D rigidbody2D;
    private CircleCollider2D collider2D;

    private void Awake(){
        spring = GetComponent<SpringJoint2D>();
        catapult = spring.connectedBody.transform;
    }

    void Start () {
        rigidbody2D = GetComponent<Rigidbody2D>();
        LineRendererSetup();
        rayToMouse = new Ray(catapult.position, Vector3.zero);
        maxStretchSqr = maxStretch * maxStretch;
        leftCatapultToProjectile = new Ray(leftArm.transform.position, Vector3.zero);
        CircleCollider2D circle = collider2D as CircleCollider2D;
        circleRadius = circle.radius;
    }

	void Update () {
        if (clickedOn)
            Dragging();
        if(spring != null) {
            if(!rigidbody2D.isKinematic && prevVelocity.sqrMagnitude > rigidbody2D.velocity.sqrMagnitude){
                Destroy(spring);
                rigidbody2D.velocity = prevVelocity;
            }
            if (!clickedOn)
                prevVelocity = rigidbody2D.velocity;

            LineRendererUpdate();
        }
        else{
            leftArm.enabled = false;
            rightArm.enabled = false;
        }
	}

    void LineRendererSetup()
    {
        leftArm.SetPosition(0, leftArm.transform.position);
        rightArm.SetPosition(0, rightArm.transform.position);

        leftArm.sortingLayerName = "MiddleGround";
        rightArm.sortingLayerName = "MiddleGround";

        leftArm.sortingOrder = 2;
        rightArm.sortingOrder = 1;
    }

    void LineRendererUpdate(){
        Vector2 catapultToProjectile = transform.position - leftArm.transform.position;
        leftCatapultToProjectile.direction = catapultToProjectile;
        Vector3 holdpoint = leftCatapultToProjectile.GetPoint(catapultToProjectile.magnitude + circleRadius);
        leftArm.SetPosition(1, holdpoint);
        rightArm.SetPosition(1, holdpoint);
    }

    void OnMouseDown(){
        spring.enabled = false;
        clickedOn = true;
    }

    void OnMouseUp(){
        spring.enabled = true;
        rigidbody2D.isKinematic = false;
        clickedOn = false;
    }

    void Dragging()
    {
        Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 catapultToMouse = mouseWorldPoint - catapult.position;
        if (catapultToMouse.sqrMagnitude > maxStretchSqr){
            rayToMouse.direction = catapultToMouse;
            mouseWorldPoint = rayToMouse.GetPoint(maxStretch);
        }
        mouseWorldPoint.z = 0f;
        transform.position = mouseWorldPoint;
    }
}
