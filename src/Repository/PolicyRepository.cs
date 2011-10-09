using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Caching;

namespace Snippets.Repository
{
    /// <summary>
    /// Manages data access for all Policy related data (VW_CIR_POLICY).
    /// </summary>
    public class PolicyRepository : BaseRepository<Policy>, IPolicyRepository
    {
        #region Public constructors

        public PolicyRepository(ObjectCache              cache,
                                bool                     cacheEnabled,
                                TimeSpan                 cacheAbsoluteExpiration,
                                IPolicyVersionRepository policyVersionRepository)
            : base(cache,
                   cacheEnabled,
                   cacheAbsoluteExpiration)
        {
            _policyVersionRepository = policyVersionRepository;
        }

        #endregion

        #region Public methods

        public Policy GetPolicyByPolicyId(int                                policyId,
                                          Expression<Func<Policy, object>>[] includePaths,
                                          bool                               refreshCache)
        {
            Policy policy;

            // Get from cache
            if (!refreshCache
                && TryGetCache(out policy,
                               policyId,
                               includePaths))
            {
                return policy;
            }

            // Get from database
            policy = GetPolicyByPolicyId(policyId,
                                         refreshCache);

            // Include navigational properties
            if (policy != null
                && includePaths != null)
            {
                foreach (Expression<Func<Policy, object>> includePath in includePaths.Where(p => p != null))
                {
                    // PolicyVersions
                    if (includePath.PathEquals(p => p.PolicyVersions)
                        && policy.PolicyVersions == null)
                    {
                        policy.PolicyVersions = _policyVersionRepository.GetPolicyVersionsByPolicyId(policyId,
                                                                                                     null,
                                                                                                     refreshCache);
                    }
                    else
                    {
                        throw new NotSupportedException(string.Format("The include path {0} is not supported", includePath.ToPathString()));
                    }
                }
            }

            // Insert into cache
            TrySetCache(policy,
                        policyId,
                        includePaths);

            return policy;
        }

        public IEnumerable<Policy> GetPoliciesByClientId(int                                clientId,
                                                         Expression<Func<Policy, object>>[] includePaths,
                                                         bool                               refreshCache)
        {
            IEnumerable<Policy> policies;

            // Get from cache
            if (!refreshCache
                && TryGetCache(out policies,
                               clientId,
                               includePaths))
            {
                return policies;
            }

            // Get from database
            policies = GetPoliciesByClientId(clientId,
                                             refreshCache)
                           .ToList();

            // Include navigational properties
            if (includePaths != null)
            {
                IEnumerable<PolicyVersion> policyVersions = null;

                foreach (Expression<Func<Policy, object>> includePath in includePaths.Where(p => p != null))
                {
                    // PolicyVersions
                    if (includePath.PathEquals(p => p.PolicyVersions)
                        && policyVersions == null)
                    {
                        policyVersions = _policyVersionRepository.GetPolicyVersionsByClientId(clientId,
                                                                                              null,
                                                                                              refreshCache);
                    }
                    else
                    {
                        throw new NotSupportedException(string.Format("The include path {0} is not supported", includePath.ToPathString()));
                    }
                }

                policies.ToList().ForEach(p =>
                                              {
                                                  p.PolicyVersions = policyVersions == null ? null : policyVersions.Where(pv => pv.PolicyId == p.PolicyId)
                                                                                                                   .ToList();
                                              });
            }

            // Insert into cache
            TrySetCache(policies,
                        clientId,
                        includePaths);

            return policies;
        }

        #endregion

        #region Private methods

        private Policy GetPolicyByPolicyId(int  policyId,
                                           bool refreshCache)
        {
            Policy policy;

            // Get from cache
            if (!refreshCache
                && TryGetCache(out policy,
                               policyId))
            {
                return policy;
            }

            // Insert code here to get from database
            policy = new Policy()
                         {
                             PolicyId = policyId
                         };

            // Insert into cache
            TrySetCache(policy,
                        policyId);

            return policy;
        }

        private IEnumerable<Policy> GetPoliciesByClientId(int  clientId,
                                                          bool refreshCache)
        {
            IEnumerable<Policy> policies;

            // Get from cache
            if (!refreshCache
                && TryGetCache(out policies,
                               clientId))
            {
                return policies;
            }

            // Insert code here to get from database
            policies = new[]
                          {
                              new Policy()
                                  {
                                      PolicyId = 1
                                  },
                              new Policy()
                                  {
                                      PolicyId = 2
                                  },
                              new Policy()
                                  {
                                      PolicyId = 3
                                  }
                          };

            // Insert into cache
            TrySetCache(policies,
                        clientId);

            return policies;
        }

        #endregion

        #region Private fields

        private readonly IPolicyVersionRepository _policyVersionRepository;

        #endregion
    }
}
