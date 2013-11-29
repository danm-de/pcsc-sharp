namespace Mifare1kTest
{
    public class GeneralAuthenticate
    {
        private byte _version = 0x01;

        public byte Version {
            get { return _version; }
            set { _version = value; }
        }

        public byte MSB { get; set; }
        public byte LSB { get; set; }
        public KeyType KeyType { get; set; }
        public byte KeyNumber { get; set; }

        public byte[] ToArray() {
            unchecked {
                return new[] {_version, MSB, LSB, (byte) KeyType, KeyNumber };
            }
        }
    }
}