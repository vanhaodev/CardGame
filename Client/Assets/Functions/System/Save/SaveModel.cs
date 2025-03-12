
using UnityEngine;

namespace Save
{
    [System.Serializable]
    public class SaveModel
    {
        /// <summary>
        /// The name of data type
        /// </summary>
        public string DataName { get; protected set; }

        public SaveModel()
        {
            DataName = GetType().Name;
        }

        public virtual void SetDefault()
        {
        }
    }
}