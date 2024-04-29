using System.Collections.Generic;
using UnityEngine;

public class ScrollList : MonoBehaviour
{
    [SerializeField] private Transform cameraBaseTransform;
    [SerializeField] private CarList cars;
    [SerializeField] private Transform modelShowcase;
    [SerializeField] private Transform cameraList;
    [SerializeField] private Transform content;
    [SerializeField] private CarItem carItem;
    [SerializeField] private int textureWidth, textureHeight;
    [SerializeField] private LayerMask ignoreLight;

    // Store created textures and release when close shop
    private List<RenderTexture> renderTextures;

    private void Start() {

        renderTextures = new List<RenderTexture>();

        for (int i = 0; i < cars.Count; i++) {
            CarInfo info = cars.GetCar(i);
            GameObject car =  Instantiate(info.model, modelShowcase);
            car.transform.localPosition = new Vector3(0, 0, i * 10);

            GameObject cameraObject = new GameObject("Camera");
            cameraObject.transform.parent = cameraList;
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.transform.position = cameraBaseTransform.position;
            camera.transform.rotation = cameraBaseTransform.rotation;
            camera.transform.localPosition = new Vector3(
                camera.transform.localPosition.x,
                camera.transform.localPosition.y,
                i * 10
            );
            
            camera.cullingMask = ignoreLight;

            RenderTexture renderTexture = new RenderTexture(textureWidth, textureHeight, 16);
            renderTexture.Create();
            camera.targetTexture = renderTexture;
            
            renderTextures.Add(renderTexture);
            
            CarItem item = Instantiate(carItem, content);
            item.Init(renderTexture, info.cost);
        }
    }

    public void UnLoad() {
        foreach (RenderTexture renderTexture in renderTextures) {
            renderTexture.Release();
        }
    }

}
