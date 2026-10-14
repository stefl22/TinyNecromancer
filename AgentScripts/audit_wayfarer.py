"""Read the saved Blender source without changing or saving it."""
import bpy, json, os
report={'source':bpy.data.filepath,'meshes':[],'rigs':[],'actions':[]}
for obj in bpy.data.objects:
    if obj.type=='MESH':
        me=obj.data; me.calc_loop_triangles()
        links=[set() for _ in me.vertices]
        for edge in me.edges:
            a,b=edge.vertices; links[a].add(b);links[b].add(a)
        seen=set();islands=[]
        for v in me.vertices:
            if v.index in seen:continue
            stack=[v.index];seen.add(v.index);count=0
            while stack:
                n=stack.pop();count+=1
                for j in links[n]:
                    if j not in seen:seen.add(j);stack.append(j)
            islands.append(count)
        report['meshes'].append({'name':obj.name,'vertices':len(me.vertices),'triangles':len(me.loop_triangles),
            'mesh_islands':len(islands),'island_sizes':sorted(islands,reverse=True),'uv_layers':len(me.uv_layers),
            'material_slots':len(me.materials),'multiple_weight_vertices':sum(sum(g.weight>.001 for g in v.groups)>1 for v in me.vertices),
            'unweighted_vertices':sum(not v.groups for v in me.vertices)})
    if obj.type=='ARMATURE':
        report['rigs'].append({'name':obj.name,'bones':[b.name for b in obj.data.bones],
            'constraints':sum(len(b.constraints) for b in obj.pose.bones)})
for action in bpy.data.actions:
    report['actions'].append({'name':action.name,'frames':list(action.frame_range)})
root=os.path.abspath(os.path.join(os.path.dirname(__file__),'..'))
os.makedirs(os.path.join(root,'ArtReview'),exist_ok=True)
with open(os.path.join(root,'ArtReview','ProtagonistSourceAudit.json'),'w') as f:json.dump(report,f,indent=2)
print(json.dumps(report,indent=2))
