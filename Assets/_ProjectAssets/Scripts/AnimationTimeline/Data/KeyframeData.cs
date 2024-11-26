
using System;
using UnityEngine;

[Serializable]
public class KeyframeData<T>
{
    [SerializeField]
    public int frameIndex;
    [SerializeField]
    public T value;
}
