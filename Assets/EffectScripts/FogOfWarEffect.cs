using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FogOfWarEffect : MonoBehaviour
{
    public Material fogOfWarMaterial;

	public void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(source, destination, fogOfWarMaterial);
	}
}
