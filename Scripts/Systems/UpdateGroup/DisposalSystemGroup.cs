using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;

namespace Reactive {

    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class DisposalGroup : ComponentSystemGroup { }
}