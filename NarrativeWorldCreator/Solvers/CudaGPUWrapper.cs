using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Solvers
{
    public class CudaGPUWrapper
    {
        [DllImport("Kernel.dll")]
        internal static extern float KernelWrapper(float[] input);

        public static void CudaGPUWrapperCall()
        {
            int N = 2000;
            float[] farr1 = new float[N];
            float result = KernelWrapper(farr1);
            return;
        }
    }
}
