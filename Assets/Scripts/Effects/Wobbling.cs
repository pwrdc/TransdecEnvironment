using System;

namespace UnityEngine.Rendering.PostProcessing
{
    [Serializable]
    [PostProcess(typeof(WobblingEffect), PostProcessEvent.AfterStack, "Custom/Wobbling")]
    public sealed class Wobbling : PostProcessEffectSettings
    {
        public FloatParameter Intensity = new FloatParameter { value = 20.0f };
        public FloatParameter Scale = new FloatParameter { value = 1.0f };
        public FloatParameter Speed = new FloatParameter { value = 0.0f };
    }

    public sealed class WobblingEffect : PostProcessEffectRenderer<Wobbling>
    {

        public override void Render(PostProcessRenderContext context)
        {
            PropertySheet sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/Wobble"));
            sheet.properties.Clear();
            sheet.properties.SetFloat("_Intensity", settings.Intensity);
            sheet.properties.SetFloat("_Scale", settings.Scale);
            sheet.properties.SetFloat("_Speed", settings.Speed);
            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
        }
    }
}