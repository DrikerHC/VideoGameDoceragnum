using System.Collections.Generic;
using UnityEngine;

namespace Doceragnum
{
    public abstract class BaseErrorType : ScriptableObject
    {
        [TextArea(2, 5)]
        public string ErrorDescription;
        public List<F1Manager.DocumentType> AppliesTo;

        public abstract void Apply(DocumentUI document);
    }
}
