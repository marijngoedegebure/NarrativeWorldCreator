/**************************************************************************
 * 
 * CustomTuple.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2009-2012
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

namespace Common
{

    #region Class: CustomTuple
    /// <summary>
    /// A tuple.
    /// </summary>
    public static class CustomTuple
    {

        #region Method: Create<T>(T item1)
        /// <summary>
        /// Create a tuple with one element.
        /// </summary>
        /// <typeparam name="T">The type of the item1 element.</typeparam>
        /// <param name="item1">The item1 element.</param>
        /// <returns>The tuple.</returns>
        public static CustomTuple<T> Create<T>(T item1)
        {
            return new CustomTuple<T>(item1);
        }
        #endregion Method: Create<T>(T item1)

        #region Method: Create<T, T2>(T item1, T2 item2)
        /// <summary>
        /// Create a tuple with two elements.
        /// </summary>
        /// <typeparam name="T">The type of the item1 element.</typeparam>
        /// <typeparam name="T2">The type of the item2 element.</typeparam>
        /// <param name="item1">The item1 element.</param>
        /// <param name="item2">The item2 element.</param>
        /// <returns>The tuple.</returns>
        public static CustomTuple<T, T2> Create<T, T2>(T item1, T2 item2)
        {
            return new CustomTuple<T, T2>(item1, item2);
        }
        #endregion Method: Create<T, T2>(T item1, T2 item2)

        #region Method: Create<T, T2, T3>(T item1, T2 item2, T3 item3)
        /// <summary>
        /// Create a tuple with three elements.
        /// </summary>
        /// <typeparam name="T">The type of the item1 element.</typeparam>
        /// <typeparam name="T2">The type of the item2 element.</typeparam>
        /// <typeparam name="T3">The type of the item3 element.</typeparam>
        /// <param name="item1">The item1 element.</param>
        /// <param name="item2">The item2 element.</param>
        /// <param name="item3">The item3 element.</param>
        /// <returns>The tuple.</returns>
        public static CustomTuple<T, T2, T3> Create<T, T2, T3>(T item1, T2 item2, T3 item3)
        {
            return new CustomTuple<T, T2, T3>(item1, item2, item3);
        }
        #endregion Method: Create<T, T2, T3>(T item1, T2 item2, T3 item3)

        #region Method: Create<T, T2, T3, T4>(T item1, T2 item2, T3 item3, T4 item4)
        /// <summary>
        /// Create a tuple with four elements.
        /// </summary>
        /// <typeparam name="T">The type of the item1 element.</typeparam>
        /// <typeparam name="T2">The type of the item2 element.</typeparam>
        /// <typeparam name="T3">The type of the item3 element.</typeparam>
        /// <typeparam name="T4">The type of the item4 element.</typeparam>
        /// <param name="item1">The item1 element.</param>
        /// <param name="item2">The item2 element.</param>
        /// <param name="item3">The item3 element.</param>
        /// <param name="item4">The item4 element.</param>
        /// <returns>The tuple.</returns>
        public static CustomTuple<T, T2, T3, T4> Create<T, T2, T3, T4>(T item1, T2 item2, T3 item3, T4 item4)
        {
            return new CustomTuple<T, T2, T3, T4>(item1, item2, item3, item4);
        }
        #endregion Method: Create<T, T2, T3, T4>(T item1, T2 item2, T3 item3, T4 item4)

        #region Method: Create<T, T2, T3, T4, T5>(T item1, T2 item2, T3 item3, T4 item4, T5 item5)
        /// <summary>
        /// Create a tuple with five elements.
        /// </summary>
        /// <typeparam name="T">The type of the item1 element.</typeparam>
        /// <typeparam name="T2">The type of the item2 element.</typeparam>
        /// <typeparam name="T3">The type of the item3 element.</typeparam>
        /// <typeparam name="T4">The type of the item4 element.</typeparam>
        /// <typeparam name="T5">The type of the item5 element.</typeparam>
        /// <param name="item1">The item1 element.</param>
        /// <param name="item2">The item2 element.</param>
        /// <param name="item3">The item3 element.</param>
        /// <param name="item4">The item4 element.</param>
        /// <param name="item5">The item5 element.</param>
        /// <returns>The tuple.</returns>
        public static CustomTuple<T, T2, T3, T4, T5> Create<T, T2, T3, T4, T5>(T item1, T2 item2, T3 item3, T4 item4, T5 item5)
        {
            return new CustomTuple<T, T2, T3, T4, T5>(item1, item2, item3, item4, item5);
        }
        #endregion Method: Create<T, T2, T3, T4, T5>(T item1, T2 item2, T3 item3, T4 item4, T5 item5)

        #region Method: Create<T, T2, T3, T4, T5, T6>(T item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
        /// <summary>
        /// Create a tuple with six elements.
        /// </summary>
        /// <typeparam name="T">The type of the item1 element.</typeparam>
        /// <typeparam name="T2">The type of the item2 element.</typeparam>
        /// <typeparam name="T3">The type of the item3 element.</typeparam>
        /// <typeparam name="T4">The type of the item4 element.</typeparam>
        /// <typeparam name="T5">The type of the item5 element.</typeparam>
        /// <typeparam name="T6">The type of the item6 element.</typeparam>
        /// <param name="item1">The item1 element.</param>
        /// <param name="item2">The item2 element.</param>
        /// <param name="item3">The item3 element.</param>
        /// <param name="item4">The item4 element.</param>
        /// <param name="item5">The item5 element.</param>
        /// <param name="item6">The item6 element.</param>
        /// <returns>The tuple.</returns>
        public static CustomTuple<T, T2, T3, T4, T5, T6> Create<T, T2, T3, T4, T5, T6>(T item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
        {
            return new CustomTuple<T, T2, T3, T4, T5, T6>(item1, item2, item3, item4, item5, item6);
        }
        #endregion Method: Create<T, T2, T3, T4, T5, T6>(T item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)

        #region Method: Create<T, T2, T3, T4, T5, T6, T7>(T item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7)
        /// <summary>
        /// Create a tuple with seven elements.
        /// </summary>
        /// <typeparam name="T">The type of the item1 element.</typeparam>
        /// <typeparam name="T2">The type of the item2 element.</typeparam>
        /// <typeparam name="T3">The type of the item3 element.</typeparam>
        /// <typeparam name="T4">The type of the item4 element.</typeparam>
        /// <typeparam name="T5">The type of the item5 element.</typeparam>
        /// <typeparam name="T6">The type of the item6 element.</typeparam>
        /// <typeparam name="T7">The type of the item7 element.</typeparam>
        /// <param name="item1">The item1 element.</param>
        /// <param name="item2">The item2 element.</param>
        /// <param name="item3">The item3 element.</param>
        /// <param name="item4">The item4 element.</param>
        /// <param name="item5">The item5 element.</param>
        /// <param name="item6">The item6 element.</param>
        /// <param name="item7">The item7 element.</param>
        /// <returns>The tuple.</returns>
        public static CustomTuple<T, T2, T3, T4, T5, T6, T7> Create<T, T2, T3, T4, T5, T6, T7>(T item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7)
        {
            return new CustomTuple<T, T2, T3, T4, T5, T6, T7>(item1, item2, item3, item4, item5, item6, item7);
        }
        #endregion Method: Create<T, T2, T3, T4, T5, T6, T7>(T item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7)

        #region Method: Create<T, T2, T3, T4, T5, T6, T7, T8>(T item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8)
        /// <summary>
        /// Create a tuple with item8 elements.
        /// </summary>
        /// <typeparam name="T">The type of the item1 element.</typeparam>
        /// <typeparam name="T2">The type of the item2 element.</typeparam>
        /// <typeparam name="T3">The type of the item3 element.</typeparam>
        /// <typeparam name="T4">The type of the item4 element.</typeparam>
        /// <typeparam name="T5">The type of the item5 element.</typeparam>
        /// <typeparam name="T6">The type of the item6 element.</typeparam>
        /// <typeparam name="T7">The type of the item7 element.</typeparam>
        /// <typeparam name="T8">The type of the item8 element.</typeparam>
        /// <param name="item1">The item1 element.</param>
        /// <param name="item2">The item2 element.</param>
        /// <param name="item3">The item3 element.</param>
        /// <param name="item4">The item4 element.</param>
        /// <param name="item5">The item5 element.</param>
        /// <param name="item6">The item6 element.</param>
        /// <param name="item7">The item7 element.</param>
        /// <param name="item8">The item8 element.</param>
        /// <returns>The tuple.</returns>
        public static CustomTuple<T, T2, T3, T4, T5, T6, T7, T8> Create<T, T2, T3, T4, T5, T6, T7, T8>(T item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8)
        {
            return new CustomTuple<T, T2, T3, T4, T5, T6, T7, T8>(item1, item2, item3, item4, item5, item6, item7, item8);
        }
        #endregion Method: Create<T, T2, T3, T4, T5, T6, T7, T8>(T item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8)

    }
    #endregion Class: CustomTuple

    #region Class: CustomTuple<T>
    /// <summary>
    /// A tuple with one element.
    /// </summary>
    public class CustomTuple<T> {

        private T item1;
        
        /// <summary>
        /// The item1 element of the tuple.
        /// </summary>
        public T Item1
        {
            get { return item1; }
            set { item1 = value; }
        }

        /// <summary>
        /// A tuple with one element.
        /// </summary>
        public CustomTuple(T item1)
        {
            this.item1 = item1;
        }
    }
    #endregion Class: CustomTuple<T>

    #region Class: CustomTuple<T, T2>
    /// <summary>
    /// A tuple with two elements.
    /// </summary>
    public class CustomTuple<T, T2> : CustomTuple<T>
    {
        private T2 item2;

        /// <summary>
        /// The item2 element of the tuple.
        /// </summary>
        public T2 Item2 {
            get { return item2; }
            set { item2 = value; }
        }
        /// <summary>
        /// A tuple with two elements.
        /// </summary>
        public CustomTuple(T item1, T2 item2)
            : base(item1)
        {
            this.item2 = item2;
        }
    }
    #endregion Class: CustomTuple<T, T2>

    #region Class: CustomTuple<T, T2, T3>
    /// <summary>
    /// A tuple with three elements.
    /// </summary>
    public class CustomTuple<T, T2, T3> : CustomTuple<T, T2>
    {
        private T3 item3;

        /// <summary>
        /// The item3 element of the tuple.
        /// </summary>
        public T3 Item3
        {
            get { return item3; }
            set { item3 = value; }
        }

        /// <summary>
        /// A tuple with three elements.
        /// </summary>
        public CustomTuple(T item1, T2 item2, T3 item3)
            : base(item1, item2)
        {
            this.item3 = item3;
        }
    }
    #endregion Class: CustomTuple<T, T2, T3>

    #region Class: CustomTuple<T, T2, T3, T4>
    /// <summary>
    /// A tuple with four elements.
    /// </summary>
    public class CustomTuple<T, T2, T3, T4> : CustomTuple<T, T2, T3>
    {
        private T4 item4;

        /// <summary>
        /// The item4 element of the tuple.
        /// </summary>
        public T4 Item4
        {
            get { return item4; }
            set { item4 = value; }
        }
        /// <summary>
        /// A tuple with four elements.
        /// </summary>
        public CustomTuple(T item1, T2 item2, T3 item3, T4 item4)
            : base(item1, item2, item3)
        {
            this.item4 = item4;
        }
    }
    #endregion Class: CustomTuple<T, T2, T3, T4>

    #region Class: CustomTuple<T, T2, T3, T4, T5>
    /// <summary>
    /// A tuple with five elements.
    /// </summary>
    public class CustomTuple<T, T2, T3, T4, T5> : CustomTuple<T, T2, T3, T4>
    {
        private T5 item5;

        /// <summary>
        /// The item5 element of the tuple.
        /// </summary>
        public T5 Item5
        {
            get { return item5; }
            set { item5 = value; }
        }

        /// <summary>
        /// A tuple with five elements.
        /// </summary>
        public CustomTuple(T item1, T2 item2, T3 item3, T4 item4, T5 item5)
            : base(item1, item2, item3, item4)
        {
            this.item5 = item5;
        }
    }
    #endregion Class: CustomTuple<T, T2, T3, T4, T5>

    #region Class: CustomTuple<T, T2, T3, T4, T5, T6>
    /// <summary>
    /// A tuple with six elements.
    /// </summary>
    public class CustomTuple<T, T2, T3, T4, T5, T6> : CustomTuple<T, T2, T3, T4, T5>
    {
        private T6 item6;

        /// <summary>
        /// The item6 element of the tuple.
        /// </summary>
        public T6 Item6
        {
            get { return item6; }
            set { item6 = value; }
        }

        /// <summary>
        /// A tuple with six elements.
        /// </summary>
        public CustomTuple(T item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
            : base(item1, item2, item3, item4, item5)
        {
            Item6 = item6;
        }

    }
    #endregion Class: CustomTuple<T, T2, T3, T4, T5, T6>

    #region Class: CustomTuple<T, T2, T3, T4, T5, T6, T7>
    /// <summary>
    /// A tuple with seven elements.
    /// </summary>
    public class CustomTuple<T, T2, T3, T4, T5, T6, T7> : CustomTuple<T, T2, T3, T4, T5, T6>
    {
        private T7 item7;

        /// <summary>
        /// The item7 element of the tuple.
        /// </summary>
        public T7 Item7
        {
            get { return item7; }
            set { item7 = value; }
        }

        /// <summary>
        /// A tuple with seven elements.
        /// </summary>
        public CustomTuple(T item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7)
            : base(item1, item2, item3, item4, item5, item6)
        {
            Item7 = item7;
        }

    }
    #endregion Class: CustomTuple<T, T2, T3, T4, T5, T6, T7>

    #region Class: CustomTuple<T, T2, T3, T4, T5, T6, T7, T8>
    /// <summary>
    /// A tuple with item8 elements.
    /// </summary>
    public class CustomTuple<T, T2, T3, T4, T5, T6, T7, T8> : CustomTuple<T, T2, T3, T4, T5, T6, T7>
    {
        private T8 item8;

        /// <summary>
        /// The item8 element of the tuple.
        /// </summary>
        public T8 Item8
        {
            get { return item8; }
            set { item8 = value; }
        }

        /// <summary>
        /// A tuple with item8 elements.
        /// </summary>
        public CustomTuple(T item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8)
            : base(item1, item2, item3, item4, item5, item6, item7)
        {
            Item8 = item8;
        }

    }
    #endregion Class: CustomTuple<T, T2, T3, T4, T5, T6, T7, T8>

}