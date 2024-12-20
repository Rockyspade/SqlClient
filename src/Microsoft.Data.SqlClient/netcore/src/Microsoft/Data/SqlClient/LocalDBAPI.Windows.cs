// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Data.SqlClient;
using Interop.Windows.Sni;

namespace Microsoft.Data
{
    internal static partial class LocalDBAPI
    {
        private static IntPtr LoadProcAddress() => SafeNativeMethods.GetProcAddress(UserInstanceDLLHandle, "LocalDBFormatMessage");

        private static IntPtr UserInstanceDLLHandle
        {
            get
            {
                if (s_userInstanceDLLHandle == IntPtr.Zero)
                {
                    lock (s_dllLock)
                    {
                        if (s_userInstanceDLLHandle == IntPtr.Zero)
                        {
                            SNINativeMethodWrapper.SNIQueryInfo(QueryType.SNI_QUERY_LOCALDB_HMODULE, ref s_userInstanceDLLHandle);
                            if (s_userInstanceDLLHandle != IntPtr.Zero)
                            {
                                SqlClientEventSource.Log.TryTraceEvent("LocalDBAPI.UserInstanceDLLHandle | LocalDB - handle obtained");
                            }
                            else
                            {
                                SNINativeMethodWrapper.SNIGetLastError(out SniError sniError);
                                throw CreateLocalDBException(StringsHelper.GetString("LocalDB_FailedGetDLLHandle"), sniError.sniError);
                            }
                        }
                    }
                }
                return s_userInstanceDLLHandle;
            }
        }
    }
}
