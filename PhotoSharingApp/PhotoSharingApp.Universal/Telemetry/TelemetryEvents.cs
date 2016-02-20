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

namespace PhotoSharingApp.Universal.Telemetry
{
    /// <summary>
    /// Specifies telemetry event names.
    /// </summary>
    public static class TelemetryEvents
    {
        public const string AddAnnotationToPhoto = "Annotation added to photo";
        public const string AppOnLaunch = "App OnLaunched";
        public const string AppOnNotificationActivated = "App on activated from notification";
        public const string AppOnShareTargetActivated = "App OnShareTargetActivated";
        public const string BackButtonInvoked = "Back button invoked";
        public const string CategoryChooseDialogCanceled = "Category chooser dialog canceled";
        public const string CategoryChooseOk = "Category Chooser Ok";
        public const string CategoryPreviewSelected = "Category preview selected";
        public const string CreateCategoryDialogCanceled = "Create category dialog canceled";
        public const string CreateCategoryDialogOk = "Create category dialog Ok";
        public const string CreateCategoryInitiated = "Create category initiated";
        public const string CreateCategoryInvoked = "Create category invoked";
        public const string CreateCategorySuccess = "Create category success";
        public const string DeleteAnnotationCanceled = "Annotation deleted canceled";
        public const string DeleteAnnotationFailed = "Annotation deleted failed";
        public const string DeleteAnnotationInitiated = "Delete annotation initiated";
        public const string DeleteAnnotationSuccess = "Annotation deleted success";
        public const string DeletePhotoCommandInvoked = "Delete photo invoked";
        public const string DoFullfillment = "DoFulfillment";
        public const string EditCategoryInvoked = "Edit category invoked";
        public const string GiveGoldDialogCanceled = "Give gold dialog canceled";
        public const string GiveGoldDialogFailed = "Give gold dialog failed";
        public const string GiveGoldDialogSuccess = "Give gold dialog success";
        public const string GiveGoldInitiated = "Give gold initiated";
        public const string GoToCameraCommandInvoked = "Go to camera command invoked";
        public const string HeroImageSelected = "Hero image selected";
        public const string NavigationItemInvoked = "Navigation item invoked";
        public const string NextCommandInvoked = "On next invoked";
        public const string NoSelected = "No selected";
        public const string PhotoStreamItemSelected = "Photo stream item selected";
        public const string PurchaseGoldFail = "PurchaseGold failed";
        public const string PurchaseGoldInitiated = "PurchaseGold initiated";
        public const string PurchaseGoldPurchaseResultsAvailable = "PurchaseGold PurchaseResults available";
        public const string PurchaseGoldSuccess = "PurchaseGold success";
        public const string RefreshCommandInvoked = "Refresh command invoked";
        public const string ReportPhotoCommandInvoked = "Report photo invoked";
        public const string RotateClockwiseCommandInvoked = "Rotate clockwise command invoked";
        public const string SendFeedbackCommandInvoked = "Send feedback invoked";
        public const string SetProfilePhotoInvoked = "Set profile photo invoked";
        public const string ShareAppCommandInvoked = "Share app invoked";
        public const string ShowAboutCommandInvoked = "Show About invoked";
        public const string ShowCategoryChooserDialog = "Show category chooser dialog";
        public const string ShowCreateCategoryDialog = "Show create category dialog";
        public const string ShowGiveGoldDialog = "Show give gold dialog";
        public const string ShowPrivacyAndTermsInvoked = "Show Privacy and Terms invoked";
        public const string ShowSignInDialog = "Show sign-in dialog";
        public const string ShowYesNoDialog = "Show yes no dialog";
        public const string SignInCanceled = "Sign-in canceled by user";
        public const string SignInFail = "Sign-in failed";
        public const string SignInInitiated = "Sign-in initiated";
        public const string SignInSuccess = "Sign-in success";
        public const string SignOutFail = "Sign-out failed";
        public const string SignOutInvoked = "Sign-out invoked";
        public const string SignOutSuccess = "Sign-out success";
        public const string UpdatePhotoInitiated = "Update photo initiated";
        public const string UpdatePhotoSuccess = "Update photo success";
        public const string UploadPhotoInitiated = "Upload photo initiated";
        public const string UploadPhotoSuccess = "Upload photo success";
        public const string YesSelected = "Yes selected";
    }
}