using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DrawToRTFeature : ScriptableRendererFeature
{
    class DrawToRTPass : ScriptableRenderPass
    {
        private static ShaderTagId k_DefaultUnlitShaderTagId => new("SRPDefaultUnlit");

        private readonly Material _fireMaterial;
        private readonly Material _iceMaterial;

        private RTHandle _groundRTHandle;

        private static string GroundRTName           => "_groundRTMask";
        private static int    _groundRTMask_ShaderId => Shader.PropertyToID(GroundRTName);
        
        
        private FilteringSettings _filteringSettings;

        public DrawToRTPass(string name, LayerMask layerMask, Material fireEffectMat, Material iceEffectMat)
        {
            profilingSampler = new ProfilingSampler(name);
            renderPassEvent  = RenderPassEvent.AfterRenderingOpaques;

            var renderQueueRange = RenderQueueRange.opaque;
            _filteringSettings = new FilteringSettings(renderQueueRange, layerMask);

            _fireMaterial = fireEffectMat;
            _iceMaterial  = iceEffectMat;
        }
        

        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderTextureDescriptor groundRTDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            groundRTDescriptor.depthBufferBits = (int) DepthBits.None;
            groundRTDescriptor.colorFormat     = RenderTextureFormat.ARGBHalf;
            // groundRTDescriptor.useMipMap       = false;

            // Create temp RTHandle to use as target for particle mask output
            // To change FilterMode, add FilterMode parameter after groundRTDescriptor
            RenderingUtils.ReAllocateIfNeeded(ref _groundRTHandle, groundRTDescriptor, name: GroundRTName);

            ConfigureTarget(_groundRTHandle);
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cameraData = renderingData.cameraData;
            CommandBuffer cmd = CommandBufferPool.Get();

            using (new ProfilingScope(cmd, profilingSampler))
            {
                SortingCriteria sortingCriteria = cameraData.defaultOpaqueSortFlags;
                
                DrawingSettings drawingSettings = CreateDrawingSettings(k_DefaultUnlitShaderTagId, ref renderingData, sortingCriteria);
                drawingSettings.overrideMaterialPassIndex = 0;
                drawingSettings.perObjectData = PerObjectData.None;
                
                // Draw Fire to _groundRTHandle
                drawingSettings.overrideMaterial = _fireMaterial;
                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref _filteringSettings);
                
                // Draw Ice  to _groundRTHandle
                drawingSettings.overrideMaterial = _iceMaterial;
                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref _filteringSettings);
                
                cmd.SetGlobalTexture(_groundRTMask_ShaderId, _groundRTHandle.nameID);
            }

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
        
        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd) { _groundRTHandle.Release(); }
    }

    public LayerMask layerMask;
    public Material fireMask, iceMask;

    private DrawToRTPass _groundEffectRTPass;

    /// <inheritdoc/>
    public override void Create()
    {
        // Inverted if so debug statement only used when needed.
        if (fireMask && iceMask)
        {
            _groundEffectRTPass = new DrawToRTPass("DrawToRTPass: GroundMask", layerMask, fireMask, iceMask);
        }
        else
        {
            Debug.LogWarningFormat
            (
                "Missing fireMask or iceMask Material." +
                "{0} renders pass will not execute. Check for missing reference in the assigned renderer.", GetType().Name
            );
        }

    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) { renderer.EnqueuePass(_groundEffectRTPass); }
}