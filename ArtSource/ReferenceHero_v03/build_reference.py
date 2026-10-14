"""Editable reference study. No downloaded mesh, no image-to-3D inference.
Retains anatomical surface helpers from v02; clothing/equipment/hair rebuilt to reference.
Appearance only: requires artist review and deformation retopology before rigging.
"""
import os
HERE=os.path.dirname(os.path.abspath(__file__))
ROOT=os.path.abspath(os.path.join(HERE,'../..'))
source=open(ROOT+'/ArtSource/ProtagonistReview_v02/build_review.py',encoding='utf-8').read()
source=source.split('# Objects stay separately named')[0]
source=source.replace('ProtagonistReview_v02','ReferenceHero_v03')
source=source.replace("mat('Coat',(.045,.18,.19)","mat('Coat',(.105,.078,.084)")
source=source.replace("mat('CoatEdge',(.09,.255,.26)","mat('CoatEdge',(.245,.060,.071)")
source=source.replace("mat('Skin',(.57,.345,.225)","mat('Skin',(.60,.365,.23)")
source=source.replace("mat('Steel',(.34,.40,.42)","mat('Steel',(.21,.185,.20)")
source=source.replace("mat('Brass',(.47,.31,.11)","mat('Brass',(.56,.345,.15)")
source=source.replace('(1.617,.049,.048','(1.617,.055,.048').replace('(1.638,.063,.058','(1.638,.069,.058')
source=source.replace('(-.014,.0065),(0,.008),(.014,.0055)','(-.014,.004),(0,.0055),(.014,.005)')
source=source.replace('(.014,-.0045),(0,-.0055),(-.014,-.004)','(.014,-.003),(0,-.0038),(-.014,-.003)')
exec(compile(source,__file__,'exec'))
import bmesh
mat('Burgundy',(.255,.043,.057),'Linen',rough=.8)
mat('BurgundyLight',(.34,.068,.075),'Linen',rough=.76)
mat('GoldCloth',(.42,.265,.15),'Linen',rough=.77)

def remove_prefixes(prefixes):
    for ob in list(objects):
        if any(ob.name.startswith(p) for p in prefixes):
            objects.remove(ob);bpy.data.objects.remove(ob,do_unlink=True)
remove_prefixes(['Split coat skirt','Skirt hem','Folded coat lapel','Collar piping','Forged shoulder','Rolled steel','Armor rivet','Lace','L fitted sleeve','R fitted sleeve','L rolled cuff','R rolled cuff','L cuff edge','R cuff edge'])

def panel(name,vs,fs,ma,thick=.004):
    ob=mesh(name,vs,fs,ma);ob.modifiers.new('Tailored thickness','SOLIDIFY').thickness=thick;return ob
def ribbon_panel(name,left,right,ma):
    vs=[]
    for a,b in zip(left,right):vs.extend([a,b])
    return panel(name,vs,[(2*i,2*i+1,2*i+3,2*i+2) for i in range(len(left)-1)],ma)

