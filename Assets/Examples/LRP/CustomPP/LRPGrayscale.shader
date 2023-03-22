//Nazwa naszego shader'a ta sama której używamy w LRPGrayscaleRenderer. Hidden z przodu powoduje, że nie da się tego shader'a znaleść przy tworzeniu materiałów
Shader "Hidden/LRPGrayscale"
{
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM
            //zawiera podstawowe funkcje do operowania w shader'ach
            #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

            //Podpięcie głównych metod shader'a
            //Metoda 'vertex' nie jest definiowana, bo korzystamy z domyślnej 'VertDefault' z StdLib.hlsl include'owanego powyżej
            #pragma vertex VertDefault
            //Metoda 'fragment' zdefiniowana poniżej jako 'Frag'
            #pragma fragment Frag

            //Tutaj znajdują się zmienne shader'a

            //To jest domyślna tekstura. Metoda Blit zawsze wrzuca obraz z kamery do zmiennej o nazwie '_MainTex'
            TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
            //To jest zmienna której wartość zmieniamy za pomocą SetFloat() wewnątrz klasy LRPGrayscaleRenderer 
            float _Blend;

            //Nasza główna metoda z obliczeniami. Można ją nazwać inaczej niż 'Frag', ale trzeba wtedy pamiętać o poprawieniu #pragma powyżej            
            //Korzystamy z domyślnych 'VaryingsDefault' z StdLib.hlsl include'owanego powyżej
            //'SV_Target' musi być, bo tak i już
            float4 Frag(VaryingsDefault i) : SV_Target
            {
                //Sprawdzamy aktualny kolor pixel'a
                float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
                //Robimy magiczne vodoo z przeliczeniem koloru na jasność (przejście z przestrzeni RGB na HSV itd.)
                float luminance = dot(color.rgb, float3(0.2126729, 0.7151522, 0.0721750));
                //Ustawiamy jak mocno ma blendować się wartość jasności z oryginalnym kolorem
                color.rgb = lerp(color.rgb, luminance.xxx, _Blend.xxx);
                //Zwracamy nowy kolor pixel'a
                return color;
            }

            ENDHLSL
        }
    }
}