using System;
using System.Collections.Generic;
using System.Text;

namespace rbkApiModules.UIAnnotations
{
    public class FormDefinition
    {
        public FormDefinition(List<FormGroup> createInputs, List<FormGroup> updateInputs)
        {
            Create = createInputs;
            Update = updateInputs;
        }

        public List<FormGroup> Create { get; }
        public List<FormGroup> Update { get; }
    }
}
