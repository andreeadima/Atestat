using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Resources;

namespace Snake
{
    public partial class Form1 : Form
    {
        struct Coord
        {
            public int x, y;
        };
        int punctaj;
        struct Cap
        {
            public int x, y, dir;
        };

        PictureBox[,] tabla;
        Image[] img;
        List<Coord> sarpe;
        int[] dx;
        int[] dy;
        Cap capul;
        Timer T;
        int viteza;
        bool contor;

        bool permis;

        public void GameOver ()
        {   
            T.Stop();
            MessageBox.Show(" FELICITARI !"+'\n'+" Ati obtinut " + punctaj.ToString() + " puncte!");
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;

        }
        public void Adaugare(int xa,int ya)
        {
            //Daca sarpele nu intalneste niciun obstacol
            if (tabla[ya, xa].Image == img[1] || tabla[ya,xa].Image==img[7]) 
            {
                //transforma vechiul cap in corp
                tabla[capul.y, capul.x].Image = img[2];
                //adauga noul cap
                tabla[ya, xa].Image = img[2 + capul.dir];
                Coord aux;
                aux.x = xa;
                aux.y = ya;
                sarpe.Add(aux);
                capul.x = xa;
                capul.y = ya;
            }
            else
                GameOver();
        }

        //stergere ultimul element al cozii
        public void Stergere()
        {
            Coord aux;
            aux = sarpe.First();
            tabla[aux.y, aux.x].Image = img[1]; 
            sarpe.Remove(aux);
        }
        public void Initializare() 
        {
            //initializare vectori de deplasare 
            dx = new int[5];
            dy = new int[5];
            dx[4] = 1;
            dx[1] = dx[3] = 0;
            dx[2] = -1;
            dy[2] = dy[4] = 0;
            dy[1] = 1;
            dy[3] = -1;

            //initializare vector imagini
            img = new Image[8];
            img[0] = Image.FromFile("wall.jpg");
            img[1] = Image.FromFile("floor.jpg");
            img[2] = Image.FromFile("body.jpg");
            img[3] = Image.FromFile("head1.jpg");
            img[4] = Image.FromFile("head2.jpg"); 
            img[5] = Image.FromFile("head3.jpg"); 
            img[6] = Image.FromFile("head4.jpg");
            img[7] = Image.FromFile("food.jpg");

            //initializare tabla
            tabla = new PictureBox[17, 17];
            for (int i = 0; i < 17; ++i)
                for (int j = 0; j < 17; ++j)
                {
                    tabla[i, j] = new PictureBox();
                    tabla[i, j].Size = new Size(35, 35);
                    tabla[i, j].Location = new Point(10 + i * 35, 10 + j * 35);
                    this.Controls.Add(tabla[i, j]);
                }
            //construire tabla
            for (int i = 0; i <= 16; i++)
                //pune zid
                tabla[0, i].Image = tabla[i, 16].Image = tabla[16, i].Image = tabla[i, 0].Image = img[0];
            for(int i = 1; i <= 15; ++i)
                for (int j = 1; j <= 15; j++)
                //pune podea
                    tabla[i, j].Image = img[1];

        }
        public void Resetare()
        {
            for (int i = 0; i <= 16; i++)
                tabla[0, i].Image = tabla[i, 16].Image = tabla[16, i].Image = tabla[i, 0].Image = img[0];
            for (int i = 1; i <= 15; ++i)
                for (int j = 1; j <= 15; j++)
                    tabla[i, j].Image = img[1];

            //creeare sarpe
            sarpe = new List<Coord>(400);
            capul.dir = 1;
            capul.x = 2;
            capul.y = 2;
            Coord aux;
            aux.x = capul.x;
            aux.y = capul.y;
            sarpe.Add(aux);
            Adaugare(2, 3);
            Adaugare(2, 4);

            //initializare punctaj
            punctaj = 0;
            scor.Text = "0";
            permis = true;
            Mancare();

            //initializare timer
            contor = true;
            T = new Timer();
            T.Interval = viteza;
            T.Tick += T_Tick;
            T.Start();

        }
        public void Mancare()
        {
            Random random = new Random();
            int xp = random.Next(1,15);
            int yp=random.Next(1,15);
            if (tabla[xp, yp].Image != img[1])
                Mancare();
            else
                tabla[xp, yp].Image = img[7];
        }
        public Form1()
        {
            InitializeComponent();
            viteza = 400;
            nivel.Text = "1";
            Initializare();
            scor.Text = "0";
        }

        void T_Tick(object sender, EventArgs e)
        {
            //pot fi date comenzi de la tastatura
            permis = true;
            //in cazul in care sarpele intalneste cirese
            if (tabla[capul.y + dy[capul.dir], capul.x + dx[capul.dir]].Image == img[7])
            {
                //adauga un nou element
                Adaugare(capul.x + dx[capul.dir], capul.y + dy[capul.dir]);
                //creste punctajul
                punctaj = punctaj + 3*(5-viteza/100);
                scor.Text = punctaj.ToString();
                //genereaza noi cirese pe tabla
                Mancare();
            }
            else
            { 
                //in cazul in care sarpele intalneste un obstacol
                if (tabla[capul.y + dy[capul.dir], capul.x + dx[capul.dir]].Image == img[0] ||
                    tabla[capul.y + dy[capul.dir], capul.x + dx[capul.dir]].Image == img[2])
                    GameOver();
                     else
                     {
                    //in cazul in care nu intalneste cirese sau obstacol
                            Adaugare(capul.x + dx[capul.dir], capul.y + dy[capul.dir]);
                            Stergere();
                     }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode==Keys.Space)
            {
                if (contor == true)
                {
                    contor = false;
                    T.Stop();
                    MessageBox.Show("Pauza Joc - Inchideti fereastra si apasati SPACE pentru a reincepe jocul");
                }
                else
                {
                    T.Start();
                    contor = true;
                }
            }
            //daca s-a mai primit o comanda in acest interval de timp
            if (!permis)
                return;
            //daca se apasa tasta Down
            if (e.KeyCode == Keys.Down && capul.dir != 2 && capul.dir != 4)
            {
                capul.dir = 4;
                permis = false;
            }
            //daca se apasa tasta Up
            if (e.KeyCode == Keys.Up && capul.dir!= 2 && capul.dir != 4)
            {
                capul.dir = 2;
                permis = false;
            }
            //daca se apasa tasta Right
            if (e.KeyCode == Keys.Right && capul.dir != 1 && capul.dir != 3)
            {
                capul.dir = 1;
                permis = false;
            }
            //daca se apasa tasta Left
            if (e.KeyCode == Keys.Left && capul.dir != 1 && capul.dir != 3)
            {
                capul.dir = 3;
                permis = false;
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Resetare();
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            scor.Enabled = false;
            nivel.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form2 despre;
            despre = new Form2();
            despre.Show();

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            nivel.Text = "1";
            viteza = 400;
            button1.Enabled=true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            viteza = 300 ;
            nivel.Text = "2";
            button1.Enabled = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            viteza = 100;
            nivel.Text = "3";
            button1.Enabled = true;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        
    }
}
