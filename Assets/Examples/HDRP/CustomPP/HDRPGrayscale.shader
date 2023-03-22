//Nazwa naszego shader'a ta sama której używamy w HDRPGrayscale. Hidden z przodu powoduje, że nie da się tego shader'a znaleść przy tworzeniu materiałów
Shader "Hidden/HDRPGrayscale"
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
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
                #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
                //zawiera predefiniowane operacje na kolorach
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

                //Podpięcie głównych metod shader'a
                //Metoda 'vertex' zdefiniowana poniżej jako 'Vert'
                #pragma vertex Vert
                //Metoda 'fragment' zdefiniowana poniżej jako 'Frag'
                #pragma fragment Frag

                //weryfikacja aby karta graficzna wspierała odpowiednio nowe API
                #pragma target 4.5
                //weryfikacja na których platformach shader jest kompatybilny
                #pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch

                //struktura danych wejściowych do metody Vert()
                struct Attributes
                {
                    uint vertexID : SV_VertexID;
                };

                //struktura danych wyjściowych z metody Vert() oraz danych wejściowych do metody Frag()
                struct Varyings
                {
                    float4 positionCS : SV_POSITION;
                    float2 texcoord   : TEXCOORD0;
                };

                //Tutaj znajdują się zmienne shader'a
                //To jest domyślna tekstura. Metoda Blit zawsze wrzuca obraz z kamery do zmiennej o nazwie '_MainTex'
                TEXTURE2D_X(_MainTex);
                //To jest zmienna której wartość zmieniamy za pomocą SetFloat() wewnątrz klasy HDRPGrayscale 
                float _Blend;

                //Nasza metoda vertex, która weryfikuje wszystkie vertexy (dla PP są to 4 wieszchołki ekranu)
                Varyings Vert(Attributes i)
                {
                    Varyings output;

                    //przeliczenie ID vertex'ów na współrzędne wieszchołków ekranu
                    output.positionCS = GetFullScreenTriangleVertexPosition(i.vertexID);
                    //przeliczenie ID vertex'ów na wartosci UV
                    output.texcoord = GetFullScreenTriangleTexCoord(i.vertexID);

                    return output;
                }

                //Nasza główna metoda z obliczeniami. Można ją nazwać inaczej niż 'Frag', ale trzeba wtedy pamiętać o poprawieniu #pragma powyżej    
                //Varyings i, to wartości wyliczone z powyższej metody Vert
                //'SV_Target' musi być, bo tak i już
                float4 Frag(Varyings i) : SV_Target
                {
                    //Sprawdzamy aktualny kolor pixel'a
                    float4 color = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, i.texcoord);
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