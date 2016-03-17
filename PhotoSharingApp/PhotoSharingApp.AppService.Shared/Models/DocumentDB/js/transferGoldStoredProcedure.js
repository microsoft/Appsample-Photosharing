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

// Store Procedure for Gold Transaction
//   @param goldReceiver - The id of the user that gold is being given to
//   @param goldGiver - The id of the user that is giving the gold
//   @param goldValue - The gold amount for this transaction
//   @param transactionType - The type of this transaction
//   @param photoId - The photoId connected to this transaction, if any
//   @param systemGiven - True if this transaction is being given by the system
//   @param currentDocumentVersion - The current document version number that the service is using
function transferGold(goldReceiverId, goldGiverId, goldValue, transactionType, photoId, systemGiven, currentDocumentVersion) {

    // Query for goldReceiver's JSON Document.
    getUserById(goldReceiverId, function (goldReceiverDocument) {
        if (!systemGiven) {

            // If this gold isn't coming from the system we need to query
            // for the user that is giving the gold.
            // Query for goldGiverId's JSON Document.
            getUserById(goldGiverId, function (goldGiverDocument) {

                // Change users' gold balances and create record of the transaction.
                executeTransaction(goldReceiverDocument, goldGiverDocument);
            });
        } else {

            // The system is giving gold here, so no second user is needed.
            executeTransaction(goldReceiverDocument, null);
        }
    });

    function getUserById(userId, callback) {

        // Perform Query using JavaScript Language Integrated Query.
        __.filter(function (doc) {
            return doc.id == userId && doc.DocumentVersion == currentDocumentVersion;
        }, function (err, documents) {
            if (err) {
                throw new Error("Unable to query for user " + userId + ", aborting.");
            }

            if (documents.length != 1) {
                throw new Error("Unable to find user " + userId + ", aborting.");
            }

            callback(documents[0]);
        });
    }

    function executeTransaction(toUser, fromUser) {

        // Perform the gold balance updates for the users.
        toUser.GoldBalance += goldValue;
        if (!systemGiven) {
            fromUser.GoldBalance -= goldValue;
            fromUser.GoldGiven += goldValue;
        }

        // Update toUser's Document.
        __.replaceDocument(toUser._self, toUser,
            function (err, docReplaced1) {
                if (err) {
                    throw new Error("Unable to update toUser " + toUser.id + ", aborting.");
                }

                if (!systemGiven) {

                    // Update fromUser's Document if this gold isn't coming from the system.
                    __.replaceDocument(fromUser._self, fromUser,
                        function (err2, docReplaced2) {
                            if (err2) {
                                throw new Error("Unable to update fromUser " + fromUser.id + ", aborting.");
                            }
                        });
                }

                // Whether the system is giving the gold or another user,
                // record the transaction with both ids
                recordTransaction(goldReceiverId, goldGiverId);
            });
    }

    function recordTransaction(toUserId, fromUserId) {
        var now = new Date();

        var goldTransactionDocument = {
            DocumentType: "GOLD_TRANSACTION",
            DocumentVersion: currentDocumentVersion,
            id: null,
            ToUserId: toUserId,
            FromUserId: fromUserId,
            TransactionType: transactionType,
            GoldCount: goldValue,
            PhotoId: photoId,
            CreatedDateTime: {
                Date: now.toUTCString(),
                Epoch: Math.round(now.getTime() / 1000)
            }
        };

        __.createDocument(__.getSelfLink(), goldTransactionDocument,
            function (err, documentCreated) {
                if (err) {
                    throw new Error("Unable to record the gold transaction, aborting.");
                }

                // Return the recorded gold transaction in the response.
                __.response.setBody(documentCreated);
            });
    }
}