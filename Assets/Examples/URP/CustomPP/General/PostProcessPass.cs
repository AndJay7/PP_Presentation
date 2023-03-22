using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

//nasz RenderPass do PP
public class PostProcessPass : ScriptableRenderPass
{
    //nazwa naszej tymczasowej tekstury
    private static readonly int _tempBufferId = UnityEngine.Shader.PropertyToID("_TempBuffer");

    //nasza nazwa dla passa w renderowaniu
    private string _renderTag;
    //ID tekstury wejściowej
    private RenderTargetIdentifier _renderSourceId;
    //ID tekstury tymczasowej (u nas pełni rolę destination)
    private RenderTargetIdentifier _tempRenderTextureId;

    //nasz PP
    private MyVolumeComponent _volumeComponent;

    //Inicjalizacja
    public PostProcessPass(RenderPassEvent passEvent)
    {
        _tempRenderTextureId = new RenderTargetIdentifier(_tempBufferId);
        _renderTag = $"PostProcessPass {passEvent}";
        //tutaj przypisujemy kiedy RenderPass z naszym PP ma być renderowany
        renderPassEvent = passEvent;
    }

    //Aktualizacja parametrów
    public void Setup(in RenderTargetIdentifier sourceTextureId, MyVolumeComponent volumeComponent)
    {
        //ID source'a
        _renderSourceId = sourceTextureId;
        //Aktualne wartości dla naszego PP
        _volumeComponent = volumeComponent;
    }

    //wywołanie naszego RenderPass'a z
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        //nie robi nic jeśli kamera nie wspiera post processów
        if (!renderingData.cameraData.postProcessEnabled)
            return;

        //Stworzenie commandBuffer'a. Przechowuje on listę komend dla GPU
        var commandBuffer = CommandBufferPool.Get(_renderTag);
        //Dodanie komend do commandBuffer'a
        AddRenderCommands(commandBuffer, ref renderingData);
        //Wywołanie commandBuffer'a na GPU
        context.ExecuteCommandBuffer(commandBuffer);
        //Zwalnianie pamięci
        CommandBufferPool.Release(commandBuffer);
    }

    private void AddRenderCommands(CommandBuffer commandBuffer, ref RenderingData renderingData)
    {
        var source = _renderSourceId;
        var dest = _tempRenderTextureId;

        //Stworzenie tekstury tymczasowej dla destination
        SetupRenderTexture(commandBuffer, ref renderingData);

        //Wyrenderowanie naszego PP
        _volumeComponent.Render(commandBuffer, source, dest);

        //Skopiowanie wyniku PP spowrotem do tekstury początkowej
        //Robione jest to po to, aby kolejne PP mogły z niej skorzystać
        commandBuffer.Blit(dest, source);

        //Czyszczenie pamięci
        CleanupRenderTexture(commandBuffer, ref renderingData);
    }

    //Tworzenie tekstury tymczasowej
    private void SetupRenderTexture(CommandBuffer commandBuffer, ref RenderingData renderingData)
    {
        ref var cameraData = ref renderingData.cameraData;

        var desc = new RenderTextureDescriptor(cameraData.camera.scaledPixelWidth, cameraData.camera.scaledPixelHeight);
        desc.colorFormat = cameraData.isHdrEnabled ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;

        commandBuffer.GetTemporaryRT(_tempBufferId, desc);
    }

    //Zwalnianie pamięci
    private void CleanupRenderTexture(CommandBuffer commandBuffer, ref RenderingData renderingData)
    {
        commandBuffer.ReleaseTemporaryRT(_tempBufferId);
    }
}