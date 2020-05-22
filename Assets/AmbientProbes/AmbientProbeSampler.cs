using UnityEngine;

namespace AmbientProbes
{
    /// <summary>
    /// Samples ambient probes and applies to the color to its renderers through a MaterialPropertyBlock
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu("Rendering/Ambient Probes/Sampler")]
    public class AmbientProbeSampler : MonoBehaviour
    {
        public Renderer[] renderers;
        [Tooltip("The color property that should be set in the shader.")]
        public string propertyName = "_Color";
        [Tooltip("The property that should be set in the shader after applying the color.")]
        public string OcclussionPropertyName = "_Occlusion";

        private MaterialPropertyBlock props;

        [HideInInspector]
        [ColorUsage(false,true)]public Color color;
        public float occlusion;

        private void OnEnable()
        {
            if (props == null) props = new MaterialPropertyBlock();
        }

        private void Update()
        {
            if (renderers == null) return;

            color = AmbientProbeSystem.GetInterpolatedColor(this.transform.position);
            occlusion = AmbientProbeSystem.GetInterpolatedOcclusion(this.transform.position);

            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i])
                {
                    renderers[i].GetPropertyBlock(props);

                    props.SetFloat("_AmbientProbes", 1); //Enable sampling in character shader
                    props.SetFloat(OcclussionPropertyName, occlusion);
                    props.SetColor(propertyName, color);

                    renderers[i].SetPropertyBlock(props);
                }
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i])
                {
                    renderers[i].GetPropertyBlock(props);
                    props.SetFloat("_AmbientProbes", 0); //Disable sampling in character shader
                    renderers[i].SetPropertyBlock(props);
                }
            }
        }

    }
}