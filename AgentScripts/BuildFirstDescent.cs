using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

public static class BuildFirstDescent
{
    const string Root = "Assets/_Game/Art/FirstDescent";
    const string ScenePath = "Assets/_Game/Scenes/FirstDescent_Cave.unity";
    static System.Random random = new System.Random(717);
    static float R(float min, float max) => min + (float)random.NextDouble() * (max-min);
    static void Set(UnityEngine.Object obj, string field, object value)
    {
        var so = new SerializedObject(obj); var p = so.FindProperty(field);
        if (p == null) throw new Exception(obj.name + ": missing " + field);
        if (value is UnityEngine.Object o) p.objectReferenceValue = o;
        else if (value == null) p.objectReferenceValue = null;
        else if (value is bool b) p.boolValue=b;
        else if (value is float f) p.floatValue=f;
        else if (value is Vector3 v) p.vector3Value=v;
        so.ApplyModifiedPropertiesWithoutUndo();
    }
    static Material Mat(string name, Color color, float metal=0, float smooth=.25f, bool emissive=false)
    {
        string path=Root+"/Materials/"+name+".mat";
        var mat=AssetDatabase.LoadAssetAtPath<Material>(path);
        if(mat!=null) return mat;
        mat=new Material(Shader.Find("Universal Render Pipeline/Lit")); mat.name=name;
        mat.SetColor("_BaseColor",color); mat.SetFloat("_Metallic",metal); mat.SetFloat("_Smoothness",smooth);
        mat.SetFloat("_Cull",0);
        if(emissive) {mat.EnableKeyword("_EMISSION"); mat.SetColor("_EmissionColor",color*2);}
        AssetDatabase.CreateAsset(mat,path); return mat;
    }
    public static object Import()
    {
        Directory.CreateDirectory(Root+"/Materials"); Directory.CreateDirectory(Root+"/Animations");
        Directory.CreateDirectory(Root+"/Meshes"); AssetDatabase.Refresh();
        var output=new List<object>();
        foreach(string name in new[]{"Wayfarer","SlateWarden"})
        {
            string path=Root+"/Models/"+name+".fbx";
            var importer=(ModelImporter)AssetImporter.GetAtPath(path);
            importer.animationType=ModelImporterAnimationType.Generic;
            importer.importAnimation=true; importer.importCameras=false; importer.importLights=false;
            importer.materialImportMode=ModelImporterMaterialImportMode.ImportStandard;
            importer.optimizeGameObjects=false; importer.SaveAndReimport();
            var model=AssetDatabase.LoadAssetAtPath<GameObject>(path);
            var transforms=model.GetComponentsInChildren<Transform>(true);
            var human=new List<HumanBone>();
            foreach(var humanName in HumanTrait.BoneName)
            {
                string boneName=humanName.Replace(" ","");
                if(transforms.Any(t=>t.name==boneName))
                    human.Add(new HumanBone { humanName=humanName,boneName=boneName,limit=new HumanLimit{useDefaultValues=true} });
            }
            var desc=new HumanDescription { human=human.ToArray(),
                skeleton=transforms.Select(t=>new SkeletonBone{name=t.name,position=t.localPosition,rotation=t.localRotation,scale=t.localScale}).ToArray(),
                upperArmTwist=.5f,lowerArmTwist=.5f,upperLegTwist=.5f,lowerLegTwist=.5f,armStretch=.05f,legStretch=.05f,feetSpacing=0,hasTranslationDoF=false };
            importer.animationType=ModelImporterAnimationType.Human; importer.avatarSetup=ModelImporterAvatarSetup.CreateFromThisModel;
            importer.humanDescription=desc;
            var clips=importer.defaultClipAnimations;
            foreach(var clip in clips)
            {
                string suffix=clip.name.Split('|').Last();
                string state=suffix.Substring(suffix.LastIndexOf('_')+1);
                clip.name=state; clip.loopTime=new[]{"Idle","Walk","Run","Block"}.Contains(state);
                clip.lockRootRotation=true; clip.lockRootHeightY=true; clip.lockRootPositionXZ=true;
                clip.keepOriginalOrientation=true; clip.keepOriginalPositionXZ=true; clip.keepOriginalPositionY=true;
            }
            importer.clipAnimations=clips;
            foreach(var source in AssetDatabase.LoadAllAssetsAtPath(path).OfType<Material>())
            {
                var color=source.HasProperty("_Color")?source.color:Color.gray;
                bool metal=source.name.Contains("Steel")||source.name.Contains("Brass")||source.name.Contains("Blade");
                var mat=Mat(source.name,color,metal?.7f:0,metal?.45f:.2f);
                importer.AddRemap(new AssetImporter.SourceAssetIdentifier(source),mat);
            }
            importer.SaveAndReimport();
            var avatar=AssetDatabase.LoadAllAssetsAtPath(path).OfType<Avatar>().FirstOrDefault();
            if(avatar==null || !avatar.isValid || !avatar.isHuman) throw new Exception(name+" humanoid Avatar invalid");
            string controllerPath=Root+"/Animations/"+name+".controller";
            var controller=AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
            if(controller==null)
            {
                controller=AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
                var sm=controller.layers[0].stateMachine;
                var imported=AssetDatabase.LoadAllAssetsAtPath(path).OfType<AnimationClip>().Where(c=>!c.name.StartsWith("__preview__")).ToArray();
                int i=0;
                foreach(var clip in imported)
                {
                    var state=sm.AddState(clip.name,new Vector3(250+(i%3)*220,(i/3)*90)); state.motion=clip;
                    if(clip.name=="Idle") sm.defaultState=state;
                    i++;
                }
                EditorUtility.SetDirty(controller);
            }
            output.Add(new {name, human=avatar.isHuman,valid=avatar.isValid,
                clips=AssetDatabase.LoadAllAssetsAtPath(path).OfType<AnimationClip>().Where(c=>!c.name.StartsWith("__preview__")).Select(c=>new {c.name,c.length}).ToArray()});
        }
        AssetDatabase.SaveAssets(); return output;
    }
    static Transform Child(GameObject go,string name) => go.GetComponentsInChildren<Transform>(true).First(t=>t.name==name);
    static void Visual(GameObject root,string name,bool enemy)
    {
        var old=root.transform.Find("Visual"); old.gameObject.SetActive(false);
        // Preserve the complete prototype hierarchy, references and geometry as a disabled backup.
        var model=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(Root+"/Models/"+name+".fbx"));
        model.name=name+"_Visual"; model.transform.SetParent(root.transform,false);
        model.transform.localScale=Vector3.one*(enemy?1.09f:1f);
        var animator=model.GetComponent<Animator>() ?? model.AddComponent<Animator>();
        animator.runtimeAnimatorController=AssetDatabase.LoadAssetAtPath<AnimatorController>(Root+"/Animations/"+name+".controller");
        animator.applyRootMotion=false; animator.cullingMode=AnimatorCullingMode.AlwaysAnimate;
        foreach(var r in model.GetComponentsInChildren<SkinnedMeshRenderer>()) r.updateWhenOffscreen=true;
        var bridge=root.AddComponent<CombatAnimator>(); bridge.animator=animator;
        bridge.bladeBase=Child(model,"SwordBase"); bridge.bladeTip=Child(model,"SwordTip");
        bridge.attackDuration=enemy?1.266667f:.866667f; bridge.damage=enemy?10:25;
        // Disable primitive-only visual effects, preserving their settings on the original scene.
        foreach(var b in root.GetComponents<MonoBehaviour>())
            if(b is EnemySwordSwing || b is EnemyAttackFeedback || b is PlayerAttackFeedback || b is HitReaction) b.enabled=false;
        var cc=root.GetComponent<CharacterController>(); cc.height=enemy?1.95f:1.8f;
        cc.center=new Vector3(0,cc.height*.5f,0); cc.radius=.27f; cc.stepOffset=.25f; cc.skinWidth=.035f;
        if(enemy)
        {
            Set(root.GetComponent<EnemyAI>(),"attackWindup",.65f); Set(root.GetComponent<EnemyAI>(),"attackCooldown",1.0f);
            Set(root.GetComponent<EnemyAI>(),"recoilDistance",0f);
            Set(root.GetComponent<EnemyAI>(),"moveSpeed",2.25f);
            Set(root.GetComponent<EnemyAI>(),"stoppingDistance",1.10f); Set(root.GetComponent<EnemyAI>(),"attackRange",1.65f);
            Set(root.GetComponent<EnemyAI>(),"useGravity",true);
            Set(root.GetComponent<EnemyPerception>(),"eyeHeight",1.5f); Set(root.GetComponent<EnemyPerception>(),"targetHeight",1.2f);
            Set(root.GetComponent<EnemyDeath>(),"animationDelay",2f);
            var marker=root.transform.Find("FacingMarker"); if(marker!=null) marker.gameObject.SetActive(false);
        }
        else
        {
            Set(root.GetComponent<PlayerMovement>(),"moveSpeed",2.65f); Set(root.GetComponent<PlayerMovement>(),"runMultiplier",1.85f);
            Set(root.GetComponent<PlayerMovement>(),"useGravity",true);
            Set(root.GetComponent<PlayerCombat>(),"attackWindup",.45f); Set(root.GetComponent<PlayerCombat>(),"attackCooldown",.90f);
            Set(root.GetComponent<PlayerDefense>(),"leftHandPoint",null); // Animator now owns the block pose.
        }
    }
    static GameObject MeshObject(string name,List<Vector3> vertices,List<int> tris,Transform parent,Material[] mats,bool collider)
    {
        // Separate triangle vertices for intentional rock facets and material variation.
        var flat=new List<Vector3>(); var groups=new List<int>[mats.Length];
        for(int j=0;j<groups.Length;j++) groups[j]=new List<int>();
        for(int i=0;i<tris.Count;i+=3)
        {
            int k=flat.Count; flat.Add(vertices[tris[i]]);flat.Add(vertices[tris[i+1]]);flat.Add(vertices[tris[i+2]]);
            groups[random.Next(groups.Length)].AddRange(new[]{k,k+1,k+2});
        }
        var mesh=new Mesh{name=name}; mesh.SetVertices(flat);mesh.subMeshCount=mats.Length;
        for(int j=0;j<groups.Length;j++)mesh.SetTriangles(groups[j],j);
        mesh.RecalculateNormals();mesh.RecalculateBounds(); AssetDatabase.CreateAsset(mesh,Root+"/Meshes/"+name+".asset");
        var go=new GameObject(name);go.transform.SetParent(parent,false);go.AddComponent<MeshFilter>().sharedMesh=mesh;
        go.AddComponent<MeshRenderer>().sharedMaterials=mats;
        if(collider)go.AddComponent<MeshCollider>().sharedMesh=mesh;
        return go;
    }
    static GameObject Rock(string name,Vector3 pos,Vector3 scale,Transform parent,Material[] mats,bool spike=false)
    {
        int n=7; var vs=new List<Vector3>();var ts=new List<int>();
        for(int row=0;row<4;row++)
        {
            float y=row/3f;float rad=spike?Mathf.Lerp(1,.015f,y):Mathf.Sin(Mathf.PI*(.12f+y*.8f));
            for(int j=0;j<n;j++)
            {float a=j*Mathf.PI*2/n;vs.Add(new Vector3(Mathf.Cos(a)*rad*R(.8f,1.15f),y,Mathf.Sin(a)*rad*R(.8f,1.15f)));}
        }
        for(int row=0;row<3;row++)for(int j=0;j<n;j++)
        {int a=row*n+j,b=row*n+(j+1)%n;ts.AddRange(new[]{a,a+n,b,b,a+n,b+n});}
        for(int j=1;j<n-1;j++)ts.AddRange(new[]{3*n,3*n+j+1,3*n+j});
        var go=MeshObject(name,vs,ts,parent,mats,true);go.transform.localPosition=pos;go.transform.localScale=scale;return go;
    }
    static void Light(string name,Vector3 position,Color color,float intensity,float range,Transform parent)
    {
        var go=new GameObject(name);go.transform.SetParent(parent);go.transform.position=position;
        var light=go.AddComponent<Light>();light.type=LightType.Point;light.color=color;light.intensity=intensity;light.range=range;
        light.shadows=LightShadows.Soft;light.shadowBias=.035f;light.shadowNormalBias=.12f;
    }
    public static object Scene()
    {
        if(EditorApplication.isPlaying)throw new Exception("Exit Play Mode first");
        if(File.Exists(ScenePath))throw new Exception("Scene exists: do not overwrite an existing iteration");
        var current=SceneManager.GetActiveScene(); if(current.isDirty)throw new Exception("Save current user work before building");
        if(current.path!="Assets/_Game/Scenes/Prototype.unity")throw new Exception("Start with original Prototype scene");
        EditorSceneManager.SaveScene(current,ScenePath,true);var scene=EditorSceneManager.OpenScene(ScenePath);
        var player=GameObject.Find("Player");var enemy=GameObject.Find("Enemy_Dummy");
        Visual(player,"Wayfarer",false); Visual(enemy,"SlateWarden",true);
        player.transform.position=new Vector3(0,.2f,-3);player.transform.rotation=Quaternion.identity;
        enemy.transform.position=new Vector3(0,.2f,3.5f);enemy.transform.rotation=Quaternion.Euler(0,180,0);
        var ground=GameObject.Find("Ground"); if(ground!=null)ground.SetActive(false);
        var sun=GameObject.Find("Directional Light");if(sun!=null)sun.SetActive(false);
        var volume=GameObject.Find("Global Volume");if(volume!=null)volume.SetActive(false);
        var camera=Camera.main;var follow=camera.GetComponent<CameraFollow>();if(follow!=null)follow.enabled=false;
        var orbit=camera.gameObject.AddComponent<CaveCamera>();orbit.target=player.transform;
        camera.fieldOfView=52;camera.nearClipPlane=.08f;camera.farClipPlane=60;
        camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.035f,.045f,.055f);
        camera.transform.position=player.transform.position+new Vector3(0,3.4f,-4.8f);camera.transform.LookAt(player.transform.position+Vector3.up*1.2f);
        var cave=new GameObject("First Descent — provisional combat cavern").transform;
        var stone=new[]{Mat("Basalt",new Color(.18f,.21f,.23f)),Mat("BasaltCool",new Color(.15f,.18f,.20f)),Mat("StoneOchre",new Color(.23f,.22f,.19f)),Mat("StonePale",new Color(.24f,.27f,.28f))};
        var floor=new[]{Mat("PackedStone",new Color(.23f,.24f,.23f)),Mat("PackedStoneWarm",new Color(.26f,.25f,.22f)),Mat("PackedStoneDark",new Color(.20f,.22f,.22f))};
        var vs=new List<Vector3>();var ts=new List<int>();int size=26;
        for(int z=0;z<=size;z++)for(int x=0;x<=size;x++)vs.Add(new Vector3(x-size*.5f+R(-.22f,.22f),R(-.045f,.025f),z-size*.5f+R(-.22f,.22f)));
        for(int z=0;z<size;z++)for(int x=0;x<size;x++){int a=z*(size+1)+x;ts.AddRange(new[]{a,a+size+1,a+1,a+1,a+size+1,a+size+2});}
        MeshObject("CavernFloor",vs,ts,cave,floor,true);
        vs=new List<Vector3>();ts=new List<int>();int segments=36;
        float[] heights={-.3f,1.8f,3.6f,5.7f,7.3f};float[] radii={1,1.04f,.94f,.68f,.20f};
        for(int row=0;row<5;row++)for(int j=0;j<segments;j++)
        {
            float a=j*Mathf.PI*2/segments;float radius=radii[row]*R(.94f,1.07f);
            vs.Add(new Vector3(Mathf.Cos(a)*11*radius,heights[row]+R(-.2f,.2f),Mathf.Sin(a)*9*radius));
        }
        for(int row=0;row<4;row++)for(int j=0;j<segments;j++)
        {int a=row*segments+j,b=row*segments+(j+1)%segments;ts.AddRange(new[]{a,b,a+segments,b,b+segments,a+segments});}
        MeshObject("CavernWallsAndVault",vs,ts,cave,stone,true);
        for(int i=0;i<30;i++)
        {
            float a=i*Mathf.PI*2/30;float radius=R(.77f,.94f);
            Rock("Rubble_"+i,new Vector3(Mathf.Cos(a)*10.5f*radius,-.06f,Mathf.Sin(a)*8.5f*radius),new Vector3(R(.5f,1.4f),R(.3f,1.3f),R(.5f,1.2f)),cave,stone);
        }
        for(int i=0;i<16;i++)
        {
            float a=R(0,Mathf.PI*2);Vector3 p=new Vector3(Mathf.Cos(a)*R(7.7f,9f),0,Mathf.Sin(a)*R(6.4f,7.5f));
            Rock("Stalagmite_"+i,p,new Vector3(R(.22f,.48f),R(.8f,2.2f),R(.22f,.48f)),cave,stone,true);
            Rock("Stalactite_"+i,new Vector3(p.x*.72f,5.7f,p.z*.72f),new Vector3(.32f,-R(.7f,1.7f),.30f),cave,stone,true);
        }
        var crystal=new[]{Mat("MineralJade",new Color(.075f,.40f,.37f),.25f,.6f,true)};
        for(int i=0;i<5;i++)Rock("Mineral_"+i,new Vector3(6.9f+R(-.4f,.4f),.1f,2.5f+R(-.5f,.5f)),new Vector3(.12f,R(.3f,.65f),.14f),cave,crystal,true);
        Light("Vault soft light",new Vector3(0,5,0),new Color(.70f,.81f,1),22,18,cave);
        Light("Warm entrance bounce",new Vector3(-3,3,-4),new Color(1,.69f,.39f),12,12,cave);
        Light("Mineral bounce",new Vector3(6,2,2),new Color(.25f,.8f,.73f),5,9,cave);
        Light("Distant warm bounce",new Vector3(-5,2,5),new Color(1,.56f,.30f),5,10,cave);
        RenderSettings.ambientMode=AmbientMode.Flat;RenderSettings.ambientLight=new Color(.19f,.22f,.25f);
        RenderSettings.fog=true;RenderSettings.fogMode=FogMode.ExponentialSquared;RenderSettings.fogDensity=.022f;RenderSettings.fogColor=new Color(.055f,.075f,.087f);
        var scenes=EditorBuildSettings.scenes.ToList();if(!scenes.Any(s=>s.path==ScenePath))scenes.Add(new EditorBuildSettingsScene(ScenePath,true));EditorBuildSettings.scenes=scenes.ToArray();
        EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
        Selection.activeGameObject=player;SceneView.lastActiveSceneView?.Frame(new Bounds(player.transform.position+Vector3.up,Vector3.one*4),false);
        return new {scene=ScenePath,playerAvatar=player.GetComponent<CombatAnimator>().animator.isHuman,enemyAvatar=enemy.GetComponent<CombatAnimator>().animator.isHuman,roots=scene.rootCount};
    }

    public static object Polish()
    {
        if(EditorApplication.isPlaying || SceneManager.GetActiveScene().path!=ScenePath)throw new Exception("Open cave in Edit Mode");
        GameObject.Find("Vault soft light").GetComponent<Light>().intensity=22;
        GameObject.Find("Warm entrance bounce").GetComponent<Light>().intensity=12;
        var mesh=AssetDatabase.LoadAssetAtPath<Mesh>(Root+"/Meshes/CavernFloor.asset");
        var vertices=mesh.vertices;
        var offsets=new Dictionary<Vector3,Vector2>();
        for(int i=0;i<vertices.Length;i++)
        {
            var v=vertices[i];
            if(!offsets.TryGetValue(v,out var off)){off=new Vector2(R(-.22f,.22f),R(-.22f,.22f));offsets[v]=off;}
            vertices[i]=v+new Vector3(off.x,0,off.y);
        }
        mesh.vertices=vertices;mesh.RecalculateNormals();mesh.RecalculateBounds();EditorUtility.SetDirty(mesh);
        var collider=GameObject.Find("CavernFloor").GetComponent<MeshCollider>();collider.sharedMesh=null;collider.sharedMesh=mesh;
        AssetDatabase.SaveAssets();EditorSceneManager.SaveScene(SceneManager.GetActiveScene());return true;
    }
}
