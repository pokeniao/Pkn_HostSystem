// See https://aka.ms/new-console-template for more information
using System.Security.Cryptography;
using System.Text;

Console.WriteLine("请输入机器码(公钥)");
//读取机器码
string? machineCode = Console.ReadLine();
byte[] data = Encoding.UTF8.GetBytes(machineCode);


// using var rsa = RSA.Create(2048);
// string publicKey = Convert.ToBase64String(rsa.ExportSubjectPublicKeyInfo());
// string privateKey = Convert.ToBase64String(rsa.ExportPkcs8PrivateKey());
//
// Console.WriteLine("公钥(放在软件里)：");
// Console.WriteLine(publicKey);
// Console.WriteLine("私钥(自己保存)：");
// Console.WriteLine(privateKey);

// --- 许可证生成 ---
byte[] signature;

string privateKey =
    "";
using (var rsa = RSA.Create())
{
    //导入私钥. 如果不写这行,会自动生成私钥和公钥
    rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(privateKey), out _);
    signature = rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    // Console.WriteLine("公钥：");
    // Console.WriteLine(Convert.ToBase64String(rsa.ExportSubjectPublicKeyInfo()));
    // Console.WriteLine("私钥：");
    // Console.WriteLine(Convert.ToBase64String(rsa.ExportPkcs8PrivateKey()));
}
string license = Convert.ToBase64String(signature);
Console.WriteLine("许可证：");
Console.WriteLine(license);



Console.ReadLine();






