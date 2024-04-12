
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace Open_FileDialog_ResimAcma
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        ArrayList resim_parlaklik = new ArrayList();//resimdeki parlaklık degerlerini tutacak dizi. Global tanımlandı.
        int[] parlakliksayisi = new int[256];  //resimde a parlaklığına sahip piksel sayısını tutacak.Global olarak tanımlandı.
        Bitmap my_gri_image=null;
        Bitmap binary_image = null;
        double Yontemlerin_esik_degeri; 
        
       
      

       public Bitmap SetInvert( Bitmap orjresim1)
        {
            Bitmap temp = (Bitmap)orjresim1;
            Bitmap bmap = (Bitmap)temp.Clone();
            Color c;

           
                for (int i = 0; i < bmap.Width; i++)
                {
                    for (int j = 0; j < bmap.Height; j++)
                    {
                        c = bmap.GetPixel(i, j);
                        bmap.SetPixel(i, j, Color.FromArgb(255 - c.R, 255 - c.G, 255 - c.B));
                    }
                }
                orjresim1 = (Bitmap)bmap.Clone();

              
              return orjresim1;
                
           
        }//sorun  yok

       public Bitmap griresimYapma(Bitmap resim)
        {
            Bitmap griresim = new Bitmap(resim.Width, resim.Height);
            for (int i = 1; i < resim.Height-1; i++)
            {
                for (int j = 1; j < resim.Width-1; j++)
                {
                    Color orjinalrenk = resim.GetPixel(j, i); //current color
                    int Gri_ton = (int)(orjinalrenk.R * 0.3 + orjinalrenk.G * 0.59 + orjinalrenk.B * 0.11);
                    //  Color Grirenk = Color.FromArgb(Gri_ton,Gri_ton,Gri_ton);
                    griresim.SetPixel(j, i, Color.FromArgb(Gri_ton, Gri_ton, Gri_ton));//gri resimde tüm piksel degerleri eşittir.

                }
            }
                     return griresim;

        }//sorun  yok

       public Bitmap Binary_cevir(Bitmap orjresim) 
        {
            Bitmap binary_resim = new Bitmap(orjresim.Width, orjresim.Height);

              for (int i = 0; i < orjresim.Height; i++)
                {
                    for (int j = 0; j < orjresim.Width; j++)
                    {
                        Color orjinalrenk = orjresim.GetPixel(j, i); //current color
                        int T = (int)(orjinalrenk.R * 0.3 + orjinalrenk.G * 0.59 + orjinalrenk.B * 0.11);

                        if (T > 128)  // eşik degerini 128 olarak aldım.
                            T = 255;  //beyaz yapar.
                        else
                            T = 0;   //siyah  yapar.

                        binary_resim.SetPixel(j, i, Color.FromArgb(T, T, T));
                       
                    }
                }
              textBox3.Text = Convert.ToString(128);
              Yontemlerin_esik_degeri = 128;
                return binary_resim;
        }//sorun  yok

       public Bitmap Thresold_Donusum_iki_esik_deger(Bitmap orj_resim)
        {
           
            Bitmap binary_resim = new Bitmap(orj_resim.Width, orj_resim.Height);
            Color orjinalrenk;

            int[,] image = new int[orj_resim.Width, orj_resim.Height];
            double MB = 0;
            double MO = 0, M = 0;



                //Orjinal resmi önce gri yaptım.Gri üzerinden thresold T1 uyguladım.
                orjinalrenk = orj_resim.GetPixel(0, 0);
                int kose_0_0 = (int)(orjinalrenk.R * 0.3 + orjinalrenk.G * 0.59 + orjinalrenk.B * 0.11);

                orjinalrenk = orj_resim.GetPixel(orj_resim.Width - 1, 0);
                int kose_w1_0 = (int)(orjinalrenk.R * 0.3 + orjinalrenk.G * 0.59 + orjinalrenk.B * 0.11);

                orjinalrenk = orj_resim.GetPixel(0, orj_resim.Height - 1);
                int kose_0_h1 = (int)(orjinalrenk.R * 0.3 + orjinalrenk.G * 0.59 + orjinalrenk.B * 0.11);

                orjinalrenk = orj_resim.GetPixel(orj_resim.Width - 1, orj_resim.Height - 1);
                int kose_w1_h1 = (int)(orjinalrenk.R * 0.3 + orjinalrenk.G * 0.59 + orjinalrenk.B * 0.11);

                //MessageBox.Show(Convert.ToString("kose_0_0:" + kose_0_0));
                //MessageBox.Show(Convert.ToString("kose_w_0:" + kose_w1_0));
                //MessageBox.Show(Convert.ToString("kose_0_h:" + kose_0_h1));
                //MessageBox.Show(Convert.ToString("kose_w_h:" + kose_w1_h1));

                image[0, 0] = kose_0_0;
                image[orj_resim.Width - 1, 0] = kose_w1_0;
                image[0, orj_resim.Height - 1] = kose_0_h1;
                image[orj_resim.Width - 1, orj_resim.Height - 1] = kose_w1_h1;

                MB = ((double)(kose_0_0 + kose_0_h1 + kose_w1_0 + kose_w1_h1) / 4);
                textBox2.Text = Convert.ToString(MB);

                for (int i = 0; i < orj_resim.Width - 1; i++)
                {
                    double T1 = 0;

                    for (int j = 1; j < orj_resim.Height - 1; j++) //getpixel(genişlik,yukseklik)
                    {
                        orjinalrenk = orj_resim.GetPixel(i, j); //current color .dikey aşağı dogru tarıyor.
                        T1 = (double)(orjinalrenk.R * 0.3 + orjinalrenk.G * 0.59 + orjinalrenk.B * 0.11);
                        M = M + T1;  //köşe noktalar haric diger noktaların toplamını buluyor.Thresold tek T degeri kullanma
                        image[i, j] = (int)T1; //piksel parlaklıgını diziye koydum.

                    }
                }

                MO = M / ((orj_resim.Height * orj_resim.Width - 4));
                textBox1.Text = Convert.ToString(MO);
                //MessageBox.Show(Convert.ToString("  MB = ((double)(kose_0_0 + kose_0_h + kose_w_0 + kose_w_h) / 4):" + MB));
                //MessageBox.Show(Convert.ToString(" MO = M / ((orj_resim.Height * orj_resim.Width-4)):"+MO));


               // double T = (MO + MB) / 2;   //Eşik degerri(T) bulundu.
                //MessageBox.Show(" double T = (MO + MB) / 2:"+T);

               
               
                double t1=MO, t2=MB;

                while (true)
                {
                    int sayact1 = 0, sayact2 = 0; //T1 ve T2 ye yakın kac nokta var bunu sayar.
                    int Toplamt1 = 0, Toplamt2 = 0; //T1 VE T2 nin etrafındaki piksel dağılımlarını sayar.

                    for (int i = 0; i < orj_resim.Height; i++)
                    {
                        for (int j = 0; j < orj_resim.Width; j++)
                        {
                            Color orjinal_renk = orj_resim.GetPixel(j,i); //current color
                            int P = (int)(orjinal_renk.R * 0.3 + orjinal_renk.G * 0.59 + orjinal_renk.B * 0.11);

                            if (Math.Abs(P - t1) < Math.Abs(t2 - P)) //Nokta t1 e  daha yakınsa  burası çalışır.
                            { Toplamt1 += P; sayact1++; }  //t1 in etrafındaki pikselleri sayar ve parlaklıkları toplar.
                            else
                            { Toplamt2 += P; sayact2++; }

                        }
                    }

                    //Şimdi de rasgele secilen noktalar Gauss denkleminin tam orta noktası mı bunu kontrol ediyoruz.  

                    //MessageBox.Show(" double ortaT1 = ((double)Toplamt1) / sayact1");

                    double ortaT1 = ((double)Toplamt1) / sayact1;
                    double ortaT2 = ((double)Toplamt2) / sayact2;

                    if (((int)Math.Abs((ortaT1 - t1))) == 0 && ((int)Math.Abs((ortaT2 - t2))) == 0)
                    {
                        break;                  //seçilen t1 ve t2 noktaları seçilmesi gereken noktalarmış.
                    }

                    else                        //Demekki rasgele bulunan degerler tam orta noktalar değilmiş.
                    {
                        t1 = ortaT1; t2 = ortaT2;
                    }

                }//while sonu


                double esik_deger = (t1 + t2) / 2;       //Eşik değeri bulduk.
                textBox3.Text = Convert.ToString(esik_deger);
                
           //     MessageBox.Show("sona ulastım.");


                for (int i = 0; i < orj_resim.Height; i++)
                {
                    for (int j = 0; j < orj_resim.Width; j++)
                    {
                        Color renk = orj_resim.GetPixel(j, i); //current color
                        int F_x_y = (int)(renk.R * 0.3 + renk.G * 0.59 + renk.B * 0.11);
                        if (F_x_y > (int)esik_deger)
                        {
                            binary_resim.SetPixel(j, i, Color.FromArgb(255, 255, 255)); ;
                        }  //beyaz yapar
                        else
                        {
                            binary_resim.SetPixel(j, i, Color.FromArgb(0, 0, 0));
                        }  //siyah yapar

                    }
                }
                Yontemlerin_esik_degeri = esik_deger;

          

           return binary_resim;
           
        }  //sorun  yok

       public void Histogram_yap( Bitmap orj_resim)
       {
            try
            {
                listBox2.Items.Clear();
                Bitmap gri_resim = new Bitmap(orj_resim.Width, orj_resim.Height);
                resim_parlaklik.Clear(); //Arraylist in içerisini temizledim.

                pictureBox2.Refresh();


               for (int k = 0; k < 256; k++)  //histogramı tutacak dizi
                        parlakliksayisi[k] = 0; //parlaklık dizisinin boyutu 256'dır.Gri resim 0-255 arası parlaklığa sahip olabilir.


                    for (int x = 0; x < orj_resim.Height; x++)
                    {
                        for (int y = 0; y < orj_resim.Width; y++)
                        {
                            Color orjinal_renk = orj_resim.GetPixel(y, x); 
                            int parlaklik = (int)(orjinal_renk.R * 0.3 + orjinal_renk.G * 0.59 + orjinal_renk.B * 0.11);
                            if (!resim_parlaklik.Contains(parlaklik))//parlaklık dizi elemanı değilse diziye koyar.
                            {
                                resim_parlaklik.Add(parlaklik); parlakliksayisi[parlaklik]++; resim_parlaklik.Sort();
                            }
                            else
                                parlakliksayisi[parlaklik]++;  //parlaklıksayisi indisi 0-255 
                        }
                        resim_parlaklik.Sort();      //diziyi sıralama yapıyor.
                    }



                    int toplam=0;
                    listBox2.Items.Clear();

                    for (int boyut = 0; boyut <256; boyut++) //gri resim parlaklığı 0-255 arası olabilir.
                    {
                       
                        if (resim_parlaklik.Contains(boyut))
                        {
                            listBox2.Items.Add(boyut + "  parlaklığındaki  piksel sayisi:-->  " + parlakliksayisi[boyut]); 
                            toplam += parlakliksayisi[boyut]; //resimde bulunan parlaklıkların  sayısı tutuluyor.
                        }                                    //resimde olmayan parlaklık sayısı 0 olacaktır.
                        else
                        {
                            listBox2.Items.Add(boyut + "  parlaklığndaki  piksel sayisi: -->  " + parlakliksayisi[boyut]);
                         
                        }
                    }                         
                                
            } //try sonu

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

            }
            finally
            {
               
            }

        }     //sorun  yok
                     
       public void Histogram_ciz( Bitmap orj_resim)
        {
            Bitmap histo = new Bitmap(orj_resim.Width,orj_resim.Height);
            ArrayList resim_parlaklik = new ArrayList();//resimdeki parlaklık degerlerini tutacak dizi
            int[] parlakliksayisi = new int[256];  //resimde a parlaklığına sahip piksel sayısını tutacak
            try
            {
               
                for (int k = 0; k < 256; k++)
                    parlakliksayisi[k] = 0; //parlaklık dizisinin boyutu 256'dır.Gri resim 0-255 arası parlaklığa sahip olabilir.


                for (int x = 0; x < orj_resim.Height; x++)
                {
                    for (int y = 0; y < orj_resim.Width; y++)
                    {
                        Color orjinal_renk = orj_resim.GetPixel(y, x); //current color
                        int parlaklik = (int)(orjinal_renk.R * 0.3 + orjinal_renk.G * 0.59 + orjinal_renk.B * 0.11);
                        if (!resim_parlaklik.Contains(parlaklik))//parlaklık dizi elemanı değilse diziye koyar.
                        {
                            resim_parlaklik.Add(parlaklik); parlakliksayisi[parlaklik]++; resim_parlaklik.Sort();
                        }
                        else
                            parlakliksayisi[parlaklik]++;  //parlaklıksayisi indisi 0-255 
                    }
                    resim_parlaklik.Sort();      //diziyi sıralama yapıyor.
                }


                int toplam = 0;

               
                //parlaklisayisi dizisinde resimde olmayan parlaklıkların sayısı 0 dır.
               //Bu yüzden histogramda sadece resimde olan parlaklık degerleri bastırılıyor.

                for (int boyut = 0; boyut < 256; boyut++) //gri resim parlaklığı 0-255 arası olabilir.
                {

                    if (resim_parlaklik.Contains(boyut))
                    {
                        
                        toplam += parlakliksayisi[boyut];
                    }
                   
                }

                //MessageBox.Show("res.wigth*res.heigth:" + Convert.ToString(orj_resim.Width * orj_resim.Height));
                //MessageBox.Show("toplam piksel:" + toplam);



                int w = 2;
                Bitmap bmv = null;
                bmv = new Bitmap(256, pictureBox3.Height);
                Graphics g=Graphics.FromImage((Image)bmv);
                Pen kalem = new Pen(Color.Red, 1);
                Pen Tkalem = new Pen(Color.Black, 3);

                for (int boyut = 0; boyut < 256; boyut++) //gri resim parlaklığı 0-255 arası olabilir.
                {
                       if(resim_parlaklik.Contains(boyut))
                          g.DrawLine(kalem, new Point(boyut, pictureBox3.Height), new Point(boyut, pictureBox3.Height - (parlakliksayisi[boyut])));
                                    
                }
                int pikselT = Convert.ToInt32(Yontemlerin_esik_degeri);
                g.DrawLine(Tkalem, new Point(pikselT, pictureBox3.Height), new Point(pikselT, pikselT / 10));

                pictureBox3.Image = bmv;         
                g.Dispose();

                           
          
            }//try sonu

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

            }

            finally
            {
               
            }
        }//sorun  yok

           //OTSU  FONKSİYONU 
       public Bitmap  Otsu_Donusum(Bitmap orj_resim)
        {
            
            Bitmap orjinalresim = new Bitmap(orj_resim);
            Bitmap binaryresim = new Bitmap(orjinalresim.Width, orjinalresim.Height);
            ArrayList image_parlaklik=new ArrayList();
            image_parlaklik.Clear();
            int []dizi_tut=new int[256];
           // dizi.Clear();
            int T;
            ArrayList en_kucuk_sigma_kare = new ArrayList();
            en_kucuk_sigma_kare.Clear();
            ArrayList Sigma_kare_w = new ArrayList();
            int minimum_T_degeri = -1, s = 1; double minR = 0;

           
           //     MessageBox.Show("Otsu medotu: mesaj 1:");

                for (int y = 0; y < 256; y++)
                {
                    dizi_tut[y] = 0; 
                   
                }  //parlaklık dizisinin boyutu 256'dır.Gri resim (0,0,0)...(255,255,255) arası parlaklığa sahip olabilir.


                for (int x = 0; x < orj_resim.Height; x++)
                {
                    for (int y = 0; y < orj_resim.Width; y++)
                    {
                        Color orjinal_renk = orj_resim.GetPixel(y,x); //current color
                        int parlaklik = (int)(orjinal_renk.R * 0.3 + orjinal_renk.G * 0.59 + orjinal_renk.B * 0.11);
                        if (!image_parlaklik.Contains(parlaklik))//parlaklık dizi elemanı değilse diziye koyar.
                        {
                            image_parlaklik.Add(parlaklik);
                            dizi_tut[parlaklik]++;
                            image_parlaklik.Sort();
                        }
                        else
                            dizi_tut[parlaklik]++;  //parlaklıksayisi indisi 0-255 
                    }
                    //image_parlaklik.Sort();      //diziyi sıralama yapıyor.
                }

  //T=0,T=1,T=2,T=3,T=4,             
  //
  //  //    
                int wb_payda = 0;
             
             for (int j = 0; j < 256; j++)  //payda =36 bulundu.
             wb_payda+=dizi_tut[j];//Belirli parlaklıktaki piksellin topla sayısı bulundu.

             int say_pikselleri = 1;

             while(say_pikselleri <(image_parlaklik.Count-1 ))
               { //piksel sayısını 1 den başlatıp n-1 bitirdim çünkü bu noktanın altıdan ve üstünden eşik deger secilemez.mantıksız olur.(0 pikseli ve n pikseli)
                   int wbackground_pay = 0;
                   int wforeund_pay = 0; 
                         
                  

                   T = (int)image_parlaklik[say_pikselleri]; //piksel parlaklık degre

                   for (int m = 0; m < T; m++)
                   {
                      wbackground_pay += dizi_tut[m]; //T parlaklığına kadar olan piksel sayısını toplamı buldum. 8+1+5
                   }

                  double wb =0;

                  ////if (wb_payda!=0)
                     wb = ((double)wbackground_pay) / (wb_payda);  //  17/36
                                      

                   for (int m = T; m < dizi_tut.Length ; m++)
                   {                      
                           wforeund_pay += dizi_tut[m]; //T parlaklığdan sonraki piksel sayısını toplamı buldum. 5+4+6+7
                   }                                
                 
                   double wf =0;

                   //if (wb_payda != 0)
                       wf = ((double)wforeund_pay) / (wb_payda);  // 18/36
                                                 
                   int meanb_pay = 0;
                   int meanf_pay = 0;                  
                                                      
                   for (int l = 0; l < T; l++)   //mean backgroud
                   {                     
                           meanb_pay += ((int)l * dizi_tut[l]); //0*8+1*9
                   }                  
                  
                   double meanback; ///ortalama 

                       meanback = ((double)meanb_pay) / (wbackground_pay);// 0*8+1*9/17           

                   for (int l = T; l < dizi_tut.Length; l++)
                   {                      
                           meanf_pay += ((int)l) * dizi_tut[l];
                   }


                   double meanfrond =0; //ortalama 
                       meanfrond = ((double)meanf_pay) / (wforeund_pay); //meanf_pay=(0*8+1*7+5*4)
                  
                  

                   double sigma_kare_background = 0;

                   for (int vary = 0; vary < T; vary++)  //varyans
                   {
                       sigma_kare_background += ((int)vary -(int)meanback)*((int)vary - (int)meanback)*(int)dizi_tut[vary];

                   }

                 
                   double rb ;
                   if (wbackground_pay != 0)
                       rb = sigma_kare_background / wbackground_pay;
                   else
                       rb = 0;

              

                   double sigma_kare_foreground = 0;

                  

                   for (int vary = T; vary < dizi_tut.Length-1; vary++)  //varyans
                   {
                       sigma_kare_foreground +=  ((int)vary - (int)meanfrond)*((int)vary -(int)meanfrond )  * (int)dizi_tut[vary];
                         
                   }

                  
                   double rf;

                   if (wforeund_pay != 0 )
                       rf = sigma_kare_foreground /wforeund_pay; //
                   else
                       rf = 0;


                   double r_w =  (wb * rb + wf * rf);  //Herhangi T degeri için  r_w  kare bulundu.


                   Sigma_kare_w.Add((r_w));
                   Sigma_kare_w.Sort();

                   if ((double)Sigma_kare_w[0] != 0)
                   {

                       if (say_pikselleri == s) { minimum_T_degeri = T; minR = (double)Sigma_kare_w[0]; }
                       else if (minR > ((double)Sigma_kare_w[0])) { minimum_T_degeri = T; minR = (double)Sigma_kare_w[0]; }
                   }
                   else { Sigma_kare_w.Remove(0); s++; }

                 
                   
                  say_pikselleri++;
                  listBox2.Items.Add(" parlaklık değerleri : " + T + "sigma kare sonucu : " + wb * rb + wf * rf);
               } //while sonu

   //           

                //double esik = (double)Sigma_kare_w[0];

           
       //         listBox2.Items.Add("  esik değeri= " + esik);
                listBox2.Items.Add("  Minimum parlaklık degeri olan  T= "+minimum_T_degeri);

               for (int i = 0; i < orj_resim.Width; i++)
               {
                   for (int j = 0; j < orj_resim.Height; j++)
                   {
                       Color renk = orj_resim.GetPixel(i, j); //current color
                       int F_x_y = (int)(renk.R * 0.3 + renk.G * 0.59 + renk.B * 0.11);
                       if (F_x_y > minimum_T_degeri)
                           binaryresim.SetPixel(i, j, Color.FromArgb(255, 255, 255));//BEYAZ
                       else
                           binaryresim.SetPixel(i, j,Color.FromArgb(0, 0, 0));     //siyahlaştırır.
                    
                  }

                 
                   Yontemlerin_esik_degeri = minimum_T_degeri;
                   textBox3.Text = Convert.ToString(minimum_T_degeri);

                   

               }
             
          


             return binaryresim;

                listBox2.Refresh();
            
           
          }      //sorun yok

       public void Etiketleme(Bitmap resim) {

         
       
       int [,]res_diz_sek_tut=new int[resim.Height,resim.Width];

       for (int i = 0; i < resim.Height; i++)
       {
           for (int j = 0; j < resim.Width; j++)
           {
               res_diz_sek_tut[i,j]=0;  //dizinin  içini 0'lama işlemi yaptım.
           }
       }


    


       int renk_deger =5;

       int m = renk_deger;
         
       /////ALGORİTMANIN  BAŞLANGICI
       for (int i = 0; i < resim.Height; i++)
       {
           for (int j = 0; j < resim.Width; j++)
           {
                  Color renk = resim.GetPixel(j, i);
                    int Y = (int)(renk.R * 0.3 + renk.G * 0.59 + renk.B * 0.11);
              if (Y==0)
               {

                   if (i == 0 && j == 0)
                   { //her za
                       res_diz_sek_tut[i, j] = renk_deger; renk_deger++; /////////////////////
                   }

                   else if (i != 0 && j == 0)
                   {
                       if (res_diz_sek_tut[i - 1, j] != 0) //her satır için bir üstü bakacak
                           res_diz_sek_tut[i, j] = res_diz_sek_tut[i - 1, j];

                       else if (res_diz_sek_tut[i - 1, j + 1] != 0)
                       {
                           res_diz_sek_tut[i, j] = res_diz_sek_tut[i - 1, j + 1];

                       }
                       else
                       {
                           res_diz_sek_tut[i, j] = renk_deger; renk_deger++;
                       }

                   }

                   else if (i == 0 && j != 0)
                   {
                       if (res_diz_sek_tut[i, j - 1] != 0) { res_diz_sek_tut[i, j] = res_diz_sek_tut[i, j - 1]; } //sol taraftan atama
                       else { res_diz_sek_tut[i, j] = renk_deger; renk_deger++; } 
                   }

                   else if (i != 0 && j != (resim.Width - 1))
                   {
                       if (res_diz_sek_tut[i, j - 1] != 0)
                       { 
                           res_diz_sek_tut[i, j] = res_diz_sek_tut[i, j - 1]; 
                       }//arka yanından atama
                       else if (res_diz_sek_tut[i - 1, j - 1] != 0) 
                       { 
                           res_diz_sek_tut[i, j] = res_diz_sek_tut[i - 1, j - 1];
                       } //kuzeybatıdan atama
                       else if (res_diz_sek_tut[i - 1, j] != 0)
                       {
                           res_diz_sek_tut[i, j] = res_diz_sek_tut[i - 1, j];
                       } // yukarıdan  atama
                       else if (res_diz_sek_tut[i - 1, j + 1] != 0) {
                           res_diz_sek_tut[i, j] =res_diz_sek_tut[i - 1, j + 1];
                       } //kuzeydoğudan atama
                       else
                       {
                           res_diz_sek_tut[i, j] = renk_deger; renk_deger++; 
                       }/////atama
                   }

                  




                   else if (i != 0 && (j == resim.Width - 1))
                   {
                       if ( res_diz_sek_tut[i, j - 1]!=0) //her satır için bir sol bakacak
                           res_diz_sek_tut[i, j] = res_diz_sek_tut[i, j - 1];

                       else if ( 0!= res_diz_sek_tut[i - 1, j - 1])
                       {
                           res_diz_sek_tut[i, j] = res_diz_sek_tut[i - 1, j - 1];
                       }

                       else if (0!= res_diz_sek_tut[i - 1, j])
                       {
                           res_diz_sek_tut[i, j] = res_diz_sek_tut[i - 1, j];
                       }
                     
                       else
                       {
                           res_diz_sek_tut[i, j] = renk_deger; renk_deger++;
                       }


                   }

                   else if (i == resim.Height - 1 && j == 0)
                   {
                       if (res_diz_sek_tut[i - 1, j] != 0)
                       {
                           res_diz_sek_tut[i, j] = res_diz_sek_tut[i - 1, j];
                       }
                       else if (res_diz_sek_tut[i - 1, j + 1] != 0)
                       {
                           res_diz_sek_tut[i, j] = res_diz_sek_tut[i - 1, j + 1];
                       }
                       else
                       {
                           res_diz_sek_tut[i, j] = renk_deger; renk_deger++;
                       }
                   }

                

              
              }}} //Matris renk_degerleri ile dolduruldu.Benim algoritmam sola öncelik veriyor.

      

       listBox3.Items.Add("ilk değerlendirme sonucu bulunan  nesne sayisi =\n"+(renk_deger));
       listBox3.Items.Add("Şimdi de kesişme varsa onlara aynı renk değerini atıyor.\n");  

//şimdide fonksiyon çagrısı  ile komşu olup farklı degerlere sahip olan pikselleri 
//kontrol edeceğim.

       int ara_deger=0, kucuk=0, aranan, koyulacakolan;
       ArrayList dizi_al = new ArrayList();
       dizi_al.Clear(); int geciçisil = 0;
       for (int i = 1; i < resim.Height - 1; i++)
       {
            
           for (int j = 1; j < resim.Width - 1; j++)
           {
               dizi_al.Clear();
               ara_deger=res_diz_sek_tut[i,j];
               if (ara_deger != 0)
               {
                   if (res_diz_sek_tut[i, j] != 0 && !dizi_al.Contains(res_diz_sek_tut[i, j]))
                       dizi_al.Add(res_diz_sek_tut[i, j]);

                   if (res_diz_sek_tut[i - 1, j - 1] != 0 && !dizi_al.Contains(res_diz_sek_tut[i - 1, j - 1]))
                      dizi_al.Add(res_diz_sek_tut[i - 1, j - 1]);

                   if (res_diz_sek_tut[i - 1, j] != 0 && !dizi_al.Contains(res_diz_sek_tut[i - 1, j]))
                       dizi_al.Add(res_diz_sek_tut[i - 1, j]);

                   if (res_diz_sek_tut[i - 1, j + 1] != 0 && !dizi_al.Contains(res_diz_sek_tut[i - 1, j + 1]))
                       dizi_al.Add(res_diz_sek_tut[i - 1, j + 1]);

                   if (res_diz_sek_tut[i, j + 1] != 0 && !dizi_al.Contains(res_diz_sek_tut[i, j + 1]))
                       dizi_al.Add(res_diz_sek_tut[i, j + 1]);

                   if (res_diz_sek_tut[i + 1, j + 1] != 0 && !dizi_al.Contains(res_diz_sek_tut[i + 1, j + 1]))
                       dizi_al.Add(res_diz_sek_tut[i + 1, j + 1]);

                   if (res_diz_sek_tut[i + 1, j] != 0 && !dizi_al.Contains(res_diz_sek_tut[i + 1, j]))
                       dizi_al.Add(res_diz_sek_tut[i + 1, j]);

                   if (res_diz_sek_tut[i + 1, j - 1] != 0 && !dizi_al.Contains(res_diz_sek_tut[i + 1, j - 1]))
                       dizi_al.Add(res_diz_sek_tut[i + 1, j - 1]);

                   if (res_diz_sek_tut[i, j - 1] != 0 && !dizi_al.Contains(res_diz_sek_tut[i, j - 1]))
                       dizi_al.Add(res_diz_sek_tut[i, j - 1]);


                   dizi_al.Sort();
                   kucuk = (int)dizi_al[0];  //minimum elemanı kucuk değişkenine atadım.
                 
              
                   for (int id = 0; id < dizi_al.Count; id++)
                   {
                       if (Convert.ToInt32(dizi_al[id]) != kucuk)
                       {
                           for (int d = 0; d < resim.Height; d++)
                           {
                               for (int e = 0; e < resim.Width; e++)
                               { 
                                   if (res_diz_sek_tut[d, e] == Convert.ToInt32(dizi_al[id])) //dizi_al  9 noktanın farklı değerleri ile doldurulmuştu. 
                                   {                                            //bu 9 değer kucuk değerinden buyukse onların değerlerinin yerine 
                                   res_diz_sek_tut[d, e] = kucuk;               //kucuk değişkenin içeriğini atadım.
                                                                 
                                   }                                         //  1 0 4
                                }                                            //  5 2 6
                           }                                                 //  7 8 4
                       } 
                   }
               


               }//if sonu 
              

           }
       } //for sonu 


       ArrayList kac_nesne = new ArrayList(); 
           kac_nesne.Clear();
       int []dizi_son_parlaklik_degeri_tut=new int[resim.Height*resim.Width];
    
      for (int i = 0; i < resim.Height*resim.Width; i++)
       {  
             dizi_son_parlaklik_degeri_tut[i]=0;
       }
          

       for(int i=0;i<resim.Height;i++)
       {
           for (int j = 0; j < resim.Width;j++ )
           {
               if (!kac_nesne.Contains(res_diz_sek_tut[i, j]) && res_diz_sek_tut[i, j] !=0 )  //0 hariç degerler konuldu.5 bu arraylistte var mı?
               {
                   kac_nesne.Add(res_diz_sek_tut[i, j]); dizi_son_parlaklik_degeri_tut[res_diz_sek_tut[i, j]]++;
               }
               else
                   dizi_son_parlaklik_degeri_tut[res_diz_sek_tut[i, j]]++;  //
           }
       }
       listBox3.Items.Add("Etiketleme sonucu nesne  sayısı = "+(kac_nesne.Count));

     //ŞİMDİ RENKLENDİRME İŞLEMİ YAPACAĞIM.//

       Random rastgele_sayi_üret = new Random();
       int yeni_R, yeni_G, yeni_B;

       for (int l = 0; l < kac_nesne.Count; l++)
       {
           int a = (int)kac_nesne[l];
           yeni_R = rastgele_sayi_üret.Next(0, 256);
           yeni_G = rastgele_sayi_üret.Next(0, 256);
           yeni_B = rastgele_sayi_üret.Next(0, 256);
           
           for(int k=0;k<resim.Height;k++)
           {
               for (int mn = 0; mn < resim.Width;mn++ )
               {
                   if (a == res_diz_sek_tut[k, mn])
                   {
                       resim.SetPixel(mn, k, Color.FromArgb(yeni_R,yeni_G,yeni_B));//gri resimde tüm piksel degerleri eşittir.
                   }
               }
               pictureBox2.Image = resim;
           }

       }
       

             
}     //sorun yok gibi

       private Bitmap GetReSizedImage(Bitmap inputBitmap, int asciiWidth)
        {
            int asciiHeight = 0;
            //Calculate the new Height of the image from its width
            asciiHeight = (int)Math.Ceiling((double)inputBitmap.Height * asciiWidth / inputBitmap.Width);

            //Create a new Bitmap and define its resolution
            Bitmap result = new Bitmap(asciiWidth, asciiHeight);
            Graphics g = Graphics.FromImage((Image)result);
            //The interpolation mode produces high quality images 
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(inputBitmap, 0, 0, asciiWidth, asciiHeight);
            g.Dispose();
            return result;
        }
      
       private void toolStripButton1_Click_1(object sender, EventArgs e)
        {

            openFileDialog1.FileName = "";
            openFileDialog1.InitialDirectory = "C:\\Users\\Kitaplıklar\\Resimler";
            openFileDialog1.Title = "Resim seçin";
            openFileDialog1.Filter = "ResimDosyaları|" + "*.bmp;*.jpg;*.gif;*.wmf;*.tif;*.png";
            openFileDialog1.ShowDialog();
            string yol1 = openFileDialog1.FileName;

            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.Image = Image.FromFile(yol1);
            pictureBox2.Image = null;
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
      }

       private void Form1_Load(object sender, EventArgs e)
      {
          timer1.Enabled = true;
          timer1.Interval = 75; //YAZININ GECİŞ SÜRESİNİ AYARLAR.
          textBox4.Visible = false;
      }

       private void timer1_Tick(object sender, EventArgs e)
      {
          this.Text = this.Text.Substring(1) + this.Text.Substring(0, 1); //YAZININ BİR SONRAKİ KARAKTERE ATLAMASINI SAĞLAR.
      }

       public int wb_payda { get; set; }

       private void toolStripMenuItem2_Click(object sender, EventArgs e)
       {
           openFileDialog1.FileName = "";
           openFileDialog1.InitialDirectory = "C:\\Users\\Kitaplıklar\\Resimler";
           openFileDialog1.Title = "Resim seçin";
           openFileDialog1.Filter = "ResimDosyaları|" + "*.bmp;*.jpg;*.gif;*.wmf;*.tif;*.png";
           openFileDialog1.ShowDialog();
           string yol1 = openFileDialog1.FileName;

           pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
           pictureBox1.Image = Image.FromFile(yol1);
           pictureBox2.Image = null;
           textBox1.Text = "";
           textBox2.Text = "";
           textBox3.Text = "";
       }

       private void toolStripMenuItem3_Click(object sender, EventArgs e)
       {
           this.Close();
       }

       private void toolStripMenuItem4_Click(object sender, EventArgs e)
       {

       }

       private void griYapToolStripMenuItem_Click(object sender, EventArgs e)
       {

           pictureBox2.Image = griresimYapma((Bitmap)pictureBox1.Image);
       }

       private void binaryYapToolStripMenuItem_Click(object sender, EventArgs e)
       {
           pictureBox2.Image = Binary_cevir((Bitmap)pictureBox1.Image);
       }

       private void invertToolStripMenuItem_Click(object sender, EventArgs e)
       {
           pictureBox2.Image = SetInvert((Bitmap)pictureBox1.Image);
       }

       private void thresoldToolStripMenuItem_Click(object sender, EventArgs e)
       {
           pictureBox2.Image = Thresold_Donusum_iki_esik_deger((Bitmap)pictureBox1.Image);
       }

       private void otsuToolStripMenuItem_Click(object sender, EventArgs e)
       {
           pictureBox2.Image = Otsu_Donusum((Bitmap)pictureBox1.Image);
       }

       private void binaryİleToolStripMenuItem_Click(object sender, EventArgs e)
       {
           pictureBox2.Image = Binary_cevir((Bitmap)pictureBox1.Image);
           Histogram_ciz((Bitmap) pictureBox1.Image);
       }

       private void thresoldİleToolStripMenuItem_Click(object sender, EventArgs e)
       {
           Thresold_Donusum_iki_esik_deger((Bitmap)pictureBox1.Image);
           Histogram_ciz((Bitmap)pictureBox1.Image);
       }

       private void otsuİleToolStripMenuItem_Click(object sender, EventArgs e)
       {
           Otsu_Donusum((Bitmap)pictureBox1.Image);
           Histogram_ciz((Bitmap)pictureBox1.Image);
       }

       private void geriYüklemeToolStripMenuItem_Click(object sender, EventArgs e)
       {
           if (pictureBox1.Image != null)
               pictureBox1.Image = pictureBox2.Image;

           else
               MessageBox.Show("Lütfen pictureBox1 e resim yükledikten sonra  bu işlemi yapınız");
       }

       private void binaryİleToolStripMenuItem1_Click(object sender, EventArgs e)
       {
          
           Etiketleme( Binary_cevir((Bitmap)pictureBox1.Image));
       }

       
          

       private void matriseAktarToolStripMenuItem_Click(object sender, EventArgs e)
       {
          

           if (pictureBox1.Image == null)
           {
               MessageBox.Show("Resim yükleyiniz."); 
           }
 
             
       }

       private void thresoldİleToolStripMenuItem1_Click(object sender, EventArgs e)
       {
           Etiketleme(Thresold_Donusum_iki_esik_deger((Bitmap)pictureBox1.Image));
       }

       private void otsuİleToolStripMenuItem1_Click(object sender, EventArgs e)
       {
           Etiketleme(Otsu_Donusum((Bitmap)pictureBox1.Image));
       }

       private void richTextBox1_TextChanged(object sender, EventArgs e)
       {

       }

       private void acıklamaKısmıToolStripMenuItem_Click(object sender, EventArgs e)
       {
           textBox4.Visible = true;
       }

       private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
       {

       }

       private void histogramGösterToolStripMenuItem_Click(object sender, EventArgs e)
       {
           Histogram_yap( (Bitmap)pictureBox1.Image);
       }

           

       

       
    }
}