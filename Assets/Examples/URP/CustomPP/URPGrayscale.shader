//Nazwa naszego shader'a ta sama której używamy w URPGrayscale. Hidden z przodu powoduje, że nie da się tego shader'a znaleść przy tworzeniu materiałów
Shader "Hidden/URPGrayscale"
{
    Properties
    {
        // This property is necessary to make the CommandBuffer.Blit bind the source texture to _MainTex
        _MainTex("Main Texture", 2DArray) = "grey" {}
    }

    SubShader
    {
        Pass
        {
            Name "GrayScale"

            HLSLPROGRAM

                //zawierają podstawowe funkcje do operowania w shader'ach
                #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"

                //Podpięcie głównych metod shader'a
                //Metoda 'vertex', korzystamy z domyślnej metody zdefiniowanej w powyższym includ'zie Common.hlsl
                #pragma vertex Vert
                //Metoda 'fragment' zdefiniowana poniżej jako 'Frag'
                #pragma fragment Frag

                //Tutaj znajdują się zmienne shader'a
                //To jest domyślna tekstura. Metoda Blit zawsze wrzuca obraz z kamery do zmiennej o nazwie '_MainTex'
                TEXTURE2D_X(_MainTex);
                //To jest zmienna której wartość zmieniamy za pomocą SetFloat() wewnątrz klasy HDRPGrayscale 
                float _Blend;

                //Nasza główna metoda z obliczeniami. Można ją nazwać inaczej niż 'Frag', ale trzeba wtedy pamiętać o poprawieniu #pragma powyżej    
                //Varyings i, to wartości wyliczone z powyższej metody Vert
                //'SV_Target' musi być, bo tak i już
                float4 Frag(Varyings i) : SV_Target
                {
                    //Sprawdzamy aktualny kolor pixel'a
                    float4 color = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv);
                    //korzystamy z Luminance() zdefiniowanego w include'zie Color.hlsl
                    float luminance = Luminance(color);
                    //Ustawiamy jak mocno ma blendować się wartość jasności z oryginalnym kolorem
                    color.rgb = lerp(color.rgb, luminance, _Blend);
                    //Zwracamy nowy kolor pixel'a
                    return color;
                }

            ENDHLSL
        }
    }
    //Jakby coś nie działało można tutaj podpiąc inny shader
    Fallback Off
}