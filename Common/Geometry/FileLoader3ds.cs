using System.IO;
using Header = System.Tuple<ushort, uint>;
using System.Collections.Generic;
using System;

namespace Common.Geometry
{
    public static class FileLoader3ds
    {
        private static FileData fileData = null;

        private class TexMapData
        {
            internal Common.Geometry.FileMaterialMap fileMaterialMap;
        }

        private class NodeHeader
        {
            internal string objname;
            internal ushort flags1, flags2, hierarchy;
        }

        private class TrackTagElement
        {
            internal ushort framenum;
            internal Vec3 vec;
            internal float possibleRotation;
        }

        private class TrackTag
        {
            internal ushort flags, keys;
            internal List<TrackTagElement> elements = new List<TrackTagElement>();
        }

        private class ObjectNode
        {
            internal ushort id;
            internal NodeHeader header;
            internal Vec3 pivot;
            internal TrackTag pos, rot, scl;
            internal float morph_smooth_angle = 0;
            internal string instance_name;
            internal Object geometryObject;

            internal Node CreateObject()
            {
                if (geometryObject != null)
                    return geometryObject;
                NamedTriangleObject nto = fileData.GetObject(header.objname);
                Object obj = new Object();
                if (nto == null)
                {
                    if (instance_name != null)
                        obj.Name = instance_name;
                    geometryObject = obj;
                    return obj;
                }
                if (instance_name == null)
                {
                    nto.originalPosition = pos.elements[0].vec;
                    nto.originalRotationAxis = rot.elements[0].vec;
                    nto.originalRotation = rot.elements[0].possibleRotation;
                }
                else 
                {
                    //if (nto.originalRotationAxis.Equals(Vec3.UnitY))
                    //    obj.Rotation = new Vec3(0, -nto.originalRotation, 0);
                }
                nto.AddToObject(obj);
                obj.Position = -pivot;
                CustomRotationObject cro = new CustomRotationObject(obj, rot.elements[0].possibleRotation, rot.elements[0].vec);
                //Object temp = new Object();
                //temp.Position = -pivot;
                //temp.AddNode(cro);
                obj = new Object();
                if (instance_name != null && instance_name != "")
                    obj.Name = instance_name;
                else
                    obj.Name = header.objname + "-node";
                foreach (Mesh m in cro.ImproveMeshes())
                    obj.AddNode(m);
                //obj.AddNode(cro);
                obj.Position = pos.elements[0].vec;// *cro.GetTotalTransformation();
                //cro.Position = pos.elements[0].vec * cro.GetTransformation();
                geometryObject = obj;
                return obj;
            }
        }

        private class Group
        {
            internal ushort revision;
            internal string filename;
            internal uint animlen;
            internal uint start, end, currframe;
            internal ObjectNode currentObject = null;
            internal Object obj = null;
        }

        private class MaterialBlockData
        {
            internal Material material = new Material();
            internal List<TexMapData> texmaps = new List<TexMapData>();
            internal TexMapData LastTexMap { get { return texmaps[texmaps.Count - 1]; } }
        }

        private class MeshMaterialGroup
        {
            internal string material_name;
            internal List<ushort> faces;
        }

        private class NamedTriangleObject
        {
            internal float[] mesh_matrix;
            internal List<Vec3> point_array = null;
            internal List<Vec3> transformed_point_array;
            internal List<Vec2> tex_array = null;
            internal List<ushort> point_flag_array;
            internal List<Tuple<ushort, ushort, ushort, ushort>> face_array;
            internal List<MeshMaterialGroup> meshMaterialGroups = new List<MeshMaterialGroup>();
            internal string name;
            private List<int> normalSet = new List<int>();
            internal Vec3 originalPosition = new Vec3(), originalRotationAxis = new Vec3();
            internal float originalRotation = 0;

