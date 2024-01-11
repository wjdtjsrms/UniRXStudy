using JSGCode.Util;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomSlider : MonoBehaviour
{
    private VisualElement root;
    private VisualElement slider;
    private VisualElement dragger;
    private VisualElement bar;
    private VisualElement newDragger;
    private VisualElement bubble;
    private Label bubbleLabel;

    public Color color_A;
    public Color color_B;

    // Start is called before the first frame update
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        slider = root.Q<Slider>("MySlider");
        dragger = root.Q<VisualElement>("unity-dragger");

        AddElements();

        slider.RegisterCallback<ChangeEvent<float>>(SliderValueChanged);
        slider.RegisterCallback<GeometryChangedEvent>(SliderInit);

        slider.RegisterCallback<PointerCaptureEvent>(_ =>
        {
            bubble.RemoveFromClassList("bubble--hidden");
        });

        slider.RegisterCallback<PointerCaptureOutEvent>(_ =>
        {
            bubble.AddToClassList("bubble--hidden");
        });
    }

    private void AddElements()
    {
        bar = new VisualElement();
        dragger.Add(bar);
        bar.name = "Bar";
        bar.AddToClassList("bar");

        newDragger = new VisualElement();
        slider.Add(newDragger);
        newDragger.name = "NewDragger";
        newDragger.AddToClassList("newDragger");
        newDragger.pickingMode = PickingMode.Ignore;

        bubble = new VisualElement();
        slider.Add(bubble);
        bubble.name = "Bubble";
        bubble.AddToClassList("bubble");
        bubble.AddToClassList("bubble--hidden");
        bubble.pickingMode = PickingMode.Ignore;

        bubbleLabel = new Label();
        bubble.Add(bubbleLabel);
        bubbleLabel.name = "Bubble_Label";
        bubbleLabel.AddToClassList("bubble_label");
        bubbleLabel.pickingMode = PickingMode.Ignore;
    }

    private void SliderValueChanged(ChangeEvent<float> evt)
    {
        Vector2 offset = new Vector2((newDragger.layout.width - dragger.layout.width) / 2, (newDragger.layout.height - dragger.layout.height) / 2);
        Vector2 offset_Bubble = new Vector2((bubble.layout.width - dragger.layout.width) / 2, (bubble.layout.height - dragger.layout.height) / 2 + 90f);

        Vector2 pos = dragger.parent.LocalToWorld(dragger.transform.position);
        pos = newDragger.parent.WorldToLocal(pos);

        newDragger.transform.position = pos - offset;
        bubble.transform.position = pos - offset_Bubble;

        float v = Mathf.Round(evt.newValue);

        bubbleLabel.text = v.ToString();

        bar.style.backgroundColor = Color.Lerp(color_A, color_B, v / 100f);
        bubble.style.unityBackgroundImageTintColor = Color.Lerp(color_A, color_B, v / 100f);
    }

    private void SliderInit(GeometryChangedEvent evt)
    {
        Vector2 offset = new Vector2((newDragger.layout.width - dragger.layout.width) / 2, (newDragger.layout.height - dragger.layout.height) / 2);
        Vector2 offset_Bubble = new Vector2((bubble.layout.width - dragger.layout.width) / 2, (bubble.layout.height - dragger.layout.height) / 2 + 90f);

        Vector2 pos = dragger.parent.LocalToWorld(dragger.transform.position);
        pos = newDragger.parent.WorldToLocal(pos);

        newDragger.transform.position = pos - offset;
        bubble.transform.position = pos - offset_Bubble;

        bar.style.backgroundColor = color_A;
        bubble.style.unityBackgroundImageTintColor = color_A;
    }
}