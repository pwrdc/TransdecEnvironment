using System;

namespace UnityEngine.Rendering.PostProcessing
{
    [Serializable]
    [PostProcess(typeof(ParticlesEffect), PostProcessEvent.BeforeStack, "Custom/Particles")]
    public sealed class Particles : PostProcessEffectSettings
    {
        public FloatParameter Scale = new FloatParameter { value = 1.0f };
        public FloatParameter Scale2 = new FloatParameter { value = 1.0f };
        public FloatParameter Speed = new FloatParameter { value = 0.0f };
        public FloatParameter Cutout = new FloatParameter { value = 0.5f };
        public ColorParameter Color = new ColorParameter { value = UnityEngine.Color.blue };
    }

    public sealed class ParticlesEffect : PostProcessEffectRenderer<Particles>
    {

        public override void Render(PostProcessRenderContext context)
        {
            PropertySheet sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/Particles"));
            sheet.properties.Clear();
            sheet.properties.SetFloat("_Scale", settings.Scale);
            sheet.properties.SetFloat("_Scale2", settings.Scale2);
            sheet.properties.SetFloat("_Speed", settings.Speed);
            sheet.properties.SetFloat("_Cutout", settings.Cutout);
            sheet.properties.SetColor("_Color", settings.Color);
            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
        }
    }
}