"""Wayfarer v02 APPEARANCE STUDY. Original authored surfaces, no downloaded assets.
Separate from the playable rig. Skinning/animation follow appearance approval.
Blender coordinates: Z up, -Y forward. Measurements in metres.
"""
import bpy, math, os, random
import numpy as np
from mathutils import Vector

ROOT=os.path.abspath(os.path.join(os.path.dirname(__file__),'../..'))
OUT=os.path.join(ROOT,'Assets/_Game/Art/ProtagonistReview_v02')
os.makedirs(OUT+'/Textures',exist_ok=True)
random.seed(2002)
bpy.ops.object.select_all(action='SELECT');bpy.ops.object.delete(use_global=False)
for m in list(bpy.data.materials):bpy.data.materials.remove(m)
materials={}; objects=[]

def texture(kind):
    n=512; yy,xx=np.mgrid[0:n,0:n]; rng=np.random.default_rng(82)
    noise=rng.random((n,n))
    if kind=='Linen':
        height=.50+.13*np.sin(xx*math.tau/8)+.13*np.sin(yy*math.tau/8)+.035*(noise-.5)
        value=.91+.055*np.sin(xx*math.tau/8)*np.sin(yy*math.tau/8)+.018*(noise-.5);rough=.78;metal=0
    elif kind=='Leather':
        height=.5+.045*np.sin(xx*.34+np.sin(yy*.25)*2)+.06*(noise-.5)
        value=.9+.045*np.sin(xx*.03+yy*.015)+.035*(noise-.5);rough=.55;metal=0
    else:
        height=.5+.018*np.sin(xx*.8)+.018*(noise-.5)
        value=.93+.035*np.sin(xx*.02)+.025*(noise-.5);rough=.36;metal=.8
    dy,dx=np.gradient(height);normal=np.stack((-dx*.7,-dy*.7,np.ones_like(dx)),axis=-1)
    normal/=np.linalg.norm(normal,axis=-1)[...,None];normal=normal*.5+.5
    outputs={}
    for suffix in ['Base','Normal','MetalSmooth']:
        pixels=np.ones((n,n,4),dtype=np.float32)
        if suffix=='Base':pixels[:,:,:3]=value[:,:,None]
        elif suffix=='Normal':pixels[:,:,:3]=normal
        else:pixels[:,:,:3]=metal;pixels[:,:,3]=np.clip(1-rough+(noise-.5)*.07,0,1)
        im=bpy.data.images.new(kind+'_'+suffix,width=n,height=n,alpha=True)
        if suffix!='Base':im.colorspace_settings.name='Non-Color'
        im.pixels.foreach_set(pixels.ravel());im.filepath_raw=OUT+'/Textures/'+kind+'_'+suffix+'.png';im.file_format='PNG';im.save()
        outputs[suffix]=im
    return outputs
textures={k:texture(k) for k in ['Linen','Leather','Steel']}
def mat(name,color,kind=None,metal=0,rough=.6):
    m=bpy.data.materials.new(name);m.diffuse_color=(*color,1);m.use_nodes=True
    n=m.node_tree.nodes;l=m.node_tree.links;bs=n.get('Principled BSDF');bs.inputs['Base Color'].default_value=(*color,1)
    bs.inputs['Roughness'].default_value=rough;bs.inputs['Metallic'].default_value=metal
    if kind:
        tx=n.new('ShaderNodeTexImage');tx.image=textures[kind]['Base']
        mul=n.new('ShaderNodeMixRGB');mul.blend_type='MULTIPLY';mul.inputs[0].default_value=1;mul.inputs[2].default_value=(*color,1)
        l.new(tx.outputs['Color'],mul.inputs[1]);l.new(mul.outputs[0],bs.inputs['Base Color'])
        tx=n.new('ShaderNodeTexImage');tx.image=textures[kind]['Normal'];nm=n.new('ShaderNodeNormalMap');nm.inputs['Strength'].default_value=.35
        l.new(tx.outputs['Color'],nm.inputs['Color']);l.new(nm.outputs[0],bs.inputs['Normal'])
    materials[name]=m
    return m
