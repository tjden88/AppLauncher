using System.Collections.Generic;

namespace AppLauncher.Models
{
    /// <summary>
    /// Группа ссылок
    /// </summary>
    public class AppLinkGroup
    {
        public int Id { get; set; }

        public AppLink BigLink { get; set; }

        public AppLink Link1 { get; set; }

        public AppLink Link2 { get; set; }

        public AppLink Link3 { get; set; }

        public AppLink Link4 { get; set; }

        public int GroupId { get; set; }


        public List<AppLink> GetAllLinks()
        {
            var result = new List<AppLink>();
            if (Link1 != null)
                result.Add(Link1);
            if (Link2 != null)
                result.Add(Link2);
            if (Link3 != null)
                result.Add(Link3);
            if (Link4 != null)
                result.Add(Link4);

            return result;
        }

        public void Remove(AppLink link)
        {
            if(ReferenceEquals(Link1, link))
                Link1 = null;
            if (ReferenceEquals(Link2, link))
                Link2 = null;
            if (ReferenceEquals(Link3, link))
                Link3 = null; 
            if (ReferenceEquals(Link4, link))
                Link4 = null;

        }
    }
}
