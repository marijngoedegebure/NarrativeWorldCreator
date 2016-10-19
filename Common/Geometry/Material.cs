using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml;
using System.ComponentModel;
using System.IO;

namespace Common.Geometry
{
    public class Material : IDisposable, INotifyPropertyChanged
    {
        private static MaterialMap.Comparer materialMapComparer = new MaterialMap.Comparer();
        public static string DefaultMaterialName = "Default material";
        static Dictionary<Vec4, Material> basicColorMaterials = new Dictionary<Vec4, Material>();

        static long matCount = 0;

        public static Material Default()
        {
            Material mat = new Material();
            mat.m_ambientColor = new Vec4(0.8f, 0.8f, 0.8f);
            mat.m_diffuseColor = new Vec4(0.8f, 0.8f, 0.8f);
            mat.m_shininess = 0.0f;
            mat.m_specularColor = new Vec4(0.0f, 0.0f, 0.0f);
            mat.Transparency = 0f;
            mat.m_twoSided = false;
            mat.Name = DefaultMaterialName;
            
            return mat;
        }

        public Material() { m_name = "Empty material"; }

        public Material(Material copy)
        {
            m_ambientColor = new Vec4(copy.Ambient);
            m_diffuseColor = new Vec4(copy.Diffuse);
            m_specularColor = new Vec4(copy.m_specularColor);
            m_shininess = copy.m_shininess;
            Transparency = copy.Transparency;
            foreach(MaterialMap map in copy.m_texMaps)
                AddMaterialMap(MaterialMap.Clone(map));
            if (copy.m_opacityMap != null)
                m_opacityMap = MaterialMap.Clone(copy.m_opacityMap);
            m_twoSided = copy.m_twoSided;
            m_name = copy.m_name;
            m_scale = copy.m_scale;
            remapUV = copy.remapUV;
            hasTexTransform = copy.hasTexTransform;
            foreach (Shader s in copy.shaders)
            {
                shaders.Add(new Shader(s));
            }
        }

        public Material(XmlNode node)
        {
            if (node.Attributes["name"] != null)
                m_name = node.Attributes["name"].Value;
            else
                m_name = "Unnamed material " + (matCount++);

            if (node.Attributes["id"] != null)
                m_id = int.Parse(node.Attributes["id"].Value);

            if (node.Attributes["diffuse"] != null)
                m_diffuseColor = new Vec4(node.Attributes["diffuse"].Value);
            else
                m_diffuseColor = new Vec4(0.8f, 0.8f, 0.85f);
            if (node.Attributes["ambient"] != null)
                m_ambientColor = new Vec4(node.Attributes["ambient"].Value);
            else
                m_ambientColor = 0.8f * Diffuse;
            if (node.Attributes["specular"] != null)
                m_specularColor = new Vec4(node.Attributes["specular"].Value);
            else
                m_specularColor = new Vec4(0.25f, 0.25f, 0.25f);

            if (node.Attributes["shininess"] != null)
                m_shininess = (float)double.Parse(node.Attributes["shininess"].Value, CommonSettings.Culture);
            else
                m_shininess = 0;
            if (node.Attributes["transparency"] != null)
                Transparency = (float)double.Parse(node.Attributes["transparency"].Value, CommonSettings.Culture);
            else
                Transparency = 0;
            if (node.Attributes["twosided"] != null)
                m_twoSided = bool.Parse(node.Attributes["twosided"].Value);
            else
                m_twoSided = false;
            if (node.Attributes["texture"] != null)
            {
                MaterialMap.MapType mapType = MaterialMap.MapType.Diffuse;
                if (node.Attributes["type"] != null)
                    mapType = (MaterialMap.MapType)Enum.Parse(typeof(MaterialMap.MapType), node.Attributes["type"].Value);
                AddMaterialMap(new FileMaterialMap(node.Attributes["texture"].Value, mapType));
            }
            else if (node.Attributes["file"] != null)
            {
                MaterialMap.MapType mapType = MaterialMap.MapType.Diffuse;
                if (node.Attributes["type"] != null)
                    mapType = (MaterialMap.MapType)Enum.Parse(typeof(MaterialMap.MapType), node.Attributes["type"].Value);
                AddMaterialMap(new FileMaterialMap(node.Attributes["file"].Value, mapType));
            }
            if (node.Attributes["scale"] != null)
                Scale = (float)double.Parse(node.Attributes["scale"].Value, CommonSettings.Culture);
            else
                Scale = 1;
            if (node.Attributes["remap"] != null)
            {
                string remap = node.Attributes["remap"].Value;
                string[] split = remap.Split(';');
                RemapUV(new Vec2((float)double.Parse(split[0]), (float)double.Parse(split[1])),
                            new Vec2((float)double.Parse(split[2]), (float)double.Parse(split[3])));
            }
            foreach (XmlNode subnode in node.ChildNodes)
            {
                if (subnode.Name == "Shader")
                {
                    Shader s = Shader.Load(subnode);
                    AddShader(s);
                }
                else if (subnode.Name == "Texture")
                {
                    MaterialMap.MapType mapType = MaterialMap.MapType.Diffuse;
                    if (subnode.Attributes["type"] != null)
                        mapType = (MaterialMap.MapType)Enum.Parse(typeof(MaterialMap.MapType), subnode.Attributes["type"].Value);
                    AddMaterialMap(new FileMaterialMap(subnode.Attributes["file"].Value, mapType));
                }
            }
        }

