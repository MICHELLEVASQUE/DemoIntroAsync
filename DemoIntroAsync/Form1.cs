using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace DemoIntroAsync
{
    public partial class Form1 : Form
    {
        HttpClient httpClient = new HttpClient();
        public Form1()
        {
            InitializeComponent();
        }

        //Async void debe ser evitado excepto en eventos como este
        private async void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Visible = true;

            var directorioActual = AppDomain.CurrentDomain.BaseDirectory;
            var destinoBaseSecuencial = Path.Combine(directorioActual, @"Imagenes\resultado-secuencial");
            var destinoBasePralelo = Path.Combine(directorioActual, @"Imagenes\resultado-paralelo");
            PrepararEjecución(destinoBasePralelo, destinoBaseSecuencial);

            Console.WriteLine("Inicio");
            List<Imagen> imagenes = ObtenerImagenes();



            //Parte secuencial

            var SW = new Stopwatch();
            SW.Start();

            foreach (var imagen in imagenes) 
            {
                await ProcesarImagen(destinoBaseSecuencial, imagen);
            }
            Console.WriteLine("Secuencial - duración en segundos: {0} ",
                SW.ElapsedMilliseconds / 1000.0);
            SW.Reset();

            SW.Start();
            var tareasEnumerable = imagenes.Select(async imagen =>
            {
                await ProcesarImagen(destinoBasePralelo, imagen);

            });

            await Task.WhenAll(tareasEnumerable);
            Console.WriteLine("Paralelo - duración en segundos: {0}",
                SW.ElapsedMilliseconds / 1000.0);
            
            SW.Stop();

            

            pictureBox1.Visible = false;
        }
        private async Task ProcesarImagen(string directorio,Imagen imagen)
        {
            var respuesta = await httpClient.GetAsync(imagen.URL);
            var contenido = await respuesta.Content.ReadAsByteArrayAsync();
            Bitmap bitmap;
            using(var ms = new MemoryStream(contenido))
            {
                bitmap = new Bitmap(ms);
            }
            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
            var destino = Path.Combine(directorio, imagen.Nombre);
            bitmap.Save(destino);
        }

        private static List<Imagen> ObtenerImagenes()
        {
            var imagenes = new List<Imagen>();
            for(int i = 0; i < 7; i++)
            {
                imagenes.Add(
                    new Imagen()
                    {
                        Nombre = $"Cuba{i}.jpeg",
                        URL = "https://th.bing.com/th/id/R.64475e219aa4b7d97e2ba77fc02339d5?rik=zoCbykCg%2fxu3xQ&riu=http%3a%2f%2fstatic6.businessinsider.com%2fimage%2f5491dffceab8ea043094f59d%2f21-photos-that-will-make-you-want-to-travel-to-cuba.jpg&ehk=EdkudY3In5s1rwlVnvUCWbCgCyl6cbZi9Ikp0MW6kOo%3d&risl=&pid=ImgRaw&r=0"
                    });
                imagenes.Add(
                    new Imagen()
                    {
                        Nombre = $"Cuba2 {i}.png",
                        URL = "https://th.bing.com/th/id/R.fa64bf10bcdb75b8908ea61e2fe9cad3?rik=2KXb3eojv2MRaQ&riu=http%3a%2f%2fupload.wikimedia.org%2fwikipedia%2fcommons%2fd%2fdc%2fCuba_flag.png&ehk=eylLRs9%2bZJ0FG8yPqiJkle21dcX4nBIbHWFy%2bbvgfnM%3d&risl=&pid=ImgRaw&r=0"

                    });

                imagenes.Add(
                    new Imagen()
                    {
                        Nombre = $"Cuba3 {i}.png",
                        URL = "https://th.bing.com/th/id/OIP.MkD9dfX7-9qeGF1tHIO6fwHaDJ?rs=1&pid=ImgDetMain"

                    });

            }
            return imagenes;
        }

        private void BorrarArchivos(string directorio)
        {
            var archivos = Directory.EnumerateFiles(directorio);
            foreach (var archivo in archivos)
            {
                File.Delete(archivo);
            }
        }
        private void PrepararEjecución(string destinoBasePralelo,
            string destinoBaseSecuencial)
        {
            if (!Directory.Exists(destinoBasePralelo))
            {
                Directory.CreateDirectory(destinoBasePralelo);

            }
            if (!Directory.Exists(destinoBaseSecuencial))
            {
                Directory.CreateDirectory(destinoBaseSecuencial);
            }
            BorrarArchivos(destinoBaseSecuencial);
            BorrarArchivos(destinoBasePralelo);
        }

        private async Task<string> ProcesamientoLargo()
        {
            await Task.Delay(5000); //Asincrono
            return "Felipe";
        }
        private async Task RealizarProcesamientoLargoA()
        {
            await Task.Delay(1000);//Asincrona
            Console.WriteLine("Proceso A finalizado");
        }
        private async Task RealizarProcesamientoLargoB()
        {
            await Task.Delay(1000);//Asincrona
            Console.WriteLine("Proceso B finalizado");
        }
        private async Task RealizarProcesamientoLargoC()
        {
            await Task.Delay(1000);//Asincrona
            Console.WriteLine("Proceso C finalizado");
        }
    }
}
