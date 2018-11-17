using UnityEngine;

namespace Components
{
    [ExecuteInEditMode]
    public class CameraRenderHeightInformation : MonoBehaviour
    {
        public Shader Shader;
        public Camera Camera;

        [Range(0, 1000)]
        public float HeightmapScale = 10;

        public void Start()
        {
            int id = Shader.PropertyToID("heightmapScale");
            Shader.SetGlobalFloat(id, HeightmapScale);
            Camera.SetReplacementShader(Shader, string.Empty);
        }
    }
}