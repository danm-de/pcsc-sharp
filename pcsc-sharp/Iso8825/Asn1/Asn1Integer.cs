using System;
using System.Collections.Generic;
using System.Text;

namespace PCSC.Iso8825.Asn1
{
    public class Asn1Integer:DataObject, IAsn1Type
    {
        /*
        private LinkedList<uint> intlst = new LinkedList<uint>();
         */

        // Needed for DataObjectFactory
        public Asn1Integer() { }
        public Asn1Integer(byte[] packet, int startIndex, int length)
            : base(packet, startIndex, length)
        {
            /*
            intlst = BuildLinkedList(packet, startIndex, length);   
             */
        }

        /*
        private LinkedList<uint> BuildLinkedList(byte[] packet, int startIndex, int length)
        {
            
            LinkedList<uint> lst = new LinkedList<uint>();

            if (length > 0)
            {
                // Is the supplied Integer negative?
                bool isNegative = ((packet[startIndex] & (1 << 7)) == (1 << 7)) ? true : false;
                int endIndex = startIndex + length - 1;
                
                int intsize = sizeof(uint);
                int intsizecnt = 0;

                for (int i = endIndex; i <= endIndex; i++)
                {

                }
            }

            return lst;
        }
         */

        public bool IsPositive
        {
            get 
            {
                if (data == null || data.Length == 0)
                    return false;
                if ((data[startIndex] & (1 << 7)) == (1 << 7)) 
                    return false; // Sign bit set -> number is in two's complement
                else
                    return true;
            }
        }

        public long ToLong()
        {
            if (length == 0)
                return 0;

            bool IsIntArrayPositive = IsPositive;
            bool convValuePositive;

            long tmp = 0;

            int octetcounter = EndsAt;
            int intSize = 0;

            while (octetcounter >= startIndex)
            {
                // shift & set
                int shiftfactor = (8 * intSize);
                tmp |= ((long)data[octetcounter] << shiftfactor);
                
                // current size of the Integer in bytes
                intSize++;

                // decrease the octet counter in order to address the next byte
                octetcounter--;

                if (intSize == sizeof(long))
                    break; // we reached the maximum size of long
            }
            
            // Is the current (converted) temporary long value positive or negative?
            if (tmp >= 0)
                convValuePositive = true;
            else
                convValuePositive = false;

            /* We did not reach the "beginning" of the ASN.1 Integer
             * Check if we have an arithmetic overflow.
             */
            while (octetcounter >= startIndex)
            {
                if (convValuePositive == true)
                {
                    if (data[octetcounter] != 0)
                        throw new OverflowException();
                }
                else
                {
                    if (data[octetcounter] != 255)
                        throw new OverflowException();
                }
                octetcounter--;
            }

            /* If the original ASN.1 Integer is negative we must add the 
             * missing 0xFF bytes in order to have a correct two's complement form.
             */
            if (!IsIntArrayPositive)
            {
                if (length < sizeof(long))
                {
                    int ffBytesNeeded = sizeof(long) - length;
                    int shiftfactor = (sizeof(long) * 8) - 8;
                    /* it is probably faster to run a little loop than calculating 
                     * the following expression:
                     * tmp |= (((long)Math.Pow(2, ffBytesNeeded * 8)) - 1) << ((sizeof(long) - ffBytesNeeded) * 8);
                     */
                    
                    while (ffBytesNeeded > 0)
                    {
                        tmp |= (long)0xFF << (int)(shiftfactor);
                        
                        shiftfactor = shiftfactor - 8;
                        ffBytesNeeded--;
                    }
                    
                }
            }

            return tmp;
        }

        public override long Tag
        {
            get { return (long)GetAsn1Type(); }
        }

        #region IAsn1Type Member

        public Asn1Type GetAsn1Type()
        {
            return Asn1Type.Integer;
        }

        #endregion
    }
}
