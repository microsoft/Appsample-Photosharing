//-----------------------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//
//  The MIT License (MIT)
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
// 
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
// 
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//  ---------------------------------------------------------------------------------

using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.ApplicationInsights;
using Microsoft.Azure.Mobile.Server.Config;
using PhotoSharingApp.AppService.ServiceCore;
using PhotoSharingApp.AppService.Shared;
using PhotoSharingApp.AppService.Shared.Repositories;
using PhotoSharingApp.Portable.DataContracts;

namespace PhotoSharingApp.AppService.Controllers
{
    /// <summary>
    /// Controller to manage leaderboards.
    /// </summary>
    [MobileAppController]
    public class LeaderboardController : ApiController
    {
        private readonly IRepository _repository;
        private readonly TelemetryClient _telemetryClient;

        /// <summary>
        /// Leaderboard controller constructor.
        /// </summary>
        /// <param name="repository">The repository interface.</param>
        /// <param name="telemetryClient">The application insights telemetry client.</param>
        public LeaderboardController(IRepository repository, TelemetryClient telemetryClient)
        {
            _repository = repository;
            _telemetryClient = telemetryClient;
        }

        /// <summary>
        /// Gets client leaderboard report.
        /// </summary>
        /// <verb>GET</verb>
        /// <url>http://{host}/api/leaderboard?mostGoldCategoriesCount={mostGoldCategoriesCount}&mostGoldPhotosCount={mostGoldPhotosCount}
        /// &mostGoldUsersCount={mostGoldUsersCount}&mostGivingUsersCount={mostGivingUsersCount}</url>
        /// <param name="mostGoldCategoriesCount">Count of top categories.</param>
        /// <param name="mostGoldPhotosCount">Count of top photos.</param>
        /// <param name="mostGoldUsersCount">Count of wealthiest users.</param>
        /// <param name="mostGivingUsersCount">Count of most giving users.</param>
        /// <returns>LeaderboardContract object.</returns>
        [Route("api/leaderboard")]
        public async Task<LeaderboardContract> GetAsync([FromUri] int mostGoldCategoriesCount,
            [FromUri] int mostGoldPhotosCount,
            [FromUri] int mostGoldUsersCount,
            [FromUri] int mostGivingUsersCount)
        {
            try
            {
                _telemetryClient.TrackEvent("LeaderboardController GetAsync invoked");

                var leaderboardContract =
                    await
                        _repository.GetLeaderboard(mostGoldCategoriesCount, mostGoldPhotosCount, mostGoldUsersCount,
                            mostGivingUsersCount);

                return leaderboardContract;
            }
            catch (DataLayerException ex)
            {
                _telemetryClient.TrackException(ex);

                if (ex.Error == DataLayerError.Unknown)
                {
                    throw ServiceExceptions.UnknownInternalFailureException(ServiceExceptions.Source);
                }

                throw ServiceExceptions.DataLayerException(ex.Message);
            }
        }
    }
}