mat('Skin',(.57,.345,.225),rough=.56)
mat('Lips',(.37,.16,.13),rough=.53)
mat('Crease',(.16,.085,.06),rough=.7)
mat('Coat',(.045,.18,.19),'Linen',rough=.78)
mat('CoatEdge',(.09,.255,.26),'Linen',rough=.75)
mat('Undershirt',(.47,.43,.32),'Linen',rough=.85)
mat('Trousers',(.14,.17,.16),'Linen',rough=.82)
mat('Leather',(.18,.075,.033),'Leather',rough=.57)
mat('LeatherEdge',(.30,.15,.068),'Leather',rough=.55)
mat('DarkLeather',(.055,.038,.026),'Leather',rough=.6)
mat('Steel',(.34,.40,.42),'Steel',.8,.34)
mat('Brass',(.47,.31,.11),'Steel',.7,.35)
mat('Hair',(.045,.022,.012),rough=.7)
mat('HairLight',(.09,.044,.021),rough=.65)
mat('EyeWhite',(.64,.62,.52),rough=.35)
mat('Iris',(.14,.075,.032),rough=.3)
mat('Pupil',(.009,.007,.005),rough=.25)
mat('Thread',(.42,.32,.19),rough=.8)

def mesh(name,vs,fs,material,uv=None,smooth=False):
    me=bpy.data.meshes.new(name);me.from_pydata(vs,[],fs);me.update()
    ob=bpy.data.objects.new(name,me);bpy.context.collection.objects.link(ob);objects.append(ob)
    ob.data.materials.append(materials[material]);layer=me.uv_layers.new(name='SurfaceUV')
    for poly in me.polygons:
        poly.use_smooth=smooth
        for li in poly.loop_indices:
            idx=me.loops[li].vertex_index;v=me.vertices[idx].co
            layer.data[li].uv=uv[idx] if uv else (v.x*3+v.y,v.z*3)
    return ob
def loft(name,rows,material,n=20,smooth=False):
    # Elliptical section rings with an optional deliberate fold modulation.
    vs=[];uv=[];fs=[]
    for k,row in enumerate(rows):
        x,y,z,rx,ry=row[:5];fold=row[5] if len(row)>5 else 0
        for j in range(n):
            a=math.tau*j/n;r=1+fold*math.sin(a*5+k*.85)
            vs.append((x+math.cos(a)*rx*r,y+math.sin(a)*ry*r,z));uv.append((j/n,z*2.4))
    for k in range(len(rows)-1):
        for j in range(n):a=k*n+j;b=k*n+(j+1)%n;fs.append((a,b,b+n,a+n))
    fs += [tuple(range(n-1,-1,-1)),tuple((len(rows)-1)*n+j for j in range(n))]
    return mesh(name,vs,fs,material,uv,smooth)
def path(name,points,radii,material,n=8,elliptic=1,smooth=False):
    vs=[];fs=[];uv=[]
    for i,p in enumerate(points):
        p=Vector(p);axis=(Vector(points[min(i+1,len(points)-1)])-Vector(points[max(0,i-1)])).normalized()
        u=axis.cross(Vector((0,1,0))).normalized()
        if u.length<.1:u=axis.cross(Vector((1,0,0))).normalized()
        v=axis.cross(u)
        for j in range(n):
            a=j*math.tau/n;vs.append(p+(u*math.cos(a)+v*math.sin(a)*elliptic)*radii[i]);uv.append((j/n,i/max(1,len(points)-1)))
    for i in range(len(points)-1):
        for j in range(n):a=i*n+j;b=i*n+(j+1)%n;fs.append((a,b,b+n,a+n))
    fs.extend([tuple(range(n-1,-1,-1)),tuple((len(points)-1)*n+j for j in range(n))])
    return mesh(name,vs,fs,material,uv,smooth)
def bead(name,p,scale,material,n=10,rings=6):
    rows=[]
    for i in range(rings+1):
        a=math.pi*i/rings;r=max(.025,math.sin(a));rows.append((p[0],p[1],p[2]-scale[2]*math.cos(a),scale[0]*r,scale[1]*r))
    return loft(name,rows,material,n,True)
