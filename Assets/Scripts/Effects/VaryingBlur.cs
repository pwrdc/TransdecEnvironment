using System;

namespace UnityEngine.Rendering.PostProcessing
{
    [Serializable]
    [PostProcess(typeof(VaryingBlurEffect), PostProcessEvent.AfterStack, "Custom/VaryingBlur")]
    public sealed class VaryingBlur : PostProcessEffectSettings
    {
        public VaryingBlurModeParameter Mode = new VaryingBlurModeParameter();
        public IntParameter Downsample = new IntParameter { value = 1 };
        public IntParameter BlurIterations = new IntParameter { value = 1 };
        public FloatParameter BlurSize = new FloatParameter { value = 3.0f };

        public FloatParameter NoiseScale = new FloatParameter { value = 5f };
        public FloatParameter NoiseChangeRate = new FloatParameter { value = 2f };
        public ColorParameter Color = new ColorParameter { value = UnityEngine.Color.black };
        [Range(0, 1)]
        public FloatParameter ColorIntensity = new FloatParameter { value = 0f };
        [Range(0, 1)]
        public FloatParameter BlurredPart = new FloatParameter { value = 0.5f };
    }

    [Serializable]
    public sealed class VaryingBlurModeParameter : ParameterOverride<VaryingBlurEffect.Mode>
    {

    }

    public sealed class VaryingBlurEffect : PostProcessEffectRenderer<VaryingBlur>
    {
        public enum Mode
        {
            StandardGaussian,
            SgxGaussian
        }

        public enum Pass
        {
            Downsample = 0,
            BlurVertical = 1,
            BlurHorizontal = 2,
        }

        public override void Render(PostProcessRenderContext context)
        {
            CommandBuffer command = context.command;

            command.BeginSample("VaryingBlurPostEffect");

            int downsample = settings.Downsample;
            int blurIterations = settings.BlurIterations;
            float blurSize = settings.BlurSize;
            float widthMod = 1.0f / (1.0f * (1 << downsample));

            int rtW = context.width >> downsample;
            int rtH = context.height >> downsample;

            PropertySheet sheet = context.propertySheets.Get(Shader.Find("Hidden/Legacy/VaryingBlur"));
            sheet.properties.Clear();
            sheet.properties.SetVector("_Parameter", new Vector4(blurSize * widthMod, -blurSize * widthMod, 0.0f, 0.0f));
            
            sheet.properties.SetFloat("_NoiseScale", settings.NoiseScale);
            sheet.properties.SetFloat("_NoiseChangeRate", settings.NoiseChangeRate);
            sheet.properties.SetColor("_Color", settings.Color);
            sheet.properties.SetFloat("_ColorIntensity", settings.ColorIntensity);
            sheet.properties.SetFloat("_BlurredPart", settings.BlurredPart);

            int blurId = Shader.PropertyToID("_BlurPostProcessEffect");
            command.GetTemporaryRT(blurId, rtW, rtH, 0, FilterMode.Bilinear);
            command.BlitFullscreenTriangle(context.source, blurId, sheet, (int)Pass.Downsample);

            int pass = settings.Mode.value == Mode.SgxGaussian ? 2 : 0;

            int rtIndex = 0;
            for (int i = 0; i < blurIterations; i++)
            {
                float iterationOffs = i * 1.0f;
                sheet.properties.SetVector("_Parameter", new Vector4(blurSize * widthMod + iterationOffs, -blurSize * widthMod - iterationOffs, 0.0f, 0.0f));

                // Vertical blur..
                int rtId2 = Shader.PropertyToID("_VaryingBlurPostEffect" + rtIndex++);
                command.GetTemporaryRT(rtId2, rtW, rtH, 0, FilterMode.Bilinear);
                command.BlitFullscreenTriangle(blurId, rtId2, sheet, (int)Pass.BlurVertical + pass);
                command.ReleaseTemporaryRT(blurId);
                blurId = rtId2;

                // Horizontal blur..
                rtId2 = Shader.PropertyToID("_VaryingBlurPostEffect" + rtIndex++);
                command.GetTemporaryRT(rtId2, rtW, rtH, 0, FilterMode.Bilinear);
                command.BlitFullscreenTriangle(blurId, rtId2, sheet, (int)Pass.BlurHorizontal + pass);
                command.ReleaseTemporaryRT(blurId);
                blurId = rtId2;
            }

            command.Blit(blurId, context.destination);
            command.ReleaseTemporaryRT(blurId);

            command.EndSample("VaryingBlurPostEffect");
        }
    }
}