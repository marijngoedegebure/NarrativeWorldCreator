using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Common
{
    public class IntervalList : List<Interval>
    {
        public new void Add(Interval interval)
        {
            if (interval == null)
                return;
            if (interval.Length < 0)
                return;
            List<Interval> overlappingIntervals = new List<Interval>();
            foreach (Interval interv in this)
                if (interv.Overlaps(interval))
                    overlappingIntervals.Add(interv);
            if (overlappingIntervals.Count == 0)
                base.Add(interval);
            else if (overlappingIntervals.Count == 1)
                overlappingIntervals[0].Add(interval);
            else
            {
                Interval iFirst = overlappingIntervals[0];
                iFirst.Add(interval);
                foreach (Interval i in overlappingIntervals)
                {
                    if (i != iFirst)
                    {
                        this.Remove(i);
                        iFirst.Add(i);
                    }
                }
            }
        }

        public new void Remove(Interval interval)
        {
            if (interval.Length <= 0)
                return;
            List<Interval> overlappingIntervals = new List<Interval>();
            foreach (Interval interv in this)
                if (interv.Overlaps(interval))
                    overlappingIntervals.Add(interv);
            foreach (Interval i in overlappingIntervals)
            {
                base.Remove(i);
                List<Interval> newInts = i.Remove(interval);
                foreach (Interval i2 in newInts)
                    base.Add(i2);
            }
        }

        public float GetRandomValue(Random r)
        {
            if (this.Count == 0)
                return 0;
            if (this.Count == 1)
                return this[0].GetRandom(r);
            float totalLength = 0;
            foreach (Interval i in this)
                totalLength += i.Length;
            float rand = (float)r.NextDouble() * totalLength;
            float temp = 0;
            int c = 0;
            while (temp <= rand)
            {
                temp += (float)this[c].Length;
                ++c;
            }
            return this[c - 1].GetRandom(r);
        }

        public override string ToString()
        {
            string ret = "";
            foreach (Interval i in this)
            {
                if (ret == "")
                    ret = i.ToString();
                else
                    ret += ", " + i.ToString();
            }
            return "{ " + ret + " }";
        }
    }

    public class IntervaliList : List<Intervali>
    {
        public new void Add(Intervali interval)
        {
            if (interval == null)
                return;
            if (interval.Length < 0)
                return;
            List<Intervali> overlappingIntervals = new List<Intervali>();
            foreach (Intervali interv in this)
                if (interv.Overlaps(interval))
                    overlappingIntervals.Add(interv);
            if (overlappingIntervals.Count == 0)
                base.Add(interval);
            else if (overlappingIntervals.Count == 1)
                overlappingIntervals[0].Add(interval);
            else
            {
                Intervali iFirst = overlappingIntervals[0];
                iFirst.Add(interval);
                foreach (Intervali i in overlappingIntervals)
                {
                    if (i != iFirst)
                    {
                        this.Remove(i);
                        iFirst.Add(i);
                    }
                }
            }
        }

        public new void Remove(Intervali interval)
        {
            if (interval.Length <= 0)
                return;
            List<Intervali> overlappingIntervals = new List<Intervali>();
            foreach (Intervali interv in this)
                if (interv.Overlaps(interval))
                    overlappingIntervals.Add(interv);
            foreach (Intervali i in overlappingIntervals)
            {
                base.Remove(i);
                List<Intervali> newInts = i.Remove(interval);
                foreach (Intervali i2 in newInts)
                    base.Add(i2);
            }
        }

        public int GetRandomValue(Random r)
        {
            if (this.Count == 0)
                return 0;
            if (this.Count == 1)
                return this[0].GetRandom(r);
            int totalLength = 0;
            foreach (Intervali i in this)
                totalLength += i.Length;
            int rand = r.Next(totalLength);
            int temp = 0;
            int c = 0;
            while (temp <= rand)
            {
                temp += this[c].Length;
                ++c;
            }
            return this[c - 1].GetRandom(r);
        }

        public override string ToString()
        {
            string ret = "";
            foreach (Intervali i in this)
            {
                if (ret == "")
                    ret = i.ToString();
                else
                    ret += ", " + i.ToString();
            }
            return "{ " + ret + " }";
        }

        public int GetClosestAngleTo(int rotation, int loop)
        {
            int closestRotation = int.MaxValue, closestDistance = int.MaxValue;
            foreach (Intervali i in this)
            {
                int rot, dist;
                i.GetClosestAngleTo(rotation, out rot, out dist, loop);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestRotation = rot;
                }
            }
            return closestRotation;
        }
    }

    public class Intervali
    {
        int min, max;
        int length;

        public int Min { get { return min; } }
        public int Max { get { return max; } }
        public int Mid { get { return (int)Math.Round(((double)max + (double)min) * 0.5); } }
        public int Length { get { return length; } }

        public Intervali(int min, int max)
        {
            if (min > max)
                throw new Exception("Minimum cannot exceed maximum in interval!");
            this.min = min;
            this.max = max;
            this.length = max - min;
        }

        public Intervali(Intervali interval)
        {
            this.min = interval.min;
            this.max = interval.max;
            this.length = interval.length;
        }

        public static Intervali operator +(Intervali interv, int move)
        {
            return new Intervali(interv.min + move, interv.max + move);
        }

        internal bool Overlaps(Intervali interval)
        {
            return max > interval.min && min < interval.max;
        }

        public int GetRandom(Random rand)
        {
            return rand.Next(max - min + 1) + min;
        }

        public void Add(Intervali interval)
        {
            this.min = Math.Min(min, interval.min);
            this.max = Math.Max(max, interval.max);
            length = this.max - this.min;
        }

        internal List<Intervali> Remove(Intervali interval)
        {
            List<Intervali> ret = new List<Intervali>();
            if (interval.max < min || interval.min > max)
                return ret;
            Intervali newInt = new Intervali(min, interval.min);
            if (newInt.Length > 0)
                ret.Add(newInt);
            newInt = new Intervali(interval.max, max);
            if (newInt.Length > 0)
                ret.Add(newInt);
            return ret;
        }
        public override string  ToString()
        {
            return "[" + min + ", " + max + "]";
        }

        public void SetMinBetween0AndMax(int max)
        {
            while (this.min < 0)
            {
                this.min += max;
                this.max += max;
            }
            while (this.min >= max)
            {
                this.min -= max;
                this.max -= max;
            }
        }

        public void Move(int move)
        {
            min += move;
            max += move;
        }

        public bool InInterval(int val)
        {
            return val >= min && val <= max;
        }

        public Intervali Merge(Intervali interval)
        {
            if (interval.max >= this.min && this.max >= interval.min)
                return new Intervali(Math.Max(this.min, interval.min), Math.Min(this.max, interval.max));
            return null;
        }

        internal Tuple<IntervaliList, IntervaliList> Cut(Intervali i)
        {
            IntervaliList free = new IntervaliList();
            IntervaliList overlapped = new IntervaliList();
            if (min < i.min)
                free.Add(new Intervali(min, i.min));
            if (max > i.max)
                free.Add(new Intervali(i.max, max));
            overlapped.Add(Merge(i));
            return new Tuple<IntervaliList, IntervaliList>(free, overlapped);
        }

        internal void GetClosestAngleTo(int rotation, out int rot, out int dist, int loop)
        {
            if (InInterval(rotation))
            {
                rot = rotation;
                dist = 0;
            }
            else
            {
                int rotMinDist, rotMaxDist;
                if (rotation < min)
                {
                    rotMinDist = min - rotation;
                    rotMaxDist = rotation + loop - max;
                }
                else
                {
                    rotMinDist = min - (rotation - loop);
                    rotMaxDist = rotation - max;
                }
                if (rotMinDist < rotMaxDist)
                {
                    rot = min;
                    dist = rotMinDist;
                }
                else
                {
                    rot = max;
                    dist = rotMaxDist;
                }
            }
        }

        internal int GetClosestValueTo(int p)
        {
            if (InInterval(p))
                return p;
            if (p < min)
                return min;
            return max;
        }
    }

    public class Interval
    {
        float min, max;
        float length;

        public float Min
        {
            get { return min; }
            set { min = value; length = this.max - this.min; }
        }

        public float Max
        {
            get { return max; }
            set { max = value; length = this.max - this.min; }
        }

        public float Mid
        {
            get { return min + 0.5f * (max - min); }
        }

        public float Length
        {
            get { return length; }
        }

        public Interval(float min, float max)
        {
            this.min = min;
            this.max = max;
            length = this.max - this.min;
        }

        public Interval(Interval copy)
        {
            this.min = copy.min;
            this.max = copy.max;
            length = this.max - this.min;
        }

        public bool InInterval(float val)
        {
            return val >= min && val <= max;
        }

        public bool Overlaps(Interval interval)
        {
            return max > interval.min && min < interval.max;
        }

        public void Add(Interval interval)
        {
            this.min = Math.Min(min, interval.min);
            this.max = Math.Max(max, interval.max);
            length = this.max - this.min;
        }

        public List<Interval> Remove(Interval interval)
        {
            List<Interval> ret = new List<Interval>();
            if (interval.max < min || interval.min > max)
                return ret;
            Interval newInt = new Interval(min, interval.min);
            if (newInt.Length > 0)
                ret.Add(newInt);
            newInt = new Interval(interval.max, max);
            if (newInt.Length > 0)
                ret.Add(newInt);
            return ret;
        }

        public float GetRandom(Random rand)
        {
            return (float)rand.NextDouble() * (max - min) + min;
        }

        public override string ToString()
        {
            return "[" + min + ", " + max + "]";
        }

        public Interval Merge(Interval interval)
        {
            if (interval.max >= this.min && this.max >= interval.min)
                return new Interval(Math.Max(this.min, interval.min), Math.Min(this.max, interval.max));
            return null;
        }

        public void SetMinBetween0And2Pi()
        {
            float pi2 = (float)Math.PI * 2;
            while (min < 0)
            {
                min += pi2;
                max += pi2;
            }
            while (min >= pi2)
            {
                min -= pi2;
                max -= pi2;
            }
        }

        public void Move(float p)
        {
            min += p;
            max += p;
        }

        public void Save(System.IO.BinaryWriter w)
        {
            w.Write((double)this.length);
            w.Write((double)this.max);
            w.Write((double)this.min);
        }

        public Interval(BinaryReader r)
        {
            this.length = (float)r.ReadDouble();
            this.max = (float)r.ReadDouble();
            this.min = (float)r.ReadDouble();
        }

        public Interval Intersect(Interval interval)
        {
            float newMin = Math.Max(min, interval.min);
            float newMax = Math.Min(max, interval.max);
            if (newMin <= newMax)
                return new Interval(newMin, newMax);
            return null;
        }
    }
}
