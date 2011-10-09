using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Snippets.Repository
{
    /// <summary>
    /// Represents a repository for Client entities.
    /// </summary>
    public interface IClientRepository
    {
        /// <summary>
        /// Gets all the Client entities specified by <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="includePaths"></param>
        /// <param name="refreshCache"></param>
        /// <returns></returns>
        IEnumerable<Client> GetClients(Expression<Func<Client, object>>[] includePaths = null,
                                       bool                               refreshCache = false);

        /// <summary>
        /// Gets the Client entity specified by <paramref name="userId"/> and <paramref name="clientId"/>.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="clientId"></param>
        /// <param name="includePaths"></param>
        /// <param name="refreshCache"></param>
        /// <returns></returns>
        Client GetClientByClientId(int                                clientId,
                                   Expression<Func<Client, object>>[] includePaths = null,
                                   bool                               refreshCache = false);
    }
}
