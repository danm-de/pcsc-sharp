/*
Copyright (C) 2010
    Daniel Mueller <daniel@danm.de>

All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions
are met:

1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright
   notice, this list of conditions and the following disclaimer in the
   documentation and/or other materials provided with the distribution.
3. The name of the author may not be used to endorse or promote products
   derived from this software without specific prior written permission.

Changes to this license can be made only by the copyright author with
explicit written consent.

THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
using System;
using System.Collections.Generic;
using System.Text;

using PCSC.Iso8825.Asn1;

namespace PCSC.Iso8825
{
    public class DataObjectFactory
    {
        private static Dictionary<long, Type>  objectlist = new Dictionary<long, Type>();
        static DataObjectFactory()
        {
            // Add pre-definied Asn.1 types
            DataObjectFactory.Add<Asn1IA5String>();
            DataObjectFactory.Add<Asn1Boolean>();
            DataObjectFactory.Add<Asn1Integer>();
        }
        public static bool Add<T>() where T: DataObject, new()
        {

            Type t = typeof(T);
            object obj = Activator.CreateInstance(t);

            long tagNumber = ((DataObject)obj).Tag;
            
            if (objectlist.ContainsKey(tagNumber))
                return false;
            else
            {
                objectlist.Add(tagNumber, t);
                return true;
            }
        }
        public static bool Add(Type t) 
        {
            object obj = Activator.CreateInstance(t);
                if (obj is DataObject)
                {
                    long tagnumber = ((DataObject)obj).Tag;
                    if (!objectlist.ContainsKey(tagnumber))
                    {
                        objectlist.Add(tagnumber, t);
                        return true;
                    }
                }
                else
                    throw new ArgumentException();
            
            return false;
        }
        public static Type GetTypeByTag(long tagNumber)
        {
            return objectlist[tagNumber];
        }
        public static bool CanCreate(long tagNumber)
        {
            return objectlist.ContainsKey(tagNumber);
        }
        public static DataObject CreateInstance(long tagNumber, byte[] packet, int startIndex, int length)
        {
            if (!CanCreate(tagNumber))
                throw new ArgumentException();

            object obj = Activator.CreateInstance(
                objectlist[tagNumber],
                new object[] {
                    packet,
                    startIndex,
                    length
                });

            return (DataObject)obj;
        }
        public static T CreateInstance<T>(byte[] packet, int startIndex, int length) where T : DataObject, new()
        {
            Type t = typeof(T);
            return (T)Activator.CreateInstance(t,
                new object[] {
                    packet,
                    startIndex,
                    length
                });
        }
    }
}
