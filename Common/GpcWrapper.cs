using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace Common {
 
    public class TriStrip {

        public List<Vec2d> this[int index] {
            get {
                List<Vec2d> result = null;
                if (index >= 0 && index < this.Count) {
                    result = strips[index];
                }
                return result;
            }
        }

        public IEnumerable<List<Vec2d>> Strips
        {
            get
            {
                foreach (List<Vec2d> strip in strips)
                    yield return strip;
            }
        }

        public int Count {
            get { return strips.Length; }
        }
        
        private readonly List<Vec2d>[] strips;

        public TriStrip(int numStrips) {
            this.strips = new List<Vec2d>[numStrips];
            for (int i = 0; i < this.Count; i++) {
                this.strips[i] = new List<Vec2d>();
            }
        }

        public List<float> ToTriangleList() {
            List<float> vertices = new List<float>();
            for (int stripID = 0; stripID < strips.Length; ++stripID) {
                int numTriangles = (strips[stripID].Count - 2);
                for (int triangle = 0; triangle < numTriangles; ++triangle) {
                    vertices.Add((float)strips[stripID][triangle].X);
                    vertices.Add((float)strips[stripID][triangle].Y);
                    vertices.Add((float)strips[stripID][triangle + 1].X);
                    vertices.Add((float)strips[stripID][triangle + 1].Y);
                    vertices.Add((float)strips[stripID][triangle + 2].X);
                    vertices.Add((float)strips[stripID][triangle + 2].Y);
                }
            }
            return vertices;
        }
    }

    public class GpcWrapper {

        public enum GpcOperation { Difference, Intersection, XOr, Union }

        private GpcWrapper() {
            // Unused
        }

        public static TriStrip PolygonToTristrip(Polygon polygon) {
            gpc_tristrip gpc_strip = new gpc_tristrip();
            gpc_polygon gpc_pol = GpcWrapper.PolygonTo_gpc_polygon(polygon);
            gpc_polygon_to_tristrip(ref gpc_pol, ref gpc_strip);
            TriStrip tristrip = GpcWrapper.gpc_strip_ToTristrip(gpc_strip);

            GpcWrapper.Free_gpc_polygon(gpc_pol);
            GpcWrapper.gpc_free_tristrip(ref gpc_strip);

            return tristrip;
        }

        public static TriStrip ClipToTristrip(GpcOperation operation, Polygon subject_polygon, Polygon clip_polygon) {
            gpc_tristrip gpc_strip = new gpc_tristrip();
            gpc_polygon gpc_subject_polygon = GpcWrapper.PolygonTo_gpc_polygon(subject_polygon);
            gpc_polygon gpc_clip_polygon = GpcWrapper.PolygonTo_gpc_polygon(clip_polygon);

            gpc_tristrip_clip(operation, ref gpc_subject_polygon, ref gpc_clip_polygon, ref gpc_strip);
            TriStrip tristrip = GpcWrapper.gpc_strip_ToTristrip(gpc_strip);

            GpcWrapper.Free_gpc_polygon(gpc_subject_polygon);
            GpcWrapper.Free_gpc_polygon(gpc_clip_polygon);
            GpcWrapper.gpc_free_tristrip(ref gpc_strip);

            return tristrip;
        }

        public static Polygon Clip(GpcOperation operation, Polygon subject_polygon, Polygon clip_polygon) {
            gpc_polygon gpc_polygon = new gpc_polygon();
            gpc_polygon gpc_subject_polygon = GpcWrapper.PolygonTo_gpc_polygon(subject_polygon);
            gpc_polygon gpc_clip_polygon = GpcWrapper.PolygonTo_gpc_polygon(clip_polygon);

            gpc_polygon_clip(operation, ref gpc_subject_polygon, ref gpc_clip_polygon, ref gpc_polygon);
            Polygon polygon = GpcWrapper.gpc_polygon_ToPolygon(gpc_polygon);

            GpcWrapper.Free_gpc_polygon(gpc_subject_polygon);
            GpcWrapper.Free_gpc_polygon(gpc_clip_polygon);
            GpcWrapper.gpc_free_polygon(ref gpc_polygon);

            return polygon;
        }

        private static gpc_polygon PolygonTo_gpc_polygon(Polygon polygon) {
            gpc_polygon gpc_pol = new gpc_polygon();
            if (polygon != null)
            {               
                gpc_pol.num_contours = polygon.NumContours;
                int[] hole = new int[polygon.NumContours];
                for (int i = 0; i < polygon.NumContours; ++i)
                {
                    hole[i] = (polygon.IsHole(i) ? 1 : 0);
                }
                gpc_pol.hole = Marshal.AllocCoTaskMem(polygon.NumContours * Marshal.SizeOf(hole[0]));
                if (polygon.NumContours > 0)
                {
                    Marshal.Copy(hole, 0, gpc_pol.hole, polygon.NumContours);
                    gpc_pol.contour = Marshal.AllocCoTaskMem(polygon.NumContours * Marshal.SizeOf(new gpc_vertex_list()));
                }
                IntPtr ptr = gpc_pol.contour;
                for (int i = 0; i < polygon.NumContours; ++i)
                {
                    gpc_vertex_list gpc_vtx_list = new gpc_vertex_list();
                    gpc_vtx_list.num_vertices = polygon[i].Count;
                    gpc_vtx_list.vertex = Marshal.AllocCoTaskMem(polygon[i].Count * Marshal.SizeOf(new gpc_vertex()));
                    IntPtr ptr2 = gpc_vtx_list.vertex;
                    for (int j = 0; j < polygon[i].Count; ++j)
                    {
                        gpc_vertex gpc_vtx = new gpc_vertex();
                        gpc_vtx.x = polygon[i][j].X;
                        gpc_vtx.y = polygon[i][j].Y;
                        Marshal.StructureToPtr(gpc_vtx, ptr2, false);
                        ptr2 = (IntPtr)(((int)ptr2) + Marshal.SizeOf(gpc_vtx));
                    }
                    Marshal.StructureToPtr(gpc_vtx_list, ptr, false);
                    ptr = (IntPtr)(((int)ptr) + Marshal.SizeOf(gpc_vtx_list));
                }
            }
            return gpc_pol;
        }

        private static Polygon gpc_polygon_ToPolygon(gpc_polygon gpc_polygon) {
            Polygon polygon = null;
            if (gpc_polygon.num_contours > 0) {
                polygon = new Polygon(gpc_polygon.num_contours);
                int[] holeInt = new int[polygon.NumContours];
                IntPtr ptr = gpc_polygon.hole;
                if (polygon.NumContours > 0) {
                    Marshal.Copy(gpc_polygon.hole, holeInt, 0, polygon.NumContours);
                }
                for (int i = 0; i < polygon.NumContours; ++i) {
                    polygon.SetHole(i, holeInt[i] != 0);
                }
                ptr = gpc_polygon.contour;
                for (int i = 0; i < polygon.NumContours; ++i) {
                    gpc_vertex_list gpc_vtx_list = (gpc_vertex_list)Marshal.PtrToStructure(ptr, typeof(gpc_vertex_list));
                    int numVertices = gpc_vtx_list.num_vertices;
                    polygon[i].Capacity = numVertices;
                    IntPtr ptr2 = gpc_vtx_list.vertex;
                    for (int j = 0; j < numVertices; ++j) {
                        gpc_vertex gpc_vtx = (gpc_vertex)Marshal.PtrToStructure(ptr2, typeof(gpc_vertex));
                        polygon[i].Add(new Vec2d(gpc_vtx.x, gpc_vtx.y));
                        ptr2 = (IntPtr)(((int)ptr2) + Marshal.SizeOf(gpc_vtx));
                    }
                    ptr = (IntPtr)(((int)ptr) + Marshal.SizeOf(gpc_vtx_list));
                }
            }
            return polygon;
        }

        private static TriStrip gpc_strip_ToTristrip(gpc_tristrip gpc_strip) {
            int numStrips = gpc_strip.num_strips;
            TriStrip tristrip = new TriStrip(numStrips); 
            IntPtr ptr = gpc_strip.strip;
            for (int i = 0; i < numStrips; ++i) {
                gpc_vertex_list gpc_vtx_list = (gpc_vertex_list)Marshal.PtrToStructure(ptr, typeof(gpc_vertex_list));
                int numVertices = gpc_vtx_list.num_vertices;
                tristrip[i].Capacity = numVertices;
                IntPtr ptr2 = gpc_vtx_list.vertex;
                for (int j = 0; j < numVertices; ++j) {
                    gpc_vertex gpc_vtx = (gpc_vertex)Marshal.PtrToStructure(ptr2, typeof(gpc_vertex));
                    tristrip[i].Add(new Vec2d(gpc_vtx.x, gpc_vtx.y));
                    ptr2 = (IntPtr)(((int)ptr2) + Marshal.SizeOf(gpc_vtx));
                }
                ptr = (IntPtr)(((int)ptr) + Marshal.SizeOf(gpc_vtx_list));
            }
            return tristrip;
        }

        private static void Free_gpc_polygon(gpc_polygon gpc_pol) {
            Marshal.FreeCoTaskMem(gpc_pol.hole);
            IntPtr ptr = gpc_pol.contour;
            for (int i = 0; i < gpc_pol.num_contours; ++i) {
                gpc_vertex_list gpc_vtx_list = (gpc_vertex_list)Marshal.PtrToStructure(ptr, typeof(gpc_vertex_list));
                Marshal.FreeCoTaskMem(gpc_vtx_list.vertex);
                ptr = (IntPtr)(((int)ptr) + Marshal.SizeOf(gpc_vtx_list));
            }
            Marshal.FreeCoTaskMem(gpc_pol.contour);
        }

        [DllImport("GPCLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void gpc_polygon_to_tristrip([In]     ref gpc_polygon polygon,
                                                           [In, Out] ref gpc_tristrip tristrip);

        [DllImport("GPCLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void gpc_polygon_clip([In]     GpcOperation set_operation,
                                                    [In]     ref gpc_polygon subject_polygon,
                                                    [In]     ref gpc_polygon clip_polygon,
                                                    [In, Out] ref gpc_polygon result_polygon);

        [DllImport("GPCLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void gpc_tristrip_clip([In]     GpcOperation set_operation,
                                                     [In]     ref gpc_polygon subject_polygon,
                                                     [In]     ref gpc_polygon clip_polygon,
                                                     [In, Out] ref gpc_tristrip result_tristrip);

        [DllImport("GPCLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void gpc_free_tristrip([In] ref gpc_tristrip tristrip);

        [DllImport("GPCLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void gpc_free_polygon([In] ref gpc_polygon polygon);

        [DllImport("GPCLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void gpc_read_polygon([In] IntPtr fp, [In] int read_hole_flags, [In, Out] ref gpc_polygon polygon);

        [DllImport("GPCLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void gpc_write_polygon([In] IntPtr fp, [In] int write_hole_flags, [In] ref gpc_polygon polygon);

        enum gpc_op                                   /* Set operation type                */
        {
            GPC_DIFF = 0,                             /* Difference                        */
            GPC_INT = 1,                             /* Intersection                      */
            GPC_XOR = 2,                             /* Exclusive or                      */
            GPC_UNION = 3                              /* Union                             */
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct gpc_vertex                    /* Polygon vertex structure          */
        {
            public double x;            /* Vertex x component                */
            public double y;            /* vertex y component                */
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct gpc_vertex_list               /* Vertex list structure             */
        {
            public int num_vertices; /* Number of vertices in list        */
            public IntPtr vertex;       /* Vertex array pointer              */
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct gpc_polygon                   /* Polygon set structure             */
        {
            public int num_contours; /* Number of contours in polygon     */
            public IntPtr hole;         /* Hole / external contour flags     */
            public IntPtr contour;      /* Contour array pointer             */
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct gpc_tristrip                  /* Tristrip set structure            */
        {
            public int num_strips;   /* Number of tristrips               */
            public IntPtr strip;        /* Tristrip array pointer            */
        }
    }
}
