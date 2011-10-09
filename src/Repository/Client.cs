using System.Collections.Generic;

namespace Snippets.Repository
{
    public class Client : Entity
    {
        public int?                ClientId { get; set; }
        public string              Name     { get; set; }
        public IEnumerable<Policy> Policies { get; set; }
    }
}
