
// source: https://forum.unity.com/threads/legacy-blur-for-post-processing-stack-v2.488222/

using System;

namespace UnityEngine.Rendering.PostProcessing
{
    [Serializable]
    [PostProcess(typeof(DepthReaderEffect), PostProcessEvent.AfterStack, "Custom/DepthReader")]
    public sealed class DepthReader : PostProcessEffectSettings
    {
        public FloatParameter MaxDepth = new FloatParameter { value = 0.05f };
        public ColorParameter ClearColor = new ColorParameter { value = Color.black };
    }

    public sealed class DepthReaderEffect : PostProcessEffectRenderer<DepthReader>
    {

        public override void Render(PostProcessRenderContext context)
        {
            PropertySheet sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/DepthReader"));
            sheet.properties.Clear();
            sheet.properties.SetFloat("_MaxDepth", settings.MaxDepth);
            sheet.properties.SetColor("_ClearColor", settings.ClearColor);
            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
        }
    }
}