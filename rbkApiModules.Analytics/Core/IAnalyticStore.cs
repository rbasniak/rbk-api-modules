using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace rbkApiModules.Analytics
{
    /// <summary>
    /// Interface que define como devem ser as implementações de novas stores
    /// </summary>
    public interface IAnalyticStore
    {
        /// <summary>
        /// Store recivied request. Internally invoked by Analytics
        /// </summary>
        /// <param name="request">Request collected by Analytics</param>
        /// <returns></returns>
        Task StoreWebRequestAsync(WebRequest request);

        /// <summary>
        /// Return unique identities that made at least a request on that day
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        Task<IEnumerable<string>> UniqueIdentitiesAsync(DateTime day);

        /// <summary>
        /// Return unique identities that made at least a request inside the gived ime interval
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        Task<IEnumerable<string>> UniqueIdentitiesAsync(DateTime from, DateTime to);

        /// <summary>
        /// Return unique identities that made at least a request on that day
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        Task<long> CountUniqueIdentitiesAsync(DateTime day);

        /// <summary>
        /// Return unique identities that made at least a request inside the gived ime interval
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        Task<long> CountUniqueIdentitiesAsync(DateTime from, DateTime to);

        /// <summary>
        /// Return the raw number of request served in the time interval
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        Task<long> CountAsync(DateTime from, DateTime to);

        /// <summary>
        /// Return distinct Ip Address served during that day
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        Task<IEnumerable<IPAddress>> IpAddressesAsync(DateTime day);

        /// <summary>
        /// Return distinct IP addresses served during given time interval
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        Task<IEnumerable<IPAddress>> IpAddressesAsync(DateTime from, DateTime to);

        /// <summary>
        /// Return any request that was served during this time range
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        Task<IEnumerable<WebRequest>> InTimeRange(DateTime from, DateTime to);

        /// <summary>
        /// Return all the request made by this identity
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        Task<IEnumerable<WebRequest>> RequestByIdentityAsync(string identity);

        /// <summary>
        /// Remove all item in request collection
        /// </summary>
        /// <returns></returns>
        Task PurgeRequestAsync();
    }
}
