using System;

namespace UnityEngine.Rendering.PostProcessing
{
    [Serializable]
    [PostProcess(typeof(VaryingBlurEffect), PostProcessEvent.AfterStack, "Custom/VaryingBlur")]
    public sealed class VaryingBlur : PostProcessEffectSettings
    {
        public ColorParameter NoiseColor = new ColorParameter { value = Color.black };
        public FloatParameter NoiseScale = new FloatParameter { value = 5f };
        public FloatParameter NoiseChangeRate = new FloatParameter { value = 2f };
    }

    public sealed class VaryingBlurEffect : PostProcessEffectRenderer<VaryingBlur>
    {

        public override void Render(PostProcessRenderContext context)
        {
            PropertySheet sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/VaryingBlur"));
            sheet.properties.Clear();
            sheet.properties.SetColor("_Color", settings.NoiseColor);
            sheet.properties.SetFloat("_NoiseScale", settings.NoiseScale);
            sheet.properties.SetFloat("_NoiseChangeRate", settings.NoiseChangeRate);
            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
        }
    }
}