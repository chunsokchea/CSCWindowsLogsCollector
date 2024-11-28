using Microsoft.VisualBasic;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CSCWindowsLogsCollector
{
    internal class Encryption
    {
        private static byte[] _arrBytDESKey = new byte[] { 11, 0x17, 0x5d, 0x66, 0x48, 0x29, 0x12, 12 };
        private static byte[] _arrBytDESIV = new byte[] { 0x4b, 0x9e, 0x2e, 0x61, 0x4e, 0x39, 0x11, 0x24 };

        public static string Decode(string p_strEncode)
        {
            return new StreamReader(new CryptoStream(new MemoryStream(Convert.FromBase64String(p_strEncode)), new DESCryptoServiceProvider().CreateDecryptor(_arrBytDESKey, _arrBytDESIV), CryptoStreamMode.Read)).ReadToEnd();
        }

        public static string Encode(string p_strDecode)
        {
            MemoryStream stream = new MemoryStream();
            CryptoStream stream2 = new CryptoStream(stream, new DESCryptoServiceProvider().CreateEncryptor(_arrBytDESKey, _arrBytDESIV), CryptoStreamMode.Write);
            StreamWriter writer = new StreamWriter(stream2);
            writer.Write(p_strDecode);
            writer.Flush();
            stream2.FlushFinalBlock();
            stream.Flush();
            return Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length);
        }

		public  string Encode2(string Password)
		{
			string text = this.ToBinary(Password);
			string pstrValue = this.ToBinary(this.GetSeed());
			text = this.EncryTransform(text, pstrValue);
			text = this.GetString(text) + Password.Length.ToString("00");
			return Convert.ToBase64String(Encoding.Unicode.GetBytes(text));
		}

		// Token: 0x06000025 RID: 37 RVA: 0x0000267C File Offset: 0x00000A7C
		public string Decode2(string Password)
		{
			bool flag = Password.Trim().Length == 0;
			string result;
			if (flag)
			{
				result = "";
			}
			else
			{
				Password = Encoding.Unicode.GetString(Convert.FromBase64String(Password));
				short num = short.Parse(Strings.Right(Password, 2));
				short length = num;
				string text = this.ToBinary(Password.Substring(0, (int)length));
				text = this.DecryTransform(text);
				text = this.GetString(text);
				result = text;
			}
			return result;
		}

		private string DecryTransform(string pstrValue)
		{
			StringBuilder stringBuilder = new StringBuilder("");
			short num = checked((short)(pstrValue.Length / 8 - 1));
			short num2 = 0;
			short num3 = num;
			short num4 = num2;
			for (; ; )
			{
				short num5 = num4;
				short num6 = num3;
				if (num5 > num6)
				{
					break;
				}
				char[] array = pstrValue.Substring(0, 8).ToCharArray();
				pstrValue = pstrValue.Remove(0, 8);
				short num7 = 4;
				short num8;
				do
				{
					char c = array[(int)num7];
					checked
					{
						array[(int)num7] = array[(int)(num7 + 2)];
						array[(int)(num7 + 2)] = c;
					}
					num7 += 1;
					num8 = num7;
					num6 = 5;
				}
				while (num8 <= num6);
				stringBuilder.Append(array, 0, 8);
				num4 += 1;
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002788 File Offset: 0x00000B88
		private string EncryTransform(string pstrValue1, string pstrValue2)
		{
			StringBuilder stringBuilder = new StringBuilder("");
			short num = checked((short)(pstrValue1.Length / 8 - 1));
			short num2 = 0;
			short num3 = num;
			short num4 = num2;
			for (; ; )
			{
				short num5 = num4;
				short num6 = num3;
				if (num5 > num6)
				{
					break;
				}
				char[] array = pstrValue1.Substring(0, 8).ToCharArray();
				char[] array2 = pstrValue2.Substring(0, 8).ToCharArray();
				pstrValue1 = pstrValue1.Remove(0, 8);
				pstrValue2 = pstrValue2.Remove(0, 8);
				short num7 = 4;
				short num8;
				do
				{
					char c = array[(int)num7];
					checked
					{
						array[(int)num7] = array[(int)(num7 + 2)];
						array[(int)(num7 + 2)] = c;
					}
					num7 += 1;
					num8 = num7;
					num6 = 5;
				}
				while (num8 <= num6);
				stringBuilder.Append(array, 0, 8);
				num4 += 1;
			}
			num = checked((short)(pstrValue2.Length / 8 - 1));
			short num9 = 0;
			short num10 = num;
			short num11 = num9;
			for (; ; )
			{
				short num12 = num11;
				short num6 = num10;
				if (num12 > num6)
				{
					break;
				}
				char[] array3 = pstrValue2.Substring(0, 8).ToCharArray();
				pstrValue2 = pstrValue2.Remove(0, 8);
				short num13 = 4;
				short num14;
				do
				{
					char c2 = array3[(int)num13];
					checked
					{
						array3[(int)num13] = array3[(int)(num13 + 2)];
						array3[(int)(num13 + 2)] = c2;
					}
					num13 += 1;
					num14 = num13;
					num6 = 5;
				}
				while (num14 <= num6);
				stringBuilder.Append(array3, 0, 8);
				num11 += 1;
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000028 RID: 40 RVA: 0x000028B8 File Offset: 0x00000CB8
		private string GetSeed()
		{
			StringBuilder stringBuilder = new StringBuilder("");
			short num = 15;
			short num2 = 0;
			short num3 = num;
			short num4 = num2;
			for (; ; )
			{
				short num5 = num4;
				short num6 = num3;
				if (num5 > num6)
				{
					break;
				}
				VBMath.Randomize();
				short charCode = checked((short)Math.Round((double)(unchecked(VBMath.Rnd() * 44f + 48f))));
				stringBuilder.Append(Strings.Chr((int)charCode));
				num4 += 1;
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002928 File Offset: 0x00000D28
		private string ToBinary(string strText)
		{
			char[] array = strText.ToCharArray();
			StringBuilder stringBuilder = new StringBuilder();
			foreach (char chr in array)
			{
				stringBuilder.Append(this.MakeBinary(chr));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002980 File Offset: 0x00000D80
		private string MakeBinary(char chr)
		{
			short num = checked((short)Strings.Asc(chr));
			StringBuilder stringBuilder = new StringBuilder();
			while (num != 1 && num != 0)
			{
				stringBuilder.Append((int)(num % 2));
				num /= 2;
			}
			stringBuilder.Append(num);
			while (stringBuilder.ToString().Length < 8)
			{
				stringBuilder.Append("0");
			}
			return Strings.StrReverse(stringBuilder.ToString());
		}

		// Token: 0x0600002B RID: 43 RVA: 0x000029F8 File Offset: 0x00000DF8
		private string GetString(string strBinary)
		{
			short num = 0;
			StringBuilder stringBuilder = new StringBuilder();
			while (strBinary.Length >= 8)
			{
				short num2 = 0;
				short num3;
				short num4;
				do
				{
					num = checked((short)Math.Round(unchecked((double)num + (double)short.Parse(strBinary.Substring((int)num2, 1)) * Math.Pow(2.0, (double)(checked(7 - num2))))));
					num2 += 1;
					num3 = num2;
					num4 = 7;
				}
				while (num3 <= num4);
				stringBuilder.Append(Strings.Chr((int)num));
				strBinary = strBinary.Remove(0, 8);
				num = 0;
			}
			return stringBuilder.ToString();
		}
		//public static string Encode2(string data)
		//{
		//	byte[] bytes = Encoding.ASCII.GetBytes("20178888");
		//	byte[] bytes2 = Encoding.ASCII.GetBytes("20178888");
		//	DESCryptoServiceProvider descryptoServiceProvider = new DESCryptoServiceProvider();
		//	int keySize = descryptoServiceProvider.KeySize;
		//	MemoryStream memoryStream = new MemoryStream();
		//	CryptoStream cryptoStream = new CryptoStream(memoryStream, descryptoServiceProvider.CreateEncryptor(bytes, bytes2), CryptoStreamMode.Write);
		//	StreamWriter streamWriter = new StreamWriter(cryptoStream);
		//	streamWriter.Write(data);
		//	streamWriter.Flush();
		//	cryptoStream.FlushFinalBlock();
		//	streamWriter.Flush();
		//	return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
		//}

		//// Token: 0x060000D0 RID: 208 RVA: 0x0000B0BC File Offset: 0x000092BC
		//public static string Decode2(string data)
		//{
		//	byte[] bytes = Encoding.ASCII.GetBytes("20178888");
		//	byte[] bytes2 = Encoding.ASCII.GetBytes("20178888");
		//	string result;
		//	try
		//	{
		//		byte[] buffer = Convert.FromBase64String(data);
		//		DESCryptoServiceProvider descryptoServiceProvider = new DESCryptoServiceProvider();
		//		MemoryStream stream = new MemoryStream(buffer);
		//		CryptoStream stream2 = new CryptoStream(stream, descryptoServiceProvider.CreateDecryptor(bytes, bytes2), CryptoStreamMode.Read);
		//		StreamReader streamReader = new StreamReader(stream2);
		//		result = streamReader.ReadToEnd();
		//	}
		//	catch
		//	{
		//		result = null;
		//	}
		//	return result;
		//}

		public static string Encode3(string data)
		{
			byte[] bytes = Encoding.ASCII.GetBytes("20148888");
			byte[] bytes2 = Encoding.ASCII.GetBytes("20148888");
			DESCryptoServiceProvider descryptoServiceProvider = new DESCryptoServiceProvider();
			int keySize = descryptoServiceProvider.KeySize;
			MemoryStream memoryStream = new MemoryStream();
			CryptoStream cryptoStream = new CryptoStream(memoryStream, descryptoServiceProvider.CreateEncryptor(bytes, bytes2), CryptoStreamMode.Write);
			StreamWriter streamWriter = new StreamWriter(cryptoStream);
			streamWriter.Write(data);
			streamWriter.Flush();
			cryptoStream.FlushFinalBlock();
			streamWriter.Flush();
			return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
		}

		public static string Decode3(string data)
		{
			byte[] bytes = Encoding.ASCII.GetBytes("20148888");
			byte[] bytes2 = Encoding.ASCII.GetBytes("20148888");
			byte[] buffer;
			try
			{
				buffer = Convert.FromBase64String(data);
			}
			catch
			{
				return null;
			}
			DESCryptoServiceProvider descryptoServiceProvider = new DESCryptoServiceProvider();
			MemoryStream stream = new MemoryStream(buffer);
			CryptoStream stream2 = new CryptoStream(stream, descryptoServiceProvider.CreateDecryptor(bytes, bytes2), CryptoStreamMode.Read);
			StreamReader streamReader = new StreamReader(stream2);
			return streamReader.ReadToEnd();
		}
	}
}