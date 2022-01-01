using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YanmaYakitlar_01
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public void ListBoxTemizle() //ListBox elementlerinin verilerini temizlemek için kullanılacak.
        {
            lst_ozgulisi.Items.Clear();
            lst_entalpi.Items.Clear();
            lst_entropi.Items.Clear();

            lst_a1.Items.Clear();
            lst_a2.Items.Clear();
            lst_a3.Items.Clear();
            lst_a4.Items.Clear();
            lst_a5.Items.Clear();
            lst_a6.Items.Clear();
            lst_a7.Items.Clear();
            lst_a8.Items.Clear();
            lst_a9.Items.Clear();
            lst_a10.Items.Clear();
            lst_a11.Items.Clear();
            lst_a12.Items.Clear();
            lst_a13.Items.Clear();
            lst_a14.Items.Clear();

            lst_molekulAgirligi.Items.Clear();
            lst_elementAdi.Items.Clear();
        }
        private void button1_Click(object sender, EventArgs e) //Hesapla butonu işlevi
        {
            ListBoxTemizle();

            string dosyaYolu = "";

            if (txt_dosyaYolu.Text != "" && txt_dosyaYolu.Text != " " && txt_dosyaYolu.Text != null)
            {
                dosyaYolu = txt_dosyaYolu.Text; //Dosya yolunu al

                var veriler = System.IO.File.ReadAllLines(@dosyaYolu); //txt dosyasındaki tüm satırları oku

                string[] duzenliVeriler = new string[veriler.Length];

                for (int i = 0; i < veriler.Length; i++) //Verileri birbirinden ayırmak için okuduktan sonra yapılan işlemler
                {
                    duzenliVeriler[i] = veriler[i].Replace("-", " -");
                    duzenliVeriler[i] = duzenliVeriler[i].Replace("E -", "E-");
                }

                var elementler = duzenliVeriler //Verileri 4 er gruplar halinde tut
                       .Skip(6) //İlk 6 satırı okuma
                       .Select((value, index) => new { PairNum = index / 4, value })
                       .GroupBy(pair => pair.PairNum)
                       .Select(grp => grp.Select(g => g.value).ToArray())
                       .ToArray();

                string[] elementDegerleri = new string[elementler.Length];
                string[] elementVerileri = new string[elementDegerleri.Length * 90];

                var seperator = ' ';

                for (int i = 0; i < elementler.Length - 2; i++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        elementDegerleri[i] += elementler[i][k]; //Her elemente ait tüm değerleri ayrı tut
                    }
                }

                double istenenSicaklik = 0;
                if (textBox1.Text != null && textBox1.Text != "" && textBox1.Text != " ")
                {
                    istenenSicaklik = Convert.ToDouble(textBox1.Text); //Kullanıcıdan sıcaklık değerini al
                }
                else
                {
                    MessageBox.Show("Lütfen bir sıcaklık değeri giriniz.");
                }

                string istenenElement = comboBox1.Text; //Kullanıcıdan istenen element değerini al
                string istenenFormul = comboBox2.Text; //Kullanıcıdan istenen formül değerini al
                string okunanElementAdi = "";

                string[] sayisalVeriler = new string[15];
                int sayisalVeriIndex = 0; //Sayısal değerleri aynı sırayla elde edebilmek için kullanılacak index verisi
                decimal[] sayisalVerilerExp = new decimal[15];//Sayısal değerleri (14 Değer) Exponantial değerden double tipine çevirip bu dizide tutacağız

                double[] sicaklikVerileri = new double[3]; //Alt ve üst sıcaklık değerlerini burada tutacağız
                int sicaklikVeriIndex = 0;

                double dusukSicaklik = 0;
                double yuksekSicaklik = 0;

                double molekulAgirligi = 0; //Mol ağırlığını burada tutacağız

                double kontrol = 0.0;
                bool doubleMi = false;

                //Katsayı değerleri (14)
                decimal a1 = 0; decimal a2 = 0; decimal a3 = 0; decimal a4 = 0; decimal a5 = 0; decimal a6 = 0; decimal a7 = 0;
                decimal a8 = 0; decimal a9 = 0; decimal a10 = 0; decimal a11 = 0; decimal a12 = 0; decimal a13 = 0; decimal a14 = 0;

                //Sabit değer
                double R = 8.314;

                //Sıcaklık değeri
                decimal T = 0;

                //OzgulIsı,Entropi ve Entalpinin tutulacağı değişkenler
                decimal c = 0;
                decimal cR = 0;

                decimal s = 0;
                decimal sR = 0;

                decimal h = 0;
                decimal hR = 0;

                for (int i = 0; i < elementDegerleri.Length - 2; i++)
                {
                    sayisalVeriIndex = 0;
                    sicaklikVeriIndex = 0;

                    elementVerileri = elementDegerleri[i].Split(seperator, 30, StringSplitOptions.RemoveEmptyEntries);//Elemente ait tüm bilgileri oku.
                    okunanElementAdi = elementVerileri[0];//Elementin adını al.

                    if (okunanElementAdi == istenenElement)//Dosyadaki element ile istenen element aynı ise;
                    {
                        for (int k = 0; k < elementVerileri.Length; k++)//Element verileri kadar döner, her veriyi gezer ve uzunluğuna bakar 12 den büyükse yeni diziye alır.
                        {
                            if (elementVerileri[k].Length > 12 && elementVerileri[k] != null && elementVerileri[k] != "" && elementVerileri[k] != " " && elementVerileri[k].Contains('E'))
                            {
                                sayisalVeriler[sayisalVeriIndex] = elementVerileri[k];
                                sayisalVerilerExp[sayisalVeriIndex] = (decimal)double.Parse(sayisalVeriler[sayisalVeriIndex], CultureInfo.InvariantCulture);

                                sayisalVeriIndex++;
                            }

                            doubleMi = Double.TryParse(elementVerileri[k], out kontrol);
                            if (doubleMi) //Okunan veri double tipindeyse yeni bir dizide tut -> bunlar sıcaklık değerleri olacak.
                            {
                                if (elementVerileri[k].Length > 3 && elementVerileri[k].Length < 12 && elementVerileri[k] != null && elementVerileri[k] != "" && elementVerileri[k] != " ")
                                {
                                    //elementVerileri[k] = elementVerileri[k].Replace('.', ',');
                                    //sicaklikVerileri[sicaklikVeriIndex] = Convert.ToDecimal(elementVerileri[k]);
                                    sicaklikVerileri[sicaklikVeriIndex] = double.Parse(elementVerileri[k], CultureInfo.InvariantCulture);

                                    dusukSicaklik = sicaklikVerileri[0];
                                    yuksekSicaklik = sicaklikVerileri[1];

                                    sicaklikVeriIndex++;

                                    if (sicaklikVerileri.Length == 3) //3 değer okuduğunda 3.değeri molekül ağırlığı olarak al.
                                    {
                                        molekulAgirligi = sicaklikVerileri[2];
                                    }
                                }
                            }
                        }

                        //Birden fazla ekleme sorununu çözmek için bu kontrol eklendi.
                        if (lst_molekulAgirligi.Items.Count == 0)
                        {
                            lst_molekulAgirligi.Items.Add(molekulAgirligi);
                        }
                        if (lst_elementAdi.Items.Count == 0)
                        {
                            lst_elementAdi.Items.Add(okunanElementAdi);
                        }

                        if (istenenSicaklik >= dusukSicaklik && istenenSicaklik <= 1000) //DÜŞÜK SICAKLIK İŞLEMLERİ
                        {
                            T = (decimal)istenenSicaklik;

                            a8 = sayisalVerilerExp[7];
                            a9 = sayisalVerilerExp[8];
                            a10 = sayisalVerilerExp[9];
                            a11 = sayisalVerilerExp[10];
                            a12 = sayisalVerilerExp[11];
                            a13 = sayisalVerilerExp[12];
                            a14 = sayisalVerilerExp[13];

                            if (istenenFormul == "Özgül Isı (Cp)")
                            {
                                c = (a8) + ((a9 * T)) + ((a10 * Convert.ToDecimal((Math.Pow(Convert.ToDouble(T), 2))))) + ((a11 * Convert.ToDecimal((Math.Pow(Convert.ToDouble(T), 3))))) + (a12 * Convert.ToDecimal((Math.Pow(Convert.ToDouble(T), 4))));
                                cR = (c * Convert.ToDecimal(R));

                                lst_ozgulisi.Items.Add(cR);
                            }
                            else if (istenenFormul == "Entalpi (h)")
                            {
                                h = (a8) + (a9 * T / 2) + (a10 * Convert.ToDecimal((Math.Pow(Convert.ToDouble(T), 2))) / 3) + (a11 * Convert.ToDecimal((Math.Pow(Convert.ToDouble(T), 3))) / 4) + (a12 * Convert.ToDecimal((Math.Pow(Convert.ToDouble(T), 4))) / 5) + (a13 / T);
                                hR = h * Convert.ToDecimal(R) * T;

                                lst_entalpi.Items.Add(hR);
                            }
                            else if (istenenFormul == "Entropi (s)")
                            {
                                s = (a8 * Convert.ToDecimal(Math.Log(Convert.ToDouble(T)))) + (a9 * T) + (a10 * Convert.ToDecimal((Math.Pow(Convert.ToDouble(T), 2))) / 2) + (a11 * Convert.ToDecimal((Math.Pow(Convert.ToDouble(T), 3))) / 3) + (a12 * Convert.ToDecimal((Math.Pow(Convert.ToDouble(T), 4))) / 4) + a14;
                                sR = s * Convert.ToDecimal(R);

                                lst_entropi.Items.Add(sR);
                            }
                            else if (istenenFormul == "Tümü")
                            {
                                c = (a8) + ((a9 * T)) + ((a10 * Convert.ToDecimal((Math.Pow(Convert.ToDouble(T), 2))))) + ((a11 * Convert.ToDecimal((Math.Pow(Convert.ToDouble(T), 3))))) + (a12 * Convert.ToDecimal((Math.Pow(Convert.ToDouble(T), 4))));
                                cR = (c * Convert.ToDecimal(R));

                                h = (a8) + (a9 * T / 2) + (a10 * Convert.ToDecimal((Math.Pow(Convert.ToDouble(T), 2))) / 3) + (a11 * Convert.ToDecimal((Math.Pow(Convert.ToDouble(T), 3))) / 4) + (a12 * Convert.ToDecimal((Math.Pow(Convert.ToDouble(T), 4))) / 5) + (a13 / T);
                                hR = h * Convert.ToDecimal(R) * T;

                                s = (a8 * Convert.ToDecimal(Math.Log(Convert.ToDouble(T)))) + (a9 * T) + (a10 * Convert.ToDecimal((Math.Pow(Convert.ToDouble(T), 2))) / 2) + (a11 * Convert.ToDecimal((Math.Pow(Convert.ToDouble(T), 3))) / 3) + (a12 * Convert.ToDecimal((Math.Pow(Convert.ToDouble(T), 4))) / 4) + a14;
                                sR = s * Convert.ToDecimal(R);

                                lst_ozgulisi.Items.Add(cR);
                                lst_entalpi.Items.Add(hR);
                                lst_entropi.Items.Add(sR);
                            }
                            else
                            {
                                MessageBox.Show("Lütfen işlem seçiniz.");
                            }
                        }

                        if (istenenSicaklik >= 1000 && istenenSicaklik <= yuksekSicaklik) //YÜKSEK SICAKLIK İŞLEMLERİ
                        {
                            T = (decimal)istenenSicaklik;

                            a1 = sayisalVerilerExp[0];
                            a2 = sayisalVerilerExp[1];
                            a3 = sayisalVerilerExp[2];
                            a4 = sayisalVerilerExp[3];
                            a5 = sayisalVerilerExp[4];
                            a6 = sayisalVerilerExp[5];
                            a7 = sayisalVerilerExp[6];

                            if (istenenFormul == "Özgül Isı (Cp)")
                            {
                                c = (a1) + ((a2 * T)) + ((a3 * Convert.ToDecimal((Math.Pow(Convert.ToDouble(T), 2))))) + ((a4 * Convert.ToDecimal((Math.Pow(Convert.ToDouble(T), 3))))) + (a5 * Convert.ToDecimal((Math.Pow(Convert.ToDouble(T), 4))));
                                cR = (c * Convert.ToDecimal(R));

                                lst_ozgulisi.Items.Add(cR);
                            }

                            else if (istenenFormul == "Entalpi (h)")
                            {
                                h = (a1) + (a2 * T / 2) + (a3 * Convert.ToDecimal((Math.Pow(Convert.ToDouble(T), 2))) / 3) + (a4 * Convert.ToDecimal((Math.Pow(Convert.ToDouble(T), 3))) / 4) + (a5 * Convert.ToDecimal((Math.Pow(Convert.ToDouble(T), 4))) / 5) + (a6 / T);
                                hR = h * Convert.ToDecimal(R) * (T);

                                lst_entalpi.Items.Add(hR);
                            }

                            else if (istenenFormul == "Entropi (s)")
                            {
                                s = (a1 * Convert.ToDecimal(Math.Log(Convert.ToDouble(T)))) + (a2 * T) + (a3 * Convert.ToDecimal((Math.Pow(Convert.ToDouble(T), 2))) / 2) + (a4 * Convert.ToDecimal((Math.Pow(Convert.ToDouble(T), 3))) / 3) + (a5 * Convert.ToDecimal((Math.Pow(Convert.ToDouble(T), 4))) / 4) + a7;
                                sR = s * Convert.ToDecimal(R);

                                lst_entropi.Items.Add(sR);
                            }

                            else if (istenenFormul == "Tümü")
                            {
                                c = (a1) + ((a2 * T)) + ((a3 * Convert.ToDecimal((Math.Pow(Convert.ToDouble(T), 2))))) + ((a4 * Convert.ToDecimal((Math.Pow(Convert.ToDouble(T), 3))))) + (a5 * Convert.ToDecimal((Math.Pow(Convert.ToDouble(T), 4))));
                                cR = (c * Convert.ToDecimal(R));

                                h = (a1) + (a2 * T / 2) + (a3 * Convert.ToDecimal((Math.Pow(Convert.ToDouble(T), 2))) / 3) + (a4 * Convert.ToDecimal((Math.Pow(Convert.ToDouble(T), 3))) / 4) + (a5 * Convert.ToDecimal((Math.Pow(Convert.ToDouble(T), 4))) / 5) + (a6 / T);
                                hR = h * Convert.ToDecimal(R) * (T);

                                s = (a1 * Convert.ToDecimal(Math.Log(Convert.ToDouble(T)))) + (a2 * T) + (a3 * Convert.ToDecimal((Math.Pow(Convert.ToDouble(T), 2))) / 2) + (a4 * Convert.ToDecimal((Math.Pow(Convert.ToDouble(T), 3))) / 3) + (a5 * Convert.ToDecimal((Math.Pow(Convert.ToDouble(T), 4))) / 4) + a7;
                                sR = s * Convert.ToDecimal(R);

                                lst_ozgulisi.Items.Add(cR);
                                lst_entalpi.Items.Add(hR);
                                lst_entropi.Items.Add(sR);
                            }
                            else
                            {
                                MessageBox.Show("Lütfen işlem seçiniz.");
                            }
                        }
                    }
                }

                //Katsayı değerlerini forma gönder
                lst_a1.Items.Add(sayisalVerilerExp[0]);
                lst_a2.Items.Add(sayisalVerilerExp[1]);
                lst_a3.Items.Add(sayisalVerilerExp[2]);
                lst_a4.Items.Add(sayisalVerilerExp[3]);
                lst_a5.Items.Add(sayisalVerilerExp[4]);
                lst_a6.Items.Add(sayisalVerilerExp[5]);
                lst_a7.Items.Add(sayisalVerilerExp[6]);
                lst_a8.Items.Add(sayisalVerilerExp[7]);
                lst_a9.Items.Add(sayisalVerilerExp[8]);
                lst_a10.Items.Add(sayisalVerilerExp[9]);
                lst_a11.Items.Add(sayisalVerilerExp[10]);
                lst_a12.Items.Add(sayisalVerilerExp[11]);
                lst_a13.Items.Add(sayisalVerilerExp[12]);
                lst_a14.Items.Add(sayisalVerilerExp[13]);
            }
            else
            {
                MessageBox.Show("Lütfen geçerli bir dosya yolu giriniz.");
            }


        }//Hesapla butonu bitişi

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e) //Dosyadan Veri Okuma İşlemleri
        {
            try
            {
                string dosyaYolu = "";
                dosyaYolu = txt_dosyaYolu.Text;

                var veriler = System.IO.File.ReadAllLines(@dosyaYolu);

                string[] duzenliVeriler = new string[veriler.Length];

                for (int i = 0; i < veriler.Length; i++)
                {
                    duzenliVeriler[i] = veriler[i].Replace("-", " -");
                    duzenliVeriler[i] = duzenliVeriler[i].Replace("E -", "E-");
                }

                var elementler = duzenliVeriler
                        .Skip(6)
                        .Select((value, index) => new { PairNum = index / 4, value })
                        .GroupBy(pair => pair.PairNum)
                        .Select(grp => grp.Select(g => g.value).ToArray())
                        .ToArray();

                string[] elementDegerleri = new string[elementler.Length];
                string[] elementVerileri = new string[elementDegerleri.Length * 90];
                var seperator = ' ';

                for (int i = 0; i < elementler.Length - 2; i++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        elementDegerleri[i] += elementler[i][k];
                    }
                }
                string istenenElement = comboBox1.Text;
                string okunanElementAdi = "";

                string[] sayisalVeriler = new string[15];

                decimal[] sayisalVerilerExp = new decimal[15];//Sayısal değerleri Exponantial değerden double tipine çevirip bu dizide tutacağız.

                for (int i = 0; i < elementDegerleri.Length - 2; i++)
                {
                    elementVerileri = elementDegerleri[i].Split(seperator, 30, StringSplitOptions.RemoveEmptyEntries);//Elemente ait tüm bilgileri oku.
                    okunanElementAdi = elementVerileri[0];//Elementin adını al.
                    comboBox1.Items.Add(elementVerileri[0]);//DROPDOWN LISTESINE ELEMENTLER GONDERILIYOR.
                }
                MessageBox.Show("Dosyadan veri okuma işlemi tamamlandı !");
            }
            catch (Exception)
            {
                MessageBox.Show("Veri okuma işlemi sırasında bir hata meydana geldi ! Dosya yolunu kontrol ediniz.");
                throw;
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            //listBox1.Items.Clear();
            //listBox2.Items.Clear();
            //listBox3.Items.Clear();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lbl_entropi_Click(object sender, EventArgs e)
        {

        }

        private void lbl_entalpi_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void lst_entropi_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label20_Click(object sender, EventArgs e)
        {

        }

        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click_1(object sender, EventArgs e) //Birim kütle cinsinden hesaplama işlemi
        {

            lst_birimK1.Items.Clear();
            lst_birimK2.Items.Clear();
            lst_birimK3.Items.Clear();


            decimal ozgulIsi;
            decimal entropi;
            decimal entalpi;
            double mAgirligi;

            if (lst_molekulAgirligi.Items.Count != 0)
            {
                mAgirligi = (double)lst_molekulAgirligi.Items[0];

                if (lst_ozgulisi.Items.Count != 0)
                {
                    ozgulIsi = (decimal)lst_ozgulisi.Items[0];
                    ozgulIsi = ozgulIsi / Convert.ToDecimal(mAgirligi);
                    lst_birimK1.Items.Add(ozgulIsi + " kJ/kgK");
                }

                if (lst_entropi.Items.Count != 0)
                {
                    entropi = (decimal)lst_entropi.Items[0];
                    entropi = entropi / Convert.ToDecimal(mAgirligi);
                    lst_birimK2.Items.Add(entropi + " kJ/kgK");
                }

                if (lst_entalpi.Items.Count != 0)
                {
                    entalpi = (decimal)lst_entalpi.Items[0];
                    entalpi = entalpi / Convert.ToDecimal(mAgirligi);
                    lst_birimK3.Items.Add(entalpi + " kJ/kg");
                }
            }
        }
    }
}