def strip(name,points,width,material):
    # Solid strap with modeled thickness, not a floating flat line.
    vs=[]
    for i,p in enumerate(points):
        p=Vector(p);t=(Vector(points[min(i+1,len(points)-1)])-Vector(points[max(0,i-1)])).normalized()
        side=t.cross(Vector((0,-1,0))).normalized()*width*.5
        vs.extend([p-side,p+side,p-side+Vector((0,.008,0)),p+side+Vector((0,.008,0))])
    fs=[]
    for i in range(len(points)-1):
        a=i*4;b=a+4;fs.extend([(a,b,b+1,a+1),(a+2,a+3,b+3,b+2),(a,a+2,b+2,b),(a+1,b+1,b+3,a+3)])
    fs.extend([(0,1,3,2),(len(vs)-4,len(vs)-2,len(vs)-1,len(vs)-3)])
    return mesh(name,vs,fs,material)

# A clothed torso with chest, rib cage, waist and pelvis, rather than a cylinder.
loft('Fitted coat body',[(0,0,1.015,.192,.121,.012),(0,0,1.09,.177,.108,.01),(0,0,1.17,.163,.103,.006),
    (0,.003,1.25,.182,.119,.008),(0,.009,1.34,.214,.136,.01),(0,.014,1.415,.232,.128,.004),
    (0,.018,1.475,.23,.114),(0,.012,1.515,.172,.086),(0,0,1.545,.066,.060)],'Coat',24)
loft('Undershirt neck',[(0,-.002,1.50,.076,.07),(0,-.002,1.565,.062,.058)],'Undershirt',20)
# Collar halves and inset opening. Rounded edge cords follow the actual modeled cloth.
for s in [-1,1]:
    vs=[(s*.035,-.077,1.544),(s*.085,-.094,1.53),(s*.12,-.115,1.465),(s*.063,-.145,1.395),(s*.034,-.121,1.485)]
    ob=mesh('Folded coat lapel',vs,[(0,1,4),(1,2,3,4)],'CoatEdge')
    sol=ob.modifiers.new('Cloth thickness','SOLIDIFY');sol.thickness=.006
    path('Collar piping',[vs[0],vs[1],vs[2],vs[3]],[.003]*4,'LeatherEdge',6)
strip('Front placket',[(.012,-.119,1.07),(.012,-.11,1.20),(.012,-.14,1.36)],.022,'CoatEdge')
for z in [1.11,1.19,1.27,1.35]:bead('Coat button',(.013,-(.115 if z<1.25 else .14),z),(.008,.005,.008),'Brass',8,4)
# Tailored split coat skirts with real thickness and individually shaped hems.
for s in [-1,1]:
    rows=[];vs=[];fs=[]
    for k,z in enumerate([1.06,1.00,.92,.84]):
        for j in range(9):
            a=-math.pi*.48 + j*math.pi*.89/8
            r=.192+(1.06-z)*.13
            vs.append((s*(.018+r*math.cos(a)),.004+r*.66*math.sin(a)-.015,z+(j%2)*.003))
    for k in range(3):
        for j in range(8):a=k*9+j;fs.append((a,a+1,a+10,a+9))
    ob=mesh('Split coat skirt',vs,fs,'Coat');mod=ob.modifiers.new('Hem thickness','SOLIDIFY');mod.thickness=.007
    path('Skirt hem',vs[-9:],[.004]*9,'CoatEdge',6)
# Back yoke and center seam, split in the coat to allow movement.
path('Back seam',[(0,.126,1.42),(0,.143,1.34),(0,.119,1.22),(0,.12,1.08)],[.0025]*4,'CoatEdge',5)
path('Back yoke',[(-.18,.082,1.44),(-.10,.124,1.41),(0,.136,1.405),(.10,.124,1.41),(.18,.082,1.44)],[.003]*5,'CoatEdge',5)

