using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rbkApiModules.Faqs
{
    public class Faq : BaseEntity
    {
        protected Faq()
        {

        }

        public Faq(string tag, string question, string answer)
        {
            Update(tag, question, answer);
        }

        public virtual string Tag { get; private set; }
        public virtual string Question { get; private set; }
        public virtual string Answer { get; private set; }

        public void Update(string tag, string question, string answer)
        {
            Tag = tag;
            Question = question;
            Answer = answer;
        }
    }
}
