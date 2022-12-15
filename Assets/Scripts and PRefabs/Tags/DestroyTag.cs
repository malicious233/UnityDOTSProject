using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

//Note how we're not adding a [generateauthoringcomponent] macro to this tag, this is because this data w'll at at runtime, not authoring time.
public struct DestroyTag : IComponentData
{

}
