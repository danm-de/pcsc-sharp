using System;
using System.Diagnostics;
using PCSC;
using PCSC.Iso7816;

namespace Mifare1kTest
{
    public class MifareCard
    {
        private readonly IIsoReader _isoReader;

        public MifareCard(IIsoReader isoReader) {
            if (isoReader == null) {
                throw new ArgumentNullException(nameof(isoReader));
            }
            _isoReader = isoReader;
        }

        public bool LoadKey(KeyStructure keyStructure, int keyNumber, byte[] key) {
            unchecked {

                var loadKeyCmd = new CommandApdu(IsoCase.Case3Short, SCardProtocol.Any) {
                    CLA = 0xFF,
                    Instruction = InstructionCode.ExternalAuthenticate,
                    P1 = (byte) keyStructure,
                    P2 = (byte) keyNumber,
                    Data = key
                };

                Debug.WriteLine("Load Authentication Keys: {0}", BitConverter.ToString(loadKeyCmd.ToArray()));
                var response = _isoReader.Transmit(loadKeyCmd);
                Debug.WriteLine("SW1 SW2 = {0:X2} {1:X2}", response.SW1, response.SW2);

                return Success(response);
            }
        }

        private static bool Success(Response response) {
            unchecked {
                return (response.SW1 == (byte) SW1Code.Normal) && (response.SW2 == 0x00);
            }
        }

        public bool Authenticate(int msb, int lsb, KeyType keyType, int keyNumber) {
            unchecked {
                var authBlock = new GeneralAuthenticate {
                    MSB = (byte)msb,
                    LSB = (byte)lsb,
                    KeyNumber = (byte)keyNumber,
                    KeyType = keyType
                };

                var authKeyCmd = new CommandApdu(IsoCase.Case3Short, SCardProtocol.Any) {
                    CLA = 0xFF,
                    Instruction = InstructionCode.InternalAuthenticate,
                    P1 = 0x00,
                    P2 = 0x00,
                    Data = authBlock.ToArray()
                };

                Debug.WriteLine("General Authenticate: {0}", BitConverter.ToString(authKeyCmd.ToArray()));
                var response = _isoReader.Transmit(authKeyCmd);
                Debug.WriteLine("SW1 SW2 = {0:X2} {1:X2}", response.SW1, response.SW2);

                return (response.SW1 == 0x90) && (response.SW2 == 0x00);
            }
        }

        public byte[] ReadBinary(int msb, int lsb, int size) {
            unchecked {
                var readBinaryCmd = new CommandApdu(IsoCase.Case2Short, SCardProtocol.Any) {
                    CLA = 0xFF,
                    Instruction = InstructionCode.ReadBinary,
                    P1 = (byte) msb,
                    P2 = (byte) lsb,
                    Le = size
                };

                Debug.WriteLine("Read Binary (before update): {0}", BitConverter.ToString(readBinaryCmd.ToArray()));
                var response = _isoReader.Transmit(readBinaryCmd);
                Debug.WriteLine("SW1 SW2 = {0:X2} {1:X2} Data: {2}",
                    response.SW1,
                    response.SW2,
                    BitConverter.ToString(response.GetData()));

                return Success(response)
                    ? response.GetData() ?? new byte[0]
                    : null;
            }
        }

        public bool UpdateBinary(int msb, int lsb, byte[] data) {
            unchecked {
                var updateBinaryCmd = new CommandApdu(IsoCase.Case3Short, SCardProtocol.Any) {
                    CLA = 0xFF,
                    Instruction = InstructionCode.UpdateBinary,
                    P1 = (byte)msb,
                    P2 = (byte)lsb,
                    Data = data
                };

                Debug.WriteLine("Update Binary: {0}", BitConverter.ToString(updateBinaryCmd.ToArray()));
                var response = _isoReader.Transmit(updateBinaryCmd);
                Debug.WriteLine("SW1 SW2 = {0:X2} {1:X2}", response.SW1, response.SW2);

                return Success(response);
            }
        }
    }
}