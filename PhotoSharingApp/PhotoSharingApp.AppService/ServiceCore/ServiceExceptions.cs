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

using System.Web.Http;

namespace PhotoSharingApp.AppService.ServiceCore
{
    /// <summary>
    /// Provides exceptions for the App Service.
    /// </summary>
    public static class ServiceExceptions
    {
        /// <summary>
        /// Fault source for the App Service.
        /// </summary>
        public const string Source = "PhotoSharingAppService";

        /// <summary>
        /// Returns exception when there is an exception calling data layer.
        /// </summary>
        /// <param name="details">Fault inner message.</param>
        /// <returns>Exception wrapping the fault.</returns>
        public static HttpResponseException DataLayerException(string details)
        {
            return ServiceFault.InternalServerError(
                Source,
                DataLayerError,
                "An unknown error occurred while communicating with data layer.",
                details);
        }

        /// <summary>
        /// Returns exception when a duplicate category is found during category
        /// creation.
        /// </summary>
        /// <param name="details">Fault inner message.</param>
        /// <returns>Exception wrapping the fault.</returns>
        public static HttpResponseException DuplicateCategoryException(string details)
        {
            return ServiceFault.Forbidden(
                Source,
                DuplicateCategory,
                "Category of the same name exists.",
                details);
        }

        /// <summary>
        /// Returns exception when the Iap validation fails.
        /// </summary>
        /// <param name="details">Fault message.</param>
        /// <returns>Exception wrapping the fault.</returns>
        public static HttpResponseException IapValidationException(string details)
        {
            return ServiceFault.BadRequest(
                Source,
                IapValidationError,
                "Iap could not be validated.",
                details);
        }

        /// <summary>
        /// Returns an exception wrapping a fault response indicating that the Uri Id and the request body Id differ.
        /// </summary>
        /// <param name="pathArgumentId">Id from the Uri path argument.</param>
        /// <param name="requestId">Id from the request body.</param>
        /// <returns>Exception wrapping the fault response.</returns>
        public static HttpResponseException IdMismatchException(string pathArgumentId, string requestId)
        {
            return ServiceFault.BadRequest(
                Source,
                IdMismatch,
                "The ID value present in the URI differs from the one present in the request body.",
                pathArgumentId,
                requestId);
        }

        /// <summary>
        /// Returns exception when the current user isn't allowed to perform this operation.
        /// </summary>
        /// <returns>Exception wrapping the fault.</returns>
        public static HttpResponseException NotAllowed()
        {
            return ServiceFault.Forbidden(
                Source,
                IncorrectUser,
                "The logged in user does not have access to perform this operation.",
                null);
        }

        /// <summary>
        /// Returns an exception wrapping a fault response indicating that an unknown internal failure occurred.
        /// </summary>
        /// <param name="faultSource">Source to use when creating ServiceFaults.</param>
        /// <returns>Exception wrapping the fault response.</returns>
        public static HttpResponseException UnknownInternalFailureException(string faultSource)
        {
            return ServiceFault.InternalServerError(
                faultSource,
                UnknownInternalFailure,
                "The service has encountered an unknown internal server error.");
        }

        /// <summary>
        /// Returns exception when the current operation will overdraw user balance.
        /// </summary>
        /// <returns>Exception wrapping the fault.</returns>
        public static HttpResponseException UserBalanceTooLow()
        {
            return ServiceFault.Forbidden(
                Source,
                UserBalanceTooLowForOperation,
                "User's balance is too low for this operation.",
                null);
        }

        /// <summary>
        /// Returns exception when the current authenticated user fetched from Azure Mobile Service is null.
        /// </summary>
        /// <returns>Exception wrapping the fault.</returns>
        public static HttpResponseException UserNullException()
        {
            return ServiceFault.Forbidden(
                Source,
                UserNullError,
                "This can only be performed by signed in clients.",
                null);
        }

        #region 2000-2999: Bad Request

        /// <summary errorCategory="2000-2999: Bad Request">
        /// The ID value present in the URI differs from the one present in the request body.
        /// </summary>
        public const ushort IdMismatch = 2000;

        /// <summary errorCategory="2000-2999: Iap validation Error" httpStatusCode="400">
        /// Something is wrong with the Iap purchase receipt.
        /// </summary>
        public const ushort IapValidationError = 2500;

        #endregion

        #region 3000-3999: Forbidden Request

        /// <summary errorCategory="3000-3999: Forbidden" httpStatusCode="403">
        /// Authentication failed.
        /// </summary>
        public const ushort UserNullError = 3000;

        /// <summary errorCategory="3000-3999: Invalid action.  Photo not owned by user." httpStatusCode="403">
        /// Photo not owned by user.
        /// </summary>
        public const ushort IncorrectUser = 3500;

        /// <summary errorCategory="3000-3999: Category of same name exists." httpStatusCode="403">
        /// User balance too low to support this operation.
        /// </summary>
        public const ushort DuplicateCategory = 3750;

        /// <summary errorCategory="3000-3999: Can't overdraw user balance" httpStatusCode="403">
        /// User balance too low to support this operation.
        /// </summary>
        public const ushort UserBalanceTooLowForOperation = 3999;

        #endregion

        #region 6000-6999: Internal Server Error

        /// <summary errorCategory="6000-6999: Internal Server Error" httpStatusCode="500">
        /// An unknown error occurred while communicating with Data access layer.
        /// </summary>
        public const ushort DataLayerError = 6000;

        /// <summary errorCategory="6000-6999: Standard Internal Server Error" httpStatusCode="500">
        /// The service has encountered an unknown internal server error.
        /// </summary>
        public const ushort UnknownInternalFailure = 6001;

        #endregion
    }
}