            internal void AddToObject(Object o)
            {
                Matrix4 rot = Matrix4.ArbitraryRotation(originalRotation, originalRotationAxis);
                Matrix4 originalTransformation = rot.Inverse();
                if (point_array == null)
                    return;
                transformed_point_array = new List<Vec3>(point_array.Count);
                foreach (Vec3 v in point_array)
                    transformed_point_array.Add((v - originalPosition) * originalTransformation);
                List<ushort> usedFaces = new List<ushort>();
                foreach (MeshMaterialGroup mmg in meshMaterialGroups)
                {
                    Mesh m = CreateMesh(fileData.materials[mmg.material_name].material);
                    foreach (ushort face in mmg.faces)
                    {
                        AddFace(m, face);
                        usedFaces.Add(face);
                    }
                    o.AddNode(m);
                }

                Mesh defMesh = null;
                for(ushort i = 0; i < face_array.Count; ++i)
                {
                    if (!usedFaces.Contains(i))
                    {
                        if (defMesh == null)
                        {
                            defMesh = CreateMesh(Material.Default());
                            o.AddNode(defMesh);
                        }
                        AddFace(defMesh, i);
                    }
                }
            }

            private Mesh CreateMesh(Material mat)
            {
                Mesh m = new Mesh(transformed_point_array.Count, mat);
                m.m_name = name;
                normalSet.Clear();
                for (int i = 0; i < point_array.Count; ++i)
                    normalSet.Add(0);
                m.m_type = MeshType.TRIANGLE_LIST;
                AddVertices(m);
                return m;
            }

            private void AddFace(Mesh m, ushort faceIndex)
            {
                Tuple<ushort, ushort, ushort, ushort> face = face_array[faceIndex];
                VertexInfoSimple v1 = m.smesh.vertexBuffer.array[face.Item1];
                VertexInfoSimple v2 = m.smesh.vertexBuffer.array[face.Item2];
                VertexInfoSimple v3 = m.smesh.vertexBuffer.array[face.Item3];

                Vec3 normal = (v3.m_position.Vec3() - v1.m_position.Vec3()).normalize().Cross((v2.m_position.Vec3() - v1.m_position.Vec3()).normalize()).normalize();
                AddNormal(face.Item1, m, face.Item1, normal);
                AddNormal(face.Item2, m, face.Item2, normal);
                AddNormal(face.Item3, m, face.Item3, normal);
                m.m_indexBuffer.Add(face.Item1);
                m.m_indexBuffer.Add(face.Item3);
                m.m_indexBuffer.Add(face.Item2);
            }

            private void AddNormal(ushort p, Mesh m, ushort index, Vec3 normal)
            {
                int set = normalSet[p];
                if (set == 0)
                {
                    normalSet[p] = 1;
                    m.smesh.vertexBuffer.array[index].m_normal.Set(normal);
                }
                else
                {
                    Vec3 currentNormal = (float)set * m.smesh.vertexBuffer.array[index].m_normal.Vec3();
                    currentNormal += normal;
                    ++set;
                    currentNormal /= (float)set;
                    currentNormal = currentNormal.normalize();
                    normalSet[p] = set;
                    m.smesh.vertexBuffer.array[index].m_normal.Set(currentNormal);
                }
            }

            private void AddVertices(Mesh m)
            {
                int count = 0;
                foreach (Vec3 v in transformed_point_array)
                {
                    Vec2 tex = (tex_array == null) ? new Vec2(0, 0) : tex_array[count];
                    m.smesh.vertexBuffer.Add(new VertexInfoSimple(v, tex, new Vec3(0, 0, 0), new Vec4(1, 1, 1)));
                    ++count;
                }
            }
        }

        private class FileData
        {
            internal Object mainObject;
            internal short m3dVersion;
            internal short meshVersion;
            internal Stack<MaterialBlockData> currentMaterialStack = new Stack<MaterialBlockData>();
            internal MaterialBlockData CurrentMaterial { get { return currentMaterialStack.Peek(); } }
            internal Dictionary<string, MaterialBlockData> materials = new Dictionary<string, MaterialBlockData>();
            internal float masterScale;
            internal string currentNamedObjectName;
            internal NamedTriangleObject currentTriangleObject = null;
            internal Group currentGroup = null;
            internal Dictionary<string, NamedTriangleObject> namedTriangleObjects = new Dictionary<string, NamedTriangleObject>();
            internal Dictionary<ushort, ObjectNode> objectNodes = new Dictionary<ushort, ObjectNode>();

            internal FileData() { }

            internal NamedTriangleObject GetObject(string p)
            {
                if (namedTriangleObjects.ContainsKey(p))
                    return namedTriangleObjects[p];
                return null;
            }