# Continuous longitudinal rings through elbow/forearm; no ball elbow stuck between cylinders.
for side,s in [('L',1),('R',-1)]:
    pts=[(s*.178,.013,1.469),(s*.226,.006,1.445),(s*.273,-.008,1.36),(s*.300,-.015,1.30),
         (s*.321,-.024,1.232),(s*.326,-.027,1.202),(s*.333,-.041,1.14),(s*.337,-.056,1.08),(s*.339,-.065,1.035)]
    path(side+' fitted sleeve',pts,[.055,.087,.080,.072,.064,.063,.064,.053,.047],'Coat',16,.94)
    path(side+' rolled cuff',pts[-2:],[.061,.052],'CoatEdge',16,1.02)
    for off in [0,.013]:
        path(side+' cuff edge',[(s*.338,-.06,1.07+off),(s*.339,-.063,1.064+off)],[.059,.057],'CoatEdge',16)
    # Gloved wrist/palm and individually bent fingers with three phalangeal sections.
    cx=s*.34;cy=-.07
    path(side+' glove palm',[(cx,cy,1.055),(cx+s*.007,cy-.003,1.018),(cx+s*.01,cy-.008,.978),(cx+s*.01,cy-.007,.952)],
        [.042,.047,.049,.04],'DarkLeather',16,.50,True)
    for j,length in enumerate([.069,.083,.078,.060]):
        x=cx+s*(-.028+j*.021)
        points=[(x,cy-.009,.967),(x,cy-.016,.940),(x,cy-.028,.967-length*.72),(x,cy-.040,.967-length)]
        path(side+' finger '+str(j),points,[.012,.0122,.010,.0075],'Leather',8,1,True)
        bead(side+' knuckle',(x,cy-.027,.958),(.012,.006,.013),'LeatherEdge',8,4)
    path(side+' opposed thumb',[(cx-s*.036,cy-.003,1.007),(cx-s*.056,cy-.027,.988),(cx-s*.063,cy-.054,.972),(cx-s*.054,cy-.064,.959)],
        [.017,.016,.013,.009],'Leather',9,1,True)
    strip(side+' glove back seam',[(cx,cy-.034,1.03),(cx+s*.005,cy-.038,.988)],.004,'LeatherEdge')
    # Shaped trousers taper through knees and calves, with measured cloth folds.
    loft(side+' trousers',[(s*.099,.005,1.055,.108,.106,.012),(s*.108,.006,.94,.114,.113,.012),
        (s*.117,.006,.83,.104,.108,.015),(s*.121,-.003,.70,.087,.086,.020),
        (s*.124,-.024,.60,.079,.081,.024),(s*.125,-.025,.545,.075,.080,.02),
        (s*.125,-.008,.49,.073,.075,.026),(s*.129,.01,.415,.073,.078,.025),
        (s*.13,.014,.32,.063,.071)],'Trousers',20)
    path(side+' trouser seam',[(s*.214,.01,.91),(s*.213,0,.79),(s*.200,-.013,.64),(s*.199,-.022,.545),(s*.198,.005,.44)], [.002]*5,'DarkLeather',5)
    # Boots: welt, toe box, instep, ankle, shaft, tongue and actual laces.
    x=s*.13
    outline=[(-.064,.095),(.064,.095),(.082,.01),(.084,-.15),(.061,-.238),(.024,-.262),(-.037,-.255),(-.075,-.222),(-.084,-.12),(-.08,.018)]
    for label,z0,z1,scale,ma in [('Sole',.012,.035,1.05,'DarkLeather'),('Welt',.035,.05,1.045,'LeatherEdge')]:
        vs=[(x+px*scale,py,z) for z in [z0,z1] for px,py in outline];n=len(outline)
        fs=[tuple(range(n-1,-1,-1)),tuple(range(n,2*n))]+[(j,(j+1)%n,(j+1)%n+n,j+n) for j in range(n)]
        mesh(side+label,vs,fs,ma)
    loft(side+' shaped boot foot',[(x,-.068,.046,.085,.179),(x,-.068,.078,.085,.177),(x,-.044,.116,.080,.154),
        (x,-.002,.153,.062,.102),(x,.012,.197,.056,.072)],'Leather',20)
    loft(side+' boot shaft',[(x,.010,.15,.059,.076),(x,.014,.235,.064,.079),(x,.013,.33,.076,.090),
        (x,.011,.393,.083,.093),(x,.011,.421,.080,.09)],'Leather',20)
    loft(side+' boot cuff',[(x,.011,.388,.087,.097),(x,.011,.417,.088,.098),(x,.011,.429,.085,.095)],'DarkLeather',20)
    path(side+' toe seam',[(x-.069,-.159,.09),(x-.052,-.191,.108),(x,-.204,.114),(x+.05,-.19,.108),(x+.07,-.156,.09)],[.002]*5,'LeatherEdge',6)
    strip(side+' boot tongue',[(x,-.074,.365),(x,-.076,.29),(x,-.084,.20),(x,-.13,.123)],.056,'DarkLeather')
    for k,z in enumerate([.19,.226,.262,.298,.334]):
        y=-.079
        for sign in [-1,1]:bead(side+' eyelet',(x+sign*.028,y-.003,z),(.006,.004,.006),'Brass',8,4)
        path(side+' lace',[(x-.026,y-.006,z),(x+.026,y-.008,z+.024)],[.0022,.0022],'Thread',5)
        path(side+' lace',[(x+.026,y-.006,z),(x-.026,y-.008,z+.024)],[.0022,.0022],'Thread',5)
    # Stitch rows along boot rear and outer panel.
    for z in np.linspace(.18,.38,17):path(side+' stitch',[(x+s*.067,.045,float(z)),(x+s*.068,.045,float(z)+.005)],[.0008,.0008],'Thread',4)

