// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.Cryptography;

namespace Microsoft.AspNet.DataProtection.AuthenticatedEncryption
{
    internal static class AlgorithmAssert
    {
        // Our analysis re: IV collision resistance for CBC only holds if we're working with block ciphers
        // with a block length of 64 bits or greater.
        private const uint SYMMETRIC_ALG_MIN_BLOCK_SIZE_IN_BITS = 64;

        // Min security bar: encryption algorithm must have a min 128-bit key.
        private const uint SYMMETRIC_ALG_MIN_KEY_LENGTH_IN_BITS = 128;

        // Min security bar: authentication tag must have at least 128 bits of output.
        private const uint HASH_ALG_MIN_DIGEST_LENGTH_IN_BITS = 128;

        // Since we're performing some stack allocs based on these buffers, make sure we don't explode.
        private const uint MAX_SIZE_IN_BITS = Constants.MAX_STACKALLOC_BYTES * 8;

        public static void IsAllowableSymmetricAlgorithmBlockSize(uint blockSizeInBits)
        {
            if (!IsValidCore(blockSizeInBits, SYMMETRIC_ALG_MIN_BLOCK_SIZE_IN_BITS))
            {
                throw new InvalidOperationException(Resources.FormatAlgorithmAssert_BadBlockSize(blockSizeInBits));
            }
        }

        public static void IsAllowableSymmetricAlgorithmKeySize(uint keySizeInBits)
        {
            if (!IsValidCore(keySizeInBits, SYMMETRIC_ALG_MIN_KEY_LENGTH_IN_BITS))
            {
                throw new InvalidOperationException(Resources.FormatAlgorithmAssert_BadKeySize(keySizeInBits));
            }
        }

        public static void IsAllowableValidationAlgorithmDigestSize(uint digestSizeInBits)
        {
            if (!IsValidCore(digestSizeInBits, HASH_ALG_MIN_DIGEST_LENGTH_IN_BITS))
            {
                throw new InvalidOperationException(Resources.FormatAlgorithmAssert_BadDigestSize(digestSizeInBits));
            }
        }

        private static bool IsValidCore(uint value, uint minValue)
        {
            return (value % 8 == 0) // must be whole bytes
                && (value >= minValue) // must meet our basic security requirements
                && (value <= MAX_SIZE_IN_BITS); // mustn't overflow our stack
        }
    }
}
