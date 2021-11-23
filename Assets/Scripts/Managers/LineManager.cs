using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using quiet;
using quiet.Timers;

public class LineManager : MonoBehaviour
{
    private DoEvery addPoint;
    private LineRenderer lineRenderer;
    private GameObject line;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartDrawing()
    {
        NewLine();
        addPoint = DoEvery.DoEveryFactory(gameObject, AddPoint, 0.03f);
    }

    public void StopDrawing()
    {
        Destroy(addPoint);
        lineRenderer.loop = true;
        line.AddComponent<DestroySelf>();
    }

    public void AddPoint()
    {
        Vector2 cur = GameMaster.Instance.Input.MousePosition;
        AddPoint(cur.x, cur.y);
    }

    public void AddPoint(float x, float y)
    {
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, new Vector3(x, y, 1));
    }

    public void NewLine()
    {
        line = new GameObject("Line Input", typeof(LineRenderer));
        lineRenderer = line.GetComponent<LineRenderer>();
        lineRenderer.widthMultiplier = 0.1f;
        lineRenderer.numCapVertices = 4;
        lineRenderer.positionCount = 0;
    }
}
