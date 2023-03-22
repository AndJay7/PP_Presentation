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
    //przykładowy inny parametr. Istnieją klasy parametrów nadpisujące większość podstawowych typów (float,Vector,int etc.)
    // public Vector2Parameter blend2 = new Vector2Parameter {  value = new Vector2() };

    public bool IsActive() => m_Material != null && blend.value > 0f;
    // opisuje kiedy ma działać nasz PP. Przed podstawowymi/po nich/w trakcie?
    public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

    //zmienna na przechowanie materiału aby go nie tworzyć za każdym razem
    Material m_Material;

    public override void Setup()
    {
        //tworzenie materiału z naszym shader'em
        if (Shader.Find("Hidden/HDRPGrayscale") != null)
            m_Material = new Material(Shader.Find("Hidden/HDRPGrayscale"));
    }

    //ta metoda jest wewnętrznie wywoływana przez HDRP w momencie aktywacji naszego PP
    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
    {
        //zabezpieczenie przed null error'ami
        if (m_Material == null)
            return;

        //zmiana wartości parametrów materiału
        m_Material.SetFloat("_Blend", blend.value);
        //wywołanie komendy na obrazie (source) i przekazanie wyniku do (destination), z wykorzystaniem naszego materiału
        //source staje się naszym _MainTex
        //destination to wynik działania materiału
        cmd.Blit(source, destination, m_Material, 0);
    }

    //czyszczenie
    public override void Cleanup() => CoreUtils.Destroy(m_Material);

}