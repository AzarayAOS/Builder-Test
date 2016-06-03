using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;


namespace Builder_Test
{
    public partial class Form1 : Form
    {
        int nomber = 0; // количесвто вопросов
        TextBox[] Tb;   // массив полей для ввода ответов
        Label[] Lb;     // массив для подписей
        int kolotv=0;     // количество элементов эдитов
        string znach = "";// строка всех вопросов и ответов
        int actotv;     // переменная для количества активных ответов (без произвольного)
        string NewNabor(int ao) // тело функции формирования строки набора
        {
            String s = "";

            // формируем команду начала набора
            s += "<begin-" + nomber + "-" + kolotv + "-" + (checkBox1.Checked ? "t" : "f") + ">";


            // ЗАНОСИМ В СТРОКУ ВОПРОС

            // формируем команду вопроса
            s += "<que>";

            // заносим сам вопрос
            s += textBox2.Text;

            // формируем окончание команды вопроса
            s += "<qend>";

            // формируем команду ответов и сами ответы
            for (int i = 0; i < ao; i++)
            {
                s += "<a-" + (i + 1) + ">";
                s += Tb[i].Text;
                s += "<aend-" + (i + 1) + ">";
            }

            // формируем команду окончания набора
            s += "<bend-" + nomber + ">";
            
            // дабавляем команду переноса на новую строку
            s += "\n";
            return s;
        }

        // алгоритм дешифровки 
        static string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Проверка аргументов.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Объявляем строку, используемую для хранения
            // расшифрованного текста.
            string plaintext = null;

            // Создание RijndaelManaged объекта
            // с указанным ключом и IV.
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Создание декоратора для выполнения поток преобразования.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Создание потоков, используемых для дешифрования.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Чтение зашифрованных байтов из дешифрование потока
                            // и помещение их в строку.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;
        }


        static byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
        {
            // Проверка аргументов
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;
            // Создание RijndaelManaged объекта
            // с указанным ключом и IV.
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Создание декоратора для выполнения поток преобразования.
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Создание потоков, используемых для шифрования.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Запишите все данные в поток.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }


