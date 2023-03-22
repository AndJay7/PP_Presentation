using System;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcess
{

    public class PostProcessRenderFeature : ScriptableRendererFeature
    {
        private PostProcessPass _grayscalePass = null;

        public override void Create()
        {
            _grayscalePass = new PostProcessPass(RenderPassEvent.BeforeRenderingPostProcessing);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            URPGrayscale grayscaleVolume = VolumeManager.instance.stack.GetComponent<URPGrayscale>();

            _grayscalePass.Setup(renderer.cameraColorTarget, grayscaleVolume);

            renderer.EnqueuePass(_grayscalePass);
        }
    }
}