# Belt with beveled thickness, holes, double frame buckle and a modeled satchel.
loft('Main belt',[(0,0,1.06,.188,.121),(0,0,1.112,.178,.119)],'Leather',36)
for z in [1.066,1.105]:
    pts=[(.19*math.cos(a),.124*math.sin(a),z) for a in np.linspace(0,math.tau,65)]
    path('Belt edge stitch',pts,[.0012]*len(pts),'Thread',4)
for xx in [-.04,.04]:path('Buckle vertical',[(xx,-.137,1.063),(xx,-.137,1.11)],[.0045]*2,'Brass',6)
for zz in [1.063,1.11]:path('Buckle horizontal',[(-.04,-.137,zz),(.04,-.137,zz)],[.0045]*2,'Brass',6)
path('Buckle tongue',[(0,-.142,1.065),(0,-.142,1.105)],[.002]*2,'Steel',6)
strip('Shoulder strap',[(-.175,-.035,1.075),(-.125,-.104,1.18),(-.04,-.153,1.32),(.064,-.141,1.447),(.157,-.095,1.52),(.168,.021,1.535),(.15,.118,1.45),(.02,.15,1.29),(-.13,.117,1.13)],.037,'Leather')
for x,y,z in [(-.10,-.124,1.225),(-.085,-.136,1.245),(-.07,-.144,1.265)]:bead('Strap stud',(x,y-.007,z),(.004,.004,.004),'Brass',8,4)
loft('Travel satchel',[(-.235,.020,.845,.075,.045),(-.239,.019,.86,.094,.05),(-.239,.020,.99,.096,.049),(-.233,.02,1.035,.073,.047)],'Leather',16)
ob=mesh('Satchel shaped flap',[(-.32,-.034,1.026),(-.153,-.034,1.026),(-.154,-.039,.956),(-.194,-.046,.929),(-.279,-.046,.933),(-.325,-.039,.963)],[(0,1,2,3,4,5)],'LeatherEdge');ob.modifiers.new('Flap thickness','SOLIDIFY').thickness=.006
strip('Satchel clasp',[(-.24,-.050,.997),(-.24,-.053,.919)],.019,'DarkLeather');bead('Satchel clasp rivet',(-.24,-.058,.935),(.008,.005,.008),'Brass',10,4)

# Restrained left shoulder guard: overlapping forged plates, rolled rims and rivets.
for layer in range(3):
    vs=[];fs=[];n=13
    for row in range(3):
        x=.205+layer*.034+row*.031
        for j in range(n):
            a=math.pi*.08+j*math.pi*.84/(n-1)
            vs.append((x,.007+math.cos(a)*(.108-layer*.008),1.468+math.sin(a)*(.085-layer*.018)-layer*.018))
    for row in range(2):
        for j in range(n-1):a=row*n+j;fs.append((a,a+1,a+n+1,a+n))
    ob=mesh('Forged shoulder lame '+str(layer),vs,fs,'Steel');ob.modifiers.new('Plate thickness','SOLIDIFY').thickness=.005
    path('Rolled steel rim',vs[-n:],[.004]*n,'Steel',6)
    for j in [2,n-3]:bead('Armor rivet',vs[n+j],(.0055,.0055,.0055),'Brass',8,4)

# Continuous neck and carefully sectioned craniofacial surface.
loft('Neck',[(0,.008,1.52,.054,.053),(0,.006,1.57,.051,.054),(0,0,1.626,.057,.057)],'Skin',24,True)
headrows=[(1.604,.030,.035,-.021),(1.617,.049,.048,-.018),(1.638,.063,.058,-.012),
 (1.657,.071,.066,-.005),(1.673,.080,.071,0),(1.692,.087,.077,.003),(1.71,.092,.083,.005),
 (1.731,.09,.083,.006),(1.746,.087,.081,.008),(1.760,.089,.085,.010),(1.781,.09,.087,.013),
 (1.811,.087,.086,.014),(1.837,.073,.076,.014),(1.858,.047,.053,.014),(1.869,.012,.022,.014)]
