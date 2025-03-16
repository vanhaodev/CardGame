using System.Collections.Generic;
using UnityEngine;

namespace Globals
{
    public class GlobalNeeder : MonoBehaviour
    {
        /// <summary>
        /// If compnent want to singleton in other gameobject, just drag ref to _outers
        /// </summary>
        [SerializeField] List<MonoBehaviour> _needers;
        public List<MonoBehaviour> GetNeeders() => _needers;
    }

}