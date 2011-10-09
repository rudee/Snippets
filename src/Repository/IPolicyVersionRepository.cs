using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Snippets.Repository
{
    /// <summary>
    /// Represents a repository for PolicyVersion entities.
    /// </summary>
    public interface IPolicyVersionRepository
    {
        /// <summary>
        /// Gets the PolicyVersion entity specified by <paramref name="policyVersionId"/>.
        /// </summary>
        /// <param name="policyVersionId"></param>
        /// <param name="includePaths"></param>
        /// <param name="refreshCache"></param>
        /// <returns></returns>
        PolicyVersion GetPolicyVersionByPolicyVersionId(int                                       policyVersionId,
                                                        Expression<Func<PolicyVersion, object>>[] includePaths    = null,
                                                        bool                                      refreshCache    = false);

        /// <summary>
        /// Gets all the PolicyVersion entities specified by <paramref name="policyId"/>.
        /// </summary>
        /// <param name="policyId"></param>
        /// <param name="includePaths"></param>
        /// <param name="refreshCache"></param>
        /// <returns></returns>
        IEnumerable<PolicyVersion> GetPolicyVersionsByPolicyId(int                                       policyId,
                                                               Expression<Func<PolicyVersion, object>>[] includePaths = null,
                                                               bool                                      refreshCache = false);

        /// <summary>
        /// Gets all the PolicyVersion entities specified by <paramref name="clientId"/>.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="includePaths"></param>
        /// <param name="refreshCache"></param>
        /// <returns></returns>
        IEnumerable<PolicyVersion> GetPolicyVersionsByClientId(int                                       clientId,
                                                               Expression<Func<PolicyVersion, object>>[] includePaths = null,
                                                               bool                                      refreshCache = false);
    }
}
