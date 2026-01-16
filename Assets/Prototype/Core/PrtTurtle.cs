using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrtTurtle : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer turtleImage;

    [SerializeField]
    private BoxCollider2D turtleCollider;

    private int turtleIndex;

    private Action<int> onClicked;

    public void ResetTurtle()
    {
        turtleIndex = -1;
        onClicked = null;

        SetAlpha(0.0f);
    }

    // public Methods
    public void Setindex(int index)
    {
        turtleIndex = index;
    }

    public void SetTurtleImage(Sprite newSprite)
    {
        turtleImage.sprite = newSprite;
    }

    public void SetTurtleSize(Vector2 newSize)
    {
        gameObject.transform.localScale = new Vector3(newSize.x, newSize.y, 1f);
    }

    public void SetTurtlePosition(Vector2 newPosition)
    {
        gameObject.transform.localPosition = new Vector3(newPosition.x, newPosition.y, 0f);
    }

    public void SetAlpha(float alpha)
    {
        Color color = turtleImage.color;
        color.a = alpha;
        turtleImage.color = color;
    }

    public void SetOnTurtle(Action<int> action)
    {
        onClicked = action;
    }

    public void OnMouseDown()
    {
        Debug.Log($"Turtle {turtleIndex} clicked!");
        if (turtleIndex < 0)
            return;
        onClicked?.Invoke(turtleIndex);
    }
}
