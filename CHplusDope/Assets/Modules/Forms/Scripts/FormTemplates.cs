using Forms.Types;
using UnityEngine;

namespace Forms
{
    public class FormTemplates : MonoBehaviour
    {
        [Header("EntryTypes")]
        public HeaderEntryController headerEntryTemplate = null;
        public ArrayEntryController arrayEntryTemplate = null;
        public BoolEntryController boolEntryTemplate = null;
        public MultiSelectEntryController multiSelectEntryTemplate = null;
        public FloatEntryController _floatEntryTemplate = null;
        public SliderEntryController sliderEntryTemplate = null;
        public SectionEntryController sectionEntryTemplate = null;
        public SelectEntryController selectEntryTemplate = null;
        public StringEntryController stringEntryTemplate = null;
    }
}