# Reference: burgundy split long tabard over dark fitted doublet.
for s in [-1,1]:
    inner=[(s*.035,-.133,1.085),(s*.055,-.15,.94),(s*.080,-.163,.79),(s*.10,-.158,.66)]
    outer=[(s*.18,-.075,1.085),(s*.218,-.10,.94),(s*.252,-.11,.76),(s*.26,-.10,.56)]
    ribbon_panel('Burgundy front tabard',inner,outer,'Burgundy')
    ribbon_panel('Tabard woven border',[(x-s*.009,y-.002,z) for x,y,z in outer],[(x-s*.032,y-.004,z+.014) for x,y,z in outer],'GoldCloth')
    left=[(s*.02,.136,1.08),(s*.035,.16,.93),(s*.06,.18,.72),(s*.083,.17,.55)]
    right=[(s*.18,.078,1.08),(s*.235,.10,.90),(s*.267,.12,.69),(s*.25,.11,.60)]
    ribbon_panel('Split rear coat tail',left,right,'Burgundy')
    ribbon_panel('Rear hem border',[(x,y+.003,z+.035) for x,y,z in [left[-1],right[-1]]],[(x,y+.005,z) for x,y,z in [left[-1],right[-1]]],'GoldCloth')
    ribbon_panel('Burgundy doublet facing',[(s*.044,-.148,1.39),(s*.042,-.123,1.24),(s*.048,-.124,1.115)],[(s*.15,-.11,1.42),(s*.13,-.116,1.26),(s*.133,-.104,1.115)],'Burgundy')
    path('Gold doublet seam',[(s*.104,-.141,1.40),(s*.091,-.134,1.28),(s*.091,-.122,1.12)],[.004]*3,'GoldCloth',5)
    # Short outer sleeve, gathered linen forearm sleeve and fitted leather bracer.
    path('Short doublet sleeve',[(s*.181,.011,1.473),(s*.225,.006,1.443),(s*.279,-.008,1.35)], [.054,.088,.078],'Coat',14)
    path('Burgundy sleeve trim',[(s*.273,-.008,1.366),(s*.284,-.01,1.34)],[.082,.081],'Burgundy',14)
    path('Linen rolled sleeve',[(s*.278,-.011,1.343),(s*.303,-.018,1.285),(s*.318,-.027,1.232)], [.071,.074,.058],'Undershirt',16)
    for k in range(3):
        path('Rolled linen fold',[(s*(.294+k*.009),-.017-k*.005,1.302-k*.025),(s*(.302+k*.009),-.02-k*.005,1.288-k*.025)],[.075-k*.004,.071-k*.004],'Undershirt',14)
    path('Articulated leather bracer',[(s*.319,-.03,1.238),(s*.332,-.045,1.14),(s*.34,-.065,1.045)], [.062,.060,.048],'Leather',12)
    for z,x,y,r in [(1.224,.321,-.033,.067),(1.14,.332,-.047,.064),(1.063,.338,-.063,.055)]:
        path('Bracer cross strap',[(s*x,y,z+.006),(s*x,y,z-.008)],[r,r],'LeatherEdge',12)
        bead('Bracer brass stud',(s*x,-.11,z),(.006,.004,.006),'Brass',8,4)
    # Upper boot wrap and diagonal ankle straps follow calf contours.
    for z in [.18,.29,.39]:
        path('Boot wrap strap',[(s*.13,.01,z-.013),(s*.13,.01,z+.015)],[.075 if z<.3 else .09]*2,'LeatherEdge',14,1.18)
    # Simple reference diamond embroidery as modeled cloth inlay.
    cx=s*.177;z=.72
    panel('Tabard diamond',[(cx,-.149,z+.027),(cx+.012,-.149,z),(cx,-.149,z-.027),(cx-.012,-.149,z)],[(0,1,2,3)],'GoldCloth',.001)

# Scarf is a continuous draped surface, low V in front, raised folded hood at back.
vs=[];fs=[];n=48
for row in range(6):
    t=row/5
    for j in range(n):
        a=j*math.tau/n;front=max(0,-math.sin(a));back=max(0,math.sin(a))
        rx=.078+t*.103;ry=.072+t*.074
        z=1.567-t*.060-front*t*.085+back*t*.025+.007*math.sin(a*4+t*7)
        vs.append((math.cos(a)*rx,math.sin(a)*ry,z))
for k in range(5):
    for j in range(n):a=k*n+j;b=k*n+(j+1)%n;fs.extend([(a,b,b+n),(a,b+n,a+n)])
panel('Draped burgundy scarf',vs,fs,'Burgundy',.008)
for k in [1,3,5]:path('Scarf folded edge',vs[k*n:(k+1)*n]+[vs[k*n]],[.006]*(n+1),'BurgundyLight',5)
panel('Folded hood back',[(-.12,.093,1.57),(.12,.093,1.57),(.17,.158,1.48),(0,.185,1.36),(-.17,.158,1.48),(0,.186,1.50)],[(0,1,5),(1,2,5),(2,3,5),(3,4,5),(4,0,5)],'Burgundy',.01)

# Broad left pauldron: convex polygon plates, brass border, two overlapping lames.
for k in range(3):
    x=.225+k*.035;z=1.477-k*.065
    rim=[(x-.045,-.095,z+.018),(x+.058,-.095,z-.015),(x+.105,-.05,z-.04),(x+.105,.067,z-.033),(x+.025,.11,z+.006),(x-.054,.061,z+.031)]
    center=(x+.032,.007,z+.035-k*.007)
    ob=panel('Faceted shoulder plate',rim+[center],[(j,(j+1)%6,6) for j in range(6)],'Steel',.009)
    path('Brass shoulder rim',rim+[rim[0]],[.008]*7,'Brass',5)
    for j in [0,2,4]:bead('Plate fastener',rim[j],(.009,.009,.009),'Brass',6,3)

