using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

//Ten atrybut powoduje że Unity potrafi zapamiętać (serializować) obiekt naszej klasy
[Serializable]
//w zmiennej eventType wybieramy w którym momencie względem innych PP ma zadziałać nasz PP
//w zmiennej menuItem podajemy ścieżkę gdzie będzie można wybrać nasz PP w Volume
[PostProcess(typeof(LRPGrayscaleRenderer),eventType: PostProcessEvent.AfterStack,menuItem: "Example/Grayscale")]
//w tej klasie dodaje się wszystkie właściwości PP, które chcemy aby były widoczne w Volume
public sealed class LRPGrayscale : PostProcessEffectSettings
{
    [Range(0f, 1f), Tooltip("Grayscale effect intensity.")]
    public FloatParameter blend = new FloatParameter { value = 0.5f };
    //przykładowy inny parametr. Istnieją klasy parametrów nadpisujące większość podstawowych typów (float,Vector,int etc.)
   // public Vector2Parameter blend2 = new Vector2Parameter {  value = new Vector2() };
}

//ta klasa jest odpowiedzialna za realną realizację działania (renderowanie) naszego PP
public sealed class LRPGrayscaleRenderer : PostProcessEffectRenderer<LRPGrayscale>
{
    //ta metoda jest wewnętrznie wywoływana przez LRP w momencie aktywacji naszego PP
    public override void Render(PostProcessRenderContext context)
    {
        //Wyszukujemy nasz shader od PP po jego nazwie
        Shader shader = Shader.Find("Hidden/LRPGrayscale");
        if (shader == null)
            return;

        //utworzenie listy parametrów z shaderem
        var sheet = context.propertySheets.Get(shader);
        //zmiana wartości parametrów shader'a
        sheet.properties.SetFloat("_Blend", settings.blend);
        //wywołanie shader'a na obrazie (source) i przekazanie wyniku do (destination), z wykorzystaniem sparametryzowanego shader'a (sheet)
        //source staje się naszym _MainTex
        //destination to wynik działania shader'a
        //Wartość 0 oznacza wywołanie tylko pierwszego pass'a z shader'a
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}