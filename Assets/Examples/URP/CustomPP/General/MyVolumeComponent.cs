using UnityEngine.Rendering;
public abstract class MyVolumeComponent : VolumeComponent
{
    public abstract void Render(CommandBuffer commandBuffer, RenderTargetIdentifier source, RenderTargetIdentifier dest);
}