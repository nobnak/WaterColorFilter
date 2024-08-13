using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace WaterColorFilterSystem {

    [RequireComponent(typeof(Camera))]
    public class Toon : MonoBehaviour {

        [SerializeField]
        protected Config currConfig;

        protected Material mat;
        protected Camera cam;

        public Config CurrConfig { get => currConfig; set => currConfig = value; }

        #region unity
        private void OnEnable() {
            mat = new Material(Shader.Find("Toon"));

            cam = GetComponent<Camera>();
            cam.depthTextureMode |= DepthTextureMode.DepthNormals;
        }
        private void OnDisable() {
            if (mat != null) {
                System.Action<Object> dispoer = Application.isPlaying ? Object.Destroy : Object.DestroyImmediate;
                dispoer(mat);
                mat = null;
            }
        }
        private void OnRenderImage(RenderTexture source, RenderTexture destination) {
            Profiler.BeginSample($"{nameof(Toon)}");

            var tmp0 = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
            var tmp1 = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
            try {
                var lightDir = currConfig.toon_lightDir.normalized;
                var light_power = new Vector4(lightDir.x, lightDir.y, lightDir.z, currConfig.tone_power);
                mat.SetColor(P_TOON_WARM, currConfig.toon_warm);
                mat.SetColor(P_TOON_COOL, currConfig.toon_cool);
                mat.SetVector(P_TOON_LIGHTDIR_POWER, light_power);
                Graphics.Blit(source, tmp1, mat, (int)Pass.Toon);
                Swap(ref tmp0, ref tmp1);

                var edge_size = currConfig.edge_size;
                var edge_power = new Vector4(edge_size, edge_size, currConfig.edge_power, 1);
                mat.SetVector(P_EDGE_SIZE_POWER, edge_power);
                Graphics.Blit(tmp0, tmp1, mat, (int)Pass.Edge);
                Swap(ref tmp0, ref tmp1);

                var wobb_scale = currConfig.wobb_scale;
                var wobb_power = new Vector4(wobb_scale, wobb_scale, currConfig.wobb_power, 1);
                mat.SetTexture(P_WOBB_TEX, CurrAsset.wobb_tex);
                mat.SetVector(P_WOBB_SCALE_POWER, wobb_power);
                Graphics.Blit(tmp0, tmp1, mat, (int)Pass.Wobb);

                var paper_scale = currConfig.paper_scale;
                var paper_power = new Vector4(paper_scale, paper_scale, currConfig.paper_power, 1);
                mat.SetTexture(P_PAPER_TEX, CurrAsset.paper_tex);
                mat.SetVector(P_PAPER_SCALE_POWER, paper_power);
                Graphics.Blit(tmp1, destination, mat, (int)Pass.Paper);
            } finally {
                RenderTexture.ReleaseTemporary(tmp0);
                RenderTexture.ReleaseTemporary(tmp1);

                Profiler.EndSample();
            }
        }
        #endregion

        #region methods
        public static void Swap(ref RenderTexture tmp0, ref RenderTexture tmp1) {
            var tmp = tmp0;
            tmp0 = tmp1;
            tmp1 = tmp;
        }
        #endregion

        #region declarations
        public enum Pass {
            Toon = 0,
            Edge,
            Wobb,
            Paper,
        }

        [System.Serializable]
        public class Config {
            public Color toon_warm;
			public Color toon_cool;
			public float tone_power;
			public Vector3 toon_lightDir;

            public float edge_size;
            public float edge_power;

            public Texture2D wobb_tex;
            public float wobb_scale;
            public float wobb_power;

            public Texture2D paper_tex;
            public float paper_scale;
            public float paper_power;
        }

        // Toon
        public static readonly int P_TOON_WARM = Shader.PropertyToID("_Toon_Warm");
        public static readonly int P_TOON_COOL = Shader.PropertyToID("_Toon_Cool");
        public static readonly int P_TOON_LIGHTDIR_POWER = Shader.PropertyToID("_Toon_LightDir_Power");

        // Edge
        public static readonly int P_EDGE_SIZE_POWER = Shader.PropertyToID("_Edge_Size_Power");

        // Paper
        public static readonly int P_PAPER_TEX = Shader.PropertyToID("_Paper_Tex");
        public static readonly int P_PAPER_SCALE_POWER = Shader.PropertyToID("_Paper_Scale_Power");
        // Wobb
        public static readonly int P_WOBB_TEX = Shader.PropertyToID("_Wobb_Tex");
        public static readonly int P_WOBB_SCALE_POWER = Shader.PropertyToID("_Wobb_Scale_Power");
        #endregion
    }
}