        public Material(string texturename, MaterialMap.MapType mapType)
        {
            m_ambientColor = new Vec4(0.8f, 0.8f, 0.8f);
            m_diffuseColor = new Vec4(0.8f, 0.8f, 0.8f);
            m_shininess = 0.5f;
            m_specularColor = new Vec4(0.1f, 0.1f, 0.1f);
            AddMaterialMap(new FileMaterialMap(texturename, mapType));
            Transparency = 0;
            m_twoSided = false;
            m_name = texturename;
        }

        public Material(string diffuseTexture, string bumpTexture, string bumpVertexShader, string bumpFragmentShader)
        {
            m_ambientColor = new Vec4(0.8f, 0.8f, 0.8f);
            m_diffuseColor = new Vec4(0.8f, 0.8f, 0.8f);
            m_shininess = 0.5f;
            m_specularColor = new Vec4(0.1f, 0.1f, 0.1f);
            AddMaterialMap(new FileMaterialMap(diffuseTexture, MaterialMap.MapType.Diffuse));
            AddMaterialMap(new FileMaterialMap(bumpTexture, MaterialMap.MapType.Diffuse));
            AddShader(new Shader(bumpVertexShader, Shader.Type.Vertex));
            AddShader(new Shader(bumpFragmentShader, Shader.Type.Fragment));
            Transparency = 0;
            m_twoSided = false;
            m_name = diffuseTexture + " (bumped)";
        }

        internal Vec4 m_ambientColor = new Vec4(0, 0, 0), m_diffuseColor = new Vec4(0, 0, 0), m_specularColor = new Vec4(0, 0, 0);
        internal float m_shininess, m_transparency;
        private List<MaterialMap> m_texMaps = new List<MaterialMap>();
        private MaterialMap m_opacityMap = null;
        private bool m_twoSided;
        private string m_name;
        float m_scale = 1;
        public int m_id;
        bool hasTexTransform = false;
        Vec2[] remapUV = null;

        public Vec4 Ambient { get { return m_ambientColor; } }
        public Vec4 Diffuse { get { return m_diffuseColor; } }
        public Vec4 Specular { get { return m_specularColor; } }
        public float Shininess { get { return m_shininess; } set { m_shininess = value; HandlePropertyChange("Shininess"); } }
        public float Transparency { get { return m_transparency; } set { m_transparency = value; HandlePropertyChange("Transparency"); } }
        public ReadOnlyCollection<MaterialMap> MaterialMaps { get { return m_texMaps.AsReadOnly(); } }
        public MaterialMap OpacityMap { get { return m_opacityMap; } set { m_opacityMap = value; HandlePropertyChange("OpacityMap"); } }
        public bool TwoSided { get { return m_twoSided; } set { m_twoSided = value; HandlePropertyChange("TwoSided"); } }

        public string Name { get { return m_name; }
            set { 
                m_name = value; 
            } }

        List<Shader> shaders = new List<Shader>();

        public ReadOnlyCollection<Shader> Shaders { get { return shaders.AsReadOnly(); } }

        public void AddShader(Shader shader)
        {
            shaders.Add(shader);
            HandlePropertyChange("Shaders");
        }

        public void AddMaterialMap(MaterialMap map)
        {
            m_texMaps.Add(map);
            m_texMaps.Sort(materialMapComparer);
            HandlePropertyChange("MaterialMaps");
        }

        public void RemoveShadersOfType(Shader.Type type)
        {
            for (int i = shaders.Count - 1; i >= 0; --i)
                if (shaders[i].ShaderType == type)
                    shaders.RemoveAt(i);
        }

        public float Scale
        {
            get
            {
                return m_scale;
            }
            set
            {
                m_scale = value;
                hasTexTransform = m_scale != 1;
            }
        }

        public void RemapUV(Vec2 v1, Vec2 v2)
        {
            remapUV = null;
            remapUV = new Vec2[2];
            remapUV[0] = new Vec2(v1);
            remapUV[1] = new Vec2(v2);
        }

        public static bool operator==(Material m1, Material m2)
        {
            if ((object)m1 == null)
            {
                if ((object)m2 == null)
                    return true;
                return false;
            }
            if ((object)m2 == null)
                return false;

            //if (m1.m_texMap != null && m2.m_texMap != null && m1.m_texMap.m_mapFilename != m2.m_texMap.m_mapFilename)
            //    return false;
            //return m1.m_name == m2.m_name;

            bool texmapsame = true;
            if (m1.MaterialMaps.Count == m2.MaterialMaps.Count)
            {
                for (int i = 0; i < m1.MaterialMaps.Count; ++i)
                {
                    if (m1.MaterialMaps[i] != m2.MaterialMaps[i])
                    {
                        texmapsame = false;
                        break;
                    }
                }
            }
            else
                texmapsame = false;

            return m1.Ambient == m2.Ambient &&
                    m1.Diffuse == m2.Diffuse &&
                    m1.m_shininess == m2.m_shininess &&
                    m1.m_specularColor == m2.m_specularColor &&
                    m1.Transparency == m2.Transparency &&
                    m1.m_twoSided == m2.m_twoSided &&
                    texmapsame &&
                    m1.m_opacityMap == m2.m_opacityMap &&
                    m1.m_name == m2.m_name;
        }

