using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragPoint : MonoBehaviour
{
    private bool isDragging = false;
    private Vector2 offset;

    void OnMouseDown()
    {
        // Calculate offset between mouse position and object position
        offset = (Vector2)transform.position - GetMouseWorldPos();
        isDragging = true;
    }

    void OnMouseUp()
    {
        isDragging = false;
    }

    void Update()
    {
        if (isDragging){
            transform.position = GetMouseWorldPos() + offset;
        }

        bool mouseOverPoint = false;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 0f);

        if (hit.collider != null){
            if (hit.collider.transform == gameObject.transform){
                mouseOverPoint = true;
            }
        }

        if(mouseOverPoint && Input.GetMouseButton(1)){
            // Destroy(gameObject);
        }
    }

    private Vector2 GetMouseWorldPos()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
