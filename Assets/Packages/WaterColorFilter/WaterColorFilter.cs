using UnityEngine;
using System.Collections;

namespace WaterColorFilterSystem {

	[ExecuteInEditMode]
	public class WaterColorFilter : MonoBehaviour {
		public const int PASS_WOBB = 0;
		public const int PASS_EDGE = 1;
		public const int PASS_PAPER = 2;

		public const string PROP_WOBB_TEX = "_WobbTex";
		public const string PROP_WOBB_TEX_SCALE = "_WobbScale";
		public const string PROP_WOBB_POWER = "_WobbPower";
		public const string PROP_EDGE_SIZE = "_EdgeSize";
		public const string PROP_EDGE_POWER = "_EdgePower";
		public const string PROP_PAPER_TEX = "_PaperTex";
		public const string PROP_PAPER_SCALE = "_PaperScale";
		public const string PROP_PAPER_POWER = "_PaperPower";

		public Shader filter;

		public Texture wobbTex;
		public float wobbScale = 1f;
		public float wobbPower = 0.01f;
		public float edgeSize = 1f;
		public float edgePower = 3f;
		public PaperData[] paperDataset;

		Material _filterMat;

		void OnEnable() {
			_filterMat = new Material(filter);
		}
		void OnDisable() {
			DestroyImmediate(_filterMat);
		}
		void OnRenderImage(RenderTexture src, RenderTexture dst) {
			_filterMat.SetTexture(PROP_WOBB_TEX, wobbTex);
			_filterMat.SetFloat(PROP_WOBB_TEX_SCALE, wobbScale);
			_filterMat.SetFloat(PROP_WOBB_POWER, wobbPower);
			_filterMat.SetFloat(PROP_EDGE_SIZE, edgeSize);
			_filterMat.SetFloat(PROP_EDGE_POWER, edgePower);
			
			var rt0 = RenderTexture.GetTemporary(src.width, src.height, 0, RenderTextureFormat.ARGB32);
			var rt1 = RenderTexture.GetTemporary(src.width, src.height, 0, RenderTextureFormat.ARGB32);
			var nPapers = paperDataset.Length;

			Graphics.Blit(src, rt1, _filterMat, PASS_WOBB);
			Swap(ref rt0, ref rt1);

			if (nPapers > 0) {
				Graphics.Blit(rt0, rt1, _filterMat, PASS_EDGE);
				Swap(ref rt0, ref rt1);
				
				for (var i = 0; i < nPapers; i++) {
					paperDataset[i].SetProps(_filterMat);
					Graphics.Blit(rt0, (i == (nPapers - 1) ? dst : rt1), _filterMat, PASS_PAPER);
					Swap(ref rt0, ref rt1);
				}
			} else {
				Graphics.Blit(rt0, dst, _filterMat, PASS_EDGE);
			}

			RenderTexture.ReleaseTemporary(rt0);
			RenderTexture.ReleaseTemporary(rt1);
		}

		void Swap(ref RenderTexture src, ref RenderTexture dst) {
			var tmp = src;
			src = dst;
			dst = tmp;
		}

		[System.Serializable]
		public class PaperData {
			public Texture paperTex;
			public float paperScale = 1f;
			public float paperPower = 1f;

			public void SetProps(Material mat) {
				mat.SetTexture(PROP_PAPER_TEX, paperTex);
				mat.SetFloat(PROP_PAPER_SCALE, paperScale);
				mat.SetFloat(PROP_PAPER_POWER, paperPower);
			}
		}
	}
}
