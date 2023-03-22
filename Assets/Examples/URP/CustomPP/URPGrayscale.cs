using UnityEngine.Rendering;
using System;
using UnityEngine;

[Serializable]
[VolumeComponentMenu("Example/Grayscale")]
public class URPGrayscale : MyVolumeComponent
{
    [SerializeField]
    private ClampedFloatParameter blend = new ClampedFloatParameter(0,0,1);

    Material m_Material;

    public override void Render(CommandBuffer commandBuffer, RenderTargetIdentifier source, RenderTargetIdentifier dest)
    {
        if (m_Material == null)
        {
            if (Shader.Find("Hidden/URPGrayscale") != null)
                m_Material = new Material(Shader.Find("Hidden/URPGrayscale"));
            else
                return;
        }

        m_Material.SetFloat("_Blend", blend.value);
        commandBuffer.Blit(source, dest, m_Material,0);
    }

}