        public static bool operator !=(Material m1, Material m2)
        {
            return !(m1 == m2);
        }

        public override bool Equals(object obj)
        {
            return this == (Material)obj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void Save(System.IO.BinaryWriter wr)
        {
            this.Ambient.Save(wr);
            this.Diffuse.Save(wr);
            wr.Write(this.m_name);
            wr.Write(this.m_opacityMap == null);
            if (this.m_opacityMap != null)
                this.m_opacityMap.Save(wr);
            wr.Write((double)this.m_shininess);
            this.m_specularColor.Save(wr);
            wr.Write(this.MaterialMaps.Count);
            if (this.MaterialMaps.Count > 0)
                foreach (MaterialMap map in this.m_texMaps)
                    map.Save(wr);
            wr.Write((double)this.Transparency);
            wr.Write(this.m_twoSided);
            wr.Write((double)this.m_scale);
            wr.Write(remapUV != null);
            if (remapUV != null)
            {
                remapUV[0].Save(wr);
                remapUV[1].Save(wr);
            }
            wr.Write('~');
            wr.Write(shaders.Count);
            foreach (Shader s in shaders)
                s.Save(wr);
        }

        public Material(System.IO.BinaryReader br)
        {
            m_ambientColor = new Vec4(br);
            m_diffuseColor = new Vec4(br);
            m_name = br.ReadString();
            bool isOpacityMapNull = br.ReadBoolean();
            if (isOpacityMapNull)
                this.m_opacityMap = null;
            else
                this.m_opacityMap = MaterialMap.Load(br);

            this.m_shininess = (float)br.ReadDouble();
            this.m_specularColor = new Vec4(br);
            int count = br.ReadInt32();
            if (count > 0)
                for (int i = 0; i < count; ++i)
                    AddMaterialMap(MaterialMap.Load(br));

            this.Transparency = (float)br.ReadDouble();
            this.m_twoSided = br.ReadBoolean();
            this.m_scale = (float)br.ReadDouble();
            if (this.m_scale != 1)
                this.hasTexTransform = true;
            if (br.ReadBoolean())
            {
                remapUV = new Vec2[2];
                remapUV[0] = Vec2.Load(br);
                remapUV[1] = Vec2.Load(br);
            }
            else
                remapUV = null;
            if (br.PeekChar() == '~')
            {
                br.ReadChar();
                count = br.ReadInt32();
                for (int i = 0; i < count; ++i)
                    AddShader(Shader.Load(br));
            }
        }

        public Vec2 UpdateTexUV(Vec2 texUV)
        {
            if (hasTexTransform)
                return Remap(m_scale * texUV);
            return Remap(texUV);
        }

        private Vec2 Remap(Vec2 texUV)
        {
            if (remapUV == null)
                return texUV;
            return new Vec2(remapUV[0].X + texUV.X * remapUV[1].X, remapUV[0].Y + texUV.Y * remapUV[1].Y);
        }

        public static Material BasicColorMaterial(Vec4 color)
        {
            return BasicColorMaterial(color, 0);
        }

        public static Material BasicColorMaterial(Vec4 color, float transparency)
        {
            if (basicColorMaterials.ContainsKey(color))
                return basicColorMaterials[color];
            Material mat = new Material();
            mat.m_name = "Basic color " + color.ToColorString();
            mat.m_ambientColor = 0.8f * color;
            mat.m_diffuseColor = color;
            mat.m_specularColor = new Vec4(0, 0, 0);
            mat.Transparency = transparency;
            basicColorMaterials.Add(color, mat);
            return mat;
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (this.m_ambientColor != null)
            {
                this.m_ambientColor.Dispose(); this.m_ambientColor = null;
            }
            if (this.m_diffuseColor != null)
            {
                this.m_diffuseColor.Dispose(); this.m_diffuseColor = null;
            }
            if (this.m_specularColor != null)
            {
                this.m_specularColor.Dispose(); this.m_specularColor = null;
            }
            this.remapUV = null;
        }

        #endregion

        private void HandlePropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public void ClearMaterialMaps()
        {
            m_texMaps.Clear();
            HandlePropertyChange("MaterialMaps");
        }

        public void SetMaterialMapAt(int index, MaterialMap map)
        {
            m_texMaps[index] = map;
            HandlePropertyChange("MaterialMaps");
        }

        public void Save(string p)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(p));
            Save(bw);
            bw.Close();
        }

        public static Material Load(string path)
        {
            BinaryReader br = new BinaryReader(System.IO.File.Open(path, System.IO.FileMode.Open));
            Material ret = new Material(br);
            br.Close();
            return ret;
        }
    }
}
