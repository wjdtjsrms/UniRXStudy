using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZXing;

public class QRCodeScanner : MonoBehaviour
{
    [SerializeField]
    private RawImage rawImageBackground;

    [SerializeField]
    private AspectRatioFitter aspectRatioFitter;

    [SerializeField]
    private TextMeshProUGUI textOut;

    [SerializeField]
    private RectTransform scanZone;

    private bool isCamAvailble;
    private WebCamTexture cameraTexture;

    void Start()
    {
        SetUPCamera();
    }

    void Update()
    {
        UpdateCameraRender();
    }

    private void SetUPCamera()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            isCamAvailble = false;
            return;
        }

        for (int i = 0; i < devices.Length; i++)
        {
            if (devices[i].isFrontFacing == false)
            {
                cameraTexture = new WebCamTexture(devices[i].name, (int)scanZone.rect.width, (int)scanZone.rect.height);
            }
        }

        cameraTexture?.Play();
        rawImageBackground.texture = cameraTexture;
        isCamAvailble = true;
    }

    private void UpdateCameraRender()
    {
        if (cameraTexture == null)
            return;

        float ratio = cameraTexture.width / cameraTexture.height;
        aspectRatioFitter.aspectRatio = ratio;

        int orientation = cameraTexture.videoRotationAngle;
        rawImageBackground.rectTransform.localEulerAngles = new Vector3(0, 0, orientation);
    }

    public void OnClickScan()
    {
        Scan();
    }

    private void Scan()
    {
        try
        {
            IBarcodeReader barcodeReader = new BarcodeReader();
            Result result = barcodeReader.Decode(cameraTexture.GetPixels32(), cameraTexture.width, cameraTexture.height);

            if (result != null)
            {
                textOut.text = result.Text;
            }
            else
            {
                textOut.text = "Failed In Try";
            }
        }
        catch
        {
            textOut.text = "Failed In Try";
        }
    }
}