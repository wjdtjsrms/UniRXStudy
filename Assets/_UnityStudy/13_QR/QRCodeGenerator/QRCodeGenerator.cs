using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZXing;
using ZXing.QrCode;
using UnityEngine.UI;
using TMPro;

public class QRCodeGenerator : MonoBehaviour
{
    [SerializeField]
    private RawImage rawImageReceiver;

    [SerializeField]
    private TMP_InputField inputField;

    private Texture2D storeEncodedTexture;


    void Start()
    {
        storeEncodedTexture = new Texture2D(256, 256);
    }

    private Color32[] Encode(string textForEcnoding, int width, int height)
    {
        BarcodeWriter writer = new()
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width,
            }
        };

        return writer.Write(textForEcnoding);
    }

    public void OnClickEncode()
    {
        EncodeTextToQRCode();
    }

    private void EncodeTextToQRCode()
    {
        string textWrite = string.IsNullOrEmpty(inputField.text) ? "You Should Write Something" : inputField.text;

        Color32[] convertPixelToTexture = Encode(textWrite, storeEncodedTexture.width, storeEncodedTexture.height);
        storeEncodedTexture.SetPixels32(convertPixelToTexture);
        storeEncodedTexture.Apply();

        rawImageReceiver.texture = storeEncodedTexture;
    }
}