using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;

public static class BuildProtagonistReview
{
    const string Root="Assets/_Game/Art/ProtagonistReview_v02";
    const string Scene="Assets/_Game/Scenes/Protagonist_Review_v02.unity";
    static Material Mat(string name,float r,float g,float b,string kind=null,float metallic=0,float smooth=.3f)
    {
        string path=Root+"/Materials/"+name+".mat";
        var m=AssetDatabase.LoadAssetAtPath<Material>(path);
        if(!m){m=new Material(Shader.Find("Universal Render Pipeline/Lit"));AssetDatabase.CreateAsset(m,path);}
        m.name=name;m.SetColor("_BaseColor",new Color(r,g,b));m.SetFloat("_Metallic",metallic);m.SetFloat("_Smoothness",smooth);
        if(kind!=null){
            m.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>(Root+"/Textures/"+kind+"_Base.png"));
            m.SetTexture("_BumpMap",AssetDatabase.LoadAssetAtPath<Texture2D>(Root+"/Textures/"+kind+"_Normal.png"));
            m.SetFloat("_BumpScale",.25f);m.EnableKeyword("_NORMALMAP");
            m.SetTexture("_MetallicGlossMap",AssetDatabase.LoadAssetAtPath<Texture2D>(Root+"/Textures/"+kind+"_MetalSmooth.png"));
            m.EnableKeyword("_METALLICSPECGLOSSMAP");
        }
        EditorUtility.SetDirty(m);return m;
    }
    public static object Build()
    {
        if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty)throw new Exception("Save current work before review scene creation.");
        if(File.Exists(Scene))throw new Exception("Review scene already exists; do not overwrite it.");
        Directory.CreateDirectory(Root+"/Materials");AssetDatabase.Refresh();
        foreach(string p in Directory.GetFiles(Root+"/Textures","*.png")){
            var t=(TextureImporter)AssetImporter.GetAtPath(p.Replace('\\','/'));
            if(p.Contains("_Normal"))t.textureType=TextureImporterType.NormalMap;
            if(p.Contains("_MetalSmooth"))t.sRGBTexture=false;
            t.SaveAndReimport();
        }
        var mats=new[]{
            Mat("Skin",.57f,.345f,.225f),Mat("Lips",.37f,.16f,.13f),Mat("Crease",.16f,.085f,.06f),
            Mat("Coat",.045f,.18f,.19f,"Linen"),Mat("CoatEdge",.09f,.255f,.26f,"Linen"),
            Mat("Undershirt",.47f,.43f,.32f,"Linen"),Mat("Trousers",.14f,.17f,.16f,"Linen"),
            Mat("Leather",.18f,.075f,.033f,"Leather"),Mat("LeatherEdge",.30f,.15f,.068f,"Leather"),Mat("DarkLeather",.055f,.038f,.026f,"Leather"),
            Mat("Steel",.34f,.40f,.42f,"Steel",.8f,.6f),Mat("Brass",.47f,.31f,.11f,"Steel",.7f,.55f),
            Mat("Hair",.045f,.022f,.012f),Mat("HairLight",.09f,.044f,.021f),Mat("EyeWhite",.64f,.62f,.52f),
            Mat("Iris",.14f,.075f,.032f),Mat("Pupil",.009f,.007f,.005f),Mat("Thread",.42f,.32f,.19f)};
        string fbx=Root+"/Wayfarer_Review_v02.fbx";var importer=(ModelImporter)AssetImporter.GetAtPath(fbx);
        importer.animationType=ModelImporterAnimationType.None;importer.importAnimation=false;
        importer.importCameras=false;importer.importLights=false;importer.materialImportMode=ModelImporterMaterialImportMode.ImportStandard;
        foreach(var m in mats)importer.AddRemap(new AssetImporter.SourceAssetIdentifier(typeof(Material),m.name),m);
        importer.SaveAndReimport();
        var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
        RenderSettings.ambientMode=AmbientMode.Flat;RenderSettings.ambientLight=new Color(.32f,.35f,.4f);RenderSettings.fog=false;
        var model=AssetDatabase.LoadAssetAtPath<GameObject>(fbx);
        string[] names={"Front — appearance study","Profile — appearance study","Back — appearance study"};
        for(int i=0;i<3;i++){
            var go=(GameObject)PrefabUtility.InstantiatePrefab(model);go.name=names[i];go.transform.position=new Vector3((i-1)*1.45f,0,0);
            go.transform.rotation=Quaternion.Euler(0,i==0?180:i==1?90:0,0);
        }
        var floor=GameObject.CreatePrimitive(PrimitiveType.Plane);floor.name="Neutral studio floor";floor.transform.localScale=Vector3.one*5;
        floor.GetComponent<Renderer>().sharedMaterial=Mat("StudioFloor",.105f,.12f,.14f);
        Light("Key",new Vector3(40,-35,0),new Color(1,.9f,.8f),2.2f,true);
        Light("Fill",new Vector3(15,140,0),new Color(.65f,.79f,1),.8f,false);
        Light("Rim",new Vector3(35,195,0),new Color(.88f,.93f,1),1.4f,false);
        var cam=new GameObject("Review Camera — front / profile / back").AddComponent<Camera>();cam.tag="MainCamera";
        cam.transform.position=new Vector3(0,1.2f,-6.5f);cam.transform.LookAt(new Vector3(0,.96f,0));cam.orthographic=true;cam.orthographicSize=1.38f;
        cam.clearFlags=CameraClearFlags.SolidColor;cam.backgroundColor=new Color(.07f,.083f,.105f);cam.nearClipPlane=.02f;cam.farClipPlane=50;
        AssetDatabase.SaveAssets();EditorSceneManager.SaveScene(scene,Scene);
        Selection.activeGameObject=GameObject.Find(names[0]);SceneView.lastActiveSceneView?.FrameSelected();
        return new {scene=Scene,meshCount=model.GetComponentsInChildren<MeshFilter>().Length,bounds=model.GetComponentsInChildren<Renderer>().Select(r=>r.bounds.size.ToString()).Take(2).ToArray()};
    }
    static void Light(string name,Vector3 rotation,Color color,float intensity,bool shadow){var l=new GameObject(name).AddComponent<Light>();l.type=LightType.Directional;l.transform.eulerAngles=rotation;l.color=color;l.intensity=intensity;l.shadows=shadow?LightShadows.Soft:LightShadows.None;l.shadowBias=.025f;}
}