            internal ObjectNode GetObjectNodeWithId(ushort p)
            {
                ObjectNode obj;
                if (objectNodes.TryGetValue(p, out obj))
                    return obj;
                return null;
            }
        }

        static internal string pathDirectory = "", filename = "";

        public static Object LoadModel(string path)
        {
            pathDirectory = (new FileInfo(path)).DirectoryName + "\\";
            filename = (new FileInfo(path)).Name;
            BinaryReader r = new BinaryReader(File.Open(path, FileMode.Open));
            Object ret = LoadModel(r);
            r.Close();
            pathDirectory = "";
            filename = "";
            return ret;
        }

        private static Object LoadModel(BinaryReader r)
        {
            fileData = new FileData();
            fileData.mainObject = new Object();
            fileData.mainObject.Name = filename;

            ReadMainChunk(r);

            Object o = fileData.mainObject;
            fileData = null;
            return o;
        }

        private static void ReadMainChunk(BinaryReader r)
        {
            Header h = ReadHeader(r);
            if (h.Item1 != 0x4D4D)
                throw new System.Exception("Main chunk expected!");
            long start = r.BaseStream.Position;
            while (r.BaseStream.Position < start + h.Item2 - 6)
                ReadMainChunkSub(r);
        }

        private static void ReadMainChunkSub(BinaryReader r)
        {
            Header h = ReadHeader(r);
            switch (h.Item1)
            {
                case 0x3D3D:
                    Read3dEditorChunk(r, h.Item2 - 6);
                    break;
                case 0x0002:
                    fileData.m3dVersion = (short)r.ReadInt32();
                    break;
                case 0xB000:
                    ReadGroup(r, h.Item2 - 6);
                    break;
                default:
                    throw new System.NotImplementedException();
            }
        }

        private static void ReadGroup(BinaryReader r, uint length)
        {
            fileData.currentGroup = new Group();
            fileData.currentGroup.obj = new Object();

            long start = r.BaseStream.Position;
            while (r.BaseStream.Position < start + length)
                ReadGroupSub(r);

            fileData.mainObject.AddNode(fileData.currentGroup.obj);
            fileData.currentGroup = null;
        }

        private static void ReadGroupSub(BinaryReader r)
        {
            Header h = ReadHeader(r);
            switch (h.Item1)
            {
                case 0xB00A:
                    ReadGroupHeader(r, h.Item2 - 6);
                    break;
                case 0xB008:
                    fileData.currentGroup.start = ReadInt(r);
                    fileData.currentGroup.end = ReadInt(r);
                    break;
                case 0xB009:
                    fileData.currentGroup.currframe = ReadInt(r);
                    break;
                case 0xB002:
                    ReadObjectNode(r, h.Item2 - 6);
                    break;
                default:
                    throw new System.NotImplementedException();
            }
        }

        private static void ReadObjectNode(BinaryReader r, uint length)
        {
            fileData.currentGroup.currentObject = new ObjectNode();
            long start = r.BaseStream.Position;
            while (r.BaseStream.Position < start + length)
                ReadObjectNodeSub(r);

            ObjectNode parent = fileData.GetObjectNodeWithId(fileData.currentGroup.currentObject.header.hierarchy);
            if (parent == null)
                fileData.currentGroup.obj.AddNode(fileData.currentGroup.currentObject.CreateObject());
            else
                parent.geometryObject.AddNode(fileData.currentGroup.currentObject.CreateObject());

            fileData.objectNodes.Add(fileData.currentGroup.currentObject.id, fileData.currentGroup.currentObject);
            fileData.currentGroup.currentObject = null;
        }

