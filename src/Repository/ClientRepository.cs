using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Caching;

namespace Snippets.Repository
{
    /// <summary>
    /// Manages data access for all Client related data (VW_CIR_CLIENT).
    /// </summary>
    public class ClientRepository : BaseRepository<Client>, IClientRepository
    {
        #region Public constructors

        public ClientRepository(ObjectCache       cache,
                                bool              cacheEnabled,
                                TimeSpan          cacheAbsoluteExpiration,
                                IPolicyRepository policyRepository)
            : base(cache,
                   cacheEnabled,
                   cacheAbsoluteExpiration)
        {
            _policyRepository = policyRepository;
        }

        #endregion

        #region Public Methods

        public IEnumerable<Client> GetClients(Expression<Func<Client, object>>[] includePaths,
                                              bool refreshCache)
        {
            IEnumerable<Client> clients;

            // Get from cache
            if (!refreshCache
               && TryGetCache(out clients,
                              includePaths))
            {
                return clients;
            }

            // Get from database
            clients = GetClients(refreshCache);

            // Include navigational properties
            if (includePaths != null)
            {
                throw new NotSupportedException();
            }

            TrySetCache(clients,
                        includePaths);

            return clients;
        }

        public Client GetClientByClientId(int                                clientId,
                                          Expression<Func<Client, object>>[] includePaths,
                                          bool                               refreshCache)
        {
            Client client;

            // Get from cache
            if (!refreshCache && TryGetCache(out client,
                                             clientId,
                                             includePaths))
            {
                return client;
            }

            // Get from database
            client = GetClientByClientId(clientId,
                                         refreshCache);

            // Include navigational properties
            if (client != null
                && includePaths != null)
            {
                IEnumerable<Policy> policies = null;

                foreach (Expression<Func<Client, object>> includePath in includePaths.Where(ip => ip != null))
                {
                    // Policies
                    if (includePath.StartsWith(c => c.Policies)
                        && policies == null)
                    {
                        Expression<Func<Policy, object>>[] policiesIncludePath = null;

                        if (includePaths.Any(ip => ip.PathEquals(c => c.Policies.Select(p => p.PolicyVersions))))
                        {
                            policiesIncludePath = new Expression<Func<Policy, object>>[]
                                                      {
                                                          p => p.PolicyVersions
                                                      };
                        }

                        policies = _policyRepository.GetPoliciesByClientId(clientId,
                                                                           policiesIncludePath,
                                                                           refreshCache);
                    }
                    else
                    {
                        throw new NotSupportedException(string.Format("The include path {0} is not supported", includePath.ToPathString()));
                    }
                }
            }

            // Insert into cache
            TrySetCache(client,
                        clientId,
                        includePaths);

            return client;
        }

        #endregion

        #region Private methods

        private Client GetClientByClientId(int  clientId,
                                           bool refreshCache)
        {
            Client client;

            // Get from cache
            if (!refreshCache
                && TryGetCache(out client,
                               clientId))
            {
                return client;
            }

            // Insert code here to get from database
            client = new Client()
                         {
                             ClientId = clientId,
                             Name     = clientId + " name"
                         };

            // Insert into cache
            TrySetCache(client,
                        clientId);

            return client;
        }

        private IEnumerable<Client> GetClients(bool refreshCache)
        {
            IEnumerable<Client> clients;

            // Get from cache
            if (!refreshCache
                && TryGetCache(out clients))
            {
                return clients;
            }

            // Insert code here to get from database
            clients = new[]
                          {
                              new Client()
                                  {
                                      ClientId = 1,
                                      Name     = "1 name"
                                  },
                              new Client()
                                  {
                                      ClientId = 2,
                                      Name     = "2 name"
                                  },
                              new Client()
                                  {
                                      ClientId = 3,
                                      Name     = "3 name"
                                  }
                          };

            // Insert into cache
            TrySetCache(clients);

            return clients;
        }

        #endregion

        #region Private fields

        private readonly IPolicyRepository _policyRepository;

        #endregion
    }
}
