using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Twonek {
	public class GPUECSShaderReplacer : EditorWindow
	{

		//		public GameObject target;
		public static string shaderName;
		
		[MenuItem("Tools/GPU ECS ShaderReplacer by 2nek")]
		static void Init()
		{
			GPUECSShaderReplacer window = (GPUECSShaderReplacer )EditorWindow.GetWindow(typeof(GPUECSShaderReplacer ));
			window.Show();
			
			
		}


		string log = "";

		void OnGUI()
		{
			SkinnedMeshRenderer smr=null;
			GameObject target;
			if (Selection.gameObjects.Length != 1) {
				GUILayout.TextArea("error: Select a object");
				return;
			}
			shaderName = "Shader Graphs/GPUECSAnimator_HDRP_SimpleShader";
			target = Selection.gameObjects[0];
			log = "";
			if (target != null) {
				GUILayout.Label(""+ target.name); 
					smr	 = target.GetComponent<SkinnedMeshRenderer>();
			} else {
				GUILayout.TextArea("No target");
				return;
			}
			if (smr == null) {
				GUILayout.TextArea("error: No skin mesh renderer");
				return;
			}
			
			if (GUILayout.Button("Replace")) {
				smr = target.GetComponent<SkinnedMeshRenderer>();
				if (smr == null) {
					GUILayout.TextArea("error: No skin mesh renderer");
					return;
				}
				
				if (smr.sharedMaterials.Length != 1) {
					GUILayout.TextArea("error: should be one material");
					return;
				}
				Material mat = smr.sharedMaterials[0];
				Shader shader = Shader.Find(shaderName);
				Debug.Log("Shader=" + shaderName);
				if (shader== null) {
					GUILayout.TextArea("error: No found " + shaderName);
					Debug.LogError("Shader not found " + shaderName);
					return;
				} else {
					log = "Shader OK " + shader.name + " " + mat.GetPassName(0);
					
				}
				Shader orgShader = mat.shader;
				Debug.Log("Shaders=" + orgShader.name + " " + shader.name);
				if (shader.name == "") {
					Debug.LogError("Shader name is null");
					return;
				}
				Texture orgMainTexture = mat.GetTexture("_BaseColorMap");
				if (orgMainTexture == null)
					orgMainTexture = mat.GetTexture("_Base");
				Texture orgNrmTexture = mat.GetTexture("_NormalMap");
				if (orgNrmTexture == null)
					orgNrmTexture = mat.GetTexture("_Normal");
				
				mat.shader = shader;
				//	mat.GetShaderPassEnabled
				mat.SetTexture("_Base", orgMainTexture);
				mat.SetTexture("_Normal", orgNrmTexture);
				
				GUILayout.TextArea(log);
				
			}
		}
	}
}