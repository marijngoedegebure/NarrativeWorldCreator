using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Exception = System.Exception;

namespace Common.Geometry
{
    public class FileLoaderObj
    {
        static char[] space = new char[] { ' ' };

        public static Object Import(string path)
        {
            List<Vec3> vertices = new List<Vec3>();
            List<Vec3> normals = new List<Vec3>();
            List<Vec2> textureCoordinates = new List<Vec2>();

            Dictionary<string, Material> materials = new Dictionary<string, Material>();
            string fileName = path.Substring(1 + path.LastIndexOfAny(new char[] { '/', '\\'}));
            string pathName = path.Substring(0, path.Length - fileName.Length);
            Object ret = new Object(fileName);
            Mesh currentMesh = null;

            foreach (string line in File.ReadAllLines(path))
            {
                string l = line.Trim();
                if (l.StartsWith("#") || l == "")
                {
                    //--- do nothing = comment
                }
                else
                {
                    if (l.StartsWith("v "))
                    {
                        string[] elements = l.Split(space, System.StringSplitOptions.RemoveEmptyEntries);
                        if (elements.Length != 4)
                            throw new Exception("Something wrong with obj file!");
                        vertices.Add(new Vec3((float)double.Parse(elements[1]), (float)double.Parse(elements[2]), (float)double.Parse(elements[3])));
                    }
                    else if (l.StartsWith("vn "))
                    {
                        string[] elements = l.Split(space, System.StringSplitOptions.RemoveEmptyEntries);
                        if (elements.Length != 4)
                            throw new Exception("Something wrong with obj file!");
                        normals.Add(new Vec3((float)double.Parse(elements[1]), (float)double.Parse(elements[2]), (float)double.Parse(elements[3])));
                    }
                    else if (l.StartsWith("vt "))
                    {
                        string[] elements = l.Split(space, System.StringSplitOptions.RemoveEmptyEntries);
                        if (elements.Length < 3)
                            throw new Exception("Something wrong with obj file!");
                        textureCoordinates.Add(new Vec2((float)double.Parse(elements[1]), (float)double.Parse(elements[2])));
                    }
                    else if (l.StartsWith("g "))
                    {
                        string groupName = l.Substring(2).Trim();
                        currentMesh = new Mesh(8);
                        currentMesh.m_name = groupName;
                        ret.AddNode(currentMesh);
                    }
                    else if (l.StartsWith("mtllib "))
                    {
                        string materialFile = pathName + l.Substring(7).Trim();
                        LoadMaterialLib(materialFile, materials);
                    }
                    else if (l.StartsWith("usemtl "))
                        currentMesh.Material = materials[l.Substring(7).Trim()];
                    else if (l.StartsWith("s "))
                    {
                        //--- ToDo: do something with smooth groups
                    }
                    else if (l.StartsWith("f "))
                    {
                        l = l.Substring(2).Trim();
                        string[] elements = l.Split(space, System.StringSplitOptions.RemoveEmptyEntries);
                        if (elements.Length > 4)
                            throw new System.NotImplementedException();
                        else if (elements.Length == 3)
                        {
                            bool setNormal;
                            VertexInfo v1 = Vert(elements[0], vertices, normals, textureCoordinates, out setNormal);
                            VertexInfo v2 = Vert(elements[1], vertices, normals, textureCoordinates, out setNormal);
                            VertexInfo v3 = Vert(elements[2], vertices, normals, textureCoordinates, out setNormal);
                            if (!setNormal)
                            {
                                Vec3 normal = Vec3.CalculateNormal(v1.m_position, v2.m_position, v3.m_position);
                                v1.m_normal = normal;
                                v2.m_normal = normal;
                                v3.m_normal = normal;
                            }
                            currentMesh.AddTriangle(v1, v2, v3);
                        }
                        else
                            throw new System.NotImplementedException();
                    }
                    else
                        throw new System.NotImplementedException();
                }
            }

            return ret;
        }

        private static VertexInfo Vert(string p, List<Vec3> vertices, List<Vec3> normals, List<Vec2> textureCoordinates, out bool setNormal)
        {
            string[] elements = p.Split('/');
            setNormal = elements.Length == 3;
            Vec3 pos = vertices[int.Parse(elements[0]) - 1];
            Vec2 tex = elements.Length > 1 && elements[1].Trim() != "" ? textureCoordinates[int.Parse(elements[1]) - 1] : new Vec2(0, 0);
            Vec3 norm = elements.Length > 2 && elements[2].Trim() != "" ? normals[int.Parse(elements[2]) - 1] : new Vec3(0, 0, 0);
            return new VertexInfo(pos, tex, norm, Vec4.White);
        }

        private static void LoadMaterialLib(string path, Dictionary<string, Material> materials)
        {
            Material currentMaterial = new Material();
            foreach (string line in File.ReadAllLines(path))
            {
                string l = line.Trim();
                if (l.StartsWith("#") || l == "")
                {
                    //--- do nothing = comment
                }
                else
                {
                    if (l.StartsWith("newmtl "))
                    {
                        string materialName = l.Substring(7).Trim();
                        currentMaterial = new Material();
                        currentMaterial.Name = materialName;
                        currentMaterial.Ambient.Set(new Vec4(0.2f, 0.2f, 0.2f, 1));
                        currentMaterial.Diffuse.Set(new Vec4(0.8f, 0.8f, 0.8f, 1));
                        currentMaterial.Specular.Set(new Vec4(1, 1, 1, 1));
                        currentMaterial.Transparency = 0;
                        currentMaterial.Shininess = 0;

                        materials.Add(materialName, currentMaterial);
                    }
                    else if (l.StartsWith("Ka "))
                        currentMaterial.Ambient.Set(Color(l.Substring(2).Trim()));
                    else if (l.StartsWith("Kd "))
                        currentMaterial.Diffuse.Set(Color(l.Substring(2).Trim()));
                    else if (l.StartsWith("Ks "))
                        currentMaterial.Specular.Set(Color(l.Substring(2).Trim()));
                    else if (l.StartsWith("d "))
                        currentMaterial.Transparency = 1 - (float)double.Parse(l.Substring(2).Trim());
                    else if (l.StartsWith("Tr "))
                        currentMaterial.Transparency = 1 - (float)double.Parse(l.Substring(3).Trim());
                    else if (l.StartsWith("Ns "))
                        currentMaterial.Shininess = 1 - (float)double.Parse(l.Substring(3).Trim());
                    else if (l.StartsWith("Tf "))
                    {
                        //--- ToDo: transparency as color: dunno how that should work
                    }
                    else if (l.StartsWith("illum "))
                    {
                        //--- ToDo: do something with illumination model
                    }
                    else if (l.StartsWith("Ni "))
                    {
                        //--- We don't handle refraction
                    }
                    else if (l.StartsWith("Ke "))
                    {
                        //--- ???
                    }
                    else
                        throw new System.NotImplementedException();
                }
            }
        }

        private static Vec4 Color(string p)
        {
            string[] elements = p.Split(space, System.StringSplitOptions.RemoveEmptyEntries);
            if (elements.Length != 3)
                throw new Exception("Something wrong with obj file!");
            return new Vec4((float)double.Parse(elements[0]), (float)double.Parse(elements[1]), (float)double.Parse(elements[2]), 1);
        }
    }
}
