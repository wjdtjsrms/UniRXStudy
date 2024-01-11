using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using JSGCode.Util;

public class UIController : MonoBehaviour
{
    private VisualElement bottomContainer;
    private Button openButton;
    private Button closeButton;
    private VisualElement bottomSheet;
    private VisualElement scrim;
    private VisualElement boy;
    private VisualElement girl;
    private Label message;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        bottomContainer = root.Q<VisualElement>("Container_Bottom");
        openButton = root.Q<Button>("Button_Open");
        closeButton = root.Q<Button>("Button_Close");
        bottomSheet = root.Q<VisualElement>("BottomSheet");
        scrim = root.Q<VisualElement>("Scrim");
        boy = root.Q<VisualElement>("image_Boy");
        girl = root.Q<VisualElement>("Image_Girl");
        message = root.Q<Label>("Message");

        bottomContainer.style.display = DisplayStyle.None;

        openButton.RegisterCallback<ClickEvent>(OnOpenButtonClicked);
        closeButton.RegisterCallback<ClickEvent>(OnCloseButtonClicked);
        bottomSheet.RegisterCallback<TransitionEndEvent>(OnBottomSheetDown);

        AnimateBoy().Forget();
    }

    private async UniTaskVoid AnimateBoy()
    {
        await UniTask.DelayFrame(2);
        boy.RemoveFromClassList("image--boy--inair");
    }

    private void OnOpenButtonClicked(ClickEvent evt)
    {
        bottomContainer.style.display = DisplayStyle.Flex;
        bottomSheet.AddToClassList("bottomsheet--up");
        scrim.AddToClassList("scrim--fadein");

        AnimateGirl();
    }

    private void AnimateGirl()
    {
        girl.ToggleInClassList("image--girl--up");
        girl.RegisterCallback<TransitionEndEvent>
        (
            evt => girl.ToggleInClassList("image--girl--up")
        );

        message.text = string.Empty;

        string m = "See in rebus apertissimis nimium longi sumus.";

        DOTween.To(() => message.text, x => message.text = x, m, 3f).SetEase(Ease.Linear);
    }


    private void OnCloseButtonClicked(ClickEvent evt)
    {
        bottomSheet.RemoveFromClassList("bottomsheet--up");
        scrim.RemoveFromClassList("scrim--fadein");
    }

    private void OnBottomSheetDown(TransitionEndEvent evt)
    {
        if (evt.target == bottomSheet && !bottomSheet.ClassListContains("bottomsheet--up"))
        {
            bottomContainer.style.display = DisplayStyle.None;
        }
    }
}
