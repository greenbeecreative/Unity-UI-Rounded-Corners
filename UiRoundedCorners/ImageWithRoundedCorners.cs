using UnityEngine;
using UnityEngine.UI;

namespace Nobi.UiRoundedCorners {
	[ExecuteInEditMode]                             //Required to check the OnEnable function
	[DisallowMultipleComponent]                     //You can only have one of these in every object.
	[RequireComponent(typeof(RectTransform))]
	public class ImageWithRoundedCorners : MonoBehaviour {
		private static readonly int Props = Shader.PropertyToID("_WidthHeightRadius");
		private static readonly int prop_OuterUV = Shader.PropertyToID("_OuterUV");

		private static Shader MaterialShader;

		public float radius = 40f;
		private Material material;
		private Vector4 outerUV = new Vector4(0, 0, 1, 1);

		private MaskableGraphic image;

		private void OnValidate() {
			Validate();
			Refresh();
		}

		private void OnDestroy() {
			if (image != null && image.material == material) {
				image.material = null;
			}

			DestroyHelper.Destroy(material);
			image = null;
			material = null;
		}

		private void OnEnable() {
			//You can only add either ImageWithRoundedCorners or ImageWithIndependentRoundedCorners
			//It will replace the other component when added into the object.
			var other = GetComponent<ImageWithIndependentRoundedCorners>();
			if (other != null) {
				radius = other.r.x;                 //When it does, transfer the radius value to this script
				DestroyHelper.Destroy(other);
			}

			Validate();
			Refresh();
		}

		private void OnRectTransformDimensionsChange() {
			if (isActiveAndEnabled && material != null) {
				Refresh();
			}
		}

		public void Validate() {
			if (MaterialShader == null)
			{
				MaterialShader = Shader.Find("UI/RoundedCorners/RoundedCorners");
			}

			if (material == null)
			{
				material = new Material(MaterialShader);
				material.hideFlags = HideFlags.DontSave;
			}

			if (image == null) {
				TryGetComponent(out image);
			}

			if (image != null) {
				image.material = material;
			}

			if (image is Image uiImage && uiImage.sprite != null) {
				outerUV = UnityEngine.Sprites.DataUtility.GetOuterUV(uiImage.sprite);
			}
		}

		public void Refresh() {
			var rect = ((RectTransform)transform).rect;

			//Multiply radius value by 2 to make the radius value appear consistent with ImageWithIndependentRoundedCorners script.
			//Right now, the ImageWithIndependentRoundedCorners appears to have double the radius than this.
			material.SetVector(Props, new Vector4(rect.width, rect.height, radius * 2, 0));
			material.SetVector(prop_OuterUV, outerUV);
			
			if (image != null) {
				image.SetMaterialDirty();
			}
		}
	}
}