            // Возврат к зашифрованным байты из потока памяти.
            return encrypted;

        }

        public Form1()
        {
            InitializeComponent();
            this.Height = 300;
            this.Width = 300;
            
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // устанавливаем размеры и положение всех элементов

            // задаём размеры 
            this.Height = 500;
            this.Width = 700;
           
            
            // задание расположения
           // button1.Location = new Point(180, 390);
           // button2.Location = new Point(420, 390);
            label1.Location = new Point(170, 35);
            textBox1.Location = new Point(333, 32);
            button3.Location = new Point(400, 30);
            checkBox1.Location = new Point(445, 33);

            // делаем нужные объекты видимыми
           // button1.Visible = true;
           // button2.Visible = true;
            label1.Visible = true;
            textBox1.Visible = true;
            button3.Visible = true;
            textBox2.Visible = true;
            label2.Visible = true;
            label3.Visible = true;
            checkBox1.Visible = true;

        }
        // ограничиваем ввод в textBox1 только цифрами
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar <= 47 || e.KeyChar >= 59) && e.KeyChar != 8)
                e.Handled = true;
        }
        // создание и редактирование вопроса
        private void button3_Click(object sender, EventArgs e)
        {
            // отчищаем поле вопроса
            button3.Enabled = false;
            button4.Enabled = true;
            textBox2.Text = "";
            //выводим в подпись формы количество вопросов
            nomber++;
            this.Text = "Builder Test    Количество вопросов: " + nomber.ToString();

            // если эдиты ответов уже были созданы,
            // удаляем их для создания новых
            for (int i = 0; i < kolotv;i++)
            {
                Controls.Remove(Tb[i]);
                Controls.Remove(Lb[i]);
            }
            
            // делаем поле ввода вопроса активным
            textBox2.Enabled = true;


                kolotv = System.Convert.ToInt32(textBox1.Text);
           
// ------------------ создаем динамическое количество ответов
            

            // создаем массив количества вопросов и ответов
            // если есть произвольный ответ, то создать эдитов и лейблов на один больше
            Tb = new TextBox[kolotv];
            Lb = new Label[kolotv];
            for(int i=0, y=71+26;i<kolotv;i++,y+=26)
            {
                Tb[i] = new TextBox();
                Tb[i].Location = new Point(159, y);
                Tb[i].Name = "Tb" + i.ToString();
                Tb[i].Size = new Size(513, 20);
                //Tb[i].TabIndex = 15 + i+1;
                Tb[i].TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
                Tb[i].Visible = true;
                Controls.Add(Tb[i]);

                Lb[i] = new Label();
                Lb[i].Location = new Point(159 - 30, y);
                Lb[i].Name = "Lb" + i.ToString();
                Lb[i].Size = new Size(30, 15);
                //Lb[i].TabIndex = 15 + i;
                Lb[i].Text = (i + 1).ToString() + ")";
                Lb[i].Visible = true;
                Controls.Add(Lb[i]);
            }
             
            // если есть произвольный ответ
            if (checkBox1.Checked)
            {
                Tb[kolotv - 1].Enabled = false;
                Tb[kolotv - 1].Text = "Это поле для произвольного ответа.";
                Controls.Add(Tb[kolotv - 1]);
            }

            // формируем кнопку сохранения заданного набора
            button4.Visible = true;
            button4.Location = new Point(315,390);


//============ ОБРАБАТЫВАЕМ ЗАПОЛНЕНИЕ ВСЕХ ПОЛЕЙ===============


            // переменная для количества активных ответов (без произвольного)
             actotv = checkBox1.Checked ? kolotv - 1 : kolotv;

// Документация в коде. Тут я пропишу все внутрестроковые команды
// по которым программа разделяет внутри строки, где какая информация
// чтоб могла правильно интерпретировать заполненную внутри информацию
// <begin-x-a-t> -- начало очередного набора, где x -- номер набора, 
            //a -- количество активных ответов, 
            //t(t\f)-- есть ли произвольный ответ
        // <bend-x> -- конец набора, где x -- номер набора
// <que> -- вопрос
        //<qend> -- конец вопроса
// <a-x> -- активный ответ, где x --  номер ответа
        //<aend-x>
//<t> -- произвольнгый ответ
        //<tend>
//<kol> -- количество вопросов
        // <kend> 

        }

        // команда для запоминания введенный данных
        private void button4_Click(object sender, EventArgs e)
        {
            button3.Enabled = true;
            button4.Enabled = false;
            // заносим введённые вопрос и ответы в общую строку
            znach += NewNabor(actotv);

            // делаем элементы не активными
            textBox2.Enabled = false;
            for (int i = 0; i < kolotv; i++)
                Tb[i].Enabled = false;

           


        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // тут будем сохранять в файл информацию
            // добавляем впереди количество вопросов
            znach = "<kol>" + nomber.ToString() + "<kend>" + znach+"\n";

            for (int i = 0; i < kolotv; i++)
            {
                Controls.Remove(Tb[i]);
                Controls.Remove(Lb[i]);
            }
            label2.Visible = false;
            label3.Visible = false;
            textBox2.Visible = false;
            saveFileDialog1.FileName = "";
            
            if (saveFileDialog1.ShowDialog()== DialogResult.OK)
            {

                string skey = "1212345678912443";
                string siv = "1212345678912469";
                //byte[] key = ASCIIEncoding.ASCII.GetBytes("1212345678912443");
                //byte[] iV = ASCIIEncoding.ASCII.GetBytes("1212345678912469");

                // пишим алгоритм шифрования 
                try
                {
                    // Создание нового экземпляра RijndaelManaged
                    // класс. Это принимает новый ключ и инициализации
                    // вектор (IV).
                    using (RijndaelManaged myRijndael = new RijndaelManaged())
                    {


                        //Этот алгоритм поддерживает ключи длиной 128, 192 и 256 бит или 16,24 и 32 байта(символа).
                        byte[] key = ASCIIEncoding.ASCII.GetBytes(skey);
                        byte[] iV = ASCIIEncoding.ASCII.GetBytes(siv);

                        myRijndael.Key = key;
                        myRijndael.IV = iV;

                        // Зашифровать строку в массив байтов.
                        byte[] encrypted = EncryptStringToBytes(znach, myRijndael.Key, myRijndael.IV);
                        // открываем бинарный файл и записываем туда набор байтов
                        File.WriteAllBytes(saveFileDialog1.FileName, encrypted);
                        //textBox2.Text = DecryptStringFromBytes(File.ReadAllBytes(saveFileDialog1.FileName), myRijndael.Key, myRijndael.IV);
                        ;
                    }

                }
                catch (Exception err)
                {
                    MessageBox.Show("Error: {0}", err.Message);
                }

            }
        } 
    }
}