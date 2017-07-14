
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
namespace Rimworld.model.io
{
    [DataContract]
    public class Templates
    {
        public Templates(DataHolder holder)
        {
            this.holder = holder;
            templates = new List<Template>();
        }
        [DataMember]
        public IList<Template> templates;


        public static Templates LoadSaved(DataHolder holder)
        {
            try
            {
                var fileStream = File.Open("saved-templates.js", FileMode.Open);
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Templates));
                Templates Templates = (Templates)ser.ReadObject(fileStream);
                fileStream.Close();
                Templates.holder = holder;
                return Templates;
            }
            catch (System.Exception e)
            {
                return null;
            }
        }

        public void SaveToFile()
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Templates));
            var fileStream = File.Create("saved-templates.js");
            ser.WriteObject(fileStream, this);
            fileStream.Close();

        }

        internal Template CreateTemplate(string name)
        {
            Template templ = new Template(name);
            templ.owner = this;
            templates.Add(templ);
            return templ;
        }

        public Template GetTemplate(string name)
        {
            return templates.Where(x => x.name == name).FirstOrDefault();
        }

        public Template GetTemplateWithTag(string tags)
        {
            IList<Template> lista = templates.Where(x => x.ContainsTags(tags)).ToList();

            if (lista.Count == 0) return null;
            return lista[Utils.Random(0, lista.Count)];

        }

        public Template GetTemplateWithTag(TagObject subComp)
        {
            return GetTemplateWithTag(subComp.tagsAsText);
        }

        public DataHolder holder { get; set; }
    }
}
