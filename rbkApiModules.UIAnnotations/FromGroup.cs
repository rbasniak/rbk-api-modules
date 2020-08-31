using System;
using System.Collections.Generic;
using System.Text;

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
