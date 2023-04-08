using System;
using System.Text;
namespace Pico.Networking;

public struct ComPacket
{
    public int Prefix;
    public byte[] Body;

    //Creating a new packet
    public ComPacket(int prefix, byte[] body)       //Manual bytes
    {
        this.Prefix = prefix;
        this.Body = body;
    }
    public ComPacket(int prefix, string text = null) //Convert object? to bytes
    {
        this.Prefix = prefix;
        if (text == null) Body = new byte[0];
        else Body = ASCIIEncoding.ASCII.GetBytes(text);
    }




    //Packet / Data Conversion
    public static ComPacket FromBytes(byte[] data)
    {
        if (data == null) return new ComPacket(0, new byte[] { });
        if (data.Length < 4) return new ComPacket(0, new byte[] { });

        var prefix = BitConverter.ToInt32(data, 0);

        var body = new byte[data.Length - 4];
        for (int i = 0; i < body.Length; i++)
        {
            body[i] = data[i + 4];
        }

        return new ComPacket(prefix, body);
    }
    public byte[] ToBytes()
    {
        //Check no body?
        //[PREFIX]
        if (Body == null) return BitConverter.GetBytes(Prefix);

        //Is body.
        //[PREFIX][BODY]
        byte[] rawData = new byte[Body.Length + 4];
        byte[] prefixBytes = BitConverter.GetBytes(Prefix); 
        prefixBytes.CopyTo(rawData, 0);     //4 bytes
        Body.CopyTo(rawData, 4);            //Remaining bytes
        return rawData;
    }

    //Body Byte[] / Text Conversion
    public void SetBodyText(string text)
    {
        if (text == null)
        {
            Body = new byte[0];
            return;
        }
        Body = ASCIIEncoding.ASCII.GetBytes(text);
    }
    public string GetBodyText() => ASCIIEncoding.ASCII.GetString(Body, 0, Body.Length);



    //To String(s)
    public override string ToString()
    {
        //[PREFIX] [X BYTES]
        return $"[{Prefix}] {Body.Length + 4} Bytes";   //+4 bytes to account for prefix int.
    }
    public string ToStringTextBody()
    {
        //[PREFIX] "BODY TEXT"
        return $"[{Prefix}] \"{GetBodyText()}\"";
    }
    
}