        private static void ReadObjectNodeSub(BinaryReader r)
        {
            Header h = ReadHeader(r);
            switch (h.Item1)
            {
                case 0xB010:
                    fileData.currentGroup.currentObject.header = ReadNodeHeader(r);
                    break;
                case 0xb013:
                    fileData.currentGroup.currentObject.pivot = ReadVec3(r, true);
                    break;
                case 0xb020:
                    fileData.currentGroup.currentObject.pos = ReadTrackTag(r, false, true);
                    break;
                case 0xb021:
                    fileData.currentGroup.currentObject.rot = ReadTrackTag(r, true, true);
                    break;
                case 0xb022:
                    fileData.currentGroup.currentObject.scl = ReadTrackTag(r, false, true);
                    break;
                case 0xB030:
                    fileData.currentGroup.currentObject.id = ReadShort(r);
                    break;
                case 0xB011:
                    fileData.currentGroup.currentObject.instance_name = ReadString(r);
                    break;
                case 0xB014:
                    //--- BOUNDBOX ??? !!!
                    Vec3 min = ReadVec3(r, true);
                    Vec3 max = ReadVec3(r, true);
                    System.Console.Out.WriteLine("Bounding box:");
                    //foreach (byte b in r.ReadBytes((int)h.Item2 - 6))
                    //    System.Console.Out.WriteLine(" - " + (int)b);
                    System.Console.Out.WriteLine(min);
                    System.Console.Out.WriteLine(max);
                    break;
                default:
                    throw new System.NotImplementedException();
            }
        }

        private static TrackTag ReadTrackTag(BinaryReader r, bool rot, bool switchYZ)
        {
            TrackTag t = new TrackTag();

            t.flags = ReadShort(r);
            r.ReadBytes(8);
            t.keys = ReadShort(r);
            ReadShort(r);
            for (ushort i = 0; i < t.keys; ++i)
            {
                TrackTagElement te = new TrackTagElement();
                te.framenum = ReadShort(r);
                r.ReadBytes(4);
                if (rot)
                    te.possibleRotation = r.ReadSingle();
                te.vec = ReadVec3(r, switchYZ);
                t.elements.Add(te);
            }

            return t;
        }

        private static Vec3 ReadVec3(BinaryReader r, bool switchYZ)
        {
            float x = r.ReadSingle();
            float y = r.ReadSingle();
            float z = r.ReadSingle();
            if (switchYZ)
                return new Vec3(x, z, y);
            return new Vec3(x, y, z);
        }

        private static NodeHeader ReadNodeHeader(BinaryReader r)
        {
            NodeHeader h = new NodeHeader();
            h.objname = ReadString(r);
            h.flags1 = ReadShort(r);
            h.flags2 = ReadShort(r);
            h.hierarchy = ReadShort(r);
            return h;
        }

        private static void ReadGroupHeader(BinaryReader r, uint length)
        {
            long start = r.BaseStream.Position;

            fileData.currentGroup.revision = ReadShort(r);
            fileData.currentGroup.filename = ReadString(r);
            fileData.currentGroup.obj.Name = fileData.currentGroup.filename;
            fileData.currentGroup.animlen = ReadInt(r);

            while (r.BaseStream.Position < start + length)
                ReadGroupHeaderSub(r);
        }

        private static uint ReadInt(BinaryReader r)
        {
            return (uint)r.ReadInt32();
        }

        private static void ReadGroupHeaderSub(BinaryReader r)
        {
            Header h = ReadHeader(r);
            switch (h.Item1)
            {
                default:
                    throw new System.NotImplementedException();
            }
        }

        private static void Read3dEditorChunk(BinaryReader r, uint length)
        {
            long start = r.BaseStream.Position;
            while (r.BaseStream.Position < start + length)
                Read3dEditorChunkSub(r);
        }

        private static ushort ReadShort(BinaryReader r)
        {
            return (ushort)r.ReadInt16();
        }

        private static void Read3dEditorChunkSub(BinaryReader r)
        {
            Header h = ReadHeader(r);
            switch (h.Item1)
            {
                case 0x3d3e:
                    fileData.meshVersion = (short)r.ReadInt32();
                    break;
                case 0xAFFF:
                    ReadMaterialBlock(r, h.Item2 - 6);
                    break;
                case 0x0100:
                    fileData.masterScale = r.ReadSingle();
                    break;
                case 0x4000:
                    fileData.currentNamedObjectName = ReadString(r);
                    break;
                case 0x4100:
                    ReadNamedTriangleObject(r, h.Item2 - 6);
                    break;
                case 0x1400:
                    //--- LO_SHADOW_BIAS ???
                    r.ReadSingle();
                    break;
                case 0x1420:
                    //--- SHADOW_MAP_SIZE
                    r.ReadInt16();
                    break;
                case 0x1450:
                    //--- SHADOW_FILTER
                    r.ReadSingle();
                    break;
                case 0x1460:
                    //--- RAY_BIAS
                    r.ReadSingle();
                    break;
                case 0x1500:
                    //--- O_CONSTS
                    r.ReadSingle();
                    r.ReadSingle();
                    r.ReadSingle();
                    break;
                default:
                    r.ReadBytes((int)h.Item2 - 6);
                    break;
                    //throw new System.NotImplementedException();
            }
        }

