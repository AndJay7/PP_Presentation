using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

//Ten atrybut powoduje że Unity potrafi zapamiętać (serializować) obiekt naszej klasy
[Serializable]
//w zmiennej menu podajemy ścieżkę gdzie będzie można wybrać nasz PP w Volume
[VolumeComponentMenu(menu: "Post-processing/Example/GrayScale")]
//ta klasa zawiera wszystkie właściwości PP oraz logikę ich wykonania
public sealed class HDRPGrayscale : CustomPostProcessVolumeComponent, IPostProcessComponent
{
    [Tooltip("Controls the intensity of the effect.")]
    public ClampedFloatParameter blend = new ClampedFloatParameter(0f, 0f, 1f);
    public bool IsActive() => m_Material != null && blend.value > 0f;
    public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

    Material m_Material;

    public override void Setup()
    {
        if (Shader.Find("Hidden/HDRPGrayscale") != null)
            m_Material = new Material(Shader.Find("Hidden/HDRPGrayscale"));
    }

    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
    {
        if (m_Material == null)
            return;

        m_Material.SetFloat("_Blend", blend.value);
        cmd.Blit(source, destination, m_Material, 0);
    }

    public override void Cleanup() => CoreUtils.Destroy(m_Material);

}