using System.Collections.Generic;

namespace Snippets.Repository
{
    public class Policy : Entity
    {
        public int                        PolicyId       { get; set; }
        public IEnumerable<PolicyVersion> PolicyVersions { get; set; }
    }
}
