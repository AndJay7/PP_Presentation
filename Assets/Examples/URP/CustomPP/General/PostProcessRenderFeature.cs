using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


//Nasz customowy RenderFeature. Dodaje on naszego Pass'a z naszym PP, do kolejki renderowania
//RenderFeature to takie pudełko na RenderPass'y, aby łatwo było je dodać do RenderPipeline'a
public class PostProcessRenderFeature : ScriptableRendererFeature
{
    //nasz RenderPass
    private PostProcessPass _grayscalePass = null;

    //Tworzenie RenderFeature'a
    public override void Create()
    {
        //Inicjalizacja RenderPass'a
        _grayscalePass = new PostProcessPass(RenderPassEvent.BeforeRenderingPostProcessing);
    }

    //Dodanie RenderPass'a do RenderPipeline'a
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        //pobranie aktualnych config'ów naszego PP
        URPGrayscale grayscaleVolume = VolumeManager.instance.stack.GetComponent<URPGrayscale>();
        //przekazanie config'ów naszego PP do naszego RenderPass
        //cameraColorTarget pozwala na obranie z Renderer'a informacji o ID tekstury z obrazem z kamery
        _grayscalePass.Setup(renderer.cameraColorTarget, grayscaleVolume);

        //wrzucenie RenderPass'a do RenderPipeline'a
        renderer.EnqueuePass(_grayscalePass);
    }
}