using System.Collections.Generic;

namespace rbkApiModules.UIAnnotations
{
    public class FormGroup
    {
        public FormGroup()
        {
            Controls = new List<InputControl>();
        }

        public string Name { get; set; }

        public List<InputControl> Controls { get; set; }
    }
}
