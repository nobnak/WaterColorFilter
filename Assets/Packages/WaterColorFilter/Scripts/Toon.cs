using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Profiling;

namespace WaterColorFilterSystem {

    [ExecuteAlways]
    [RequireComponent(typeof(Camera))]
    public class Toon : MonoBehaviour {

        [SerializeField]
        protected Events events = new();
        [SerializeField]
        protected Assets assets = new();
        [SerializeField]
        protected Config currConfig = new();

        protected Material mat;
        protected Camera cam;

        public Config CurrConfig  => currConfig;
        public Events CurrEvents => events;

        #region unity
        private void OnEnable() {
            mat = new Material(Resources.Load<Shader>("Toon"));

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
        private void OnPreRender() {
            if (assets.light != null) {
                currConfig.toon_lightDir = cam.transform.InverseTransformDirection(assets.light.forward); 
            }

            events.onPreRender?.Invoke(this);
        }
        private void OnPostRender() {
            events.onPostRender?.Invoke(this);
        }
        private void OnRenderImage(RenderTexture source, RenderTexture destination) {
            Profiler.BeginSample($"{nameof(Toon)}");

            var tmp0 = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
            var tmp1 = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
            try {
                var lightDir = currConfig.toon_lightDir;
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
                mat.SetTexture(P_WOBB_TEX, currConfig.wobb_tex);
                mat.SetVector(P_WOBB_SCALE_POWER, wobb_power);
                Graphics.Blit(tmp0, tmp1, mat, (int)Pass.Wobb);

                var papers = currConfig.paper_configs;
                for (var i = 0; i < papers.Count; i++) {
                    var paper = papers[i];
                    var paper_scale = paper.paper_scale;
                    var paper_power = new Vector4(paper_scale, paper_scale, paper.paper_power, 1);
                    mat.SetTexture(P_PAPER_TEX, paper.paper_tex);
                    mat.SetVector(P_PAPER_SCALE_POWER, paper_power);
                    Graphics.Blit(tmp0, tmp1, mat, (int)Pass.Paper);
                    Swap(ref tmp0, ref tmp1);
                }

                Graphics.Blit(tmp0, destination);
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
        public class Events {

            public ToonEvent onPreRender = new();
            public ToonEvent onPostRender = new();

            [System.Serializable]
            public class ToonEvent : UnityEvent<Toon> { }
        }
        [System.Serializable]
        public class Assets {
            public Transform light;
        }
        [System.Serializable]
        public class Config {
            public Color toon_warm = new Color(0.835f, 0.789f, 0.628f, 1f);
            public Color toon_cool = new Color(0.408f, 0.732f, 0.801f, 1f);
            public float tone_power = 1.1f;
            public Vector3 toon_lightDir = Vector3.forward;

            public float edge_size = 1f;
            public float edge_power = 3f;

            public Texture2D wobb_tex;
            public float wobb_scale = 1f;
            public float wobb_power = 0.005f;

            public List<PaperConfig> paper_configs = new List<PaperConfig>();

            [System.Serializable]
            public class PaperConfig {
                public Texture2D paper_tex;
                public float paper_scale = 1f;
                public float paper_power = 1f;
            }
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