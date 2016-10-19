using System;
using System.Collections.Generic;

using System.Text;
using System.Collections.ObjectModel;

namespace Common
{
    public static class Utility
    {
        static Dictionary<string, object> storage = new Dictionary<string, object>();

        public static void Store(string name, object obj)
        {
            if (storage.ContainsKey(name))
                storage[name] = obj;
            else
                storage.Add(name, obj);
        }

        public static object GetFromStorage(string name)
        {
            if (storage.ContainsKey(name))
                return storage[name];
            return null;
        }

        public static int PickFromWeights(Random random, List<double> weights)
        {
            return PickFromWeights(random, weights.AsReadOnly());
        }

        public static int PickFromWeights(Random random, ReadOnlyCollection<double> weights)
        {
            double total = 0;
            foreach (double d in weights)
                total += d;
            double rand = random.NextDouble() * total;

            int i = -1;
            double test = 0;
            while (test < rand)
            {
                ++i;
                test += weights[i];
            }
            return i;
        }

        public static List<Vec2> MinkowskiDifference2(List<Vec2> Aoriginal, List<Vec2> B)
        {
            if (Aoriginal.Count < 1 || B.Count < 1)
                throw new Exception("Minkowski difference cannot be calculated on empty shapes: " + Aoriginal + " or " + B);
	        List<Common.Vec2> A = new List<Vec2>();
	        for (int i = 0; i < Aoriginal.Count; ++i)
		        A.Add(new Vec2(Aoriginal[i].X, Aoriginal[i].Y));

	        int startA = 0;
	        Vec2 startVecA = A[0];
	        for(int i = 1; i < A.Count; ++i)
	        {
		        Vec2 t2 = A[i];
		        if (t2.X > startVecA.X || (t2.X == startVecA.X && t2.Y > startVecA.Y))
		        {
			        startA = i;
			        startVecA = t2;
		        }
	        }
	        int startB = 0;
	        Vec2 startVecB = B[0];
	        for(int i = 1; i < B.Count; ++i)
	        {
		        Vec2 t2 = B[i];
		        if (t2.X > startVecB.X || (t2.X == startVecB.X && t2.Y > startVecB.Y))
		        {
			        startB = i;
			        startVecB = t2;
		        }
	        }

	        List<Vec2> extremePoints = new List<Vec2>();

	        for(int i = 0; i < A.Count; ++i)
	        {
		        int ai = (startA + i) % A.Count;
		        int pStart = startB;
		        int bI = 0;
		        double currentBeta = 0;
		        double previousBeta = 0;
		        double currentAlfa = AngleWithYAxis(A[ai], A[(ai + 1) % A.Count]);
		        do
		        {
			        bI = pStart;
			        currentBeta = AngleWithYAxis(B[pStart], B[(pStart + 1) % B.Count]);
			        while(currentBeta < previousBeta)
				        currentBeta += 2 * Math.PI;
			        previousBeta = currentBeta;
			        pStart = (pStart + 1) % B.Count;
		        }
		        while(currentBeta < currentAlfa);
		        extremePoints.Add(B[bI]);
	        }

	        List<Line2> c = new List<Line2>();
	        for(int i = 0; i < A.Count; ++i)
	        {
		        int aiIndex = (startA + i) % A.Count;
		        Vec2 bj = extremePoints[i];
		        Vec2 Ai = A[aiIndex];
		        Vec2 Ai1 = A[(aiIndex + 1) % A.Count];
		        Line2 ai = new Line2(new Vec2(Ai.X, Ai.Y), new Vec2(Ai1.X, Ai1.Y));
        		
		        ai.Move(-bj); //--- becomes ci now
		        c.Add(ai);
	        }

	        List<Vec2> K = new List<Vec2>();
	        List<Vec2> P = new List<Vec2>();
            List<float> s = new List<float>();
            List<float> t = new List<float>();
	        for(int i = 0; i < A.Count; ++i)
	        {
		        Vec2 Pi;
                float ti, si;
		        Vec2.Intersection(c[i].P1, c[i].P2, c[(i + 1) % A.Count].P1, c[(i + 1) % A.Count].P2, out ti, out si, out Pi);
		        P.Add(Pi);

		        t.Add(ti);
		        s.Add(si);
	        }

	        for(int i = 0; i < A.Count; ++i)
		        if (t[i] >= 0 && t[i] <= 1 && s[i] >= 0 && s[i] <= 1 && s[(i + A.Count - 1) % A.Count] < t[i])
			        K.Add(P[i]);

	        return K;
        }

 
        public static double AngleWithYAxis(Vec2 p1, Vec2 p2)
        {
	        Vec2 t = p2 - p1;
	        t = t.normalize();
	        double angle = t.GetAngle() - 0.5 * Math.PI;
	        while (angle < 0)
		        angle += 2 * Math.PI;
	        return angle;
        }

        #region Method: ShuffleList(List<E> list)
        /// <summary>
        /// Shuffle the contents in the list
        /// </summary>
        public static List<E> ShuffleList<E>(List<E> list)
        {
            List<E> randomList = new List<E>();
            int randomIndex = 0;
            while (list.Count > 0)
            {
                randomIndex = RandomNumber.Next(0, list.Count);
                randomList.Add(list[randomIndex]);
                list.RemoveAt(randomIndex);
            }
            return randomList;
        }
        #endregion Method: ShuffleList(List<E> list)
    }
}
