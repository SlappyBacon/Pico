using System;
using Pico.Arrays;
namespace Pico.Networking
{
    public struct ComPacket
    {
        public static readonly ComPacket Empty = new ComPacket(null, null);
        public static ComPacket FromBytes(byte[] bytes)
        {
            if (bytes == null) return ComPacket.Empty;
            byte[] guid = new byte[GuidSize];
            byte[] body = new byte[bytes.Length - guid.Length];
            for (int i = 0; i < guid.Length; i++)
            {
                guid[i] = bytes[i];
            }
            for (int i = 0; i < body.Length; i++)
            {
                body[i] = bytes[guid.Length + i];
            }
            return new ComPacket(guid, body);
        }

        static readonly int GuidSize = 4;
        byte[] _guid = new byte[GuidSize];
        byte[] _body;

        public byte[] GUID { get { return _guid; } }
        public byte[] Body 
        {
            get { return _body; }
            set { _body = value; }
        }
        public ComPacket(byte[] body)
        {
            Random.Shared.NextBytes(_guid);
            _body = body;
        }
        public ComPacket(byte[] guid, byte[] body)
        {
            _guid = guid;
            _body = body;
        }
        public byte[] ToBytes()
        {
            var result = new byte[GUID.Length + Body.Length];
            for (int i = 0; i < GUID.Length; i++)
            {
                result[i] = GUID[i];
            }
            for (int i = 0; i < Body.Length; i++)
            {
                result[GUID.Length + i] = Body[i];
            }
            return result;
        }
        public override string ToString()
        {
            return $"{ArrayTools.ToString(GUID)}{ArrayTools.ToString(Body)}";
        }
    }
}
    
    
