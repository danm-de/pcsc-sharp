using System;
using System.Collections.Generic;
using System.Text;

namespace PCSC.Iso8825.Asn1
{
    public class Asn1Boolean: DataObject, IAsn1Type
    {
        // Needed for DataObjectFactory
        public Asn1Boolean() 
            :this(false)
        { }

        public Asn1Boolean(bool value)
        {
            data = new byte[1];
            startIndex = 0;
            length = 1;
            
            if (value)
                data[0] = 0xFF;
            else
                data[0] = 0x00;
        }

        public Asn1Boolean(byte[] packet, int startIndex, int length)
            :base(packet, startIndex, length)
        {
            if (length != 1)
                throw new InvalidAsn1BooleanException("The size of an ASN.1 BOOLEAN (ISO/IEC 8825 8.2) shall consist of a single octet.");
        }

        public bool Value
        {
            get { return (data[startIndex] == 0x00) ? false : true; }
            /*
             * We shall not modify the original data!
            set
            {
                if (value)
                    data[startIndex] = 0xFF;
                else
                    data[startIndex] = 0x00;
            }
             */
        }

        #region IAsn1Type Member
        public Asn1Type GetAsn1Type()
        {
            return Asn1Type.Boolean;
        }
        #endregion

        public override long Tag
        {
            get { return (long)GetAsn1Type(); }
        }

        public static implicit operator bool(Asn1Boolean b)
        {
            return b.Value;
        }
        public static implicit operator Asn1Boolean(bool b)
        {
            return new Asn1Boolean(b);
        }
    }
}
