using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Threading;

namespace Space_Invaders_game
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        bool goLeft, goRight;

        // Uma lista de items a ser removidos
        // basicamente um garbage colletor, desculpa lá Valerio
        List<Rectangle> itemToRemove = new List<Rectangle>();

        int enemyImages = 0;
        int bulletTimer = 0;
        int bulletTimerLimit = 90;
        int totalEnemies = 0;

        // A Velocidade defaut dos enimigos
        int enemySpeed = 6;
        bool gameOver = false;

        DispatcherTimer gameTimer = new DispatcherTimer();
        ImageBrush playerSkin = new ImageBrush();

        string uriPath = "pack://application:,,,/imagens/";


        public MainWindow()
        {
            InitializeComponent();

            gameTimer.Tick += GameLoop;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Start();

            playerSkin.ImageSource = new BitmapImage(new Uri(uriPath + "player.png"));
            player.Fill = playerSkin;

            myCanvas.Focus();
            makeEnemies(30);
        }

        private void GameLoop(object sender, EventArgs e)
        {
            // Nem sei para que serve...
            // To a gozar, cria a hitBox do player, acho eu
            Rect playerHitBox = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
            enemiesLeft.Content = "Enimigos que faltam: " + totalEnemies;

            // mover Jogador 
            if (goLeft && Canvas.GetLeft(player) > 0)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) - 10);
            }

            if (goRight && Canvas.GetLeft(player) + 80 < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) + 10);
            }


            // Criar as balas
            bulletTimer -= 3;

            if (bulletTimer < 0)
            {
                enemyBulletMaker(Canvas.GetLeft(player) + 20, 10);

                bulletTimer = bulletTimerLimit;
            }

            // Todas as movimentações, balas a serem disparadas, 
            // enimigos a moverem-se, são criadas aqui! :) 
            foreach (var x in myCanvas.Children.OfType<Rectangle>())
            {
                // Las balas se mueven, pew pew
                if(x is Rectangle && (string)x.Tag == "bullet")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) - 20);

                    if(Canvas.GetTop(x) < 10)
                    {
                        itemToRemove.Add(x);
                    }

                    // Para ser sincero, eu não faço a mínima idea do que isso faz
                    // mas sem isso o jogo não funciona, então fica aqui
                    Rect bullet = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    foreach (var y in myCanvas.Children.OfType<Rectangle>())
                    {
                        if (y is Rectangle && (string)y.Tag == "enemy")
                        {
                            Rect enemyHit = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);


                            if(bullet.IntersectsWith(enemyHit))
                            {
                                itemToRemove.Add(x);
                                itemToRemove.Add(y);

                                totalEnemies -= 1;
                            }
                        }
                    }


                }

                // Mexe os bichinhos AKA enimigos
                if(x is Rectangle && (string)x.Tag == "enemy")
                {
                    Canvas.SetLeft(x, Canvas.GetLeft(x) + enemySpeed);

                    if(Canvas.GetLeft(x) > 820)
                    {
                        Canvas.SetLeft(x, -80);
                        Canvas.SetTop(x, Canvas.GetTop(x) + (x.Height + 10));
                    }

                    Rect enemyHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (playerHitBox.IntersectsWith(enemyHitBox)) 
                    {
                        showGameOver("Foste matado pelos invasores!!");
                    }
                }

                // move as balas inimigas, sim inimigas
                if(x is Rectangle && (string)x.Tag == "enemyBullet")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) + 10);
                    
                    if(Canvas.GetTop(x) > 480)
                    {
                        itemToRemove.Add(x);
                    }

                    Rect enemyBulletHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (playerHitBox.IntersectsWith(enemyBulletHitBox)) 
                    {
                        showGameOver("Foste matado pelos invasores!!");
                    }


                }
            }

            // Remove alguns elementos no Canvas 
            // Supostamente melhora a performance 
            // Claro que não, isto é pior que Electron esta completamente igual
            foreach (Rectangle i in itemToRemove)
            {
                myCanvas.Children.Remove(i);
            }

            // Aumenta a velocidade dos aliens
            // Porque os familiares morreram na guerra e agora estão chateados
            // Family > tudo - Diesel, Vin
            if(totalEnemies < 10) 
            {
                enemySpeed = 12;
            }

            if(totalEnemies < 1) 
            {
                showGameOver("Ganhaste! Parabéns salvaste o mundo!");
            }
        }

        // Deteta se ast teclas foram clicadas
        private void KeyIsUp(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Left)
            {
                goLeft = false;
            }

            if (e.Key == Key.Right)
            {
                goRight = false;
            }

            if (e.Key == Key.Space)
            {
                // Design da bala friendly (vermelha)
                Rectangle newBullet = new Rectangle
                {
                    Tag = "bullet",
                    Height = 20,
                    Width = 5,
                    Fill = Brushes.White,
                    Stroke = Brushes.Red
                };


                Canvas.SetTop(newBullet, Canvas.GetTop(player) - newBullet.Height);
                Canvas.SetLeft(newBullet, Canvas.GetLeft(player) + player.Width / 2);

                myCanvas.Children.Add(newBullet);
            }

            // Fecha e cria outra janela, quando clicar no Enter
            if(e.Key == Key.Enter && gameOver)
            {
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
        }

        // Deteta se as teclas estão apertadas/clicadas
        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                goLeft = true;
            }

            if (e.Key == Key.Right)
            {
                goRight = true;
            }
        }

        private void enemyBulletMaker(double x, double y)
        {
            // Design da bala enimiga (amarela)
            Rectangle enemyBullet = new Rectangle
            {
                Tag = "enemyBullet",
                Height = 40,
                Width = 15,
                Fill = Brushes.Yellow,
                Stroke = Brushes.Black,
                StrokeThickness = 5,
            };

            Canvas.SetTop(enemyBullet, y);
            Canvas.SetLeft(enemyBullet, x);

            myCanvas.Children.Add(enemyBullet);
        }

        // Fazedor de enemigos 
        // Faz enemigos
        private void makeEnemies(int limit)
        {
            int left = 0;

            totalEnemies = limit;

            for (int i = 0; i < limit; i++)
            {
                ImageBrush enemySkin = new ImageBrush();

                
                Rectangle newEnemy = new Rectangle
                {
                    Tag = "enemy",
                    Height = 45,
                    Width = 45,
                    Fill = enemySkin
                };


                Canvas.SetTop(newEnemy, 30);
                Canvas.SetLeft(newEnemy, left);

                myCanvas.Children.Add(newEnemy);

                left -= 60;

                enemyImages++;

                if (enemyImages > 8)
                {
                    enemyImages = 1;
                }
                
                // Adiciona todos os enimigos da pasta imagens
                switch (enemyImages)
                {
                    case 1:
                        enemySkin.ImageSource = new BitmapImage(new Uri(uriPath + "invader1.gif"));
                        break;

                    case 2:
                        enemySkin.ImageSource = new BitmapImage(new Uri(uriPath + "invader2.gif"));
                        break;

                    case 3:
                        enemySkin.ImageSource = new BitmapImage(new Uri(uriPath + "invader3.gif"));
                        break;

                    case 4:
                        enemySkin.ImageSource = new BitmapImage(new Uri(uriPath + "invader4.gif"));
                        break;

                    case 5:
                        enemySkin.ImageSource = new BitmapImage(new Uri(uriPath + "invader5.gif"));
                        break;

                    case 6:
                        enemySkin.ImageSource = new BitmapImage(new Uri(uriPath + "invader6.gif"));
                        break;

                    case 7:
                        enemySkin.ImageSource = new BitmapImage(new Uri(uriPath + "invader7.gif"));
                        break;

                    case 8:
                        enemySkin.ImageSource = new BitmapImage(new Uri(uriPath + "invader8.gif"));
                        break;

                    default:
                        break;
                }
            }
        }

        // Mostra aquelas mensaguens irritantes la em cima
        // Sim fui eu que pus la, mas não sei como arranjar RIP
        private void showGameOver(string msg)
        {
            gameOver = true;
            gameTimer.Stop();
            enemiesLeft.Content += " " + msg + " Clica Enter para jogar novamente";
        }
    }
}
