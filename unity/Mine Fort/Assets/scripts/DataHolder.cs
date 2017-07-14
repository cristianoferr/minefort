using Rimworld.logic;

namespace Rimworld
{
    public class DataHolder
    {
        public DataHolder()
        {
            templateInitializer = new TemplateInitializer(this);
        }


        public TemplateInitializer templateInitializer { get; private set; }

        public model.io.Templates templates { get; set; }
    }
}