vs=[];fs=[];uv=[];n=40
for k,(z,rx,ry,cy) in enumerate(headrows):
    for j in range(n):
        a=math.tau*j/n;x=rx*math.cos(a);y=cy+ry*math.sin(a)
        front=max(0,-math.sin(a))**7
        # Integrated nose bridge, eye sockets, cheek planes, brow and muzzle.
        nose=math.exp(-(x/.020)**2)*(.041*math.exp(-((z-1.693)/.020)**2)+.020*math.exp(-((z-1.726)/.035)**2))
        socket=.012*math.exp(-((abs(x)-.041)/.023)**2)*math.exp(-((z-1.735)/.012)**2)
        cheek=.009*math.exp(-((abs(x)-.059)/.025)**2)*math.exp(-((z-1.708)/.016)**2)
        muzzle=.009*math.exp(-(x/.04)**2)*math.exp(-((z-1.657)/.020)**2)
        brow=.010*math.exp(-((abs(x)-.04)/.029)**2)*math.exp(-((z-1.755)/.012)**2)
        y+=front*(socket-nose-cheek-muzzle-brow)
        vs.append((x,y,z));uv.append((j/n,(z-1.604)/.265))
for k in range(len(headrows)-1):
    for j in range(n):a=k*n+j;b=k*n+(j+1)%n;fs.append((a,b,b+n,a+n))
fs.extend([tuple(range(n-1,-1,-1)),tuple((len(headrows)-1)*n+j for j in range(n))])
mesh('Sculpted face and cranium',vs,fs,'Skin',uv,True)
for s in [-1,1]:
    # Almond eye, lid rim and brow wrap onto the socket plane; iris is not a googly sphere.
    center=Vector((s*.039,-.0755,1.736))
    outline=[(-.022,0),(-.014,.0065),(0,.008),(.014,.0055),(.022,0),(.014,-.0045),(0,-.0055),(-.014,-.004)]
    verts=[(center.x+u,center.y+abs(u)*.13,center.z+v) for u,v in outline]+[(center.x,center.y-.002,center.z)]
    mesh('Almond eye',verts,[(j,(j+1)%8,8) for j in range(8)],'EyeWhite',smooth=True)
    for idxs in [[0,1,2,3,4],[4,5,6,7,0]]:
        pts=[Vector(verts[j])+Vector((0,-.001,0)) for j in idxs];path('Eyelid',pts,[.0025]*len(pts),'Skin',6,1,True)
    bead('Brown iris',(center.x,center.y-.004,center.z),(.0055,.0014,.0055),'Iris',12,6)
    bead('Pupil',(center.x,center.y-.0054,center.z),(.0026,.0008,.0032),'Pupil',10,5)
    browpts=[(s*.018,-.083,1.757),(s*.033,-.083,1.761),(s*.048,-.078,1.761),(s*.064,-.067,1.756)]
    path('Shaped eyebrow',browpts,[.002,.0045,.004,.0015],'Hair',6,.6)
    bead('Nostril',(s*.011,-.108,1.681),(.005,.002,.0024),'Crease',8,4)
    # Ear helix and concha, not a spherical ear.
    ear=[(s*.086,.004,1.711),(s*.103,.001,1.735),(s*.112,.015,1.73),(s*.112,.023,1.700),(s*.102,.011,1.681),(s*.09,.003,1.689)]
    path('Ear helix',ear+[ear[0]],[.004,.005,.005,.005,.006,.004,.004],'Skin',7,1,True)
    concha=mesh('Ear concha',ear,[(0,1,2),(0,2,3),(0,3,4),(0,4,5)],'Skin',smooth=True)
    concha.modifiers.new('Ear surface thickness','SOLIDIFY').thickness=.003
    path('Ear inner fold',[(s*.094,.002,1.718),(s*.103,.009,1.712),(s*.098,.002,1.696)],[.003]*3,'Skin',6)