        private static void ReadNamedTriangleObject(BinaryReader r, uint length)
        {
            fileData.currentTriangleObject = new NamedTriangleObject();
            fileData.currentTriangleObject.name = fileData.currentNamedObjectName;
            long start = r.BaseStream.Position;
            while (r.BaseStream.Position < start + length)
                ReadNamedTriangleObjectSub(r);

            fileData.namedTriangleObjects.Add(fileData.currentTriangleObject.name, fileData.currentTriangleObject);
            fileData.currentTriangleObject = null;
        }

        private static void ReadNamedTriangleObjectSub(BinaryReader r)
        {
            Header h = ReadHeader(r);
            switch (h.Item1)
            {
                case 0x4160:
                    fileData.currentTriangleObject.mesh_matrix = new float[12];
                    for (int i = 0; i < 12; ++i)
                        fileData.currentTriangleObject.mesh_matrix[i] = r.ReadSingle();
                    break;
                case 0x4110:
                    fileData.currentTriangleObject.point_array = ReadPointArray(r);
                    break;
                case 0x4111:
                    fileData.currentTriangleObject.point_flag_array = ReadPointFlagArray(r);
                    break;
                case 0x4120:
                    fileData.currentTriangleObject.face_array = ReadFaceArray(r);
                    break;
                case 0x4130:
                    fileData.currentTriangleObject.meshMaterialGroups.Add(ReadMeshMaterialGroup(r));
                    break;
                case 0x4140:
                    fileData.currentTriangleObject.tex_array = ReadTexArray(r);
                    break;
                case 0x4150:
                    //--- ToDo: smooth group ???
                default:
                    r.ReadBytes((int)(h.Item2 - 6));
                    //throw new System.NotImplementedException();
                    break;
            }
        }

        private static List<Vec2> ReadTexArray(BinaryReader r)
        {
            ushort numPoints = (ushort)r.ReadInt16();
            List<Vec2> list = new List<Vec2>(numPoints);
            for (ushort i = 0; i < numPoints; ++i)
            {
                float x = r.ReadSingle();
                float y = r.ReadSingle();
                list.Add(new Vec2(x, y));
            }
            return list;
        }

        private static MeshMaterialGroup ReadMeshMaterialGroup(BinaryReader r)
        {
            MeshMaterialGroup mmg = new MeshMaterialGroup();
            mmg.material_name = ReadString(r);
            ushort numFaces = (ushort)r.ReadInt16();
            mmg.faces = new List<ushort>(numFaces);
            for (ushort i = 0; i < numFaces; ++i)
                mmg.faces.Add((ushort)r.ReadInt16());
            return mmg;
        }

        private static List<Tuple<ushort, ushort, ushort, ushort>> ReadFaceArray(BinaryReader r)
        {
            ushort numFaces = (ushort)r.ReadInt16();
            List<Tuple<ushort, ushort, ushort, ushort>> list = new List<Tuple<ushort, ushort, ushort, ushort>>(numFaces);
            for (ushort i = 0; i < numFaces; ++i)
            {
                ushort vertex1 = (ushort)r.ReadInt16();
                ushort vertex2 = (ushort)r.ReadInt16();
                ushort vertex3 = (ushort)r.ReadInt16();
                ushort flags = (ushort)r.ReadInt16();

                list.Add(new Tuple<ushort, ushort, ushort, ushort>(vertex1, vertex2, vertex3, flags));
            }
            return list;
        }

        private static List<ushort> ReadPointFlagArray(BinaryReader r)
        {
            ushort numPoints = (ushort)r.ReadInt16();
            List<ushort> list = new List<ushort>(numPoints);
            for (ushort i = 0; i < numPoints; ++i)
                list.Add((ushort)r.ReadInt16());
            return list;
        }

        private static List<Vec3> ReadPointArray(BinaryReader r)
        {
            ushort numPoints = (ushort)r.ReadInt16();
            List<Vec3> list = new List<Vec3>(numPoints);
            for (ushort i = 0; i < numPoints; ++i)
                list.Add(ReadVec3(r, true));
            return list;
        }

