
using System.Collections.Generic;
using System.Runtime.Serialization;
namespace Rimworld.model
{
    [DataContract]
    public class TagObject
    {
        [DataMember]
        public IList<string> tags { get; private set; }

        internal void AddTag(string tag)
        {
            if (tags == null)
            {
                tags = new List<string>();
            }
            if (tag == "" || tag == null) return;
            if (!tags.Contains(tag))
                tags.Add(tag);
        }

        public bool ContainsTags(string ptags)
        {
            string[] arrTags = ptags.Split(' ');
            foreach (string tag in arrTags)
            {
                if (!tags.Contains(tag)) return false;
            }
            return true;
        }

        public string tagsAsText
        {
            get
            {
                string ret = "";
                foreach (string tag in tags)
                {
                    ret += tag + " ";
                }
                return ret.Trim();
            }
        }
    }
}
