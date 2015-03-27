using System;
using System.Text;
using System.Windows;
using System.Security.Cryptography;
using System.IO;
using System.Convert;
using System.Windows.Documents;

namespace HeliumKeygen
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            string[] brags = { "and is brought to you by the letter \"K\" and the number \"2\".",
                "and is part of a nutritious breakfast!",
                "\"WINNERS DON'T USE DRUGS\" William S. Sessions, Director, FBI",
                "It is pitch black. you are likely to be eaten by a grue.",
                "fnord",
                "Quit your job for \"Bob\"!",
                "What's your favorite thing about space? Mine is space.",
                "Healed Head Bad, Bleeding Head Good!" };
            InitializeComponent();
            Run tagline = (Run)textBlock.Inlines.LastInline;
            tagline.Text = brags[new Random().Next(0, brags.GetLength(0))];
        }
        private void generateSerial(object sender, System.Windows.RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(nameBox.Text) || String.IsNullOrEmpty(emailBox.Text))
                return;
            bool isNetwork = false;
            if (editionBox.SelectedIndex == 0)
                isNetwork = true;
            keyBox.Text = OMGHAX.generateSN(nameBox.Text, emailBox.Text, isNetwork, Int32.Parse(versionBox.Text));
        }
    }
    class OMGHAX
    {
        // This is free and unencumbered software released into the public domain.
        //
        // Anyone is free to copy, modify, publish, use, compile, sell, or
        // distribute this software, either in source code form or as a compiled
        // binary, for any purpose, commercial or non-commercial, and by any
        // means.
        //
        // In jurisdictions that recognize copyright laws, the author or authors
        // of this software dedicate any and all copyright interest in the
        // software to the public domain. We make this dedication for the benefit
        // of the public at large and to the detriment of our heirs and
        // successors. We intend this dedication to be an overt act of
        // relinquishment in perpetuity of all present and future rights to this
        // software under copyright law.
        //
        // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
        // EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
        // MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
        // IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
        // OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
        // ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
        // OTHER DEALINGS IN THE SOFTWARE.
        //
        // For more information, please refer to <http://unlicense.org/>
        //
        // All constants have been removed, and are presumed to be the property of Imploded Software.
        // If you use this software, pay the money (it's not unreasonable pricing, either). I have not
        // released the "proper" binary anywhere, and never will. Srsly you guys, this is intended to
        // be a tutorial. 
        //
        // "Oderint dum metuant"
        //
        // All of this is done as a straightforward translation of MSIL to C#. There are a few instances
        // in here where you can clearly see some of the unwinding done by the bytecode compiler; lots
        // of opportunities to turn 2-3 lines into a more elegant one liner or to use a higher level
        // branching conditional, but the framework is a stack machine, so everything gets boiled down
        // goto statements. I tried to keep the resultant code just C-ified enough to work, but left in
        // a bunch of awkward constructs. If you look at the decompiled bytecode from NetLM.dll and
        // LicenseManagerNet.dll using CodeReflect or something like that you should be able to follow
        // the comments and keep track of what the stack is doing... if you have ever programmed an old
        // HP-RPL calculator it will begin to look *very* familiar.
        //
        // There is also the possibility that the original coder was an utter noob, and I'm not ready to
        // rule that out either. I mean, really, crypto secrets stored as plain text strings? That's a day
        // one mistake right there. Also, .NET obfuscation is dumb. If you use it, you are a bad person and
        // you should feel bad. If your code open to the world is not secure, YOUR CODE IS NOT SECURE. In
        // any case, obfuscating a language that compiles down into MSIL is kind of dumb in a general sense
        // because, again, the MSIL interpreter is a stack machine and its code is really quite readable as
        // long as you can keep track of the stack on a scratchpad or something.
        //
        // And, hey, if you are the guy that wrote this code, dude. Go off and read how to do this a bit
        // better. You have to assume that all constants, however "encrypted" or "hidden" are visible to,
        // ya know, folks like me. Never assume that obscurity == security. You could at least make it
        // difficult / impossible to create a keygen like this by making serial generation an assymetric
        // algorithm... if you use a hash function, be sure that it doesn't have a rainbow table premade, for
        // one. Force me to at least have to write a patch for version 13. That's enough hints though, better
        // luck next time. If you continue to whack contstants into getHash I'll just keep adding them here,
        // it literally only took about five minutes to update this from v11 to v12. The initial crack was
        // something like 2-3 hours, and most of that was writing comments and trying to come up with clever
        // quotes for the "brag". Go forth, make better software, and prosper!
        //
        static readonly byte[] salt09 = Encoding.ASCII.GetBytes("DEADBEEFDEADBE"); //From NetLM.dll::Crypto.cctor
        static readonly byte[] salt10 = Encoding.ASCII.GetBytes("DEADBEEFDEADBE"); //From NetLM.dll::Crypto.cctor
        static readonly string[] PremiumKeys = { "DEADBEEF", "DEADBEEF", "DEADBEEF", "DEADBEEF", "DEADBEEF", "DEADBEEF", "DEADBEEF", "DEADBEEF", "DEADBEEF", "DEADBEEF", "DEADBEEF", "DEADBEEF", "DEADBEEF", "DEADBEEF", "DEADBEEF", "DEADBEEF" }; // From NetLM.dll::VersionHelper.cctor
        static readonly string[] NetworkKeys = { "DEADBEEF", "DEADBEEF", "DEADBEEF", "DEADBEEF", "DEADBEEF", "DEADBEEF", "DEADBEEF", "DEADBEEF", "DEADBEEF", "DEADBEEF", "DEADBEEF", "DEADBEEF", "DEADBEEF", "DEADBEEF", "DEADBEEF", "DEADBEEF" }; // From NetLM.dll::VersionHelper.cctor
        public static string generateSN(string userName, string email, bool isNetwork, int version)
        {
            string str1;
            // Push email, push ":", push isNetwork, concatenate, pop, pop, pop, push result, push userName, push version
            str1 = EncryptStringAES(userName + ":" + email, getHash(version), version);
            // Push isNetwork
            if (isNetwork) // Pop
                return string.Format("{0}-{1}", str1, GetNetworkKey());
            return string.Format("{0}-{1}", str1, GetPremiumKey());
        }
        static string GetNetworkKey() // WTF is this bullshit!?
        {
            int num1 = new Random().Next(0, 15);
            return NetworkKeys[num1];
        }
        static string GetPremiumKey()
        {
            int num1 = new Random().Next(0, 15);
            return PremiumKeys[num1];
        }
        static string getHash(int version)
        {
            // Dead simple, no analysis needed. It's worth noting that when they released version 11, this was the *only* change.
            switch (version)
            {
                case 9:
                    return "DEADBEEF-DEAD-BEEF-DEAD-BEEFDEADBEEF";
                case 10:
                    return "DEADBEEF-DEAD-BEEF-DEAD-BEEFDEADBEEF";
                case 11:
                    return "DEADBEEF-DEAD-BEEF-DEAD-BEEFDEADBEEF";
            }
            return System.String.Empty;
        }
        static string DecryptStringAES(string cipherText, string sharedSecret, int version)
        {
            RijndaelManaged rijndaelmanaged1 = null;
            string str1 = null;
            Rfc2898DeriveBytes rfc2898derivebytes1;
            ICryptoTransform icryptotransform1;
            byte[] array1;
            MemoryStream memorystream1;
            CryptoStream cryptostream1;
            StreamReader streamreader1;
            // Push cipherText
            if (String.IsNullOrEmpty(cipherText)) // Pop
                throw new ArgumentNullException("cipherText");
            // Push sharedSecret
            if (String.IsNullOrEmpty(sharedSecret))
                throw new ArgumentNullException("sharedSecret"); // Pop
                                                                 // Begin try L_002a to L_00e3 finally handler L_00e3 to L_00ed
                                                                 // Push (int)9, push version 
            if (version == 9) // Pop, pop
                              // Push salt09
                rfc2898derivebytes1 = new Rfc2898DeriveBytes(sharedSecret, salt09); // Pop, push, store local 2 
            else // Push salt10
                rfc2898derivebytes1 = new Rfc2898DeriveBytes(sharedSecret, salt10); // Pop, push, store local 2
                                                                                    // End try L_002a to L_00e3 finally handler L_00e3 to L_00ed
            rijndaelmanaged1 = new RijndaelManaged(); // Store local 0                                          
            rijndaelmanaged1.Key = rfc2898derivebytes1.GetBytes(rijndaelmanaged1.KeySize / 8); // Many operations, stack emptied
            rijndaelmanaged1.IV = rfc2898derivebytes1.GetBytes(rijndaelmanaged1.BlockSize / 8); // Many operations, stack emptied
            icryptotransform1 = rijndaelmanaged1.CreateDecryptor(); // Store local 3
                                                                    // Push cipherText
            array1 = FromBase64String(cipherText); // Pop, store local 4
                                                   // Push array1
            memorystream1 = new MemoryStream(array1); // Pop, store local 5
                                                      // Push memorystream1, icryptotransform1, (int)0
                                                      // There are three try blocks starting here, all have been ignored.
            cryptostream1 = new CryptoStream(memorystream1, icryptotransform1, CryptoStreamMode.Read); // Pop, pop, pop, store local 6
                                                                                                       // Push cryptostream1
            streamreader1 = new StreamReader(cryptostream1); // Pop, store local 7
                                                             // Push streamreader1
            str1 = streamreader1.ReadToEnd(); // Pop, store local 1
                                              // Push str1
            return str1; // Pop
        }
        static string EncryptStringAES(string plainText, string sharedSecret, int version)
        {
            // Try blocks not analyzed. Straightforward inverse of DecryptStringAES.
            string str1 = null;
            RijndaelManaged rijndaelmanaged1 = null;
            Rfc2898DeriveBytes rfc2898derivebytes1;
            ICryptoTransform icryptotransform1;
            System.IO.MemoryStream memorystream1;
            CryptoStream cryptostream1;
            StreamWriter streamwriter1;
            // Push plainText
            if (String.IsNullOrEmpty(plainText)) // Pop
                throw new ArgumentNullException("plainText");
            // Push sharedSecret
            if (String.IsNullOrEmpty(sharedSecret))
                throw new ArgumentNullException("sharedSecret"); // Pop
                                                                 // Push (int)9, push version 
            if (version == 9) // Pop, pop
                              // Push salt09
                rfc2898derivebytes1 = new Rfc2898DeriveBytes(sharedSecret, salt09); // Pop, push, store local 2 
            else // Push salt10
                rfc2898derivebytes1 = new Rfc2898DeriveBytes(sharedSecret, salt10); // Pop, push, store local 2
            rijndaelmanaged1 = new RijndaelManaged(); // Store local 0                                          
            rijndaelmanaged1.Key = rfc2898derivebytes1.GetBytes(rijndaelmanaged1.KeySize / 8); // Many operations, stack emptied
            rijndaelmanaged1.IV = rfc2898derivebytes1.GetBytes(rijndaelmanaged1.BlockSize / 8); // Many operations, stack emptied
            icryptotransform1 = rijndaelmanaged1.CreateEncryptor(); // Store local 3
                                                                    // Push memorystream1
            memorystream1 = new MemoryStream(); // Pop, store local 4                    
            cryptostream1 = new CryptoStream(memorystream1, icryptotransform1, CryptoStreamMode.Write); // Pop, pop, pop, store local 5
                                                                                                        // Push cryptostream1
            streamwriter1 = new StreamWriter(cryptostream1); // Pop, store local 6
                                                             // Push streamwriter1, push plainText
            streamwriter1.Write(plainText); // Pop, pop
                                            // Push streamwriter1
            streamwriter1.Dispose(); // I suspect this started as a Flush() and got optimized into a Dispose() by the compiler
                                     // Push memorystream1
            str1 = Convert.ToBase64String(memorystream1.ToArray()); // Pop, Store local 0
                                                                    // Push str1
            return str1; // Pop
        }
    }
}
