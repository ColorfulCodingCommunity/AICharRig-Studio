using UnityEngine;
using UnityEngine.UIElements;

public class AnimationKey 
{
    public int frame;
    public VisualElement key;
    private AnimationTrack track;
    private float mouseX;
    private bool isDragging;
    
    public AnimationKey(int frame, VisualElement key, AnimationTrack track)
    {
        if (frame <0)
        {
            return;
        }
        this.frame = frame;
        this.key = key;
        this.track = track;
        
        key.RegisterCallback<ClickEvent>(Select);

        key.RegisterCallback<MouseDownEvent>(OnMouseDown);
        key.RegisterCallback<MouseUpEvent>(OnMouseUp);
        key.RegisterCallback<MouseMoveEvent>(OnMouseMove);
        key.RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
        
        
    }

    private void OnMouseLeave(MouseLeaveEvent evt)
    {
        if (isDragging)
        {
            isDragging = false;
            SetFrameBasedOnPosition();
        }
        
    }

    private void OnMouseDown(MouseDownEvent evt)
    {
        if (evt.button == 1)
        {
            isDragging = true;
            mouseX = evt.mousePosition.x;
        }
       
    }

    
    private void OnMouseMove(MouseMoveEvent evt)
    {
        if (!isDragging)
        {
            return; 
        }
        key.style.left = new Length(key.style.left.value.value + evt.mousePosition.x - mouseX, LengthUnit.Pixel);
        mouseX = evt.mousePosition.x;
    }
    private void OnMouseUp(MouseUpEvent evt)
    {
        if (evt.button == 1)
        {
            isDragging = false;
            SetFrameBasedOnPosition();
        }
    }
    
    private void SetFrameBasedOnPosition()
    {
        float position = key.style.left.value.value;
        frame = track.GetFrameFromPosition(position);
    }
    
    private void Select(ClickEvent evt)
    {
        Debug.Log("Select" + frame);
        key.AddToClassList("selectedKeyFrame");
        track.SelectKeyFrame(this);
    }
    
    public void Deselect()
    {
        if (key == null)
        {
            return;
        }
        key.RemoveFromClassList("selectedKeyFrame");
    }
    
    public void Delete()
    {
        track.RemoveKey(frame);
    }
}
