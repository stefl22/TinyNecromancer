"""Original Tiny Necromancer characters. Run with Blender --background --python.
No downloaded geometry. Blender Z up, -Y forward. All actions are full-body keys.
"""
import bpy, math, os, random
from mathutils import Vector, Quaternion

ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), '../..'))
OUT = os.path.join(ROOT, 'Assets/_Game/Art/FirstDescent/Models')
os.makedirs(OUT, exist_ok=True)
random.seed(71)

def material(name, color, metal=0, rough=.65):
    m=bpy.data.materials.new(name); m.diffuse_color=(*color,1); m.use_nodes=True
    bs=m.node_tree.nodes.get('Principled BSDF'); bs.inputs['Base Color'].default_value=(*color,1)
    bs.inputs['Metallic'].default_value=metal; bs.inputs['Roughness'].default_value=rough
    return m

def build(enemy):
    bpy.ops.object.select_all(action='SELECT'); bpy.ops.object.delete(use_global=False)
    for a in list(bpy.data.actions): bpy.data.actions.remove(a)
    name='SlateWarden' if enemy else 'Wayfarer'
    mats={
        'skin': material(name+'_Skin', (.49,.30,.20) if enemy else (.66,.43,.29)),
        'cloth': material(name+'_Cloth', (.29,.075,.055) if enemy else (.055,.21,.24)),
        'clothLight': material(name+'_ClothEdge', (.44,.16,.09) if enemy else (.12,.34,.34)),
        'leather': material(name+'_Leather', (.085,.065,.05) if enemy else (.19,.09,.04)),
        'pants': material(name+'_Trousers', (.09,.105,.115) if enemy else (.12,.15,.16)),
        'metal': material(name+'_Steel', (.22,.27,.30) if enemy else (.31,.38,.40), .7,.34),
        'trim': material(name+'_Brass', (.40,.29,.11),.65,.4),
        'hair': material(name+'_Hair', (.105,.09,.07) if enemy else (.065,.032,.018)),
        'white': material(name+'_Eye', (.66,.63,.52)),
        'dark': material(name+'_Details', (.025,.033,.034)),
        'blade': material(name+'_Blade', (.57,.65,.68),.85,.24),
    }
    arm=bpy.data.armatures.new(name+'_Skeleton'); rig=bpy.data.objects.new(name+'_Rig',arm)
    bpy.context.collection.objects.link(rig); bpy.context.view_layer.objects.active=rig; rig.select_set(True)
    bpy.ops.object.mode_set(mode='EDIT')
    bones={}
    def bone(n,h,t,p=None):
        b=arm.edit_bones.new(n); b.head=h; b.tail=t
        if p: b.parent=arm.edit_bones[p]
        bones[n]=(Vector(h),Vector(t))
    bone('Hips',(0,0,.93),(0,0,1.08))
    bone('Spine',(0,0,1.08),(0,0,1.25),'Hips')
    bone('Chest',(0,0,1.25),(0,0,1.44),'Spine')
    bone('Neck',(0,0,1.44),(0,0,1.56),'Chest')
    bone('Head',(0,0,1.56),(0,0,1.79),'Neck')
    for side,s in [('Left',1),('Right',-1)]:
        bone(side+'Shoulder',(s*.045,0,1.43),(s*.20,0,1.43),'Chest')
        bone(side+'UpperArm',(s*.20,0,1.43),(s*.48,0,1.43),side+'Shoulder')
        bone(side+'LowerArm',(s*.48,0,1.43),(s*.73,0,1.43),side+'UpperArm')
        bone(side+'Hand',(s*.73,0,1.43),(s*.83,0,1.43),side+'LowerArm')
        bone(side+'UpperLeg',(s*.105,0,.95),(s*.12,.018,.53),'Hips')
        bone(side+'LowerLeg',(s*.12,.018,.53),(s*.12,0,.13),side+'UpperLeg')
        bone(side+'Foot',(s*.12,0,.13),(s*.12,-.15,.06),side+'LowerLeg')
        bone(side+'Toes',(s*.12,-.15,.06),(s*.12,-.23,.06),side+'Foot')
    bone('SwordBase',(-.89,0,1.43),(-.99,0,1.43),'RightHand')
    bone('SwordTip',(-1.63,0,1.43),(-1.68,0,1.43),'RightHand')
    bpy.ops.object.mode_set(mode='OBJECT'); rig.select_set(False)
    pieces=[]
    def mesh(n,verts,faces,mat,weights):
        me=bpy.data.meshes.new(n); me.from_pydata(verts,[],faces); me.update()
        ob=bpy.data.objects.new(n,me); bpy.context.collection.objects.link(ob); ob.data.materials.append(mats[mat])
        if isinstance(weights,str): weights=[{weights:1} for _ in verts]
        for i,ws in enumerate(weights):
            for b,w in ws.items():
                vg=ob.vertex_groups.get(b) or ob.vertex_groups.new(name=b); vg.add([i],w,'REPLACE')
        pieces.append(ob); return ob
    def rings(n,rows,mat,segments=10):
        # rows: center, cross-section widths X/Y, skin weights
        vs=[]; fs=[]; ws=[]
        for center,rx,ry,w in rows:
            for j in range(segments):
                a=2*math.pi*j/segments
                vs.append((center[0]+rx*math.cos(a),center[1]+ry*math.sin(a),center[2])); ws.append(w)
        for k in range(len(rows)-1):
            for j in range(segments):
                a=k*segments+j; b=k*segments+(j+1)%segments; c=b+segments; d=a+segments
                fs.append((a,b,c,d))
        fs.extend([tuple(range(segments-1,-1,-1)),tuple((len(rows)-1)*segments+j for j in range(segments))])
        return mesh(n,vs,fs,mat,ws)
    def ellipsoid(n,center,scale,mat,bone,sub=1):
        bpy.ops.mesh.primitive_ico_sphere_add(subdivisions=sub,radius=1,location=center)
        ob=bpy.context.object; ob.name=n; ob.scale=scale
        bpy.ops.object.transform_apply(location=False,rotation=False,scale=True)
        ob.data.materials.append(mats[mat]); vg=ob.vertex_groups.new(name=bone); vg.add(list(range(len(ob.data.vertices))),1,'REPLACE'); pieces.append(ob)
        return ob
    def tube(n,a,b,r1,r2,mat,bn,seg=8):
        a,b=Vector(a),Vector(b); axis=(b-a).normalized(); u=axis.cross(Vector((0,1,0))).normalized(); v=axis.cross(u)
        vs=[]
        for p,r in [(a,r1),(b,r2)]:
            for j in range(seg):
                vs.append(p+r*(u*math.cos(j*math.tau/seg)+v*math.sin(j*math.tau/seg)))
        fs=[tuple(range(seg-1,-1,-1)),tuple(range(seg,seg*2))]
        fs.extend((j,(j+1)%seg,(j+1)%seg+seg,j+seg) for j in range(seg))
        return mesh(n,vs,fs,mat,bn)
    bulk=1.13 if enemy else 1
    rings('TailoredTunic',[
        ((0,0,.91),.185*bulk,.125,{'Hips':1}),
        ((0,0,1.03),.163*bulk,.105,{'Hips':.65,'Spine':.35}),
        ((0,0,1.17),.17*bulk,.105,{'Spine':.75,'Chest':.25}),
        ((0,0,1.33),.225*bulk,.125,{'Chest':1}),
        ((0,0,1.43),.22*bulk,.115,{'Chest':1}),
        ((0,0,1.47),.095,.085,{'Chest':1}),], 'cloth',12)
    rings('Belt',[((0,0,1.02),.174*bulk,.117,{'Hips':1}),((0,0,1.085),.17*bulk,.115,{'Hips':1})],'leather',12)
    ellipsoid('Buckle',(0,-.12,1.052),(.044,.014,.036),'trim','Hips')
    rings('Collar',[((0,0,1.435),.107,.089,{'Chest':1}),((0,0,1.49),.094,.079,{'Neck':1})],'clothLight',10)
    rings('Neck',[((0,0,1.46),.059,.060,{'Neck':1}),((0,0,1.59),.058,.063,{'Head':1})],'skin',8)
    # Jaw, cheekbones, forehead: adult proportions, head ~1/7.5 of height.
    rings('Face',[
        ((0,-.016,1.55),.052,.054,{'Head':1}),
        ((0,-.014,1.59),.079,.071,{'Head':1}),
        ((0,0,1.65),.094,.084,{'Head':1}),
        ((0,.003,1.71),.091,.080,{'Head':1}),
        ((0,.006,1.77),.071,.063,{'Head':1}),
        ((0,.008,1.795),.025,.025,{'Head':1})], 'skin',12)
    mesh('Nose',[(-.018,-.079,1.684),(.018,-.079,1.684),(0,-.116,1.633),(-.019,-.083,1.629),(.019,-.083,1.629)],[(0,1,2),(0,2,3),(1,4,2),(3,2,4)],'skin','Head')
    for s in [-1,1]:
        ellipsoid('Ear',(s*.096,0,1.661),(.019,.024,.038),'skin','Head',1)
        ellipsoid('Eye',(s*.041,-.075,1.678),(.021,.010,.009),'white','Head',1)
        ellipsoid('Iris',(s*.042,-.084,1.678),(.007,.004,.008),'dark','Head',1)
        tube('Brow',(s*.021,-.08,1.698),(s*.067,-.069,1.696),.009,.007,'hair','Head',5)
    tube('Mouth',(-.025,-.082,1.602),(.025,-.082,1.602),.003,.003,'leather','Head',5)
    ellipsoid('HairCrown',(0,.01,1.757),(.098,.091,.061),'hair','Head',2)
    for i in range(7):
        x=-.074+i*.024
        ellipsoid('SweptHair',(x,-.045,1.756+(.012 if i>3 else 0)),(.030,.054,.044),'hair','Head',1)
    if enemy:
        rings('BrowHelm',[((0,.006,1.72),.105,.094,{'Head':1}),((0,.012,1.80),.088,.078,{'Head':1}),((0,.015,1.84),.025,.035,{'Head':1})],'metal',8)
        for s in [-1,1]: ellipsoid('TempleGuard',(s*.096,.014,1.67),(.024,.065,.075),'metal','Head',1)
    for side,s in [('Left',1),('Right',-1)]:
        up=side+'UpperLeg'; lo=side+'LowerLeg'; ft=side+'Foot'
        rings(side+'Leg',[
            ((s*.102,0,.98),.108,.105,{up:1}),
            ((s*.113,0,.80),.100,.10,{up:1}),
            ((s*.12,.015,.57),.073,.076,{up:.75,lo:.25}),
            ((s*.12,.018,.51),.066,.070,{up:.25,lo:.75}),
            ((s*.12,0,.36),.065,.070,{lo:1}),
            ((s*.12,0,.15),.047,.053,{lo:1})],'pants',9)
        rings(side+'BootShaft',[((s*.12,0,.15),.063,.07,{lo:1}),((s*.12,0,.36),.076,.083,{lo:1}),((s*.12,0,.39),.078,.084,{lo:1})],'leather',8)
        ellipsoid(side+'Boot',(s*.12,-.066,.082),(.081,.17,.082),'leather',ft,2)
        ellipsoid(side+'Sole',(s*.12,-.063,.026),(.084,.173,.029),'dark',ft,1)
        ellipsoid(side+'Knee',(s*.12,-.06,.535),(.079,.044,.091),'metal' if enemy else 'leather',lo,1)
        # Split tunic skirts deform with thighs rather than covering the crotch with a box.
        mesh(side+'TunicSkirt',[(s*.01,-.118,1.03),(s*.185,-.10,1.03),(s*.21,-.118,.78),(s*.028,-.141,.77)],[(0,1,2),(0,2,3)],'clothLight',up)
        ua=side+'UpperArm'; la=side+'LowerArm'; ha=side+'Hand'
        tube(side+'Sleeve',(s*.18,0,1.43),(s*.41,0,1.43),.104,.078,'cloth',ua,10)
        ellipsoid(side+'Elbow',(s*.48,0,1.43),(.074,.069,.070),'skin',la,1)
        tube(side+'Forearm',(s*.40,0,1.43),(s*.735,0,1.43),.070,.042,'skin',la,9)
        tube(side+'Bracer',(s*.555,0,1.43),(s*.715,0,1.43),.070,.054,'leather',la,8)
        tube(side+'BracerRim',(s*.565,0,1.43),(s*.588,0,1.43),.075,.071,'trim',la,8)
        ellipsoid(side+'Hand',(s*.78,-.002,1.43),(.067,.044,.042),'leather',ha,2)
        ellipsoid(side+'Thumb',(s*.765,-.04,1.41),(.026,.028,.036),'skin',ha,1)
        if enemy or side=='Left':
            ellipsoid(side+'Pauldron',(s*.232,0,1.455),(.145,.14,.104),'metal',ua,1)
            ellipsoid(side+'PauldronInset',(s*.235,-.03,1.505),(.103,.10,.053),'trim',ua,1)
    # Leather diagonal harness and original three-stud detail.
    tube('CrossHarness',(-.15,-.121,1.10),(.155,-.133,1.415),.025,.028,'leather','Chest',4)
    for z in [1.22,1.29,1.36]: ellipsoid('HarnessStud',((z-1.10)*.97-.15,-.15,z),(.011,.008,.011),'trim','Chest',1)
    ellipsoid('BeltPouch',(-.19,-.02,1.0),(.073,.068,.092),'leather','Hips',1)
    if enemy:
        mesh('ShortMantle',[(-.25,.08,1.43),(.25,.08,1.43),(.23,.17,1.1),(.12,.18,.94),(-.18,.18,.96),(-.26,.15,1.15)],[(0,1,2),(0,2,5),(5,2,3),(5,3,4)],'clothLight','Chest')
    # A proper modeled sword (weighted to hand); no runtime transform swing.
    tube('SwordGrip',(-.74,0,1.43),(-.91,0,1.43),.026,.024,'leather','RightHand',8)
    ellipsoid('Pommel',(-.725,0,1.43),(.035,.037,.037),'trim','RightHand',1)
    tube('CrossGuard',(-.905,0,1.30),(-.905,0,1.56),.022,.020,'trim','RightHand',6)
    length=1.72 if enemy else 1.63
    mesh('ForgedBlade',[(-.925,0,1.382),(-.925,-.015,1.43),(-.925,0,1.478),(-.925,.015,1.43),(-length,0,1.43),(-length+.14,0,1.395),(-length+.14,-.012,1.43),(-length+.14,0,1.465),(-length+.14,.012,1.43)],[(0,1,6,5),(1,2,7,6),(2,3,8,7),(3,0,5,8),(5,6,4),(6,7,4),(7,8,4),(8,5,4)],'blade','RightHand')
    if not enemy:
        # Kite-shaped travel buckler: asymmetric kit, not a toy shield.
        vs=[(.58,-.067,1.43),(.69,-.067,1.60),(.91,-.067,1.59),(1.03,-.067,1.43),(.91,-.067,1.27),(.69,-.067,1.27),(.79,-.095,1.43)]
        mesh('TravelShield',vs,[(i,(i+1)%6,6) for i in range(6)],'metal','LeftHand')
        for i in range(6): tube('ShieldRim',vs[i],vs[(i+1)%6],.013,.013,'trim','LeftHand',5)
        ellipsoid('ShieldBoss',(.79,-.098,1.43),(.052,.031,.052),'trim','LeftHand',1)
    # Join keeps material slots and weights. Faceted surfaces are deliberate.
    bpy.ops.object.select_all(action='DESELECT')
    for ob in pieces: ob.select_set(True)
    bpy.context.view_layer.objects.active=pieces[0]; bpy.ops.object.join()
    body=bpy.context.object; body.name=name+'_SkinnedMesh'
    mod=body.modifiers.new('HumanoidSkin','ARMATURE'); mod.object=rig; body.parent=rig
    for p in body.data.polygons: p.use_smooth=False
    rig.animation_data_create()
    def rotate(n,xyz):
        pb=rig.pose.bones[n]; rest=arm.bones[n].matrix_local.to_quaternion()
        q=Quaternion((1,0,0),math.radians(xyz[0])) @ Quaternion((0,1,0),math.radians(xyz[1])) @ Quaternion((0,0,1),math.radians(xyz[2]))
        pb.rotation_quaternion=rest.inverted() @ q @ rest
    def aim_right(direction):
        rest=arm.bones['RightUpperArm'].matrix_local.to_quaternion()
        q=Vector((-1,0,0)).rotation_difference(Vector(direction).normalized())
        rig.pose.bones['RightUpperArm'].rotation_quaternion=rest.inverted() @ q @ rest
    def pose(frame,kind,t):
        for pb in rig.pose.bones: pb.rotation_mode='QUATERNION'; pb.rotation_quaternion=(1,0,0,0); pb.location=(0,0,0)
        # Relaxed arms and subtly flexed elbows, feet grounded.
        rotate('LeftUpperArm',(-8,78,0)); rotate('RightUpperArm',(4,-78,0))
        rotate('LeftLowerArm',(-12,0,0)); rotate('RightLowerArm',(-8,0,0))
        rotate('Chest',(0,0,2 if enemy else -1))
        phase=math.tau*t
        if kind=='Idle':
            rotate('Spine',(1.4*math.sin(phase),0,0)); rotate('Head',(-1,0,2*math.sin(phase)))
        if kind in ['Walk','Run']:
            run=kind=='Run'; amp=40 if run else 25
            rotate('Spine',(-9 if run else -3,0,3*math.sin(phase)))
            for side,s in [('Left',1),('Right',-1)]:
                wave=math.sin(phase)*s
                rotate(side+'UpperLeg',(amp*wave,0,0))
                rotate(side+'LowerLeg',(-max(0,wave)*(55 if run else 32),0,0))
                rotate(side+'Foot',(max(0,wave)*18,0,0))
                rotate(side+'UpperArm',(-amp*.55*wave,78*s,0))
                rotate(side+'LowerArm',(-30 if run else -12,0,0))
            rig.pose.bones['Hips'].location.z=.018*math.cos(phase*2)
        if kind=='Attack':
            # Protagonist diagonal cut; Warden slower, committed overhead blow.
            if t<.42: k=t/.42
            elif t<.64: k=1-(t-.42)/.22*2
            else: k=-1+(t-.64)/.36
            rotate('Hips',(0,0,k*8))
            rotate('Chest',(-k*8,0,k*(12 if enemy else 18)))
            # Explicit shoulder poses place the blade THROUGH the forward combat space.
            # The Warden lifts high; the adventurer draws outside for a diagonal cut.
            neutral=Vector((-.26,0,-.96))
            windup=Vector((-.28,.18,.94) if enemy else (-.74,-.12,.66))
            strike=Vector((.10,-.98,-.16))
            follow=Vector((.52,-.65,-.55))
            if t<.40: direction=neutral.lerp(windup,t/.40)
            elif t<.55: direction=windup.lerp(strike,(t-.40)/.15)
            elif t<.70: direction=strike.lerp(follow,(t-.55)/.15)
            else: direction=follow.lerp(neutral,(t-.70)/.30)
            aim_right(direction)
            rotate('RightLowerArm',(0,0,-8))
            rotate('LeftUpperArm',(-25,62,-k*12)); rotate('LeftLowerArm',(-35,0,0))
            rotate('LeftUpperLeg',(-8*abs(k),0,0)); rotate('RightLowerLeg',(-12*abs(k),0,0))
            rotate('Head',(k*5,0,-k*12))
        if kind in ['Block','Parry']:
            rotate('LeftUpperArm',(-60,50,0)); rotate('LeftLowerArm',(-60,0,0))
            rotate('RightUpperArm',(-30,-55,0)); rotate('RightLowerArm',(-45,0,0)); rotate('Chest',(4,0,-12))
        if kind=='Dodge':
            k=math.sin(math.pi*t)
            rotate('Spine',(-25*k,0,0)); rotate('LeftUpperLeg',(-50*k,0,0)); rotate('RightUpperLeg',(-45*k,0,0))
            rotate('LeftLowerLeg',(-70*k,0,0)); rotate('RightLowerLeg',(-65*k,0,0)); rig.pose.bones['Hips'].location.z=-.20*k
        if kind=='Hit':
            k=math.sin(math.pi*t); rotate('Spine',(18*k,0,0)); rotate('Head',(12*k,0,0)); rotate('Chest',(0,0,-12*k))
        if kind=='Death':
            k=min(1,t*1.5); rotate('Hips',(-85*k,0,8*k)); rotate('LeftUpperLeg',(15*k,0,-10*k)); rotate('RightUpperLeg',(8*k,0,12*k))
            rig.pose.bones['Hips'].location.z=-.78*k; rotate('Head',(15*k,0,0))
        for pb in rig.pose.bones:
            pb.keyframe_insert(data_path='rotation_quaternion',frame=frame,group=pb.name)
            pb.keyframe_insert(data_path='location',frame=frame,group=pb.name)
    durations={'Idle':2.4,'Walk':1.0,'Run':.68,'Attack':1.25 if enemy else .85,'Block':1,'Parry':.5,'Dodge':.4,'Hit':.35,'Death':1.35}
    bpy.context.scene.render.fps=30
    for kind,duration in durations.items():
        action=bpy.data.actions.new(name+'_'+kind); action.use_fake_user=True; rig.animation_data.action=action
        end=round(duration*30)+1
        for f in sorted(set([1,end]+list(range(1,end+1,3)))): pose(f,kind,(f-1)/(end-1))
        # Endpoint frames remain keyed for independent action export.
    rig.animation_data.action=None
    for pb in rig.pose.bones: pb.rotation_quaternion=(1,0,0,0); pb.location=(0,0,0)
    bpy.context.scene.frame_set(1)
    bpy.ops.object.select_all(action='DESELECT'); rig.select_set(True); body.select_set(True); bpy.context.view_layer.objects.active=rig
    bpy.ops.wm.save_as_mainfile(filepath=os.path.join(os.path.dirname(__file__),name+'.blend'))
    bpy.ops.export_scene.fbx(filepath=os.path.join(OUT,name+'.fbx'),use_selection=True,object_types={'MESH','ARMATURE'},add_leaf_bones=False,bake_anim=True,bake_anim_use_all_actions=True,bake_anim_use_nla_strips=False,bake_anim_simplify_factor=0,axis_forward='-Z',axis_up='Y',apply_unit_scale=True,use_mesh_modifiers=True)
    print('EXPORTED',name,len(body.data.vertices),'vertices',len(arm.bones),'bones',len(durations),'actions')

build(False)
build(True)