# Pointed locks are closed tapered volumes with an intentional ridge, not hair cards.
def hair_lock(name,points,width):
    vs=[]
    for i,p in enumerate(points):
        p=Vector(p);t=(Vector(points[min(i+1,len(points)-1)])-Vector(points[max(0,i-1)])).normalized()
        normal=Vector((p.x,(p.y-.015),max(.035,p.z-1.78))).normalized()
        side=t.cross(normal).normalized();w=width*[.65,1,.72,.025][i]
        vs.extend([p-side*w,p+normal*w*.6,p+side*w,p-normal*w*.20])
    fs=[]
    for k in range(3):
        for j in range(4):a=k*4+j;b=k*4+(j+1)%4;fs.append((a,b,b+4,a+4))
    fs.extend([(3,2,1,0),(12,13,14,15)])
    ob=mesh(name,vs,fs,'Hair');ob.data.materials.append(materials['HairLight'])
    for f in ob.data.polygons:
        if f.index%7==1:f.material_index=1
for row in range(3):
    for j in range(9):
        a=j*math.tau/9+row*.34;z=1.795+row*.028
        p=Vector((.078*math.cos(a),.015+.077*math.sin(a),z))
        tangent=Vector((-.025,-.028,.025))
        tip=Vector((p.x+math.cos(a)*.028-.025,p.y+math.sin(a)*.025,z-.041+row*.012))
        hair_lock('Layered swept hair',[p,p+tangent,p+tangent*.7+Vector((math.cos(a)*.025,math.sin(a)*.025,.008)),tip],.023)
for j in range(7):
    x=-.068+j*.021
    hair_lock('Forehead swept fringe',[(x,.005,1.858),(x-.026,-.042,1.895),(x-.04,-.079,1.835),(x-.047,-.087,1.759+abs(j-3)*.008)],.022)

# Sheathed sword on left hip, with shaped blade sheath, brass collar and grip.
path('Sword scabbard',[(.231,.034,1.055),(.265,.055,.89),(.304,.085,.65),(.344,.103,.43)],[.026,.024,.022,.008],'DarkLeather',8,.55)
for p in [(.231,.034,1.05),(.343,.103,.445)]:path('Scabbard brass fitting',[p,(p[0]+.002,p[1],p[2]-.022)],[.029,.026],'Brass',8,.6)
path('Sword leather grip',[(.209,.025,1.17),(.23,.034,1.064)],[.014,.014],'Leather',10)
path('Sword crossguard',[(.17,.027,1.074),(.23,.034,1.066),(.29,.04,1.083)],[.008,.013,.008],'Brass',8)
bead('Sword pommel',(.207,.025,1.182),(.018,.017,.027),'Brass',6,3)

# Recalculate winding, embed the supplied reference, retain all pieces for editing.
for ob in objects:
    bm=bmesh.new();bm.from_mesh(ob.data);bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces));bm.to_mesh(ob.data);bm.free()
reference=bpy.data.images.load(HERE+'/Reference.png');reference.pack()
refob=bpy.data.objects.new('PRIMARY REFERENCE — supplied by user',None);bpy.context.collection.objects.link(refob)
refob.empty_display_type='IMAGE';refob.data=reference;refob.empty_display_size=2.5;refob.location=(2,1,1);refob.rotation_euler=(math.pi/2,0,0);refob.hide_render=True
bpy.ops.object.select_all(action='DESELECT')
for ob in objects:ob.select_set(True)
bpy.context.view_layer.objects.active=objects[0]
bpy.context.scene['review_status']='Reference likeness study. NOT replacement-approved. No rig yet.'
bpy.ops.wm.save_as_mainfile(filepath=HERE+'/ReferenceHero_v03.blend')
bpy.ops.export_scene.fbx(filepath=OUT+'/ReferenceHero_v03.fbx',use_selection=True,object_types={'MESH'},add_leaf_bones=False,bake_anim=False,axis_forward='-Z',axis_up='Y',apply_unit_scale=True,use_mesh_modifiers=True)
print('REFERENCE_REVIEW',len(objects),'editable pieces',sum(len(o.data.vertices) for o in objects),'vertices')
