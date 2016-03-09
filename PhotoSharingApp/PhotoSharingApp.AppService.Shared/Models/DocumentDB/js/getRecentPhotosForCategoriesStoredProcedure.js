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

// Store Procedure for getting the most recent photos for each category
//   @param numberOfPhotos - The number of photos to return per category
//   @param currentDocumentVersion - The current document version number that the service is using
function getRecentPhotosForCategories(numberOfPhotos, currentDocumentVersion) {

    // Instantiate the response with an empty array.
    __.response.setBody([]);

    getAllCategories(function (allCategories) {

        for (var i = 0; i < allCategories.length; i++) {

            // Retrieve the most recent photos for this category
            // and append them to the response body
            getRecentPhotosForCategory(allCategories[i].id);
        }
    });

    function getAllCategories(callback) {

        // Perform Query using JavaScript Language Integrated Query.
        __.filter(function(doc) {
            return doc.DocumentType == "CATEGORY" && doc.DocumentVersion == currentDocumentVersion;
        }, function(err, documents) {
            if (err) {
                throw new Error("Unable to query for all categories, aborting.");
            }

            if (documents.length < 1) {
                return;
            }

            callback(documents);
        });
    }

    function getRecentPhotosForCategory(categoryId) {

        // Perform Query using chained JavaScript Language Integrated Query.
        __.chain()
        .filter(function(doc) {
                return doc.DocumentType == "PHOTO" && doc.CategoryId == categoryId && doc.DocumentVersion == currentDocumentVersion;
            })
        .sortByDescending(function (photoDoc) { return photoDoc.CreatedDateTime.Epoch })
        .value({ pageSize: numberOfPhotos }, function (err, results) {
            if (err) {
                throw new Error("Unable to query for photos in category " + categoryId + ", aborting.");
            }

            // Add the photos found for this category to the response.
            var existingPhotos = __.response.getBody();
            __.response.setBody(existingPhotos.concat(results));
        });
    }
}