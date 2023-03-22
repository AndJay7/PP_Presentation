using UnityEngine.Rendering;
using System;
using UnityEngine;

//Ten atrybut powoduje że Unity potrafi zapamiętać (serializować) obiekt naszej klasy
[Serializable]
//w zmiennej menu podajemy ścieżkę gdzie będzie można wybrać nasz PP w Volume
[VolumeComponentMenu(menu: "Example/Grayscale")]
//ta klasa zawiera wszystkie właściwości PP oraz logikę ich wykonania. Dziedziczy po MyVolumeComponent, jest to moja klasa która zastępuje CustomPostProcessVolumeComponent z HDRP 
public class URPGrayscale : MyVolumeComponent
{
    [SerializeField]
    private ClampedFloatParameter blend = new ClampedFloatParameter(0,0,1);
    //przykładowy inny parametr. Istnieją klasy parametrów nadpisujące większość podstawowych typów (float,Vector,int etc.)
    // public Vector2Parameter blend2 = new Vector2Parameter {  value = new Vector2() };   

    //zmienna na przechowanie materiału aby go nie tworzyć za każdym razem
    Material m_Material;

    //ta metoda jest wewnętrznie wywoływana przez nasz RenderPass w momencie aktywacji naszego PP
    public override void Render(CommandBuffer commandBuffer, RenderTargetIdentifier source, RenderTargetIdentifier dest)
    {
        //zabezpieczenie przed null errorami
        if (m_Material == null)
        {
            if (Shader.Find("Hidden/URPGrayscale") != null)
                m_Material = new Material(Shader.Find("Hidden/URPGrayscale"));
            else
                return;
        }

        //zmiana wartości parametrów materiału
        m_Material.SetFloat("_Blend", blend.value);
        //wywołanie komendy na obrazie (source) i przekazanie wyniku do (destination), z wykorzystaniem naszego materiału
        //source staje się naszym _MainTex
        //destination to wynik działania materiału
        commandBuffer.Blit(source, dest, m_Material,0);
    }

}
