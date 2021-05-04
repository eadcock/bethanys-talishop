using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using quiet;

public class Circle : MonoBehaviour
{
    public static bool operator ==(Circle c1, Circle c2) => c1 switch
    {
        null => false,
        _ when c1.ConnectedDots.Contains(c2.ConnectedDots[0]) &&
            c1.ConnectedDots.Contains(c2.ConnectedDots[1]) &&
            c1.ConnectedDots.Contains(c2.ConnectedDots[2]) &&
            c1.ConnectedDots.Contains(c2.ConnectedDots[3]) => true,
        _ => false,
    };

    public static bool operator !=(Circle c1, Circle c2)
    {
        if (!c1 && !c2) return false;

        if (!c1 || !c2) return true;

        return !c1.ConnectedDots.Contains(c2.ConnectedDots[0]) ||
            !c1.ConnectedDots.Contains(c2.ConnectedDots[1]) ||
            !c1.ConnectedDots.Contains(c2.ConnectedDots[2]) ||
            !c1.ConnectedDots.Contains(c2.ConnectedDots[3]);
    }

    public override bool Equals(object other)
    {
        return this == other as Circle;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public void Deconstruct(out Dot d1, out Dot d2, out Dot d3, out Dot d4) => (d1, d2, d3, d4) = (connectedDots[0], connectedDots[1], connectedDots[2], connectedDots[3]);

    private float diameter;
    private float scaleFactor;
    private Vector2 currentPos;
    private Vector2 CurrentPos 
    { 
        get => currentPos; 
        set => currentPos = value; 
    }
    private Vector2 prevPos;
    private Dot[] connectedDots;
    private SpriteRenderer sr;
    private bool phantom;

    public float X => transform.position.x;

    public float Y => transform.position.y;

    public float Radius => diameter / 2;

    public bool IsDrawn => connectedDots != null;

    public Dot[] ConnectedDots => connectedDots;

    // Start is called before the first frame update
    void Start()
    {
        phantom = false;
        CurrentPos = prevPos = transform.position;
        scaleFactor = transform.localScale.x;
        connectedDots = null;
        sr = GetComponent<SpriteRenderer>();
        GameMaster.Instance.Audio.PlaySoundFX("Start circle");
    }

    // Update is called once per frame
    void Update()
    {
        if (!sr.enabled) sr.enabled = true;

        if (CurrentPos != prevPos)
        {
            transform.position = CurrentPos.FillDim(Dimension.Z, 0.5f);
            transform.localScale = new Vector3(diameter * scaleFactor, diameter * scaleFactor, 0);
        }
    }

    /// <summary>
    /// Calculates the resized circle. The circle's size is updated during the next call of Update()
    /// </summary>
    /// <param name="dotPos"></param>
    /// <param name="mousePos"></param>
    public void ResizeCircle(Vector2 dotPos, Vector2 mousePos)
    {
        //Gets displacement between mouse position and the starting dot position
        Vector2 displacement = mousePos - dotPos;
        //Gets displacements angle
        float angle = Vector2.SignedAngle(new Vector2(1, 0), displacement);

        //Decides which direction to draw circle
        Vector2 direction = new Vector2()
        {
            x = angle switch
            {
                var a when a <= 45 && a > -45       =>  1,
                var a when a <= -135 || angle > 135 => -1,
                _ => 0
            },
            y = angle switch
            {
                var a when a <= 135 && angle > 45   =>  1,
                var a when a <= -45 && angle > -135 => -1,
                _ => 0
            }
        };


        diameter = Vector3.Dot(direction, displacement);

        prevPos = currentPos;

        CurrentPos = dotPos + (direction * diameter/2);
    }

    /// <summary>
    /// Calculates the resized circle. The circle's scale is rescaled immediately during this function
    /// </summary>
    /// <param name="dotPos"></param>
    /// <param name="mousePos"></param>
    public void ResizeImmediate(Vector2 dotPos, Vector2 mousePos)
    {
        ResizeCircle(dotPos, mousePos);
        if (scaleFactor == 0) scaleFactor = 0.053f;
        transform.localScale = new Vector3(diameter * scaleFactor, diameter * scaleFactor, 0);
    }

    /// <summary>
    /// Finalizes the circle and tracks it's dots.
    /// </summary>
    /// <param name="dots"></param>
    public void AddCircle(IEnumerable<Dot> dots)
    {
        if(phantom)
        {
            gameObject.SetActive(true);
        }
        GameMaster.Instance.Audio.PlaySoundFX("Finalize circle");
        phantom = false;

        foreach(Dot d in dots)
        {
            d.AddCircle();
        }
        connectedDots = dots.ToArray();
    }

    /// <summary>
    /// Remove its own reference from every dot it's connected to before destroying itself
    /// </summary>
    public void DeleteCircle()
    {
        if(!phantom)
        {
            GameMaster.Instance.Audio.PlaySoundFX("undo");
            foreach (Dot d in connectedDots)
            {
                d.RemoveCircle();
            }
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Turns the circle into a phantom, meaning it still exists, but the dots forget about the circles existance and the circle becomes invisible
    /// i.e., GameMaster will ignore this circle and it will appear to the player as if this circle was deleted, but it can be quickly reactived if needed
    /// </summary>
    public void PhantomDelete()
    {
        GameMaster.Instance.Audio.PlaySoundFX("undo");
        foreach (Dot d in connectedDots)
        {
            d.RemoveCircle();
        }
        gameObject.SetActive(false);
        phantom = true;
    }
}
