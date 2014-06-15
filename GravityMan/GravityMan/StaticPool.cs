//----------------------------------------------------------------------------------------
//	Copyright 2009-2011 Matt Whiting, All Rights Reserved.
//  For educational purposes only.
//  Please do not distribute or republish in electronic or print form without permission.
//  Thanks - CrashLotus@gmail.com
//----------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

/// <summary>
/// StaticPool creates a static array of items.
/// T is the type of item you want a pool of.
/// The items are accessed by Handle to prevent references to dead items.
/// </summary>
/// <typeparam name="T">The type of item stored in this pool</typeparam>
public class StaticPool<T> where T : new()
{
    /// <summary>
    /// The Handle combines the index of the item with a unique ID to make sure the item is the same one the user expects it to be
    /// </summary>
    public class Handle
    {
        const UInt16 invalidId = 0xffff;                            // the id 0xffff is reserved to indicate the slot is not being used.

        static UInt16 s_nextId = invalidId;                         // this is where we'll keep track of the unique ID
        public static Handle invalidHandle = new Handle(0xffff);    // the invalid handle is used as a return value when there are no more items 
                                                                    // available in the pool

        UInt16 m_id;        // this is the unique id for this item
        UInt16 m_index;     // this is the index of this item in the pool

        public Handle()
        {
            Invalidate();
        }

        public Handle(UInt16 index)
        {
            m_id = invalidId;
            m_index = index;
        }

        /// <summary>
        /// Returns true if this handle is valid
        /// </summary>
        /// <returns>true if this handle is valid</returns>
        public bool IsValid()
        {
            return invalidId != m_id;
        }

        /// <summary>
        /// Returns the unique ID for this handle
        /// </summary>
        /// <returns>The unique ID for this handle</returns>
        public UInt16 GetId()
        {
            return m_id;
        }

        /// <summary>
        /// Returns the index of the item
        /// </summary>
        /// <returns>The index of the item</returns>
        public UInt16 GetIndex()
        {
            return m_index;
        }

        /// <summary>
        /// Give this handle a valid unique ID
        /// </summary>
        public void Validate()
        {
            m_id = NextId();
        }

        /// <summary>
        /// Mark this handle as unused by giving it the invalid ID
        /// </summary>
        public void Invalidate()
        {
            m_id = invalidId;
        }

        /// <summary>
        /// Returns the next unique ID and advances the ID's for the next time
        /// </summary>
        /// <returns>The next unique ID</returns>
        static UInt16 NextId()
        {
            ++s_nextId;
            if (invalidId == s_nextId)
                ++s_nextId;
            return s_nextId;
        }
    };


    // Constants
    const UInt16 invalidIndex = 0xffff;

    // Members of the StaticPool
    UInt16 m_poolSize = 0;      // how many elements does the pool contain (maximum of 0xffff)
    T[] m_thePool;              // the actual pool - this is the actual array of items
    Handle[] m_theHandles;      // this is the array of handles we'll use to identify the items
    Stack<UInt16> m_freeStack;  // we'll be using a stack to efficiently allocate and free items

    /// <summary>
    /// Create a StaticPool for type T with poolSize members in the pool
    /// </summary>
    /// <param name="poolSize">How many members should the pool hold</param>
    public StaticPool(UInt16 poolSize)
    {
        m_poolSize = poolSize;
        m_thePool = new T[m_poolSize];
        m_theHandles = new Handle[m_poolSize];
        m_freeStack = new Stack<UInt16>(m_poolSize);
        for (UInt16 i = 0; i < m_poolSize; ++i)
        {
            m_thePool[i] = new T();
            m_theHandles[i] = new Handle(i);
            m_freeStack.Push(i);                // start off with all items on the free stack
        }
    }

    /// <summary>
    /// Convert the handle into the matching item if there is one.
    /// Use this only temporarily.  Do not store the item.  Convert the handle to the item each time you need it.
    /// Returns null if there is no matching item.
    /// </summary>
    /// <param name="handle">The handle to the item</param>
    /// <returns>The matching item or null if no match</returns>
    public T GetItem(Handle handle)
    {
        if (handle.IsValid())
        {
            UInt16 index = handle.GetIndex();
            if (index < m_poolSize)
            {
                if (handle == m_theHandles[index])
                    return m_thePool[index];
            }
        }
        return default(T);
    }

    /// <summary>
    /// Return the handle matching the specified index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns>The handle matching index</returns>
    public Handle GetHandleByIndex(UInt16 index)
    {
        if (index < m_poolSize)
            return m_theHandles[index];
        return Handle.invalidHandle;
    }

    /// <summary>
    /// Allocate an item from the pool and return a handle to it.
    /// Returns Handle.invalidHandle if there were no free items in the pool.
    /// </summary>
    /// <returns>A handle to the allocated item.</returns>
    public Handle Allocate()
    {
        if (m_freeStack.Count > 0)
        {
            UInt16 index = m_freeStack.Pop();
            m_theHandles[index].Validate();
            return m_theHandles[index];
        }
        return Handle.invalidHandle;
    }

    /// <summary>
    /// Frees the item in the pool matching handle.
    /// </summary>
    /// <param name="handle">The handle of the item to be freed.</param>
    public void Free(Handle handle)
    {
        if (handle.IsValid())
        {
            UInt16 index = handle.GetIndex();
            if (index < m_poolSize)
            {
                if (handle == m_theHandles[index])
                {
                    m_theHandles[index].Invalidate();
                    m_freeStack.Push(index);
                }
            }
        }
    }
}