# Philtrum and lips with cupid's bow, lower lip volume and subtle mouth separation.
path('Upper lip',[(-.025,-.079,1.659),(-.011,-.085,1.663),(0,-.088,1.661),(.011,-.085,1.663),(.025,-.079,1.659)], [.0015,.003,.0027,.003,.0015],'Lips',8,.65,True)
path('Mouth seam',[(-.024,-.080,1.657),(0,-.089,1.657),(.024,-.080,1.657)],[.0008,.001,.0008],'Crease',5)
path('Lower lip',[(-.021,-.081,1.655),(0,-.089,1.652),(.021,-.081,1.655)],[.001,.0033,.001],'Lips',8,.75,True)

# A complete short haircut with a designed hairline and overlapping swept locks.
vs=[];fs=[];n=40
for row in range(15):
    for j in range(n):
        a=j*math.tau/n;front=max(0,-math.sin(a));side=abs(math.cos(a))
        bottom=1.691+front*.107+side*.045
        t=row/14;z=bottom+(1.886-bottom)*t
        # Follow the actual cranium sections; a cosine dome clips the forehead.
        rx=float(np.interp(z,[r[0] for r in headrows],[r[1] for r in headrows]))+.010
        ry=float(np.interp(z,[r[0] for r in headrows],[r[2] for r in headrows]))+.010
        if row==14:rx=.002;ry=.002
        ridge=1+.025*math.cos(a*11+t*4)*math.sin(math.pi*t)
        vs.append((math.cos(a)*rx*ridge,.015+math.sin(a)*ry*ridge,z))
for k in range(14):
    for j in range(n):a=k*n+j;b=k*n+(j+1)%n;fs.append((a,b,b+n,a+n))
mesh('Hair mass with shaped hairline',vs,fs,'Hair')
def lock(name,pts,width,ma):
    vs=[]
    for i,p in enumerate(pts):
        p=Vector(p);t=(Vector(pts[min(i+1,len(pts)-1)])-Vector(pts[max(0,i-1)])).normalized()
        side=t.cross(Vector((0,0,1))).normalized();w=width*(1 if i<len(pts)-1 else .06)
        vs.extend([p-side*w,p+Vector((0,0,.010))*math.sin(math.pi*(i+.4)/len(pts)),p+side*w])
    fs=[]
    for i in range(len(pts)-1):
        a=i*3;fs.extend([(a,a+3,a+4,a+1),(a+1,a+4,a+5,a+2)])
    ob=mesh(name,vs,fs,ma)
    ob.modifiers.new('Lock thickness','SOLIDIFY').thickness=.0015
    return ob
def scalp_height(x,y):
    for z in np.linspace(1.880,1.74,180):
        rx=float(np.interp(z,[r[0] for r in headrows],[r[1] for r in headrows]))+.007
        ry=float(np.interp(z,[r[0] for r in headrows],[r[2] for r in headrows]))+.007
        if (x/rx)**2+((y-.015)/ry)**2<1:return float(z)+.004
    return 1.78
# Swept ridges belong to the continuous cap; floating strips are unsuitable at this scale.

# Objects stay separately named for review/editing, not fused into a primitive pile.
# Apply modeled cloth thickness for a portable FBX. Preserve the editable .blend modifiers.
bpy.ops.object.select_all(action='DESELECT')
for ob in objects:
    ob.select_set(True)
    # All closed shells need outward normals, including descending leg rings.
    import bmesh
    bm=bmesh.new();bm.from_mesh(ob.data);bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces));bm.to_mesh(ob.data);bm.free()
bpy.context.view_layer.objects.active=objects[0]
bpy.context.scene.unit_settings.system='METRIC'
bpy.context.scene['review_status']='APPEARANCE REVIEW ONLY — not approved for replacement or animation'
bpy.ops.wm.save_as_mainfile(filepath=os.path.join(os.path.dirname(__file__),'Wayfarer_Review_v02.blend'))
bpy.ops.export_scene.fbx(filepath=OUT+'/Wayfarer_Review_v02.fbx',use_selection=True,object_types={'MESH'},
    add_leaf_bones=False,bake_anim=False,axis_forward='-Z',axis_up='Y',apply_unit_scale=True,use_mesh_modifiers=True)
print('REVIEW_EXPORTED',len(objects),'objects',sum(len(o.data.vertices) for o in objects),'vertices')