        private static void ReadMaterialBlock(BinaryReader r, uint length)
        {
            fileData.currentMaterialStack.Push(new MaterialBlockData());
            long start = r.BaseStream.Position;
            while (r.BaseStream.Position < start + length)
                ReadMaterialBlockSub(r);
            fileData.materials.Add(fileData.CurrentMaterial.material.Name, fileData.CurrentMaterial);
            fileData.currentMaterialStack.Pop();
        }

        private static void ReadMaterialBlockSub(BinaryReader r)
        {
            Header h = ReadHeader(r);
            switch (h.Item1)
            {
                case 0xA000:
                    fileData.CurrentMaterial.material.Name = ReadString(r);
                    break;
                case 0xA010:
                    fileData.CurrentMaterial.material.m_ambientColor = ReadColor(r, h.Item2 - 6);
                    break;
                case 0xA020:
                    fileData.CurrentMaterial.material.m_diffuseColor = ReadColor(r, h.Item2 - 6);
                    break;
                case 0xA030:
                    fileData.CurrentMaterial.material.m_specularColor = ReadColor(r, h.Item2 - 6);
                    break;
                case 0xA040:
                    fileData.CurrentMaterial.material.m_shininess = ReadPercentage(r);
                    break;
                case 0xA050:
                    fileData.CurrentMaterial.material.Transparency = ReadPercentage(r);
                    break;
                case 0xA100:
                    //--- Shading value ???
                    r.ReadInt16();
                    break;
                case 0xA041:
                    //--- MAT_SHIN2PCT ???
                    ReadPercentage(r);
                    break;
                case 0xA052:
                    //--- MAT_XPFAL ???
                    ReadPercentage(r);
                    break;
                case 0xA053:
                    //--- MAT_REFBLUR ???
                    ReadPercentage(r);
                    break;
                case 0xA084:
                    //--- MAT_SELF_ILPCT ???
                    ReadPercentage(r);
                    break;
                case 0xA081:
                    fileData.CurrentMaterial.material.TwoSided = true;
                    break;
                case 0xA08A:
                    //--- MAT_XPFALLIN ???
                    break;
                case 0xA087:
                    //--- MAT_WIRESIZE ???
                    r.ReadSingle();
                    break;
                case 0xA200:
                    ReadPercentage(r); // ???
                    ReadTexMap(r, h.Item2 - 6, MaterialMap.MapType.Diffuse);
                    break;
                case 0xA220:
                    //--- ToDo: Reflection map
                    ReadPercentage(r); // ???
                    ReadTexMap(r, h.Item2 - 6, MaterialMap.MapType.Reflection);
                    break;
                case 0xA230:
                    //--- ToDo: BUMP MAP
                    long temp = r.BaseStream.Position;
                    float percentage = ReadPercentage(r);
                    ReadTexMap(r, (uint)(h.Item2 - 6 - (r.BaseStream.Position - temp)), MaterialMap.MapType.Bump);
                    break;
                case 0xA252:
                    //--- ToDo: MAT_BUMP_PERCENT
                    ReadShort(r);
                    break;
                case 0xA08C:
                    //--- ToDo: MAT_PHONGSOFT
                    break;
                default:
                    throw new System.NotImplementedException();
            }
        }

        private static void ReadTexMap(BinaryReader r, uint length, MaterialMap.MapType mapType)
        {
            long start = r.BaseStream.Position;
            fileData.CurrentMaterial.texmaps.Add(new TexMapData());
            fileData.CurrentMaterial.LastTexMap.fileMaterialMap = new FileMaterialMap("", mapType);
            fileData.CurrentMaterial.material.AddMaterialMap(fileData.CurrentMaterial.LastTexMap.fileMaterialMap);
            while (r.BaseStream.Position < start + length)
                ReadTexMapSub(r);
        }

