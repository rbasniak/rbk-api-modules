using rbkApiModules.Commons.Core;

namespace rbkApiModules.Faqs.Core;

public class Faq : TenantEntity
{
    protected Faq()
    {

    }

    public Faq(string tenant, string tag, string question, string answer)
    {
        TenantId = tenant;
        Tag = tag;
        Update(question, answer);
    }

    public virtual string Tag { get; private set; }
    public virtual string Question { get; private set; }
    public virtual string Answer { get; private set; }

    public void Update(string question, string answer)
    {
        Question = question;
        Answer = answer;
    }
}
