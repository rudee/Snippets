using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Caching;

namespace Snippets.Repository
{
    /// <summary>
    /// Manages data access for all PolicyVersion related data (VW_CIR_POLICY_VERSION).
    /// </summary>
    public class PolicyVersionRepository : BaseRepository<PolicyVersion>, IPolicyVersionRepository
    {
        #region Public constructors

        public PolicyVersionRepository(ObjectCache cache,
                                       bool        cacheEnabled,
                                       TimeSpan    cacheAbsoluteExpiration)
            : base(cache,
                   cacheEnabled,
                   cacheAbsoluteExpiration)
        {
        }

        #endregion

        #region Public Methods

        public PolicyVersion GetPolicyVersionByPolicyVersionId(int                                       policyVersionId,
                                                               Expression<Func<PolicyVersion, object>>[] includePaths,
                                                               bool                                      refreshCache)
        {
            PolicyVersion policyVersion;

            // Get from cache
            if (!refreshCache
                && TryGetCache(out policyVersion,
                               policyVersionId,
                               includePaths))
            {
                return policyVersion;
            }

            // Get from database
            policyVersion = GetPolicyVersionByPolicyVersionId(policyVersionId,
                                                              refreshCache);

            // Include navigational properties
            if (includePaths != null)
            {
                // No navigational properties
                throw new NotSupportedException();
            }

            // Insert into cache
            TrySetCache(policyVersion,
                        policyVersionId,
                        includePaths);

            return policyVersion;
        }

        public IEnumerable<PolicyVersion> GetPolicyVersionsByPolicyId(int                                       policyId,
                                                                      Expression<Func<PolicyVersion, object>>[] includePaths,
                                                                      bool                                      refreshCache)
        {
            IEnumerable<PolicyVersion> policyVersions;

            // Get from cache
            if (!refreshCache
                && TryGetCache(out policyVersions,
                               policyId,
                               includePaths))
            {
                return policyVersions;
            }

            // Get from database
            policyVersions = GetPolicyVersionsByPolicyId(policyId,
                                                         refreshCache)
                                 .ToList();

            // Include navigational properties
            if (includePaths != null)
            {
                // No navigational properties
                throw new NotImplementedException();
            }

            // Insert into cache
            TrySetCache(policyVersions,
                        policyId,
                        includePaths);

            return policyVersions;
        }

        public IEnumerable<PolicyVersion> GetPolicyVersionsByClientId(int                                       clientId,
                                                                      Expression<Func<PolicyVersion, object>>[] includePaths,
                                                                      bool                                      refreshCache)
        {
            IEnumerable<PolicyVersion> policyVersions;

            // Get from cache
            if (!refreshCache
                && TryGetCache(out policyVersions,
                               clientId,
                               includePaths))
            {
                return policyVersions;
            }

            // Get from database
            policyVersions = GetPolicyVersionsByClientId(clientId,
                                                         refreshCache)
                                 .ToList();

            // Include navigational properties
            if (includePaths != null)
            {
                throw new NotImplementedException();
            }

            // Insert into cache
            TrySetCache(policyVersions,
                        clientId,
                        includePaths);

            return policyVersions;
        }

        #endregion

        #region Private methods

        private PolicyVersion GetPolicyVersionByPolicyVersionId(int  policyVersionId,
                                                                bool refreshCache)
        {
            PolicyVersion policyVersion;

            // Get from cache
            if (!refreshCache
                && TryGetCache(out policyVersion,
                               policyVersionId))
            {
                return policyVersion;
            }

            // Insert code here to get from database
            policyVersion = new PolicyVersion()
                                {
                                    PolicyVersionId = policyVersionId
                                };

            // Insert into cache
            TrySetCache(policyVersion,
                        policyVersionId);

            return policyVersion;
        }

        private IEnumerable<PolicyVersion> GetPolicyVersionsByPolicyId(int  policyId,
                                                                       bool refreshCache)
        {
            IEnumerable<PolicyVersion> policyVersions;

            // Get from cache
            if (!refreshCache
                && TryGetCache(out policyVersions,
                               policyId))
            {
                return policyVersions;
            }

            // Insert code here to get from database
            policyVersions = new []
                                 {
                                     new PolicyVersion()
                                         {
                                             PolicyVersionId = 1
                                         },
                                     new PolicyVersion()
                                         {
                                             PolicyVersionId = 2
                                         },
                                     new PolicyVersion()
                                         {
                                             PolicyVersionId = 3
                                         }
                                 };

            // Insert into cache
            TrySetCache(policyVersions,
                        policyId);

            return policyVersions;
        }

        private IEnumerable<PolicyVersion> GetPolicyVersionsByClientId(int  clientId,
                                                                       bool refreshCache)
        {
            IEnumerable<PolicyVersion> policyVersions;

            // Get from cache
            if (!refreshCache
                && TryGetCache(out policyVersions,
                               clientId))
            {
                return policyVersions;
            }

            // Insert code here to get from database
            policyVersions = new[]
                                {
                                    new PolicyVersion()
                                        {
                                            PolicyVersionId = 1
                                        },
                                    new PolicyVersion()
                                        {
                                            PolicyVersionId = 2
                                        },
                                    new PolicyVersion()
                                        {
                                            PolicyVersionId = 3
                                        }
                                };

            // Insert into cache
            TrySetCache(policyVersions,
                        clientId);

            return policyVersions;
        }

        #endregion
    }
}
