using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
public class SmoothNormalTool : OdinEditorWindow
{
    [MenuItem("Tools/平滑法线工具")]
    private static void OpenWindow()
    {
        GetWindow<SmoothNormalTool>("平滑法线工具");
    }
    
    [LabelText("需要平滑法线的物体")]
    public GameObject[] targets;

    [LabelText("存储方式")] public StoreMethod method;
    
    bool isPreview = false;
    //private Shader previewShader;
    private Material previewMaterial;


    //Unity内部反射OnEnable方法
    private void OnEnable()
    {
        previewMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Tool/SmoothNormalTool/M_Preview_Normal.mat");

    } 
    public enum StoreMethod
    {
        Tangent,
        VertexColor,
        uv2,
        uv3
    }
    
    
    [Button("平滑法线")]
    private void Apply()
    {
        switch (method)
        {
            case StoreMethod.Tangent:
                foreach (var obj in targets)
                {
                    if (obj.GetComponent<MeshFilter>() !=null)
                    {
                        obj.GetComponent<MeshFilter>().sharedMesh.tangents =
                            SmoothNormals(obj.GetComponent<MeshFilter>().sharedMesh);
                    }
                }
                return;
            
        }

    }

    [Button("预览列表中的物体")]
    [ShowIf("@!isPreview")]
    void EnablePreview()
    {
        Debug.Log(previewMaterial.shader.name);
        previewMaterial.EnableKeyword("Preview_Normal");
        isPreview = true;
    }
    
    [Button("关闭预览")]
    [ShowIf("@isPreview")]
    void DisablePreview()
    {
        previewMaterial.DisableKeyword("Preview_Normal");
        isPreview = false;
    }
    
    Vector4[] SmoothNormals(Mesh mesh)
    {
        //Unity中的 normal vertices uv里面的点的索引 都是对应的 按照绘制三角形的编号排列
        //取在相同位置的顶点进行平滑，存入切线或uv或顶点色中
        //先做一个字典存，顶点编号和位置的List，然后对每个位置求平均数存成新的字典，遍历字典对一个空数组的对应位置替换
        
        Dictionary<Vector3, Vector3> SmoothNormalDic = new Dictionary<Vector3, Vector3>(); // Position,Normal

        for (int i = 0; i < mesh.vertexCount; i++)
        {
            if (!SmoothNormalDic.ContainsKey(mesh.vertices[i]))
            {
                SmoothNormalDic.Add(mesh.vertices[i], mesh.normals[i]);
            }
            else
            {
                SmoothNormalDic[mesh.vertices[i]] = Vector3.Normalize(SmoothNormalDic[mesh.vertices[i]] + mesh.normals[i]);
            }
        }
        
        Vector4[] smoothNormals = new Vector4[mesh.vertexCount];
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            smoothNormals[i] = new Vector4(SmoothNormalDic[mesh.vertices[i]].x, SmoothNormalDic[mesh.vertices[i]].y, SmoothNormalDic[mesh.vertices[i]].z,0);
        }
        
        return smoothNormals;
    }

}
