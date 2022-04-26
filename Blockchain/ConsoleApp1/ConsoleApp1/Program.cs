
using System.Security.Cryptography;

RSA rsaDima = new RSACryptoServiceProvider(1024);



var publicPrivateKeyDima = rsaDima.ExportRSAPrivateKey();
var publicOnlyKeyDima = rsaDima.ExportRSAPublicKey();

RSA rsaNazar = new RSACryptoServiceProvider(1024);
var publicPrivateKeyNazar = rsaNazar.ExportRSAPrivateKey();
var publicOnlyKeyNazar = rsaNazar.ExportRSAPublicKey();


var blockInitial = new Block()
{
    FromName = publicOnlyKeyDima,
    ToName = publicOnlyKeyDima,
    Amount = 100,
    Nonce = 55748
};
blockInitial.Mine();
blockInitial.Sign(publicPrivateKeyNazar);

var success = blockInitial.Verify();



var block2=new Block()
{
    PreviousBlock = blockInitial,
    PreviousHash = "000001FEADA2A1A730470D0733A1A5EC470B4912",
    FromName = publicOnlyKeyDima,
    ToName = publicOnlyKeyNazar,
    Nonce = 815310,
    Amount = 10
};




List<Block> blocks = new List<Block>()
{
    blockInitial,
    block2,
    
};
Dictionary<string, int> amounts = new Dictionary<string, int>()
{
    {"Dima",0},
    {"Nazar",0}
};


if (block2.Verify())
{
    Console.WriteLine("Verification ok");
}
else
{
    Console.WriteLine("Verification failed");
}

foreach (Block block in blocks)
{
    if (block.PreviousHash == "")
    {
        amounts[string.Concat(block.ToName.Select(x=>x.ToString("X2")))] += block.Amount;
    }
    else
    {
        if (amounts[string.Concat(block.FromName.Select(x => x.ToString("X2")))] >= block.Amount)
        {
            amounts[string.Concat(block.FromName.Select(x => x.ToString("X2")))] -=block.Amount;
            amounts[string.Concat(block.ToName.Select(x => x.ToString("X2")))] +=block.Amount;
        }
        else
        {
            Console.WriteLine("Error");
            break;
        }
    }
}

foreach (KeyValuePair<string, int> pair in amounts)
{
    Console.WriteLine($"{pair.Key} has {pair.Value}");
}

Console.ReadLine();

class Block
{
    public uint Nonce { get; set; }
    public string PreviousHash { get; set; } = "";
    public Block PreviousBlock { get; set; }
    public byte[] FromName { get; set; } = new byte[0];
    public byte[] ToName { get; set; }
    public int Amount { get; set; }
    public byte[] Signature { get; set; }

    public byte[] Serialize()
    {
        MemoryStream ms = new MemoryStream();
        BinaryWriter bw = new BinaryWriter(ms);
        bw.Write(Nonce);
        bw.Write(PreviousHash);
        bw.Write(FromName);
        bw.Write(ToName);
        bw.Write(Amount);
        return ms.ToArray();
    }


    public void Mine()
    {
        for (uint i = 0; i < uint.MaxValue; i++)
        {
            Nonce = i;
            if(CalculateHash().StartsWith("00000"))return;
        }
    }

    public string CalculateHash()
    {
        var serialized = Serialize();
        return CreateMD5(serialized);
    }

    public void Sign(byte[] privateKey)
    {

        RSA rsa = new RSACryptoServiceProvider(1024);
        rsa.ImportRSAPrivateKey(privateKey,out _);

        Signature=rsa.SignData(Serialize(), HashAlgorithmName.MD5, RSASignaturePadding.Pkcs1);
    }

    public bool VerifySignature()
    {
        RSA rsa = new RSACryptoServiceProvider(1024);
        rsa.ImportRSAPublicKey(FromName,out _);
        return rsa.VerifyData(Serialize(), Signature, HashAlgorithmName.MD5, RSASignaturePadding.Pkcs1);
    }

    public bool Verify()
    {
        if (!VerifySignature()) return false;
        if (!CalculateHash().StartsWith("00000")) return false;
        
        if (PreviousHash == "")
        {
            return true;
        }

        if (PreviousBlock.CalculateHash() == PreviousHash)
        {
            return PreviousBlock.Verify();
        }
        return false;
    }

    public static string CreateMD5(byte[] input)
    {
        using (System.Security.Cryptography.SHA1 md5 = System.Security.Cryptography.SHA1.Create())
        {
            byte[] inputBytes = input;
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            return Convert.ToHexString(hashBytes); 
        }
    }
}