        private static void ReadTexMapSub(BinaryReader r)
        {
            Header h = ReadHeader(r);
            switch (h.Item1)
            {
                case 0xA300:
                    fileData.CurrentMaterial.LastTexMap.fileMaterialMap.MapFilename = pathDirectory + ReadString(r);
                    break;
                case 0xA351:
                    //--- MAT_MAP_TILING ???
                    ushort flags = (ushort)r.ReadInt16();
                    break;
                case 0xA353:
                    //--- MAT_MAP_TEXBLUR ???
                    float blur = r.ReadSingle();
                    break;
                case 0xAFFF:
                    ReadMaterialBlock(r, h.Item2 - 6);
                    break;
                case 0x0100:
                    fileData.masterScale = r.ReadSingle();
                    break;
                case 0xA230:
                    //--- ToDo: BUMP MAP
                    long temp = r.BaseStream.Position;
                    float percentage = ReadPercentage(r);
                    ReadTexMap(r, (uint)(h.Item2 - 6 - (r.BaseStream.Position - temp)), MaterialMap.MapType.Bump);
                    break;
                case 0xA252:
                    //--- ToDo: MAT_BUMP_PERCENT
                    ReadShort(r);
                    break;
                case 0xA040:
                    fileData.CurrentMaterial.material.m_shininess = ReadFloatPercentage(r);
                    break;
                case 0xA33D:
                    //--- ToDo: MAT_OPACMAP
                case 0xA210:
                    //--- ToDo: MAT_SELFIMAP
                case 0xA33C:
                    //--- ToDo: MAT_SHINMAP
                    r.ReadBytes((int)(h.Item2 - 6));
                    break;
                case 0xA354:
                    //--- ToDo: MAT_MAP_USCALE
                    break;
                case 0xA356:
                    //--- ToDo: MAT_MAP_VSCALE
                    break;
                case 0xA358:
                    //--- ToDo: MAT_MAP_UOFFSET
                    break;
                case 0xA35A:
                    //--- ToDo: MAT_MAP_VOFFSET
                    break;
                default:
                    throw new System.NotImplementedException();
            }
        }

        private static float ReadPercentage(BinaryReader r)
        {
            int temp = r.PeekChar();
            Header h = ReadHeader(r);
            float ret = 0;
            if (h.Item1 == 0x0030)
            {
                ret = (float)ReadIntPercentage(r);
                System.Diagnostics.Debug.Write("" + temp);
            }
            else if (h.Item1 == 0x0031)
            {
                ret = ReadFloatPercentage(r);
                System.Diagnostics.Debug.Write("" + temp);
            }
            else
            {
                throw new System.Exception(""+ temp);
            }
            return ret;
        }

        private static float ReadFloatPercentage(BinaryReader r)
        {
            return r.ReadSingle();
        }

        private static float ReadIntPercentage(BinaryReader r)
        {
            ushort percentage = (ushort)r.ReadInt16();
            return (float)percentage / (float)ushort.MaxValue;
        }

        private static Vec4 ReadColor(BinaryReader r, uint length)
        {
            if (length == 18)
            {
                Header h = ReadHeader(r);
                if (h.Item1 == 0x0010)
                {
                    float red = r.ReadSingle();
                    float green = r.ReadSingle();
                    float blue = r.ReadSingle();
                    return new Vec4(red, green, blue, 1);
                }
                else
                {
                    if (h.Item1 != 0x0011)
                        throw new System.NotImplementedException();
                    Vec4 col1 = ReadCharColor(r);
                    h = ReadHeader(r);
                    if (h.Item1 != 0x0012)
                        throw new System.NotImplementedException();
                    Vec4 col2 = ReadCharColor(r);
                    return 0.5f * (col1 + col2);
                }
            }
            else if (length == 9)
            {
                Header h = ReadHeader(r);
                if (h.Item1 == 0x0011)
                    return ReadCharColor(r);
                int a = h.Item1;
                throw new System.NotImplementedException();
            }
            else
            {
                throw new System.NotImplementedException();
            }
        }

        private static Vec4 ReadCharColor(BinaryReader r)
        {
            byte cr = r.ReadByte();
            byte cg = r.ReadByte();
            byte cb = r.ReadByte();
            return new Vec4((float)cr / 255.0f, (float)cg / 255.0f, (float)cb / 255.0f);
        }

        private static string ReadString(BinaryReader r)
        {
            string s = "";
            byte b;
            while ((b = r.ReadByte()) != 0)
                s += (char)b;
            return s;
        }

        private static Header ReadHeader(BinaryReader r)
        {
            ushort chunkIdentifier = (ushort)r.ReadInt16();
            uint length = (uint)r.ReadInt32();
            return new Header(chunkIdentifier, length);
        }
    }
}
