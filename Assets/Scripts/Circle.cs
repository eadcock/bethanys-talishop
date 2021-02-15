using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : MonoBehaviour
{
    private float diameter;
    private float scaleFactor;
    private Vector2 currentPos;
    private Vector2 prevPos;
    private Dot[] connectedDots;

    public float X
    {
        get { return transform.position.x; }
    }

    public float Y
    {
        get { return transform.position.y; }
    }

    public float Radius
    {
        get {  return diameter/2; }
    }

    public bool IsDrawn
    {
        get { return connectedDots != null; }
    }

    public Dot[] ConnectedDots => connectedDots;

    // Start is called before the first frame update
    void Start()
    {

        currentPos = prevPos = transform.position;
        Sprite circleSprite = gameObject.GetComponent<SpriteRenderer>().sprite;
        scaleFactor = transform.localScale.x;
        connectedDots = null;

        ResizeCircle(new Vector2(0, 0), new Vector2(-2, 0));
    }

    // Update is called once per frame
    void Update()
    {
        if(currentPos != prevPos)
        {
            transform.position = currentPos;
            transform.localScale = new Vector3(diameter * scaleFactor, diameter * scaleFactor, 0);
        }
    }

    public void ResizeCircle(Vector2 dotPos, Vector2 mousePos)
    {
        //Gets displacement between mouse position and the starting dot position
        Vector2 displacement = mousePos - dotPos;
        //Gets displacements angle
        float angle = Vector2.SignedAngle(new Vector2(1, 0), displacement);


        //Decides which direction to draw circle
        Vector2 directon = new Vector2(0,0);
        if (angle <= 45 && angle > -45)
        {
            //Right
            directon.x = 1;
        }
        else if(angle <= 135 && angle > 45)
        {
            //Up
            directon.y = 1;

        }
        else if(angle <= -135 || angle > 135)
        {
            //Left
            directon.x = -1;

        }
        else if(angle <= -45 && angle > -135)
        {
            //Down
            directon.y = -1;

        }

        diameter = Vector3.Dot(directon, displacement);

        prevPos = currentPos;

        currentPos = dotPos + (directon * diameter/2);
    }

    public void AddCircle(List<Dot> dots)
    {
        foreach(Dot d in dots)
        {
            d.AddCircle();
        }
        connectedDots = dots.ToArray();
    }

    public void DeleteCircle()
    {
        foreach (Dot d in connectedDots)
        {
            d.RemoveCircle();
        }
        Destroy(gameObject);
    }
}
