using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graduate_Work
{
    public class RC5
    {
        private string filePath = "data.json";
        private int W = 64;                            // половина длины блока в битах.
                                                       // Возможные значения 16, 32 и 64.
                                                       // Для эффективной реализации величину W
                                                       // рекомендуют брать равным машинному слову.
                                                       // Например, для 64-битных платформ оптимальным будет
                                                       // выбор W=64, что соответствует размеру блока в 128 бит.

        private int R = 16;                            // число раундов. Возможные значения 0…255.
                                                     // Увеличение числа раундов обеспечивает увеличение
                                                     // уровня безопасности шифра. Так, если R = 0,
                                                     // то информация шифроваться не будет.

        private UInt64 PW = 0xB7E151628AED2A6B;        // 64-битная константа
        private UInt64 QW = 0x9E3779B97F4A7C15;        // 64-битная константа

        UInt64[] L;                                  // массив слов для секретного ключа пользователя
        UInt64[] S;                                  // таблица расширенных ключей
        int t;                                       // размер таблицы
        int b;                                       // длина ключа в байтах. Возможные значения 0…255.
        int u;                                       // кол-во байтов в одном машинном слове
        int c;                                       // размер массива слов L

        public RC5(byte[] key)
        {
			if (File.Exists(filePath))
			{
				var data = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(filePath));
				W = Convert.ToInt32(data.comboBox2Value.ToString() ?? "64");
				R = data.numericUpDownRounds.ToObject<int>() ?? 16;
			}
			if (W == 16)
			{
				PW = 0xB7E1;
				QW = 0x9E37;
			}
			else if (W == 32)
			{
				PW = 0xB7E15163;
				QW = 0x9E3779B9;
			}
			else
			{
				PW = 0xB7E151628AED2A6B;
				QW = 0x9E3779B97F4A7C15;
			}

			/*
             *  Перед непосредственно шифрованием или расшифровкой данных выполняется процедура расширения ключа.
             *  Процедура генерации ключа состоит из четырех этапов:
             *      1. Генерация констант
             *      2. Разбиение ключа на слова
             *      3. Построение таблицы расширенных ключей
             *      4. Перемешивание
             */

			// основные переменные
			UInt64 x, y;
            int i, j, n;

            /*
             * Этап 1. Генерация констант
             * Для заданного параметра W генерируются две псевдослучайные величины,
             * используя две математические константы: e (экспонента) и f (Golden ratio).
             * Qw = Odd((e - 2) * 2^W);
             * Pw = Odd((f - 1) * 2^W);
             * где Odd() - это округление до ближайшего нечетного целого.
             *
             * Для оптимизации алгоритмы эти 2 величины определены заранее (см. константы выше).
             */

            /*
             * Этап 2. Разбиение ключа на слова
             * На этом этапе происходит копирование ключа K[0]..K[255] в массив слов L[0]..L[c-1], где
             * c = b/u, а u = W/8. Если b не кратен W/8, то L[i] дополняется нулевыми битами до ближайшего
             * большего размера c, при котором длина ключа b будет кратна W/8.
             */

            u = W >> 3;
            b = key.Length;
            c = b % u > 0 ? b / u + 1 : b / u;
            L = new UInt64[c];

            for (i = b - 1; i >= 0; i--)
            {
                L[i / u] = ROL(L[i / u], 8) + key[i];
            }

            /* Этап 3. Построение таблицы расширенных ключей
             * На этом этапе происходит построение таблицы расширенных ключей S[0]..S[2(R + 1)],
             * которая выполняется следующим образом:
             */

            t = 2 * (R + 1);
            S = new UInt64[t];
            S[0] = PW;
            for (i = 1; i < t; i++)
            {
                S[i] = S[i - 1] + QW;
            }

            /* Этап 4. Перемешивание
             * Циклически выполняются следующие действия:
             */

            x = y = 0;
            i = j = 0;
            n = 3 * Math.Max(t, c);

            for (int k = 0; k < n; k++)
            {
                x = S[i] = ROL((S[i] + x + y), 3);
                y = L[j] = ROL((L[j] + x + y), (int)(x + y));
                i = (i + 1) % t;
                j = (j + 1) % c;
            }
        }


        /* 
         * Циклический сдвиг битов слова влево
         * a: машинное слово = 64 бита
         * offset: смещение
         * возвращает машинное слово: 64 бита
         */
        private UInt64 ROL(UInt64 a, int offset)
        {
            UInt64 r1, r2;
            r1 = a << offset;
            r2 = a >> (W - offset);
            return (r1 | r2);

        }

       /* 
        * Циклический сдвиг битов слова вправо
        * a: машинное слово = 64 бита
        * offset: смещение
        * возвращает машинное слово: 64 бита
        */
        private UInt64 ROR(UInt64 a, int offset)
        {
            UInt64 r1, r2;
            r1 = a >> offset;
            r2 = a << (W - offset);
            return (r1 | r2);

        }


       /*
        * Свертка слова (64 бит) по 8-ми байтам
        * b: массив байтов
        * p: позиция
        */
        private static UInt64 BytesToUInt64(byte[] b, int p)
        {
            UInt64 r = 0;
            for (int i = p + 7; i > p; i--)
            {
                r |= (UInt64)b[i];
                r <<= 8;
            }
            r |= (UInt64)b[p];
            return r;
        }

       /*
        * Развертка слова (64 бит) по 8-ми байтам
        * a: 64-битное слово
        * b: массив байтов
        * p: позиция
        */
        private static void UInt64ToBytes(UInt64 a, byte[] b, int p)
        {
            for (int i = 0; i < 7; i++)
            {
                b[p + i] = (byte)(a & 0xFF);
                a >>= 8;
            }
            b[p + 7] = (byte)(a & 0xFF);
        }

       /* 
        * Операция шифрования
        * inBuf: входной буфер для шифруемых данных (64 бита)
        * outBuf: выходной буфер (64 бита)
        */
        public void Cipher(byte[] inBuf, byte[] outBuf)
        {
            UInt64 a = BytesToUInt64(inBuf, 0);
            UInt64 b = BytesToUInt64(inBuf, 8);

            a = a + S[0];
            b = b + S[1];

            for (int i = 1; i < R + 1; i++)
            {
                a = ROL((a ^ b), (int)b) + S[2 * i];
                b = ROL((b ^ a), (int)a) + S[2 * i + 1];
            }

            UInt64ToBytes(a, outBuf, 0);
            UInt64ToBytes(b, outBuf, 8);
        }

       /*
        * Операция расшифрования
        * inBuf: входной буфер для шифруемых данных (64 бита)
        * outBuf: выходной буфер (64 бита)
        */
        public void Decipher(byte[] inBuf, byte[] outBuf)
        {
            UInt64 a = BytesToUInt64(inBuf, 0);
            UInt64 b = BytesToUInt64(inBuf, 8);

            for (int i = R; i > 0; i--)
            {
                b = ROR((b - S[2 * i + 1]), (int)a) ^ a;
                a = ROR((a - S[2 * i]), (int)b) ^ b;
            }

            b = b - S[1];
            a = a - S[0];

            UInt64ToBytes(a, outBuf, 0);
            UInt64ToBytes(b, outBuf, 8);
        }
    }

    class RC5FileProcessor
    {
        private RC5 rc5;

        public RC5FileProcessor(byte[] key)
        {
            rc5 = new RC5(key);
        }

        public void EncryptFile(string inputFilePath, string outputFilePath)
        {
            using (var inputStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
            using (var outputStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
            {
                byte[] buffer = new byte[16]; // Размер блока для RC5 (2 слова по 64 бита)
                int bytesRead;
                while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    byte[] encryptedData = new byte[buffer.Length];
                    rc5.Cipher(buffer, encryptedData);
                    outputStream.Write(encryptedData, 0, buffer.Length);
                }
            }
        }

        public void DecryptFile(string inputFilePath, string outputFilePath)
        {
            using (var inputStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
            using (var outputStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
            {
                byte[] buffer = new byte[16]; // Размер блока для RC5 (2 слова по 64 бита)
                int bytesRead;
                while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    byte[] decryptedData = new byte[buffer.Length];
                    rc5.Decipher(buffer, decryptedData);
                    outputStream.Write(decryptedData, 0, buffer.Length);
                }
            }
        }
    }
}
