using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    private string targetTag = "Point";
    public GameObject point;
    public float size = .5f;

    public List<GameObject> points = new List<GameObject>();

    private Stack<Vector2> st = new Stack<Vector2>();

    public Color lineColor = Color.red;

    // Update is called once per frame
    void Update()
    {
        bool mouseOverPoint = false;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 0f);

        if (hit.collider != null){
            if (hit.collider.CompareTag(targetTag)){
                mouseOverPoint = true;
            }
        }

        if( Input.GetMouseButtonDown(0) && !mouseOverPoint ) Click();

        Rezolvare();
    }

    void Click(){
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        GameObject newPoint = Instantiate(point, mousePos, Quaternion.identity);
        newPoint.transform.localScale = size * new Vector3(1f, 1f, 1f);

        points.Add(newPoint);
    }

    int Orientare( Vector2 A, Vector2 B, Vector2 C )
    {
        if( (C.y - A.y)*(B.x - A.x) - (B.y - A.y)*(C.x - A.x) < 0 )
            return -1;
        if( (C.y - A.y)*(B.x - A.x) - (B.y - A.y)*(C.x - A.x) > 0 )
            return 1;
        return 0;
    }

    float dist( Vector2 A, Vector2 B ){
        return (A.x-B.x)*(A.x-B.x) + (A.y-B.y)*(A.y-B.y);
    }

    int cmp( GameObject A, GameObject B )
    {
        return -Orientare( points[0].transform.position, A.transform.position, B.transform.position );
    }

    void Swap(int p){
        GameObject t = points[0];
        points[0] = points[p];
        points[p] = t;
    }

    void Rezolvare()
    {
        st.Clear();
        if(points.Count < 3) return;

        int i, p = 0;
        Vector2 P = new Vector2(0f, 0f);
        float ymn = 100;
        for( i=0; i<points.Count; i++ ){
            Vector2 A = points[i].transform.position;
            if( A.y < ymn ){
                ymn = A.y;
                P = A;
                p = i;
            }
            else if( A.y == ymn ) if( A.x < P.x ){
                P = A;
                p = i;
            }
        }
        Swap(p);
        // points[0].GetComponent<SpriteRenderer>().color = lineColor;
        points.Sort(1, points.Count - 1, Comparer<GameObject>.Create((a, b) => cmp(a, b)));

        st.Push(points[0].transform.position);
        st.Push(points[1].transform.position);
        for( i=2; i<points.Count; i++ ){
            Vector2 A = st.Pop();
            Vector2 B = st.Pop();
            while( st.Count >= 2 && Orientare( B, A, points[i].transform.position ) <= 0 ){
                A = B;
                B = st.Pop();
            }
            st.Push(B);
            st.Push(A);
            st.Push(points[i].transform.position);
        }

        Vector2 First = st.Pop();
        Vector2 C = First;
        while (st.Count > 0)
        {
            Vector2 D = st.Pop();
            DrawLine(C, D);
            C = D;
        }
        DrawLine(C, First);
    }

    public void DrawLine(Vector2 start, Vector2 end)
    {
        GameObject lineObj = new GameObject("TemporaryLine");
        LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();

        // Set line properties
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // Ensures it is visible
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;

        // Destroy the line after 'lineDuration' seconds
        Destroy(lineObj, Time.deltaTime + 0.05f);
    }
}
