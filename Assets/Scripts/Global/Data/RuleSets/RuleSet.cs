using System;
using System.Collections.Generic;
using UnityEngine;
using DocumentType = Doceragnum.F1Manager.DocumentType;

namespace Doceragnum
{
    [CreateAssetMenu(fileName = "RuleSet", menuName = "Phase 1/Rule Set")]
    public class RuleSet : ScriptableObject
    {
        public CharacterData[] StoryCharacters;

        public enum VisitorType { Student, Professor }
        public VisitorModel[] VisitorModels;

        [Header("Possible Error types")]
        public List<BaseErrorType> ErrorTypes = new List<BaseErrorType>();

        [Header("Requisites")]
        public BaseRequisite[] RequisitesOfToday;

        public bool HasRandomEvents;
        [Range(0, 100)] public int RandomEventProb;
    }

    [Serializable]
    public struct VisitorModel
    {
        public RuleSet.VisitorType Type;
        public DocumentType[] Documents;
    }
}