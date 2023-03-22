using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcess
{
    public class PostProcessPass : ScriptableRenderPass
    {
        private static readonly int _tempBufferId = UnityEngine.Shader.PropertyToID("_TempBuffer");

        private string _renderTag;
        private RenderTargetIdentifier _renderSourceId;
        private RenderTargetIdentifier _tempRenderTextureId;

        private MyVolumeComponent _volumeComponent;

        public PostProcessPass(RenderPassEvent passEvent)
        {
            _tempRenderTextureId = new RenderTargetIdentifier(_tempBufferId);
            _renderTag = $"PostProcessPass {passEvent}";
            renderPassEvent = passEvent;
        }

        public void Setup(in RenderTargetIdentifier sourceTextureId, MyVolumeComponent volumeComponent)
        {
            _renderSourceId = sourceTextureId;
            _volumeComponent = volumeComponent;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (!renderingData.cameraData.postProcessEnabled)            
                return;            

            var commandBuffer = CommandBufferPool.Get(_renderTag);
            Render(commandBuffer, ref renderingData);
            context.ExecuteCommandBuffer(commandBuffer);
            CommandBufferPool.Release(commandBuffer);
        }

        private void Render(CommandBuffer commandBuffer, ref RenderingData renderingData)
        {
            var source = _renderSourceId;
            var dest = _tempRenderTextureId;

            SetupRenderTexture(commandBuffer, ref renderingData);

            _volumeComponent.Render(commandBuffer, source, dest);

            commandBuffer.Blit(dest, source);

            CleanupRenderTexture(commandBuffer, ref renderingData);
        }

        private void SetupRenderTexture(CommandBuffer commandBuffer, ref RenderingData renderingData)
        {
            ref var cameraData = ref renderingData.cameraData;

            var desc = new RenderTextureDescriptor(cameraData.camera.scaledPixelWidth, cameraData.camera.scaledPixelHeight);
            desc.colorFormat = cameraData.isHdrEnabled ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;

            commandBuffer.GetTemporaryRT(_tempBufferId, desc);
        }

        private void CleanupRenderTexture(CommandBuffer commandBuffer, ref RenderingData renderingData)
        {
            commandBuffer.ReleaseTemporaryRT(_tempBufferId);
        }
    }
}